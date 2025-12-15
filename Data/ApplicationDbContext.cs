using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ToDoListWebApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Models.DomainModels.Task> Task { get; set; } = default!;

        public DbSet<Models.DomainModels.Priorite> Priorite { get; set; }
    }
}
