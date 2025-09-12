using AnimalesPerdidos.Data;
using AnimalesPerdidos.Dtos.Publicacion;
using AnimalesPerdidos.Model.Entities;
using AnimalesPerdidos.Model.Enums;
using AnimalesPerdidos.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnimalesPerdidos.Services.Implementations
{
    public class PublicacionService : IPublicacionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageStorage _imageStorage;
        private readonly ISpamGuard _spamGuard;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PublicacionService(ApplicationDbContext dbContext, IImageStorage imageStorage, ISpamGuard spamGuard, IHttpContextAccessor httpContextAccessor )
        {
            _dbContext = dbContext;
            _spamGuard = spamGuard;
            _httpContextAccessor = httpContextAccessor;
            _imageStorage = imageStorage;
        }

       
        public async Task<IEnumerable<Publicacion>> GetAllAsync(string? zona = null, Animal? especie = null, DateTime? desde = null, EstadoDePublicacion? estado = null)
        {
            var query = _dbContext.publicacion.AsQueryable();

            if (!string.IsNullOrEmpty(zona))
                query = query.Where(p => p.Zona.Contains(zona));

            if (especie != null)
                query = query.Where(p => p.Animal == especie);

            if (desde != null)
                query = query.Where(p => p.CreatedAt >= desde);

            if (estado != null)
                query = query.Where(p => p.Estado == estado);

            return await query.ToListAsync();
        }

        public async Task<Publicacion?> GetByIdAsync(Guid id)
        {
            return await _dbContext.publicacion.FindAsync(id);
        }

        public async Task<Publicacion> CreateAsync(PublicacionForCreationDto dto, Guid authorId)
        {
            var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";

            
             var isCaptchaValid = await _spamGuard.ValidateCaptchaAsync(dto.RecaptchaToken, ip, CancellationToken.None);
              if (!isCaptchaValid)
                throw new UnauthorizedAccessException("ReCAPTCHA inválido");

            // Lista para guardar URLs
            var imageUrls = new List<string>();

            // Subir todas las imágenes de golpe usando el método de AzureBlobImageStorage
            if (dto.Imagenes != null && dto.Imagenes.Any())
            {
                // Aquí usamos directamente el método que recibe IEnumerable<IFormFile>
                var urls = await _imageStorage.UploadAsync(dto.Imagenes, CancellationToken.None);
                imageUrls.AddRange(urls);
            }

            // Crear la publicación
            var publicacion = new Publicacion
            {
                Id = Guid.NewGuid(),
                AuthorId = authorId,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Animal = dto.Animal,
                Raza = dto.Raza ?? string.Empty,
                Zona = dto.Zona,
                Contacto = dto.Contacto,
                Estado = EstadoDePublicacion.Activa,
                CreatedAt = DateTime.UtcNow,
                ImagenUrls = imageUrls
            };

            _dbContext.publicacion.Add(publicacion);
            await _dbContext.SaveChangesAsync();

            return publicacion;
        }

        public async Task<bool> UpdateAsync(Publicacion publicacion)
        {
            var exists = await _dbContext.publicacion.AnyAsync(p => p.Id == publicacion.Id);
            if (!exists) return false;

            _dbContext.publicacion.Update(publicacion);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsResolvedAsync(Guid id)
        {
            var publicacion = await _dbContext.publicacion.FindAsync(id);
            if (publicacion == null) return false;

            publicacion.Estado = EstadoDePublicacion.Archivada;
            publicacion.ResolvedAt = DateTime.UtcNow;
            // Oculte contacto cuando esté resuelta
            publicacion.Contacto = null;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReportAsync(Guid id)
        {
            var publicacion = await _dbContext.publicacion.FindAsync(id);
            if (publicacion == null) return false;

            publicacion.ReportCount++;

            // Ejemplo lógica simple: ocultar contacto si hay >5 reportes
            if (publicacion.ReportCount > 5)
                publicacion.Contacto = null;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var publicacion = await _dbContext.publicacion.FindAsync(id);
            if (publicacion == null) return false;

            _dbContext.publicacion  .Remove(publicacion);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
