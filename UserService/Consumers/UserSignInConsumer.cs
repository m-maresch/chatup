using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Consumers
{
    public class UserSignInConsumer : IConsumer<UserSignInRequestDto>
    {
        private IRepository<User> _userRepository;
        private IPublishEndpoint _publishEndpoint;

        public UserSignInConsumer(IRepository<User> userRepository, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UserSignInRequestDto> context)
        {
            try
            {
                User user = _userRepository.Get().First(u => u.UserName == context.Message.UserName);

                if (user.UserName == context.Message.UserName && user.Password == context.Message.Password)
                {
                    await context.RespondAsync(new UserSignInResponseDto()
                    {
                        UserID = user.UserID,
                        Valid = true
                    });
                }
                else
                {
                    await context.RespondAsync(new UserSignInResponseDto()
                    {
                        UserID = 0,
                        Valid = false
                    });
                }
            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "UserService - UserSignInConsumer: " + e.Message });
            }
        }
    }
}
