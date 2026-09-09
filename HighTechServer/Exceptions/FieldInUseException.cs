namespace HighTech.Exceptions
{
    public class FieldInUseException : Exception
    {
        public FieldInUseException(string message)
            : base(message)
        {
        }
    }
}
