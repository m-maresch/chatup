using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Common;
using MassTransit;
using Polly;
using UserService.Consumers;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Modules
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new UsersContext())
                .As<UsersContext>();

            builder.Register(c => new UsersRepository(c.Resolve<UsersContext>()))
                .As<UsersRepository>();

            builder.Register(c => new UsersRepositoryWithCache(c.Resolve<UsersRepository>()))
                .As<IRepository<User>>();

            builder.RegisterType<GetUserContactsConsumer>();
            builder.RegisterType<GetUserProfileDataConsumer>();
            builder.RegisterType<MailVerificationSucceededConsumer>();
            builder.RegisterType<RegisterUserConsumer>();
            builder.RegisterType<SetUserContactsConsumer>();
            builder.RegisterType<UpdateUserProfileDataConsumer>();
            builder.RegisterType<UserSignInConsumer>();
            
            builder.Register(c => Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        c = c.Resolve<IComponentContext>();

                        var host = cfg.Host(new Uri(Startup.Configuration["mbUrl"]), h =>
                        {
                            h.Username(Startup.Configuration["mbUsername"]);
                            h.Password(Startup.Configuration["mbPassword"]);
                        });
                        
                        cfg.ReceiveEndpoint(host, "getusercontactsevents", e =>
                        {
                            e.Consumer(typeof(GetUserContactsConsumer), c.Resolve);
                        });

                        cfg.ReceiveEndpoint(host, "getuserprofiledataevents", e =>
                        {
                            e.Consumer(typeof(GetUserProfileDataConsumer), c.Resolve);
                        });

                        cfg.ReceiveEndpoint(host, "mailverificationsucceededevents", e =>
                        {
                            e.Consumer(typeof(MailVerificationSucceededConsumer), c.Resolve);
                        });

                        cfg.ReceiveEndpoint(host, "registeruserevents", e =>
                        {
                            e.Consumer(typeof(RegisterUserConsumer), c.Resolve);
                        });

                        cfg.ReceiveEndpoint(host, "setusercontactsevents", e =>
                        {
                            e.Consumer(typeof(SetUserContactsConsumer), c.Resolve);
                        });

                        cfg.ReceiveEndpoint(host, "updateuserprofiledataevents", e =>
                        {
                            e.Consumer(typeof(UpdateUserProfileDataConsumer), c.Resolve);
                        });

                        cfg.ReceiveEndpoint(host, "usersigninevents", e =>
                        {
                            e.Consumer(typeof(UserSignInConsumer), c.Resolve);
                        });
                    }
                ))
                .As<IBusControl>()
                .As<IPublishEndpoint>()
                .SingleInstance();
            
            builder.Register(c => new MessageRequestClient<GetUserTagsRequestDto, GetUserTagsResponseDto>(
                    c.Resolve<IBusControl>(),
                    new Uri(Startup.Configuration["chatUpMbUrl"] + "/getusertags"),
                    TimeSpan.FromSeconds(25)
                ))
                .As<IRequestClient<GetUserTagsRequestDto, GetUserTagsResponseDto>>();

            builder.Register(c => new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<List<GetUserContactsUserDto>, List<User>>();
                        cfg.CreateMap<RegisterUserRequestDto, User>();
                        cfg.CreateMap<User, GetUserProfileDataResponseDto>();
                        cfg.CreateMap<User, UserCreatedEvent>();
                        cfg.CreateMap<User, UserProfileDataUpdatedEvent>();
                    }
                ))
                .As<MapperConfiguration>();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>();

            builder.Register(c =>
                {
                    Policy timeoutPolicy = Policy
                        .TimeoutAsync(90);

                    Policy retryPolicy = Policy
                        .Handle<RequestTimeoutException>()
                        .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                    Policy circuitBreakerPolicy = Policy
                        .Handle<RequestTimeoutException>()
                        .AdvancedCircuitBreakerAsync(
                            failureThreshold: 0.7,
                            samplingDuration: TimeSpan.FromSeconds(30),
                            minimumThroughput: 25,
                            durationOfBreak: TimeSpan.FromSeconds(300)
                        );

                    return Policy.WrapAsync(timeoutPolicy, retryPolicy, circuitBreakerPolicy);
                })
                .As<Policy>()
                .SingleInstance();
        }
    }
}
