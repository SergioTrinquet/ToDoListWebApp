using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoListWebApp.Data;
using ToDoListWebApp.Models.ViewModels;
using static ToDoListWebApp.Models.DTO.UtilityObjects;

namespace ToDoListWebApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly string _demoEmail;

        public TasksController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _demoEmail = configuration.GetValue<string>("DemoUser:Email") ?? "";
            // ou //_demoEmail = configuration["DemoUser:Email"] ?? "";
        }

        protected string? GetCurrentUserId()
        {
            return User?.Identity?.IsAuthenticated == true 
                ? _userManager.GetUserId(User) // Récupérer l'ID de l'utilisateur connecté
                : null;
        }

        // GET: Tasks
        [AllowAnonymous] // Permet de donner accès à la view de l'action sans login, contrairement à [Authorize]
        public async Task<IActionResult> Index()
        {
            IQueryable<Models.DomainModels.Task> tasksQuery = _context.Task
                                                                .Include(t => t.PrioriteNavigation)  // .Include() pour afficher les labels présents dans la table 'Priorite'
                                                                .Include(t => t.User); // .Include() pour afficher données du User de la tâche, présent dans la table 'AspNetUsers';

            var userId = GetCurrentUserId();
            if (userId != null) {
                // 👤 Utilisateur connecté
                tasksQuery = tasksQuery.Where(t => t.UserId == userId);
            } 
            else
            {
                // 👥 Utilisateur anonyme → User démo
                var demoUser = await _userManager.FindByEmailAsync(_demoEmail);
                if (demoUser == null) return View(new List<Models.DomainModels.Task>());

                tasksQuery = tasksQuery.Where(t => t.UserId == demoUser.Id);
            }

            var task = await tasksQuery.ToListAsync();

            return View(task);
        }


        // GET: Tasks/Create
        [Authorize]
        public IActionResult Create()
        {
            // return View(); // Version originale
            return View(BuildTaskViewModel(new Models.DomainModels.Task())); // Version avec ViewModel pour alimenter aussi le Select 'Priorite'
        }


        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom,Description,DateLimite,Priorite,Categorie")] Models.DomainModels.Task task)
        {
            // Supprimer l'état de validation pour UserId
            //ModelState.Remove("task.UserId");   

            if (ModelState.IsValid)
            {
                task.UserId = _userManager.GetUserId(User);
                task.DateCreation = DateTime.Now;

                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //return View(task); // Version Originale

            // En cas d'erreur de validation: Retourner TaskViewModel avec la liste des priorités afin de tjs correctement réinitialiser le Select
            return View(BuildTaskViewModel(task)); // Version avec ViewModel pour alimenter aussi le Select 'Priorite'
        }


        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var vm = BuildTaskViewModel(task); // ViewModel pour alimenter aussi le Select 'Priorite'
            return PartialView("./Partials/_EditModal", vm);
        }


        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateCreation,DerniereModification,Nom,Description,Statut,DateLimite,Priorite,Categorie, UserId")] Models.DomainModels.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    task.DerniereModification = DateTime.Now;

                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Json(new { success = true }); // Pour indiquer succès de l'opération en AJAX
            }

            // Quand erreur de validation : Retour en Html
            return PartialView("./Partials/_EditModal", BuildTaskViewModel(task));
        }


        // GET: Tasks/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .Include(t => t.PrioriteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }


        // POST: Tasks/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Task.FindAsync(id);
            if (task != null)
            {
                _context.Task.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // Méthode pour changer le statut d'une task (fait/non fait) : Méthode appelée via JS en AJAX
        [HttpPost]
        public async Task<IActionResult> ToggleStatut([FromBody] ToggleStatutDTO dto)
        {
            var task = await _context.Task.FindAsync(dto.Id);
            if (task == null)
                return NotFound();
            task.Statut = dto.Statut;
            await _context.SaveChangesAsync();
            return Ok();
        }


        // Pour récupérer les tasks classées : Méthode appelée via JS en AJAX
        public async Task<IActionResult> GetSortedAndFilteredTasks(string column, string order, string filter = "", bool displayDemoTasks = false) 
        {
            var tasks = _context.Task.AsQueryable();

            try
            {
                tasks = tasks
                    .Include(t => t.PrioriteNavigation)
                    .Include(t => t.User);

                var demoUser = await _userManager.FindByEmailAsync(_demoEmail);
                if (User?.Identity?.IsAuthenticated == true)
                {
                    // 👤 Utilisateur connecté
                    var userId = _userManager.GetUserId(User); // Récupérer l'ID de l'utilisateur connecté
                    if (displayDemoTasks)
                    {
                        tasks = tasks.Where(t => t.UserId == userId || t.UserId == demoUser.Id);
                    }
                    else
                    {
                        tasks = tasks.Where(t => t.UserId == userId);
                    }
                }
                else
                {
                    // 👥 Utilisateur anonyme → User démo
                    tasks = tasks.Where(t => t.UserId == demoUser.Id);
                }


                if (filter != "")
                {
                    filter = filter.Trim();
                    tasks = tasks.Where(t => t.Nom.Contains(filter) || t.Description.Contains(filter));
                }

                switch (column)
                {
                    case "Nom":
                        tasks = (order == "desc") ? tasks.OrderByDescending(t => t.Nom) : tasks.OrderBy(t => t.Nom);
                        break;

                    case "Priorite":
                        tasks = (order == "desc") ? tasks.OrderByDescending(t => t.Priorite).ThenBy(t => t.Nom) : tasks.OrderBy(t => t.Priorite).ThenBy(t => t.Nom);
                        break;
                }

                return PartialView("./Partials/_IndexTable", await tasks.ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }


        private bool TaskExists(int id)
        {
            return _context.Task.Any(e => e.Id == id);
        }


        // Méthode utilitaire pour retourner le ViewModel contenant les données utiles pour l'UI 
        // en plus du DomainModel qui s'occupe des données à lire/écrire daans la BDD
        private TaskViewModel BuildTaskViewModel(Models.DomainModels.Task task)
        {
            return new TaskViewModel
            {
                Task = task,
                PrioriteList = _context.Priorite.ToList()
            };
        }

    }
}
