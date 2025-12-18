using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoListWebApp.Models.DomainModels
{
    public class Task
    {
        public int Id { get; set; }

        [DataType(DataType.Date)] // Ensures only date (no time) in UI helpers
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DisplayName("Date création")]
        public DateTime DateCreation { get; set; }
        
        [DataType(DataType.Date)] // Ensures only date (no time) in UI helpers
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DisplayName("Dernière modif.")]
        public DateTime? DerniereModification { get; set; }
        
        [Required(ErrorMessage = "Le champ 'tâche' est obligatoire", AllowEmptyStrings = false)]
        [DisplayName("Tâche")]
        public string Nom { get; set; }

        public string? Description { get; set; }

        [Required]
        [DisplayName("Fait")]
        public bool Statut { get; set; }

        [DataType(DataType.Date)] // Ensures only date (no time) in UI helpers
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("A faire avant")]
        public DateOnly? DateLimite { get; set; }

        [DisplayName("Priorité")]
        public int? Priorite { get; set; }

        [DisplayName("Catégorie")]
        public string? Categorie { get; set; }

        public Task()
        {
                
        }
    }
}
