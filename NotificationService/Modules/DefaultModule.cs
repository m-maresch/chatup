using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Common;
using MassTransit;
using NotificationService.Consumers;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Modules
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

            builder.RegisterType<NotificationHandler>();

            builder.RegisterType<MessageConsumer>();
            builder.RegisterType<UserCreatedConsumer>();
            builder.RegisterType<UserProfileDataUpdatedConsumer>();

            builder.Register(c => Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        c = c.Resolve<IComponentContext>();

                        var host = cfg.Host(new Uri(Startup.Configuration["mbUrl"]), h =>
                        {
                            h.Username(Startup.Configuration["mbUsername"]);
                            h.Password(Startup.Configuration["mbPassword"]);
                        });

                        cfg.ReceiveEndpoint(host, "messages", e =>
                        {
                            e.Consumer(typeof(MessageConsumer), c.Resolve);
                        });

                        cfg.ReceiveEndpoint(host, "usercreatedevents_notificationservice", e =>
                        {
                            e.Consumer(typeof(UserCreatedConsumer), c.Resolve);
                        });

                        cfg.ReceiveEndpoint(host, "userprofiledataupdatedevents_notificationservice", e =>
                        {
                            e.Consumer(typeof(UserProfileDataUpdatedConsumer), c.Resolve);
                        });
                    }
                ))
                .As<IBusControl>()
                .As<IPublishEndpoint>()
                .SingleInstance();

            builder.Register(c => new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<SendMessageRequestDto, Notification>();
                        cfg.CreateMap<UserCreatedEvent, User>();
                        cfg.CreateMap<UserProfileDataUpdatedEvent, User>();
                    }
                ))
                .As<MapperConfiguration>();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>();
        }
    }
}
