namespace Ultimately
{
    using Reasons;

    using System;


    /// <summary>
    /// Wraps a delegate that returns a successful or an unsuccessful outcome.
    /// </summary>
    public struct LazyOption
    {
        internal Func<bool> OutcomeDelegate { get; }
        internal Success Success { get; }
        internal Error Error { get; }

        internal LazyOption(Func<bool> outcomeDelegate, Success success, Error error)
        {
            OutcomeDelegate = outcomeDelegate;
            Success = success;
            Error = error;
        }


        /// <summary>
        /// Resolves the outcome delegate of the optional, returning a regular optional whose outcome depends on the result of the delegate.
        /// </summary>
        /// <returns></returns>
        public Option Resolve()
        {
            return OutcomeDelegate() ? Optional.Some(Success) : Optional.None(Error);
        }
    }
}
