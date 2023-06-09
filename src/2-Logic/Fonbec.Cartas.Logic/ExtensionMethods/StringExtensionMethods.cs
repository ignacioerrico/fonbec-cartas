﻿using System.Globalization;

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
    }
}
