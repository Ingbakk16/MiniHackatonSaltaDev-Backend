using AnimalesPerdidos.Dtos.Usuario;

namespace AnimalesPerdidos.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto model);
    }
}
