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
    public class SetUserContactsConsumer : IConsumer<SetUserContactsRequestDto>
    {
        private IRepository<User> _userRepository;
        private IPublishEndpoint _publishEndpoint;

        public SetUserContactsConsumer(IRepository<User> userRepository, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<SetUserContactsRequestDto> context)
        {
            try
            {
                User user = _userRepository.Get()
                    .Include(u => u.Contacts)
                    .First(u => u.UserID == context.Message.UserID);
                
                foreach (var dto in context.Message.PhoneNumbers)
                {
                    User contact = _userRepository.Get().First(u => u.PhoneNumber == dto.PhoneNumber);

                    if (contact != null)
                    {
                        user.Contacts.Add(new UserContact()
                        {
                            ContactID = contact.UserID,
                            User = user,
                            UserID = user.UserID
                        });
                    }
                }

                await _userRepository.Save();

                await _publishEndpoint.Publish(new UserContactsUpdatedEvent()
                {
                    UserID = user.UserID,
                    ContactIds = user.Contacts.Select(u => u.ContactID).ToList()
                });

            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "UserService - SetUserContactsConsumer: " + e.Message });
            }
        }
    }
}
