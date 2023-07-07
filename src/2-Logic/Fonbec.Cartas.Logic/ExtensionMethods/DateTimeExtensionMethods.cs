using System.Globalization;

namespace Fonbec.Cartas.Logic.ExtensionMethods
{
    public static class DateTimeExtensionMethods
    {
        public static string ToPlanName(this DateTime dateTime) =>
            dateTime.ToString(@"MMMM \d\e yyyy", new CultureInfo("es-AR"));

        public static string ToLocalizedDate(this DateTime dateTime) =>
            dateTime.ToString(@"d \d\e MMMM \d\e yyyy", new CultureInfo("es-AR"));

        public static string ToLocalizedDateTime(this DateTime dateTime) =>
            dateTime.ToString("d MMM yyyy @ HH:mm", new CultureInfo("es-AR"));
    }
}
