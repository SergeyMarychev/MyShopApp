using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyShopApp.Application.Authorization;
using MyShopApp.Application.Contracts.Authorization.Dto;
using MyShopApp.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyShopApp.WebApi.Authorization
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly UserManager<User> _userManager;

        public TokenGenerator(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<TokenDto> GenerateJwtTokenAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: new SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new TokenDto
            {
                Token = encodedJwt,
                ExpiresIn = AuthOptions.LIFETIME * 60
            };
        }
    }
}
