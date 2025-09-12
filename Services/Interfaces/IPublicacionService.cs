using AnimalesPerdidos.Dtos.Publicacion;
using AnimalesPerdidos.Model.Entities;
using AnimalesPerdidos.Model.Enums;

namespace AnimalesPerdidos.Services.Interfaces
{
    public interface IPublicacionService
    {
        Task<IEnumerable<Publicacion>> GetAllAsync(string? zona = null, Animal? especie = null, DateTime? desde = null, EstadoDePublicacion? estado = null);
        Task<Publicacion?> GetByIdAsync(Guid id);
        Task<Publicacion> CreateAsync(PublicacionForCreationDto publicacion, Guid authorId);
        Task<bool> UpdateAsync(Publicacion publicacion);
        Task<bool> MarkAsResolvedAsync(Guid id);
        Task<bool> ReportAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
