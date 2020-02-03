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
    public class UpdateUserProfileDataConsumer : IConsumer<UpdateUserProfileDataRequestDto>
    {
        private IRepository<User> _userRepository;
        private IMapper _mapper;
        private IRequestClient<GetUserTagsRequestDto, GetUserTagsResponseDto> _getUserTagsClient;
        private IPublishEndpoint _publishEndpoint;

        public UpdateUserProfileDataConsumer(IRepository<User> userRepository, IMapper mapper, IRequestClient<GetUserTagsRequestDto, GetUserTagsResponseDto> getUserTagsClient, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _getUserTagsClient = getUserTagsClient;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UpdateUserProfileDataRequestDto> context)
        {
            try
            {
                bool changed = false;

                if (await _userRepository.Get()
                    .Select(u => u.EMail)
                    .ContainsAsync(context.Message.NewEmail))
                {
                    await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                    {
                        Valid = false
                    });

                    return;
                }

                if (await _userRepository.Get()
                    .Select(u => u.PhoneNumber)
                    .ContainsAsync(context.Message.NewPhoneNumber))
                {
                    await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                    {
                        Valid = false
                    });

                    return;
                }
                
                User user = await _userRepository.GetById(context.Message.UserID);

                if (context.Message.OldEmail == user.EMail)
                {
                    if (context.Message.NewEmail == null)
                    {
                        await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                        {
                            Valid = false
                        });

                        return;
                    }

                    user.EMail = context.Message.NewEmail;

                    changed = true;
                }
                else
                {
                    if (context.Message.OldEmail != null)
                    {
                        await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                        {
                            Valid = false
                        });

                        return;
                    }
                }

                if (context.Message.OldPhoneNumber == user.PhoneNumber)
                {
                    if (context.Message.NewPhoneNumber == null)
                    {
                        await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                        {
                            Valid = false
                        });

                        return;
                    }

                    user.PhoneNumber = context.Message.NewPhoneNumber;

                    changed = true;
                }
                else
                {
                    if(context.Message.OldPhoneNumber != null)
                    {
                        await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                        {
                            Valid = false
                        });

                        return;
                    }
                }

                if (context.Message.OldBiography == user.Biography)
                {
                    user.Biography = context.Message.NewBiography;

                    changed = true;
                }
                else
                {
                    if(context.Message.OldBiography != null)
                    {
                        await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                        {
                            Valid = false
                        });

                        return;
                    }
                }

                if (context.Message.OldPassword == user.Password)
                {
                    if (context.Message.NewPassword == null)
                    {
                        await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                        {
                            Valid = false
                        });

                        return;
                    }

                    user.Password = context.Message.NewPassword;

                    changed = true;
                }
                else
                {
                    if(context.Message.OldPassword != null)
                    {
                        await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                        {
                            Valid = false
                        });

                        return;
                    }
                }

                if (context.Message.NewTags != null)
                {
                    List<TagDto> oldTags = (await _getUserTagsClient.Request(new GetUserTagsRequestDto() { UserID = context.Message.UserID})).Tags;

                    if (context.Message.OldTags.Any())
                    {
                        foreach (var tag in context.Message.OldTags)
                        {
                            if (!oldTags.Select(t => t.Tag).Contains(tag.Tag))
                            {
                                await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                                {
                                    Valid = false
                                });

                                return;
                            }
                        }
                    }
                    else
                    {
                        if (oldTags.Any())
                        {
                            await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                            {
                                Valid = false
                            });

                            return;
                        }
                    }

                    await _publishEndpoint.Publish(new UpdateUserTagsCommand()
                    {
                        UserID = user.UserID,
                        Tags = context.Message.NewTags
                    });

                    changed = true;
                } 

                await _userRepository.Update(user);

                await _userRepository.Save();

                await context.RespondAsync(new UpdateUserProfileDataResponseDto()
                {
                    Valid = changed
                });

                await _publishEndpoint.Publish(_mapper.Map<User, UserProfileDataUpdatedEvent>(user));
            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "UserService - UpdateUserProfileDataConsumer: " + e.Message });
            }
        }
    }
}
