using Microsoft.AspNetCore.Mvc.Rendering;

using ToDoListWebApp.Models.DomainModels;

namespace ToDoListWebApp.Models.ViewModels
{
    public class TaskViewModel
    {
        public DomainModels.Task? Task { get; set; }
        public List<Priorite> PrioriteList { get; set; } = new List<Priorite>();
    }
}
