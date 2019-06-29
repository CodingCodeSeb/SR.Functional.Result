namespace Ultimately
{
    using Reasons;

    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Provides a set of functions for creating optional values.
    /// </summary>
    public static class Optional
    {
        /// <summary>
        /// Creates an optional with a successful outcome.
        /// </summary>
        /// <returns>An optional with a successful outcome.</returns>
        public static Option Some() => Some(Success.Create(""));

        /// <summary>
        /// Creates an optional with a successful outcome.
        /// </summary>
        /// <param name="success">A success object with data describing the successful outcome.</param>
        /// <returns>An optional with a successful outcome.</returns>
        public static Option Some(Success success) => new Option(true, success, null);

        /// <summary>
        /// Creates an optional with an unsuccessful outcome.
        /// </summary>
        /// <param name="message">A description of the error that caused the unsuccessful outcome.</param>
        /// <returns>An optional with an unsuccessful outcome.</returns>
        public static Option None(string message) => None(Error.Create(message));

        /// <summary>
        /// Creates an optional with an unsuccessful outcome.
        /// </summary>
        /// <param name="message">A description of the error that caused the unsuccessful outcome.</param>
        /// <param name="exception">An exception instance to attach to the error property of the unsuccessful optional.</param>
        /// <returns>An optional with an unsuccessful outcome.</returns>
        public static Option None(string message, Exception exception) => None(ExceptionalError.Create(message, exception));

        /// <summary>
        /// Creates an optional with an unsuccessful outcome.
        /// </summary>
        /// <param name="error">An error object with data describing what caused the unsuccessful outcome.</param>
        /// <returns>An optional with an unsuccessful outcome.</returns>
        public static Option None(Error error) => new Option(false, null, error);

        /// <summary>
        /// Creates an <see cref="Option"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>If the predicate evaluates to false, an empty optional is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="error">An error object with data describing what caused the unsuccessful outcome.</param>
        /// <returns>An optional whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static Option SomeWhen(bool predicate, Error error)
        {
            return predicate ? Some() : None(error);
        }

        /// <summary>
        /// Creates an <see cref="Option"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>If the predicate evaluates to true, an empty optional is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="error">An error object with data describing what caused the unsuccessful outcome.</param>
        /// <returns>An optional whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static Option NoneWhen(bool predicate, Error error)
        {
            return !predicate ? Some() : None(error);
        }

        /// <summary>
        /// Creates an <see cref="Option"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>If the predicate evaluates to false, an empty optional is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="success">A success object with data describing the successful outcome.</param>
        /// <param name="error">An error object with data describing what caused the unsuccessful outcome.</param>
        /// <returns>An optional whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static Option SomeWhen(bool predicate, Success success, Error error)
        {
            return predicate ? Some(success) : None(error);
        }

        /// <summary>
        /// Creates an <see cref="Option"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>If the predicate evaluates to true, an empty optional is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="success">A success object with data describing the successful outcome.</param>
        /// <param name="error">An error object with data describing what caused the unsuccessful outcome.</param>
        /// <returns>An optional whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static Option NoneWhen(bool predicate, Success success, Error error)
        {
            return !predicate ? Some(success) : None(error);
        }



        /// <summary>
        /// Wraps an existing value in an <see cref="Option{T}"/> instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Option<TValue> Some<TValue>(TValue value) => new Option<TValue>(true, value, Success.Create(""), null);

        /// <summary>
        /// Wraps an existing value in an <see cref="Option{T}"/> instance with a specified success message.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="successMessage">A message describing the reason or origin behind the presence of the optional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Option<TValue> Some<TValue>(TValue value, string successMessage) => new Option<TValue>(true, value, Success.Create(successMessage), null);

        /// <summary>
        /// Wraps an existing value in an <see cref="Option{T}"/> instance with a specified success object.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the optional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Option<TValue> Some<TValue>(TValue value, Success success) => new Option<TValue>(true, value, success, null);

        /// <summary>
        /// Creates an empty <see cref="Option{TValue}"/> instance with a specified error message.
        /// </summary>
        /// <param name="message">A description of why the optional is missing its value.</param>
        /// <returns>An empty optional.</returns>
        public static Option<TValue> None<TValue>(string message) => None<TValue>(Error.Create(message));

        /// <summary>
        /// Creates an empty <see cref="Option{TValue}"/> instance with a specified error message and exception.
        /// </summary>
        /// <param name="message">A description of why the optional is missing its value.</param>
        /// <param name="exception">An exception instance to attach to error property of the empty optional.</param>
        /// <returns>An empty optional.</returns>
        public static Option<TValue> None<TValue>(string message, Exception exception) => None<TValue>(ExceptionalError.Create(message, exception));

        /// <summary>
        /// Creates an empty <see cref="Option{TValue}"/> instance with a specified error object.
        /// </summary>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An empty optional.</returns>
        public static Option<TValue> None<TValue>(Error error) => new Option<TValue>(false, default, null, error);


        /// <summary>
        /// Creates an optional that wraps a delegate that when resolved, may return a successful or an unsuccessful outcome.
        /// </summary>
        public static LazyOption Lazy(Func<bool> outcomeDelegate, Success success, Error error)
        {
            return new LazyOption(outcomeDelegate, success, error);
        }

        /// <summary>
        /// Creates an optional that wraps a delegate that when resolved, may return a successful or an unsuccessful outcome.
        /// </summary>
        public static LazyOption Lazy(Func<bool> outcomeDelegate, Error error)
        {
            return Lazy(outcomeDelegate, default, error);
        }
    }
}
