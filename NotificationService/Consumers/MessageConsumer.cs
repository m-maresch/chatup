using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Extensions;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Consumers
{
    public class MessageConsumer : IConsumer<SendMessageRequestDto>
    {
        private IHubContext<NotificationsHub> _hubContext;
        private IRepository<User> _userRepository;
        private IMapper _mapper;
        private IPublishEndpoint _publishEndpoint;

        public MessageConsumer(IHubContext<NotificationsHub> hubContext, IRepository<User> userRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _hubContext = hubContext;
            _userRepository = userRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<SendMessageRequestDto> context)
        {
            try
            {
                User target = await _userRepository.GetById(context.Message.ReceiverID);

                try
                {
                    await _hubContext.Clients.Group(target.UserID.ToString()).SendAsync(new
                    {
                        SenderID = context.Message.SenderID,
                        MessageContent = context.Message.MessageContent,
                        Time = context.Message.Time
                    }.ToJson());
                }
                catch
                {
                    Notification notification = _mapper.Map<SendMessageRequestDto, Notification>(context.Message);

                    NotificationHandler.MissedNotifications.OnNext(notification);
                }
            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "NotificationService - MessageConsumer: " + e.Message});
            }
        }
    }
    
}
