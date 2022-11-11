using Microsoft.AspNetCore.Identity;

namespace BehvarTestProject.Providers
{
    public class ResetPasswordTokenProvider : TotpSecurityStampBasedTokenProvider<IdentityUser>
    {
        public const string ProviderKey = "ResetPassword";

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(false);
        }
    }
}
