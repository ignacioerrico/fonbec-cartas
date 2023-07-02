using System.Globalization;

namespace Fonbec.Cartas.Logic.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static bool ContainsIgnoringAccents(this string source, string subString)
        {
            var compareOptions =
                CompareOptions.IgnoreCase
                | CompareOptions.IgnoreSymbols
                | CompareOptions.IgnoreNonSpace;
            
            var index = CultureInfo.InvariantCulture.CompareInfo.IndexOf(source, subString, compareOptions);

            return index != -1;
        }

        public static string ToCommaSeparatedList(this List<string> list, bool useMDashIfEmpty = false)
        {
            var result = string.Join(", ", list);
            return string.IsNullOrEmpty(result) && useMDashIfEmpty
                ? "—"
                : result;
        }
    }
}
