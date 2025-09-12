using AnimalesPerdidos.Data;
using Microsoft.EntityFrameworkCore;

namespace AnimalesPerdidos.Utils
{
    public static class MigrationsUtil
    {
        public static void ApplyMigrations(this IHost app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
    }
}
