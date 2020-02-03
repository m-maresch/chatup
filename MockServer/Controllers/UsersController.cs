using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MockServer.Controllers
{
    [Route("/[controller]")]
    //[Authorize]
    public class UsersController : Controller
    {
        [HttpGet("{id}/contacts")]
        public IActionResult GetUserContacts(long id)
        {
            return Ok(new GetUserContactsResponseDto()
            {
                Users = new List<GetUserContactsUserDto>()
                {
                    new GetUserContactsUserDto()
                    {
                        UserID = 10,
                        UserName = "kauper",
                        PhoneNumber = "436604965843"
                    },
                    new GetUserContactsUserDto()
                    {
                        UserID = 11,
                        UserName = "Paulmex",
                        PhoneNumber = "436604965843"
                    }
                }
            });
        }

        [HttpPost("{id}/contacts")]
        public IActionResult SetUserContacts([FromBody] SetUserContactsRequestDto message, long id)
        {
            return Ok();
        }


        [HttpGet("{id}/profileData")]
        public IActionResult GetUserProfileData(long id)
        {
            return Ok(new GetUserProfileDataResponseDto()
            {
                UserName = "kauper",
                Email = "alex@htlkrems.at",
                Tags = new List<TagDto>()
                {
                    new TagDto() { Tag = "Photography" },
                    new TagDto() { Tag = "Developer" }
                },
                Biography = "Developer and Photographer"
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult RegisterUser([FromBody] RegisterUserRequestDto message)
        {
            return Ok(new RegisterUserResponseDto()
            {
                Valid = false,
                Error = "CU-110"
            });
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        public IActionResult VerifyRegisterUser([FromBody] VerifyRegisterUserRequestDto message)
        {
            return Ok(new VerifyRegisterUserResponseDto()
            {
                Valid = true
            });
        }

        [HttpPost("signIn")]
        [AllowAnonymous]
        public IActionResult UserSignIn([FromBody] UserSignInRequestDto message)
        {
            return Ok(new UserSignInResponseDto()
            {
                ClientId = Config.ClientId,
                Scope = Config.AppAccessScope,
                ClientSecret = Config.ClientSecret,
                UriAuthorizationServer = "http://localhost:5010/connect/token",
                Valid = false,
                UserID = 11
            });
        }

        [HttpPost("{id}/profileData")]
        public IActionResult UpdateUserProfileData([FromBody] UpdateUserProfileDataRequestDto message, long id)
        {
            return Ok(new UpdateUserProfileDataResponseDto()
            {
                Valid = false
            });
        }

        [HttpPost("{id}/settings")]
        public IActionResult ChangeUserSettings([FromBody] ChangeUserSettingsRequestDto message, long id)
        {
            return Ok();
        }
    }
}
