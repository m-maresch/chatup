using System;
using System.Threading.Tasks;
using Common;
using FluentAssertions;
using Gateway_Gatekeeper.Validator;
using Xunit;

namespace Gateway_Gatekeeper.Tests
{
    public class ValidatorTests
    {
        [Fact]
        public async Task DataValidatorTest()
        {
            DataValidator<GetUserContactsRequestDto> validator = new DataValidator<GetUserContactsRequestDto>();

            GetUserContactsRequestDto message = new GetUserContactsRequestDto()
            {
                UserID = -2
            };

            bool result = await validator.IsValid(message);
            result.Should().BeFalse();

            message.UserID = 2;

            result = await validator.IsValid(message);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task SQLInjectionValidatorTest()
        {
            SQLInjectionValidator<RegisterUserRequestDto> validator = new SQLInjectionValidator<RegisterUserRequestDto>();

            RegisterUserRequestDto message = new RegisterUserRequestDto()
            {
                UserName = null,
                Email = ";DROP TABLE test;",
                Password = null,
                PhoneNumber = null
            };

            bool result = await validator.IsValid(message);
            result.Should().BeFalse();

            message.Email = "test.user@chatup.com";

            result = await validator.IsValid(message);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task MultiValidatorTest()
        {
            MultiValidator.Validators.AddRange(new[]
            {
                typeof(DataValidator<>),
                typeof(SQLInjectionValidator<>)
            });

            UpdateUserProfileDataRequestDto message = new UpdateUserProfileDataRequestDto()
            {
                NewBiography = null,
                NewEmail = ";DROP TABLE test;",
                NewPassword = null,
                NewPhoneNumber = null,
                OldBiography = null,
                OldEmail = "test.olduser@chatup.com",
                OldPassword = null,
                OldPhoneNumber = null,
                UserID = -2
            };

            bool result = await MultiValidator<UpdateUserProfileDataRequestDto>.IsValid(message);
            result.Should().BeFalse();

            message.NewEmail = "test.newuser@chatup.com";

            result = await MultiValidator<UpdateUserProfileDataRequestDto>.IsValid(message);
            result.Should().BeFalse();

            message.NewEmail = ";DROP TABLE test;";
            message.UserID = 2;

            result = await MultiValidator<UpdateUserProfileDataRequestDto>.IsValid(message);
            result.Should().BeFalse();

            message.NewEmail = "test.newuser@chatup.com";

            result = await MultiValidator<UpdateUserProfileDataRequestDto>.IsValid(message);
            result.Should().BeTrue();
        }
    }
}
