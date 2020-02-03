using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Timers;
using Common.Extensions;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService
{
    public class NotificationHandler
    {
        public IHubContext<NotificationsHub> _hubContext;
        public IRepository<User> _userRepository;

        public static Subject<Notification> MissedNotifications { get; set; } = new Subject<Notification>();

        public NotificationHandler(IHubContext<NotificationsHub> hubContext, IRepository<User> userRepository)
        {
            this._hubContext = hubContext;
            this._userRepository = userRepository;
        }

        public void Start()
        {
            MissedNotifications.Subscribe(async n =>
            {
                await RedisStore.MessageCache.StringSetAsync(Guid.NewGuid().ToString(), n.ToJson());
            });

            Timer timer = new Timer(300000);

            timer.Elapsed += async (sender, e) =>
            {
                foreach (var key in RedisStore.MessageCacheConnection
                    .GetServer(Startup.Configuration["messageCacheUrl"])
                    .Keys())
                {
                    string message = await RedisStore.MessageCache.StringGetAsync(key);

                    Notification notification = message.ToString().ToObject<Notification>();

                    User target = await _userRepository.GetById(notification.ReceiverID);
                    try
                    {
                        await _hubContext.Clients.Group(target.UserID.ToString()).SendAsync(new
                        {
                            SenderID = notification.SenderID,
                            MessageContent = notification.MessageContent,
                            Time = notification.Time
                        }.ToJson());

                        RedisStore.MessageCache.KeyDelete(key);
                    }
                    catch { }
                }
            };

            timer.Start();
        }
    }
}
