using AnimalesPerdidos.Dtos.Usuario;
using AnimalesPerdidos.Model.Entities;
using AnimalesPerdidos.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AnimalesPerdidos.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterRequestDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exists" });
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = new[] { "Invalid credentials" }
                };
            }

            var token = await GenerateJwtToken(user);
            return new AuthResponseDto { Success = true, Token = token };
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings"); 
            var key = jwtSettings["SecretKey"];

            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("JWT Key is missing from configuration.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
         new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
         new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // Agregar roles
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpireMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

