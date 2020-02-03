using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MassTransit;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private IRepository<User> _userRepository;
        private IMapper _mapper;
        private IPublishEndpoint _publishEndpoint;

        public UserCreatedConsumer(IRepository<User> userRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            try
            {
                User user = _mapper.Map<UserCreatedEvent, User>(context.Message);

                await _userRepository.Insert(user);
                await _userRepository.Save();
            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "NotificationService - UserCreatedConsumer: " + e.Message });
            }
        }
    }
}
