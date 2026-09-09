namespace HighTech.Common
{
    public static class StringLimits
    {
        public const int Guid = 36;
        public const int Name = 100;
        public const int Email = 256;
        public const int Phone = 32;
        public const int Address = 200;
        public const int City = 100;
        public const int PostalCode = 20;
        public const int ProductManufacturer = 80;
        public const int ProductModel = 120;
        public const int ProductImage = 500;
        public const int CategoryName = 100;
        public const int FieldName = 100;
        public const int ProductFieldValue = 250;
        public const int LongText = 1000;
        public const int JobTitle = 80;

        public static void AddMaxLengthError(List<string> errors, string label, string value, int maxLength)
        {
            if (value is not null && value.Length > maxLength)
            {
                errors.Add($"{label} cannot be longer than {maxLength} characters.");
            }
        }

        public static List<string> ValidateUserText(string firstName, string lastName, string email, string username, string phoneNumber)
        {
            var errors = new List<string>();

            AddMaxLengthError(errors, "First name", firstName, Name);
            AddMaxLengthError(errors, "Last name", lastName, Name);
            AddMaxLengthError(errors, "Email", email, Email);
            AddMaxLengthError(errors, "Username", username, Email);
            AddMaxLengthError(errors, "Phone number", phoneNumber, Phone);

            return errors;
        }
    }
}
