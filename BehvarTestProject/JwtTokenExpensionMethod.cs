using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BehvarTestProject
{
    /// <summary>
    /// Extensions method for working with jwt token extensions method 
    /// </summary>
    public static class JwtTokenExpensionMethod
    {
        /// <summary>
        /// Generate Jwt bearer contains the user username
        /// </summary>
        /// <param name="user">The user details</param>
        /// <returns></returns>
        public static string GenerateJwtToken(this IdentityUser user)
        {
            // Set up our token claims
            var claims = new[]
            {
                // Unique id for this token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

                // The User name using Identity name so it fills out the HttpContext.User.Identity.Name value
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),

                // Add user id so the UsaerManger.GetUserAsync can get find the user based on id 
                new Claim(ClaimTypes.NameIdentifier, user.Id),

                // Add user email for SignalR chatHub identifier 
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Create the credentials used to generate token
            var credential = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key")),
                SecurityAlgorithms.HmacSha256);

            // Generate JWT Token 
            var token = new JwtSecurityToken(
                issuer: "Me",
                audience: "You",
                claims: claims,
                expires: DateTime.Now.AddMonths(3),
                signingCredentials: credential
                );

            // Return the generated token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
