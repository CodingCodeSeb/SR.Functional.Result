namespace Ultimately.Unsafe
{
    using System;


    public static class OptionUnsafeExtensions
    {
        /// <summary>
        /// Transforms an optional into a <see cref="Nullable{T}"/>.
        /// </summary>
        /// <param name="option">The specified optional.</param>
        /// <returns>The <see cref="Nullable{T}"/> instance.</returns>
        public static TValue? ToNullable<TValue>(this Option<TValue> option)
            where TValue : struct
        {
            if (option.HasValue)
            {
                return option.Value;
            }

            return default;
        }

        /// <summary>
        /// Returns the existing value if present, otherwise default(T).
        /// </summary>
        /// <param name="option">The specified optional.</param>
        /// <returns>The existing value or a default value.</returns>
        public static TValue ValueOrDefault<TValue>(this Option<TValue> option)
        {
            if (option.HasValue)
            {
                return option.Value;
            }

            return default;
        }

        /// <summary>
        /// Returns the existing value if present, or throws an <see cref="OptionValueMissingException"/>.
        /// </summary>
        /// <param name="option">The specified optional.</param>
        /// <returns>The existing value.</returns>
        /// <exception cref="OptionValueMissingException">Thrown when a value is not present.</exception>
        public static TValue ValueOrFailure<TValue>(this Option<TValue> option)
        {
            if (option.HasValue)
            {
                return option.Value;
            }

            throw new OptionValueMissingException();
        }

        /// <summary>
        /// Returns the existing value if present, or throws an <see cref="OptionValueMissingException"/>.
        /// </summary>
        /// <param name="option">The specified optional.</param>
        /// <param name="errorMessage">An error message to use in case of failure.</param>
        /// <returns>The existing value.</returns>
        /// <exception cref="OptionValueMissingException">Thrown when a value is not present.</exception>
        public static TValue ValueOrFailure<TValue>(this Option<TValue> option, string errorMessage)
        {
            if (option.HasValue)
            {
                return option.Value;
            }

            throw new OptionValueMissingException(errorMessage);
        }
    }
}
