namespace ToDoListWebApp.Models.DTO
{
    public class UtilityObjects
    {
        public record DateCreaModifDTO(string verb, string date);

        public record UserNamePicto(string shortName, string CssClassColor);

        public class ToggleStatutDTO { 
            public int Id { get; set; } 
            public bool Statut { get; set; } 
        }
    }
}
