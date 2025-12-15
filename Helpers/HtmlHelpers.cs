using static ToDoListWebApp.Models.DTO.UtilityObjects;

namespace ToDoListWebApp.Helpers
{
    public static class HtmlHelpers
    {
        //public static string FormatLibelleDateCreaModif(DateTime dateCreation, DateTime? dateModif)
        //{
        //    return (
        //        dateModif == null ? "Créé" : "Modifié") 
        //        + " le " + 
        //        (dateModif == null ? dateCreation.ToString("dd/MM/yyyy à HH:mm") : dateModif?.ToString("dd/MM/yyyy à HH:mm")
        //    );
        //}
        public static DateCreaModifDTO FormatLibelleDateCreaModif(DateTime dateCreation, DateTime? dateModif)
        {
            return new DateCreaModifDTO (
                (dateModif == null ? "Créé le" : "Modifié le"),
                (dateModif == null ? dateCreation : dateModif.GetValueOrDefault()).ToString("dd/MM/yyyy à HH:mm")
            );
        }

        public static string setCSSclassPriorite(int? priorite) =>
            priorite switch
            {
                1 => "priorite low",
                2 => "priorite medium",
                3 => "priorite high",
                _ => ""
            };

    }
}
