using System.ComponentModel.DataAnnotations;

namespace ToDoListWebApp.Models.DomainModels
{
    public class Priorite
    {
        public int Id { get; set; }
        public string? Libelle { get; set; }
    }
}
