using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoListWebApp.Data;
using ToDoListWebApp.Models;
using Microsoft.Extensions.Configuration;
using TaskEntity = ToDoListWebApp.Models.DomainModels.Task;
using ToDoListWebApp.Models.DomainModels;
public static class DbSeeder
{
    public static async System.Threading.Tasks.Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // USER DEMO
        var demoEmail = configuration["DemoUser:Email"];
        var demoPassword = configuration["DemoUser:Password"];

        if (string.IsNullOrWhiteSpace(demoEmail) || string.IsNullOrWhiteSpace(demoPassword))
        {
            // Pas de seed si pas de mot de passe
            logger.LogWarning("Seed STOP : Email ou Password manquant");
            return;
        }

        // Création données pour table PRIORITE
        if (!context.Priorite.Any())
        {
            context.Priorite.AddRange(
                new Priorite { Libelle = "basse" },
                new Priorite { Libelle = "moyenne" },
                new Priorite { Libelle = "haute" }
            );

            await context.SaveChangesAsync();
        }

        var demoUser = await userManager.FindByEmailAsync(demoEmail);

        if (demoUser == null)
        {
            demoUser = new IdentityUser
            {
                UserName = demoEmail,
                Email = demoEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(demoUser, demoPassword);

            if (!result.Succeeded)
            {
                logger.LogError("Seed STOP : Création user échouée : {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }
        }

        // Création données pour TASKS DEMO
        if (!context.Task.Any(t => t.UserId == demoUser.Id))
        {
            // Check avant d'alimenter prop. int? 'Propriete' dans ToDoTask, 
            // qu'il y a des données dans la table 'Priorite', sinon lève un exception SQL
            var prioriteParDefaut = await context.Priorite
                                            .OrderBy(p => p.Id)
                                            .Select(p => p.Id)
                                            .FirstOrDefaultAsync();
            if (prioriteParDefaut == 0)
            {
                logger.LogWarning("Etape du Seed : La table 'Priorite' n'est pas rempli => Cela est obligatoire en prod. pour alimenter le champ 'Priorite' des enregistrements du User Démo");
                // Pas de priorité → on ne seed pas les tâches
                return;
            }

            context.Task.AddRange(
                new TaskEntity
                {
                    Nom = "Aller à la piscine",
                    Description = "Piscine du 11eme arr. de Paris, moins de monde à midi",
                    Statut = false,
                    DateCreation = DateTime.UtcNow.AddDays(-6),
                    Priorite = 1,
                    Categorie = "Sport",
                    UserId = demoUser.Id
                },
                new TaskEntity
                {
                    Nom = "Acheter étagère IKEA",
                    Description = "A monter le weekend prochain !",
                    Statut = false,
                    DateCreation = DateTime.UtcNow.AddDays(-5),
                    DerniereModification = DateTime.UtcNow.AddHours(-32).AddSeconds(300),
                    Priorite = 3,
                    UserId = demoUser.Id
                },
                new TaskEntity
                {
                    Nom = "Faire les courses",
                    Description = "Faire les courses en ligne et demander à être livré après 18h car pas disponible avant (aller chercher les enfants à l'école à 17h).",
                    Statut = false,
                    DateCreation = DateTime.UtcNow.AddHours(-48),
                    DerniereModification = DateTime.UtcNow.AddMinutes(-252),
                    Priorite = 2,
                    DateLimite = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                    Categorie = "Courses",
                    UserId = demoUser.Id
                },
                new TaskEntity
                {
                    Nom = "Rideaux à raccourcir",
                    Description = "",
                    Statut = true,
                    DateCreation = DateTime.UtcNow.AddHours(-38),
                    Priorite = 1,
                    UserId = demoUser.Id
                },
                new TaskEntity
                {
                    Nom = "Coiffeur",
                    Description = "Demander très court sur les côtés avec la nuque longue et la moustache. C'est trop la classe!",
                    Statut = false,
                    DateCreation = DateTime.UtcNow.AddHours(-10).AddMinutes(28),
                    DerniereModification = DateTime.UtcNow.AddSeconds(-3002),
                    Priorite = 3,
                    UserId = demoUser.Id
                },
                new TaskEntity
                {
                    Nom = "Acheter des running de compet'",
                    Description = "Pour préparation au marathon de Paris dans 2 mois",
                    Statut = false,
                    DateCreation = DateTime.UtcNow.AddHours(-7).AddSeconds(1887),
                    Priorite = 3,
                    DateLimite = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60)),
                    UserId = demoUser.Id
                },
                new TaskEntity
                {
                    Nom = "Vendre anciens jouets",
                    Description = "Lego, playmobil,...",
                    Statut = false,
                    DateCreation = DateTime.UtcNow.AddHours(-4).AddSeconds(138),
                    DerniereModification = DateTime.UtcNow.AddSeconds(-416),
                    Priorite = 2,
                    Categorie = "Argent",
                    UserId = demoUser.Id
                },
                new TaskEntity
                {
                    Nom = "Prendre un café",
                    Description = "",
                    Statut = false,
                    DateCreation = DateTime.UtcNow.AddHours(-1).AddSeconds(422),
                    Priorite = 1,
                    Categorie = "Détente",
                    UserId = demoUser.Id
                }

            );

            await context.SaveChangesAsync();
        }
    }
}

