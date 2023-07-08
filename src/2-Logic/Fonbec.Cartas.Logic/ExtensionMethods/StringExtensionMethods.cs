using System.Globalization;

namespace Fonbec.Cartas.Logic.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static bool ContainsIgnoringAccents(this string source, string subString)
        {
            const CompareOptions compareOptions = CompareOptions.IgnoreCase
                | CompareOptions.IgnoreSymbols
                | CompareOptions.IgnoreNonSpace;
            
            var index = CultureInfo.InvariantCulture.CompareInfo.IndexOf(source, subString, compareOptions);

            return index != -1;
        }

        public static string ToCommaSeparatedList(this List<string>? list)
        {
            if (list is null)
            {
                return string.Empty;
            }

            return string.Join(", ", list);
        }

        public static string MDashIfEmpty(this string? value)
        {
            if (value is null)
        {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(value)
                ? "—"
                : value;
        }
    }
}
