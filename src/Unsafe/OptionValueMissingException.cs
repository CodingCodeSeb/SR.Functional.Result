namespace Ultimately.Unsafe
{
    using System;


    /// <summary>
    /// Indicates a failed retrieval of a value from an empty optional.
    /// </summary>
    public class OptionValueMissingException : Exception
    {
        internal OptionValueMissingException()
        {
        }

        internal OptionValueMissingException(string message)
            : base(message)
        {
        }
    }
}
