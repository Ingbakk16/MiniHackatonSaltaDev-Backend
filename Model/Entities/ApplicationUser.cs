using Microsoft.AspNetCore.Identity;

namespace AnimalesPerdidos.Model.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
