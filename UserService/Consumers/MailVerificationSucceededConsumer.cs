using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Extensions;
using MassTransit;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Consumers
{
    public class MailVerificationSucceededConsumer : IConsumer<MailVerificationSucceededEvent>
    {
        private IRepository<User> _userRepository;
        private IMapper _mapper;
        private IPublishEndpoint _publishEndpoint;

        public MailVerificationSucceededConsumer(IRepository<User> userRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<MailVerificationSucceededEvent> context)
        {
            try
            {
                User target = (await RedisStore.Cache.StringGetAsync(context.Message.Email))
                    .ToString()
                    .ToObject<User>();
                
                await _userRepository.Insert(target);

                await RedisStore.Cache.KeyDeleteAsync(target.EMail);

                await _userRepository.Save();

                await _publishEndpoint.Publish(_mapper.Map<User, UserCreatedEvent>(_userRepository.Get().First(u => u.EMail == context.Message.Email)));
            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "UserService - MailVerificationSucceededConsumer: " + e.Message });
            }
        }
    }
}
