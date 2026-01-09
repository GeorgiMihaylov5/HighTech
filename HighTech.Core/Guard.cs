using System;
using System.Collections.Generic;
using System.Text;

namespace HighTech.Core
{
    public class Guard
    {
        public static void NotNullOrEmpty(string? value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("Value cannot be null or whitespace.", paramName);
        }
        public static void NotNegative<T>(T value, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) < 0)
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
        }

        public static void NotBetween<T>(T value, T min, T max, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
        }

        public static void NotNull<T>(T? value, string paramName) where T : class
        {
            if (value is null)
                throw new ArgumentNullException(paramName, "Value cannot be null.");
        }
    }
}
