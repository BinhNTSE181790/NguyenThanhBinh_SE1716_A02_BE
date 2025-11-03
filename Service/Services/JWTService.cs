using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _config;

        public JWTService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(int id, string name, string email, int role)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var keyString = jwtSettings["Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT Key is missing in configuration.");

            var expireString = jwtSettings["ExpireMinutes"];
            if (string.IsNullOrEmpty(expireString))
                throw new InvalidOperationException("JWT ExpireMinutes is missing in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("AccountId", id.ToString()),
                new Claim("AccountName", name),
                new Claim("AccountEmail", email),
                new Claim("AccountRole", role.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()) // Thêm claim này để [Authorize(Roles)] hoạt động
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(expireString)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            // Refresh token nên random để tránh đoán được
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero // không cho trễ hạn
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null!; // hoặc throw exception tùy ý
            }
        }
    }
}
