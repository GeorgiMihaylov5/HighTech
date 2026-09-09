using System.Globalization;

namespace HighTech.Common
{
    public static class FieldValueValidator
    {
        // Validates that a stored-as-string spec value conforms to its declared TypeCode.
        // Empty/null values are treated as "no value" and skipped (spec values are optional).
        // Numeric values may not be negative.
        public static void AddTypeError(List<string> errors, string fieldName, TypeCode typeCode, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    if (!bool.TryParse(value, out _))
                    {
                        errors.Add($"{fieldName} must be true or false.");
                    }
                    break;

                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var wholeNumber))
                    {
                        errors.Add($"{fieldName} must be a whole number.");
                    }
                    else if (wholeNumber < 0)
                    {
                        errors.Add($"{fieldName} cannot be negative.");
                    }
                    break;

                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    if (!decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                    {
                        errors.Add($"{fieldName} must be a number.");
                    }
                    else if (number < 0)
                    {
                        errors.Add($"{fieldName} cannot be negative.");
                    }
                    break;

                case TypeCode.DateTime:
                    if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        errors.Add($"{fieldName} must be a valid date.");
                    }
                    break;
            }
        }
    }
}
