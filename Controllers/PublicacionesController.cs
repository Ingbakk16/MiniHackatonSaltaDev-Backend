using AnimalesPerdidos.Dtos.Publicacion;
using AnimalesPerdidos.Model.Entities;
using AnimalesPerdidos.Model.Enums;
using AnimalesPerdidos.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AnimalesPerdidos.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PublicacionesController : ControllerBase
    {
        private readonly IPublicacionService _publicacionService;
        private readonly IImageStorage _imageStorage;

        public PublicacionesController(IPublicacionService publicacionService, IImageStorage imageStorage)
        {
            _publicacionService = publicacionService;
            _imageStorage = imageStorage;
        }

        // GET: api/publicaciones?zona=&especie=&desde=&estado=
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? zona, [FromQuery] Animal? especie,
            [FromQuery] DateTime? desde, [FromQuery] EstadoDePublicacion? estado)
        {
            var publicaciones = await _publicacionService.GetAllAsync(zona, especie, desde, estado);
            return Ok(publicaciones);
        }

        // GET: api/publicaciones/{id}
        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var publicacion = await _publicacionService.GetByIdAsync(id);
            if (publicacion == null) return NotFound();

            var displayDto = new PublicacionForDisplayDto
            {
                Id = publicacion.Id,
                Titulo = publicacion.Titulo,
                Descripcion = publicacion.Descripcion,
                Animal = publicacion.Animal,
                Raza = publicacion.Raza,
                Zona = publicacion.Zona,
                Contacto = publicacion.Contacto,
                ImagenUrls = publicacion.ImagenUrls,
                Estado = publicacion.Estado,
                CreatedAt = publicacion.CreatedAt
            };

            return Ok(displayDto);
        }

        // POST: api/publicaciones
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] PublicacionForCreationDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener usuario logueado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid authorId))
                return Unauthorized();

            try
            {
                // 1️⃣ Subir imágenes a Azure si existen
                List<string> imagenUrls = new();
                if (dto.Imagenes != null && dto.Imagenes.Any())
                {
                    imagenUrls = await _imageStorage.UploadAsync(dto.Imagenes, ct);
                }

                // 2️⃣ Crear la publicación pasando también las URLs
                var publicacion = await _publicacionService.CreateAsync(dto, authorId);

                // 3️⃣ Mapear a DTO de respuesta
                var displayDto = new PublicacionForDisplayDto
                {
                    Id = publicacion.Id,
                    Titulo = publicacion.Titulo,
                    Descripcion = publicacion.Descripcion,
                    Animal = publicacion.Animal,
                    Raza = publicacion.Raza,
                    Zona = publicacion.Zona,
                    Contacto = publicacion.Contacto,
                    ImagenUrls = publicacion.ImagenUrls,
                    Estado = publicacion.Estado,
                    CreatedAt = publicacion.CreatedAt
                };

                return CreatedAtAction(nameof(GetById), new { id = displayDto.Id }, displayDto);
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest("Token de reCAPTCHA inválido.");
            }
        }

        // PUT: api/publicaciones/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Publicacion publicacion)
        {
            if (id != publicacion.Id)
                return BadRequest("ID no coincide");

            var updated = await _publicacionService.UpdateAsync(publicacion);
            if (!updated) return NotFound();

            return NoContent();
        }

        // POST: api/publicaciones/{id}/resolve
        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> MarkAsResolved(Guid id)
        {
            var result = await _publicacionService.MarkAsResolvedAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // POST: api/publicaciones/{id}/report
        [HttpPost("{id}/report")]
        public async Task<IActionResult> Report(Guid id)
        {
            var result = await _publicacionService.ReportAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // DELETE: api/publicaciones/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _publicacionService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
