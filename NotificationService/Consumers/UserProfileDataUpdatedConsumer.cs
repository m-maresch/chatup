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
    public class UserProfileDataUpdatedConsumer : IConsumer<UserProfileDataUpdatedEvent>
    {
        private IRepository<User> _userRepository;
        private IMapper _mapper;
        private IPublishEndpoint _publishEndpoint;

        public UserProfileDataUpdatedConsumer(IRepository<User> userRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UserProfileDataUpdatedEvent> context)
        {
            try
            {
                User user = _mapper.Map<UserProfileDataUpdatedEvent, User>(context.Message);

                if (user.PhoneNumber != null)
                {
                    await _userRepository.Update(user);
                    await _userRepository.Save();
                }
            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "NotificationService - UserProfileDataUpdatedConsumer: " + e.Message });
            }
        }
    }
}
