using AnimalesPerdidos.Model.Enums;

namespace AnimalesPerdidos.Dtos.Publicacion
{
    public class PublicationForUpdate
    {
        public EstadoDePublicacion Estado { get; set; } = EstadoDePublicacion.Activa;
    }
}
