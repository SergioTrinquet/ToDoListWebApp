using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ToDoListWebApp.Data
{

    //// Version Originale ////
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Models.DomainModels.Task> Task { get; set; } = default!;
        public DbSet<Models.DomainModels.Priorite> Priorite { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.DomainModels.Task>()
                .HasOne(t => t.PrioriteNavigation)
                .WithMany()               // Priorite n'a pas de collection Task
                .HasForeignKey(t => t.Priorite);
        }
    }


    //// V2 ////
    //public class ApplicationDbContext : IdentityDbContext
    //{
    //    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //        : base(options)
    //    {
    //    }

    //    public DbSet<Models.DomainModels.Task> Task { get; set; } = default!;
    //    public DbSet<Models.DomainModels.Priorite> Priorite { get; set; } = default!;

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);

    //        modelBuilder.Entity<Models.DomainModels.Task>()
    //            .HasOne(t => t.PrioriteNavigation)
    //            .WithMany()               // Priorite n'a pas de collection Task
    //            .HasForeignKey(t => t.Priorite);
    //    }
    //}

}

