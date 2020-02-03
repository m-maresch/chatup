using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Polly;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Consumers
{
    public class GetUserProfileDataConsumer : IConsumer<GetUserProfileDataRequestDto>
    {
        private IRepository<User> _userRepository;
        private IMapper _mapper;
        private IPublishEndpoint _publishEndpoint;
        private IRequestClient<GetUserTagsRequestDto, GetUserTagsResponseDto> _getUserTagsClient;
        private Policy _policy;

        public GetUserProfileDataConsumer(IRepository<User> userRepository, IMapper mapper, IPublishEndpoint publishEndpoint, IRequestClient<GetUserTagsRequestDto, GetUserTagsResponseDto> getUserTagsClient, Policy policy)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _getUserTagsClient = getUserTagsClient;
            _policy = policy;
        }

        public async Task Consume(ConsumeContext<GetUserProfileDataRequestDto> context)
        {
            try
            {
                User user = await _userRepository.GetById(context.Message.UserID);
                
                GetUserProfileDataResponseDto response = _mapper.Map<User, GetUserProfileDataResponseDto>(user);

                response.Tags = await _policy.ExecuteAsync(async () => (await _getUserTagsClient.Request(new GetUserTagsRequestDto() {UserID = context.Message.UserID})).Tags);
                
                await context.RespondAsync(response);
            }
            catch (Exception e)
            {
                await _publishEndpoint.Publish(new LogRequestDto() { LoggingTime = DateTime.Now, Message = "UserService - GetUserProfileDataConsumer: " + e.Message });
            }
        }
    }
}
