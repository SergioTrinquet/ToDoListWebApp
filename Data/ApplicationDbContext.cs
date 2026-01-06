using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ToDoListWebApp.Data
{
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

            // Lors de la génération de la bdd, qd création d'une tâche, champ 'UserId' est obligatoire même si pas décoré avec [Required] et nullable dans le model TAsk
            modelBuilder.Entity<Models.DomainModels.Task>()
                .Property(t => t.UserId)
                .IsRequired();

            // Pas obligatoire mais 
            // bonne pratique (=> évite des comportements de suppression non maîtrisé, évite risque de Restrict par défaut, des migrations ambiguës, des warnings EF Core)
            // et permet de supprimer ttes les tasks d'un User quand celui-ci est supprimé 
            modelBuilder.Entity<Models.DomainModels.Task>()
                .HasOne(t => t.User) // Si je veux récupérer ttes les données d'un utilisateur
                //.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

