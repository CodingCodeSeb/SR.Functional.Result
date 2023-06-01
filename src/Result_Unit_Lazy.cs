namespace SR.Functional
{
    using Reasons;

    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Wraps a predicate that returns a successful or an unsuccessful outcome.
    /// </summary>
    public readonly struct LazyResult
    {
        internal Func<bool> OutcomeDelegate { get; }

        internal Success Success { get; }

        internal Error Error { get; }


        internal LazyResult(Func<bool> outcomeDelegate, Success success, Error error)
        {
            OutcomeDelegate = outcomeDelegate;
            Success = success;
            Error = error;
        }


        /// <summary>
        /// Resolves the outcome delegate of the optional, returning a regular optional whose outcome depends on the result of the delegate.
        /// </summary>
        /// <returns></returns>
        public Result Resolve()
        {
            return OutcomeDelegate() ? Result.Success(Success) : Result.Fail(Error);
        }
    }

    /// <summary>
    /// Wraps a predicate task that returns a successful or an unsuccessful outcome.
    /// </summary>
    public readonly struct LazyResultAsync
    {
        internal Func<Task<bool>> OutcomeDelegate { get; }

        internal Success Success { get; }

        internal Error Error { get; }


        internal LazyResultAsync(Func<Task<bool>> outcomeDelegate, Success success, Error error)
        {
            OutcomeDelegate = outcomeDelegate;
            Success = success;
            Error = error;
        }


        /// <summary>
        /// Resolves the outcome delegate of the optional, returning a regular optional whose outcome depends on the result of the delegate.
        /// </summary>
        /// <returns></returns>
        public async Task<Result> Resolve()
        {
            return await OutcomeDelegate() ? Result.Success(Success) : Result.Fail(Error);
        }
    }
}
