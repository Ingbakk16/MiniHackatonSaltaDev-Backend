using AnimalesPerdidos.Model.Enums;

namespace AnimalesPerdidos.Dtos.Publicacion
{
    public class PublicacionForDisplayDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public Animal Animal { get; set; }
        public string Raza { get; set; } = null!;
        public string Zona { get; set; } = null!;
        public string? Contacto { get; set; }
        public List<string> ImagenUrls { get; set; } = new();
        public EstadoDePublicacion Estado { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
