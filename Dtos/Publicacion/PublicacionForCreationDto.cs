using System.ComponentModel.DataAnnotations;
using AnimalesPerdidos.Model.Enums;

namespace AnimalesPerdidos.Dtos.Publicacion
{
    public class PublicacionForCreationDto
    {
        [Required, MaxLength(140)] public string Titulo { get; set; } = null!;
        [Required, MaxLength(2000)] public string Descripcion { get; set; } = null!;
        [Required] public Animal Animal { get; set; }

        public string? Raza { get; set; } 
        [Required, MaxLength(140)] public string Zona { get; set; } = null!; // barrio/ciudad
        [MaxLength(200)] public string? Contacto { get; set; }
        // reCAPTCHA token del cliente
        [Required] public string RecaptchaToken { get; set; } = null!;

        public List<IFormFile>? Imagenes { get; set; }
    }
}
