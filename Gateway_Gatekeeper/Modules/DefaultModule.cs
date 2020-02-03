using System;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Common;
using Gateway_Gatekeeper.Validator;
using MassTransit;
using Polly;

namespace Gateway_Gatekeeper.Modules
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            MultiValidator.Validators.Add(typeof(DataValidator<>));
            MultiValidator.Validators.Add(typeof(SQLInjectionValidator<>));

            TimeSpan timeout = TimeSpan.FromSeconds(25);
            
            builder.Register(c => Bus.Factory.CreateUsingRabbitMq(cfg =>
                    cfg.Host(new Uri(Startup.Configuration["mbUrl"]), h =>
                    {
                        h.Username(Startup.Configuration["mbUsername"]);
                        h.Password(Startup.Configuration["mbPassword"]);
                    })
                 ))
                .As<IBusControl>()
                .As<IPublishEndpoint>()
                .SingleInstance();

            builder.Register(c => new MessageRequestClient<ChatUpRequestDto, ChatUpResponseDto>(
                    c.Resolve<IBusControl>(), 
                    new Uri(Startup.Configuration["chatUpMbUrl"] + "/chatup"),
                    timeout
                ))
                .As<IRequestClient<ChatUpRequestDto, ChatUpResponseDto>>();

            builder.Register(c => new MessageRequestClient<GetUserContactsRequestDto, GetUserContactsResponseDto>(
                    c.Resolve<IBusControl>(),
                    new Uri(Startup.Configuration["userMbUrl"] + "/getusercontactsevents"),
                    timeout
                ))
                .As<IRequestClient<GetUserContactsRequestDto, GetUserContactsResponseDto>>();

            builder.Register(c => new MessageRequestClient<GetUserProfileDataRequestDto, GetUserProfileDataResponseDto>(
                    c.Resolve<IBusControl>(),
                    new Uri(Startup.Configuration["userMbUrl"] + "/getuserprofiledataevents"),
                    timeout
                ))
                .As<IRequestClient<GetUserProfileDataRequestDto, GetUserProfileDataResponseDto>>();

            builder.Register(c => new MessageRequestClient<RegisterUserRequestDto, RegisterUserResponseDto>(
                    c.Resolve<IBusControl>(),
                    new Uri(Startup.Configuration["userMbUrl"] + "/registeruserevents"),
                    timeout
                ))
                .As<IRequestClient<RegisterUserRequestDto, RegisterUserResponseDto>>();
            
            builder.Register(c => new MessageRequestClient<VerifyRegisterUserRequestDto, VerifyRegisterUserResponseDto>(
                    c.Resolve<IBusControl>(),
                    new Uri(Startup.Configuration["mailMbUrl"] + "/mail_check_queue"),
                    timeout
                ))
                .As<IRequestClient<VerifyRegisterUserRequestDto, VerifyRegisterUserResponseDto>>();

            builder.Register(c => new MessageRequestClient<UserSignInRequestDto, UserSignInResponseDto>(
                    c.Resolve<IBusControl>(),
                    new Uri(Startup.Configuration["userMbUrl"] + "/usersigninevents"),
                    timeout
                ))
                .As<IRequestClient<UserSignInRequestDto, UserSignInResponseDto>>();

            builder.Register(c => new MessageRequestClient<UpdateUserProfileDataRequestDto, UpdateUserProfileDataResponseDto>(
                    c.Resolve<IBusControl>(),
                    new Uri(Startup.Configuration["userMbUrl"]+ "/updateuserprofiledataevents"),
                    timeout
                ))
                .As<IRequestClient<UpdateUserProfileDataRequestDto, UpdateUserProfileDataResponseDto>>();

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
