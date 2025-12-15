using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoListWebApp.Data;
using ToDoListWebApp.Models.ViewModels;

namespace ToDoListWebApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Task.ToListAsync()); // Version Originale

            /// V2 ///
            //var task = await _context.Task.ToListAsync();
            //var prioriteList = _context.Priorite.ToList();
            //task.ForEach(t => {
            //    if(t.Priorite != null)
            //    {
            //        t.Priorite = prioriteList.Find(p => p.Id == t.Priorite).Id;
            //    }
            //});

            //return View(task);
            /// FIN V2 ///
        }


        // Appel méthode pour récupérer les tasks classées
        public async Task<IActionResult> GetSortedTasks(string column, string order)
        {
            var tasks = _context.Task.AsQueryable();

            try
            {
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
            catch(Exception)
            {
                throw;
            }

        }


        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }


        // GET: Tasks/Create
        public IActionResult Create()
        {
            // return View(); // Version originale
            return View(BuildTaskViewModel(new Models.DomainModels.Task())); // Version avec ViewModel pour alimenter aussi le Select 'Priorite'
        }


        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateCreation,DerniereModification,Nom,Description,Statut,DateLimite,Priorite,Categorie")] Models.DomainModels.Task task)
        {
            if (ModelState.IsValid)
            {
                task.DateCreation = DateTime.Now;

                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //return View(task); // Version Originale

            // En cas d'erreur de validation: Retourner TaskViewModel avec la liste des priorités afin de tjscorrectement réinitialiser le Select
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

            //ViewData["PrioriteSelect"] = _context.Priorite.ToList();  // TEST: Fonctionne mais pas retenu

            //return View(task); // Version Originale

            // Version avec ViewModel pour alimenter aussi le Select 'Priorite'
            var vm = BuildTaskViewModel(task);
            return View(vm);
            /// FIN ///
        }


        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateCreation,DerniereModification,Nom,Description,Statut,DateLimite,Priorite,Categorie")] Models.DomainModels.Task task)
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
                return RedirectToAction(nameof(Index));
            }

            //return View(task); // Version Originale
            return View(BuildTaskViewModel(task)); // Version avec ViewModel pour alimenter aussi le Select 'Priorite'
        }


        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }


        // POST: Tasks/Delete/5
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

        private bool TaskExists(int id)
        {
            return _context.Task.Any(e => e.Id == id);
        }
        

        // Méthode utilitaire pour retourner le ViewModel contenant les données tiles pour l'UI en plus du DomainModel 
        // qui s'occupe des données à lire/écrire daans la BDD
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
