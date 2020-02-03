using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Consumers
{
    public class GetUserContactsConsumer : IConsumer<GetUserContactsRequestDto>
    {
        private IRepository<User> _userRepository;
        private IMapper _mapper;
        private IPublishEndpoint _publishEndpoint;

        public GetUserContactsConsumer(IRepository<User> userRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<GetUserContactsRequestDto> context)
        {
            try
            {
                User user = _userRepository.Get()
                    .Include(u => u.Contacts)
                    .First(u => u.UserID == context.Message.UserID);

                List<User> contacts = new List<User>();

                foreach (var userContact in user.Contacts)
                {
                    contacts.Add(await _userRepository.GetById(userContact.ContactID));
                }
                
                await context.RespondAsync(new GetUserContactsResponseDto()
                {
                    Users = _mapper.Map<List<User>, List<GetUserContactsUserDto>>(contacts)
                });
            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "UserService - GetUserContactsConsumer: " + e.Message });
            }
        }
    }
}
