using AnimalesPerdidos.Dtos.Usuario;
using Microsoft.AspNetCore.Identity;

namespace AnimalesPerdidos.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequestDto model);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto model);
    }
}
