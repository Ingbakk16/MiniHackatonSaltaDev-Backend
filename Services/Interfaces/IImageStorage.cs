using AnimalesPerdidos.Dtos.Publicacion;
using AnimalesPerdidos.Model.Entities;

namespace AnimalesPerdidos.Services.Interfaces
{
    public interface IImageStorage
    {
        Task<List<string>> UploadAsync(IEnumerable<IFormFile> files, CancellationToken ct);

        Task<Publicacion> CreateAsync(PublicacionForCreationDto dto, Guid authorId, List<string> imagenUrls);
    }
}
