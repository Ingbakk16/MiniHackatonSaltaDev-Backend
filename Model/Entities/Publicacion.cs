using System.ComponentModel.DataAnnotations;
using AnimalesPerdidos.Model.Enums;

namespace AnimalesPerdidos.Model.Entities
{
    public class Publicacion
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }


        [MaxLength(140)]
        public string Titulo { get; set; } = null!;


        [MaxLength(2000)]
        public string Descripcion { get; set; } = null!;


        public Animal Animal { get; set; }

        public string Raza { get; set; } = null!;   


        // Zona aproximada (barrio/ciudad). No guardar domicilio exacto
        [MaxLength(140)]
        public string Zona { get; set; } = null!;


        // campo opcional que se oculta si está Resolved o si hay muchos reportes
        [MaxLength(200)]
        public string? Contacto { get; set; }


        public List<string> ImagenUrls { get; set; } = new();


        public EstadoDePublicacion Estado { get; set; } = EstadoDePublicacion.Activa;
        public int ReportCount { get; set; } = 0;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
    }
}
