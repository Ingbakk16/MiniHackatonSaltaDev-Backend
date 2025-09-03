using System.ComponentModel.DataAnnotations;

namespace AnimalesPerdidos.Dtos.Usuario
{
    public class LoginRequestDto
    {
        [Required] public string UserName { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
    }
}
