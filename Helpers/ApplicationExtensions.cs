
using System.Reflection;

namespace MVCRESTAPI.Helpers
{
    public static class ApplicationExtensions
    {

        // Summary:
        //     Gets a substring of a string from beginning of the string.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     Thrown if str is null
        //
        //   T:System.ArgumentException:
        //     Thrown if len is bigger that string's length
        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(0, len);
        }

        // Summary:
        //     Removes first occurrence of the given postfixes from end of the given string.
        //     Ordering is important. If one of the postFixes is matched, others will not be
        //     tested.
        //
        // Parameters:
        //   str:
        //     The string.
        //
        //   postFixes:
        //     one or more postfix.
        //
        // Returns:
        //     Modified string or the same string if it has not any of given postfixes
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (postFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (string text in postFixes)
            {
                if (str.EndsWith(text))
                {
                    return str.Left(str.Length - text.Length);
                }
            }

            return str;
        }

        // Extension method to check if a Type is numeric or nullable numeric
        public static bool IsNumeric(this Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return IsNumericCore(underlyingType);
            }
            return IsNumericCore(type);
        }

        // Helper method to check if a Type is numeric
        public static bool IsNumericCore(this Type type)
        {
            return type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(float);
        }
        /// <summary>
        /// Checks whether <paramref name="enumerable"/> is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="enumerable"/>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to be checked.</param>
        /// <returns>True if <paramref name="enumerable"/> is null or empty, false otherwise.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

    }
}
