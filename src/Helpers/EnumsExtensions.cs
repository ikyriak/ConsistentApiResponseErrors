using System;
using System.Text.RegularExpressions;

namespace ConsistentApiResponseErrors.Helpers.Enums
{
    public static class EnumsExtensions
    {
        public static string ToStringSpaceCamelCase(this Enum input)
        {
            return Regex.Replace(input.ToString(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }
    }
}
