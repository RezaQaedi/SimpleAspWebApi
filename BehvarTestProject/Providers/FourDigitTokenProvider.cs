using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace BehvarTestProject.Providers
{
    public class FourDigitTokenProvider : PhoneNumberTokenProvider<IdentityUser>
    {
        public const string ProviderKey = "4ResetPassword";

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(false);
        }

        public override async Task<string> GenerateAsync(string purpose, UserManager<IdentityUser> manager, IdentityUser user)
        {
            var token = new SecurityToken(await manager.CreateSecurityTokenAsync(user));
            var modifier = await GetUserModifierAsync(purpose, manager, user);
            var code = Rfc6238AuthenticationService.GenerateCode(token, modifier, 6).ToString("D6", CultureInfo.InvariantCulture);

            return code;
        }
        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<IdentityUser> manager, IdentityUser user)
        {
            if (!int.TryParse(token, out int code))
            {
                return false;
            }
            var securityToken = new SecurityToken(await manager.CreateSecurityTokenAsync(user));
            var modifier = await GetUserModifierAsync(purpose, manager, user);
            var valid = Rfc6238AuthenticationService.ValidateCode(securityToken, code, modifier, token.Length);
            return valid;
        }
        public override Task<string> GetUserModifierAsync(string purpose, UserManager<IdentityUser> manager, IdentityUser user)
        {
            return base.GetUserModifierAsync(purpose, manager, user);
        }
    }
}
