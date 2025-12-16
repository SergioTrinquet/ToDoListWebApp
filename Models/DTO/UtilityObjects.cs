namespace ToDoListWebApp.Models.DTO
{
    public class UtilityObjects
    {
        public record DateCreaModifDTO(string verb, string date);

        public class ToggleStatutDTO { 
            public int Id { get; set; } 
            public bool Statut { get; set; } 
        }
    }
}
