using AnimalesPerdidos.Services.Interfaces;
using Azure.Storage.Blobs;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using AnimalesPerdidos.Dtos.Publicacion;
using AnimalesPerdidos.Model.Entities;
using AnimalesPerdidos.Model.Enums;
using Microsoft.EntityFrameworkCore;
using AnimalesPerdidos.Data;

namespace AnimalesPerdidos.Services.Implementations
{
    public class AzureBlobImageStorage : IImageStorage
    {
        private readonly BlobContainerClient _container;
        private readonly ApplicationDbContext _context;


        public AzureBlobImageStorage(BlobServiceClient svc, ApplicationDbContext context)
        {
            _container = svc.GetBlobContainerClient("images");
            _container.CreateIfNotExists();
            _context = context;
        }


        public async Task<List<string>> UploadAsync(IEnumerable<IFormFile> files, CancellationToken ct)
        {
            var urls = new List<string>();
            foreach (var file in files)
            {
                if (file.Length == 0) continue;


                // Re-encode para eliminar metadatos EXIF + reducir peso
                using var image = await Image.LoadAsync(file.OpenReadStream(), ct);
                // Redimensionado suave (máx 1600px lado mayor)
                var max = Math.Max(image.Width, image.Height);
                if (max > 1600)
                {
                    var ratio = 1600f / max;
                    image.Mutate(x => x.Resize((int)(image.Width * ratio), (int)(image.Height * ratio)));
                }


                var blobName = $"{Guid.NewGuid():N}.jpg";
                var blob = _container.GetBlobClient(blobName);


                await using var ms = new MemoryStream();
                await image.SaveAsJpegAsync(ms, new JpegEncoder { Quality = 80 }, ct);
                ms.Position = 0;
                await blob.UploadAsync(ms, overwrite: true, cancellationToken: ct);
                urls.Add(blob.Uri.ToString());
            }
            return urls;
        }


        public async Task<Publicacion> CreateAsync(PublicacionForCreationDto dto, Guid authorId, List<string> imagenUrls)
        {
            var publicacion = new Publicacion
            {
                Id = Guid.NewGuid(),
                AuthorId = authorId,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Animal = dto.Animal,
                Raza = dto.Raza ?? "",
                Zona = dto.Zona,
                Contacto = dto.Contacto,
                ImagenUrls = imagenUrls,
                Estado = EstadoDePublicacion.Activa,
                CreatedAt = DateTime.UtcNow
            };

            _context.publicacion.Add(publicacion);
            await _context.SaveChangesAsync();

            return publicacion;
        }
    }
    
}
