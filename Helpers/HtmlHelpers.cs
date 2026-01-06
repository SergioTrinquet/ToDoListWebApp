using static ToDoListWebApp.Models.DTO.UtilityObjects;

namespace ToDoListWebApp.Helpers
{
    public static class HtmlHelpers
    {
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

        public static UserNamePicto SetUserShortName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName.Length < 2)
                return new UserNamePicto (string.Empty, string.Empty);

            // Palette de classes CSS disponibles
            string[] classes =
            {
                "user-color-1",
                "user-color-2",
                "user-color-3",
                "user-color-4",
                "user-color-5",
                "user-color-6"
            };

            int hash = Math.Abs(userName.GetHashCode());

            return new UserNamePicto(
                char.ToUpper(userName[0]) + char.ToLower(userName[1]).ToString(),
                classes[hash % classes.Length]
            );
        }
    }
}
