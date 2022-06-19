using System;
using System.Linq;

namespace Xbim.InformationSpecifications.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Capitalises the first character of a string.
        /// Useful when building user messages to capitalise for style.
        /// </summary>
#pragma warning disable IDE0057 // Use range operator 
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
                // range operator is not available in net20
            };
#pragma warning restore IDE0057 // Use range operator
    }
}
