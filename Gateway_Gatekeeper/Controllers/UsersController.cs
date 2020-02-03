using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Gateway_Gatekeeper.Extensions;
using Gateway_Gatekeeper.Validator;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace Gateway_Gatekeeper.Controllers
{
    [Route("/[controller]")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetUserContactsRequestDto, GetUserContactsResponseDto> _getUserContactsClient;
        private readonly IRequestClient<GetUserProfileDataRequestDto, GetUserProfileDataResponseDto> _getUserProfileDataClient;
        private readonly IRequestClient<RegisterUserRequestDto, RegisterUserResponseDto> _registerUserClient;
        private readonly IRequestClient<VerifyRegisterUserRequestDto, VerifyRegisterUserResponseDto> _verifyRegisterUserClient;
        private readonly IRequestClient<UserSignInRequestDto, UserSignInResponseDto> _userSignInClient;
        private readonly IRequestClient<UpdateUserProfileDataRequestDto, UpdateUserProfileDataResponseDto> _updateUserProfileDataClient;
        private readonly Policy _policy;

        public UsersController(IPublishEndpoint publishEndpoint, 
            IRequestClient<GetUserContactsRequestDto, GetUserContactsResponseDto> getUserContactsClient,
            IRequestClient<GetUserProfileDataRequestDto, GetUserProfileDataResponseDto> getUserProfileDataClient,
            IRequestClient<RegisterUserRequestDto, RegisterUserResponseDto> registerUserClient,
            IRequestClient<VerifyRegisterUserRequestDto, VerifyRegisterUserResponseDto> verifyRegisterUserClient,
            IRequestClient<UserSignInRequestDto, UserSignInResponseDto> userSignInClient,
            IRequestClient<UpdateUserProfileDataRequestDto, UpdateUserProfileDataResponseDto> updateUserProfileDataClient,
            Policy policy)
        {
            this._publishEndpoint = publishEndpoint;
            this._getUserContactsClient = getUserContactsClient;
            this._getUserProfileDataClient = getUserProfileDataClient;
            this._registerUserClient = registerUserClient;
            this._verifyRegisterUserClient = verifyRegisterUserClient;
            this._userSignInClient = userSignInClient;
            this._updateUserProfileDataClient = updateUserProfileDataClient;
            this._policy = policy;
        }

        [HttpGet("{id}/contacts")]
        public async Task<IActionResult> GetUserContacts(long id)
        {
            GetUserContactsRequestDto message = new GetUserContactsRequestDto()
            {
                UserID = id.AsInt()
            };

            if (await message.IsValid())
            {
                var response = await _policy.ExecuteAsync(async () => await _getUserContactsClient.Request(message));
                if (response != null)
                {
                    return Ok(response);
                }
            }
            return BadRequest();
        }

        [HttpPost("{id}/contacts")]
        public async Task<IActionResult> SetUserContacts([FromBody] SetUserContactsRequestDto message, long id)
        {
            message.UserID = id.AsInt();

            if (await message.PublishWhenValid(_publishEndpoint))
            {
                return Ok();
            }
            return BadRequest();
        }


        [HttpGet("{id}/profileData")]
        public async Task<IActionResult> GetUserProfileData(long id)
        {
            GetUserProfileDataRequestDto message = new GetUserProfileDataRequestDto()
            {
                UserID = id.AsInt()
            };

            if (await message.IsValid())
            {
                var response = await _policy.ExecuteAsync(async () => await _getUserProfileDataClient.Request(message));
                if (response != null)
                {
                    return Ok(response);
                }
            }
            return BadRequest();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequestDto message)
        { 
            if (await message.IsValid())
            {
                var response = await _policy.ExecuteAsync(async () => await _registerUserClient.Request(message));
                if (response != null)
                {
                    return Ok(response);
                }
            }
            return BadRequest();
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyRegisterUser([FromBody] VerifyRegisterUserRequestDto message)
        {
            if (await message.IsValid())
            {
                var response = await _policy.ExecuteAsync(async () => await _verifyRegisterUserClient.Request(message));
                if (response != null)
                {
                    return Ok(response);
                }
            }
            return BadRequest();
        }

        [HttpPost("signIn")]
        [AllowAnonymous]
        public async Task<IActionResult> UserSignIn([FromBody] UserSignInRequestDto message)
        { 
            if (await message.IsValid())
            {
                var response = await _policy.ExecuteAsync(async () => await _userSignInClient.Request(message));
                if (response != null)
                {
                    if (response.Valid)
                    {
                        response.ClientId = Config.ClientId;
                        response.Scope = Config.AppAccessScope;
                        response.ClientSecret = Config.ClientSecret;
                        response.UriAuthorizationServer = Startup.Configuration["url"] + "/connect/token";
                    }
                    return Ok(response);
                }
            }
            return BadRequest();
        }

        [HttpPost("{id}/profileData")]
        public async Task<IActionResult> UpdateUserProfileData([FromBody] UpdateUserProfileDataRequestDto message, long id)
        {
            message.UserID = id.AsInt();

            if (await message.IsValid())
            {
                var response = await _policy.ExecuteAsync(async () => await _updateUserProfileDataClient.Request(message));
                if (response != null)
                {
                    return Ok(response);
                }
            }
            return BadRequest();
        }

        [HttpPost("{id}/settings")]
        public async Task<IActionResult> ChangeUserSettings([FromBody] ChangeUserSettingsRequestDto message, long id)
        {
            message.UserID = id.AsInt();

            if (await message.PublishWhenValid(_publishEndpoint))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
