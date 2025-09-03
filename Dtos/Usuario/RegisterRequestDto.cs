using System.ComponentModel.DataAnnotations;

namespace AnimalesPerdidos.Dtos.Usuario
{
    public class RegisterRequestDto
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, MinLength(6)] public string Password { get; set; } = null!;
        [Required] public string UserName { get; set; } = null!;
    }
}
