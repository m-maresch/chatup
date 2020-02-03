using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Consumers
{
    public class RegisterUserConsumer : IConsumer<RegisterUserRequestDto>
    {
        private IRepository<User> _userRepository;
        private IPublishEndpoint _publishEndpoint;
        private IMapper _mapper;

        public RegisterUserConsumer(IRepository<User> userRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<RegisterUserRequestDto> context)
        {
            try
            {
                if (await _userRepository.Get()
                    .Select(u => u.EMail)
                    .ContainsAsync(context.Message.Email))
                {
                    await context.RespondAsync(new RegisterUserResponseDto()
                    {
                        Error = "CU-120",
                        Valid = false
                    });

                    return;
                }

                if (await _userRepository.Get()
                    .Select(u => u.PhoneNumber)
                    .ContainsAsync(context.Message.PhoneNumber))
                {
                    await context.RespondAsync(new RegisterUserResponseDto()
                    {
                        Error = "CU-130",
                        Valid = false
                    });

                    return;
                }

                if (await _userRepository.Get()
                    .Select(u => u.UserName)
                    .ContainsAsync(context.Message.UserName))
                {
                    await context.RespondAsync(new RegisterUserResponseDto()
                    {
                        Error = "CU-110",
                        Valid = false
                    });

                    return;
                }

                if (context.Message.Password == null)
                {
                    await context.RespondAsync(new RegisterUserResponseDto()
                    {
                        Error = "CU-140",
                        Valid = false
                    });

                    return;
                }
                
                await RedisStore.Cache.StringSetAsync(context.Message.Email, _mapper.Map<RegisterUserRequestDto, User>(context.Message).ToJson());

                await _publishEndpoint.Publish(new SendMailCommand() { Email = context.Message.Email });

                await context.RespondAsync(new RegisterUserResponseDto()
                {
                    Error = null,
                    Valid = true
                });
            }
            catch (Exception e)
            {
                await context.RespondAsync(new RegisterUserResponseDto()
                {
                    Error = "CU-100",
                    Valid = false
                });

                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "UserService - RegisterUserConsumer: " + e.Message });
            }
        }
    }
}
