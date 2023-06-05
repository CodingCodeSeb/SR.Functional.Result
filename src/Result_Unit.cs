namespace SR.Functional
{
    using Reasons;

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// An result that represents either a successful or an unsuccessful outcome.
    /// </summary>
    public readonly struct Result
    {
        /// <summary>
        /// Checks whether outcome is successful.
        /// </summary>
        public bool IsSuccess { get; }

        public Success SuccessReason { get; }
        public Error ErrorReason { get; }

        internal Result(bool isSuccessful, Success success, Error error)
        {
            IsSuccess = isSuccessful;
            SuccessReason = success;
            ErrorReason = error;
        }

        public static implicit operator Result(Error error)
        => Result.Fail(error);

        public static implicit operator Result(string error)
        => Result.Fail(error);

        public static implicit operator Result(Exception exception)
        => Result.Fail(ExceptionalError.Create(exception));

        /// <summary>
        /// Returns an enumerator for the result.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Success> GetEnumerator()
        {
            if (IsSuccess)
            {
                yield return SuccessReason;
            }
        }

        /// <summary>
        /// Uses an alternative result, if outcome is unsuccessful.
        /// </summary>
        /// <param name="alternativeOption">The alternative result.</param>
        /// <returns>The alternative result, if no value is present, otherwise itself.</returns>
        public Result Else(Result alternativeOption) => IsSuccess ? this : alternativeOption;

        /// <summary>
        /// Uses an alternative result, if no existing value is present.
        /// </summary>
        /// <param name="alternativeOptionFactory">A factory function to create an alternative result.</param>
        /// <returns>The alternative result, if no value is present, otherwise itself.</returns>
        public Result Else(Func<Result> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));
            return IsSuccess ? this : alternativeOptionFactory();
        }

        /// <summary>
        /// Evaluates a specified function, based on whether the result's outcome is successful or not.
        /// </summary>
        /// <param name="success">The function to evaluate if the outcome is successful.</param>
        /// <param name="fail">The function to evaluate if the outcome is unsuccessful.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<TResult> success, Func<Error, TResult> fail)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            return IsSuccess ? success() : fail(ErrorReason);
        }

        /// <summary>
        /// Evaluates a specified function, based on whether the result's outcome is successful or not.
        /// </summary>
        /// <param name="success">The function to evaluate if the outcome is successful.</param>
        /// <param name="fail">The function to evaluate if the outcome is unsuccessful.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<Success, TResult> success, Func<Error, TResult> fail)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            return IsSuccess ? success(SuccessReason) : fail(ErrorReason);
        }

        /// <summary>
        /// Evaluates a specified action, based on whether the result's outcome is successful or not.
        /// </summary>
        /// <param name="success">The function to evaluate if the result's outcome is succesful.</param>
        /// <param name="fail">The function to evaluate if the result's outcome is unsuccesful.</param>
        public void Match(Action success, Action<Error> fail)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            if (IsSuccess)
            {
                success();
            }
            else
            {
                fail(ErrorReason);
            }
        }

        /// <summary>
        /// Evaluates a specified action, based on whether the result's outcome is successful or not.
        /// </summary>
        /// <param name="success">The function to evaluate if the result's outcome is succesful.</param>
        /// <param name="fail">The function to evaluate if the result's outcome is unsuccesful.</param>
        public void Match(Action<Success> success, Action<Error> fail)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            if (IsSuccess)
            {
                success(SuccessReason);
            }
            else
            {
                fail(ErrorReason);
            }
        }

        /// <summary>
        /// Evaluates a specified action if the result's outcome is succesful.
        /// </summary>
        /// <param name="success">The action to evaluate if the result's outcome is succesful.</param>
        public Result IfSuccess(Action success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            if (IsSuccess)
            {
                success();
            }

            return this;
        }

        /// <summary>
        /// Evaluates a specified action if the result's outcome is succesful.
        /// </summary>
        /// <param name="success">The action to evaluate if the result's outcome is succesful.</param>
        public Result IfSuccess(Action<Success> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            if (IsSuccess)
            {
                success(SuccessReason);
            }

            return this;
        }

        /// <summary>
        /// Evaluates a specified action if outcome is unsuccesful.
        /// </summary>
        /// <param name="fail">The action to evaluate if outcome is unsuccesful.</param>
        public Result IfFail(Action<Error> fail)
        {
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            if (!IsSuccess)
            {
                fail(ErrorReason);
            }

            return this;
        }

        /// <summary>
        /// Transforms the result into another result. The result is flattened, and if either result's outcome is unsuccessful, an unsuccessful result is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public Result FlatMap(Func<Result> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Result result;

            if (IsSuccess)
            {
                result = mapping();

                if (!result.IsSuccess)
                {
                    if (childError != null)
                    {
                        result = Result.Fail(childError.CausedBy(result));
                    }
                }
            }
            else
            {
                result = Result.Fail(childError != null ? childError.CausedBy(this) : ErrorReason);
            }

            return result;
        }

        /// <summary>
        /// Transforms the result into another result. The result is flattened, and if either result's outcome is unsuccessful, an unsuccessful result is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public Result FlatMap(Func<Success, Result> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Result result;

            if (IsSuccess)
            {
                result = mapping(SuccessReason);

                if (!result.IsSuccess)
                {
                    if (childError != null)
                    {
                        result = Result.Fail(childError.CausedBy(result));
                    }
                }
            }
            else
            {
                result = Result.Fail(childError != null ? childError.CausedBy(this) : ErrorReason);
            }

            return result;
        }

        /// <summary>
        /// If the current result is successful, sets its success reason as the direct reason to the specified subsequent success reason.
        /// <para>Example: "Operation succeeded" (child success reason), Anteceded by: "Sub-operation successful" (antecedent successful result)</para>
        /// </summary>
        /// <param name="childSuccess">The success object to attach the succesful result's success object to.</param>
        public Result FlatMapSuccess(Success childSuccess)
        {
            if (childSuccess == null) throw new ArgumentNullException(nameof(childSuccess));

            if (!IsSuccess)
            {
                return Result.Success(childSuccess.AntecededBy(this));
            }

            return this;
        }

        /// <summary>
        /// If the current result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para>
        /// </summary>
        /// <param name="childError">The error object to attach the unsuccessful result's error object to.</param>
        public Result FlatMapFail(Error childError)
        {
            if (childError == null) throw new ArgumentNullException(nameof(childError));

            if (!IsSuccess)
            {
                return Result.Fail(childError.CausedBy(this));
            }

            return this;
        }

        /// <summary>
        /// Returns a string that represents the current result.
        /// </summary>
        /// <returns>A string that represents the current result.</returns>
        public override string ToString()
        {
            if (IsSuccess)
            {
                return $"Success({(SuccessReason.Message != "" || SuccessReason.Metadata.Count > 0 ? SuccessReason.ToString() : "")})";
            }

            return $"Fail{(ErrorReason != null ? $"(Error={ErrorReason})" : "")}";
        }

        #region Static

        /// <summary>
        /// Creates an result with a successful outcome.
        /// </summary>
        /// <returns>An result with a successful outcome.</returns>
        public static Result Success() => Success(Reasons.Success.Default);

        /// <summary>
        /// Creates an result with a successful outcome.
        /// </summary>
        /// <param name="success">A success object with data describing the successful outcome.</param>
        /// <returns>An result with a successful outcome.</returns>
        public static Result Success(Success success) => new(true, success, null);

        /// <summary>
        /// Creates an result with a successful outcome by using the success object of the specified result with a value.
        /// <para>The provided result cannot be empty.</para>
        /// </summary>
        /// <param name="success">An result with a value whose success object to copy.</param>
        /// <returns>An result containing the specified value.</returns>
        public static Result Success<TResult>(Result<TResult> success)
        {
            if (!success.IsSuccess) throw new ArgumentException("Source success must have a value");

            return new Result(true, success.SuccessReason, null);
        }

        /// <summary>
        /// Creates an result with an unsuccessful outcome.
        /// </summary>
        /// <param name="message">A description of the error that caused the unsuccessful outcome.</param>
        /// <returns>An result with an unsuccessful outcome.</returns>
        public static Result Fail(string message) => Fail(Error.Create(message));

        /// <summary>
        /// Creates an result with an unsuccessful outcome.
        /// </summary>
        /// <param name="message">A description of the error that caused the unsuccessful outcome.</param>
        /// <param name="exception">An exception instance to attach to the error property of the unsuccessful result.</param>
        /// <returns>An result with an unsuccessful outcome.</returns>
        public static Result Fail(string message, Exception exception) => Fail(Error.Create(message).CausedBy(exception));

        /// <summary>
        /// Creates an result with an unsuccessful outcome from an exception.
        /// </summary>
        /// <param name="exception">An exception instance to create an result with an unsuccessful outcome from.</param>
        /// <returns>An result with an unsuccessful outcome.</returns>
        public static Result Fail(Exception exception) => Fail(ExceptionalError.Create(exception));

        /// <summary>
        /// Creates an result with an unsuccessful outcome.
        /// </summary>
        /// <param name="error">An error object with data describing what caused the unsuccessful outcome.</param>
        /// <returns>An result with an unsuccessful outcome.</returns>
        public static Result Fail(Error error) => new(false, null, error);

        /// <summary>
        /// Creates an result with an unsuccessful outcome by using the error object of an empty result.
        /// </summary>
        /// <param name="fail">An empty result whose error object to copy.</param>
        /// <returns>An result with an unsuccessful outcome based on the specified empty result.</returns>
        public static Result Fail<TValue>(Result<TValue> fail)
        {
            if (fail.IsSuccess)
            {
                throw new ArgumentException("Source result must be empty");
            }

            return new Result(false, null, fail.ErrorReason);
        }

        /// <summary>
        /// Creates an <see cref="Result"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>If the predicate evaluates to false, an empty result is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="error">An error object to attach to the result when its outcome is unsuccessful.</param>
        /// <returns>An result whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static Result SuccessWhen(bool predicate, Error error)
        {
            return predicate ? Success() : Fail(error);
        }

        /// <summary>
        /// Creates an <see cref="Result"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>If the predicate evaluates to true, an empty result is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="error">An error object with data describing what caused the unsuccessful outcome.</param>
        /// <returns>An result whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static Result FailWhen(bool predicate, Error error)
        {
            return !predicate ? Success() : Fail(error);
        }

        /// <summary>
        /// Creates an <see cref="Result"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>If the predicate evaluates to false, an empty result is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="success">A success object with data describing the successful outcome.</param>
        /// <param name="error">An error object to attach to the result when its outcome is unsuccessful.</param>
        /// <returns>An result whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static Result SuccessWhen(bool predicate, Success success, Error error)
        {
            return predicate ? Success(success) : Fail(error);
        }

        /// <summary>
        /// Creates an <see cref="Result"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>If the predicate evaluates to true, an empty result is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="success">A success object with data describing the successful outcome.</param>
        /// <param name="error">An error object with data describing what caused the unsuccessful outcome.</param>
        /// <returns>An result whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static Result FailWhen(bool predicate, Success success, Error error)
        {
            return !predicate ? Success(success) : Fail(error);
        }

        /// <summary>
        /// Wraps an existing value in an <see cref="Result{T}"/> instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An result containing the specified value.</returns>
        public static Result<TValue> Success<TValue>(TValue value) => new(true, value, Reasons.Success.Default, null);

        /// <summary>
        /// Wraps an existing value in an <see cref="Result{T}"/> instance with a specified success message.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="successMessage">A message describing the reason or origin behind the presence of the result value.</param>
        /// <returns>An result containing the specified value.</returns>
        public static Result<TValue> Success<TValue>(TValue value, string successMessage) => new(true, value, Reasons.Success.Create(successMessage), null);

        /// <summary>
        /// Wraps an existing value in an <see cref="Result{T}"/> instance with a specified success object.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the result value.</param>
        /// <returns>An result containing the specified value.</returns>
        public static Result<TValue> Success<TValue>(TValue value, Success success) => new(true, value, success, null);

        /// <summary>
        /// Wraps an existing value in an <see cref="Result{T}"/> instance with the success object of the specified result.
        /// <para>The outcome of the provided result must be successful.</para>
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="success">A successful result whose success object to copy.</param>
        /// <returns>An result containing the specified value.</returns>
        public static Result<TValue> Success<TValue>(TValue value, Result success)
        {
            if (!success.IsSuccess) throw new ArgumentException("Source result must be successful");

            return new Result<TValue>(true, value, success.SuccessReason, null);
        }

        /// <summary>
        /// Wraps an existing value in an <see cref="Result{T}"/> instance with the success object of the specified result.
        /// <para>The provided result cannot be empty.</para>
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="success">An result with a value whose success object to copy.</param>
        /// <returns>An result containing the specified value.</returns>
        public static Result<TValue> Success<TValue, TResult>(TValue value, Result<TResult> success)
        {
            if (!success.IsSuccess) throw new ArgumentException("Source result must have a value");

            return new Result<TValue>(true, value, success.SuccessReason, null);
        }

        /// <summary>
        /// Creates an empty <see cref="Result{TValue}"/> instance with a specified error message.
        /// </summary>
        /// <param name="message">A description of why the result is missing its value.</param>
        /// <returns>An empty result.</returns>
        public static Result<TValue> Fail<TValue>(string message) => Fail<TValue>(Error.Create(message));

        /// <summary>
        /// Creates an empty <see cref="Result{TValue}"/> instance with a specified error message and exception.
        /// </summary>
        /// <param name="message">A description of why the result is missing its value.</param>
        /// <param name="exception">An exception instance to attach to error property of the empty result.</param>
        /// <returns>An empty result.</returns>
        public static Result<TValue> Fail<TValue>(string message, Exception exception) => Fail<TValue>(Error.Create(message).CausedBy(exception));

        /// <summary>
        /// Creates an empty <see cref="Result{TValue}"/> instance with the specified exception.
        /// </summary>
        /// <param name="exception">An exception instance to create an empty result from.</param>
        /// <returns>An empty result.</returns>
        public static Result<TValue> Fail<TValue>(Exception exception) => Fail<TValue>(ExceptionalError.Create(exception));

        /// <summary>
        /// Creates an empty <see cref="Result{TValue}"/> instance with a specified error object.
        /// </summary>
        /// <param name="error">An error object with data describing why the result is missing its value.</param>
        /// <returns>An empty result.</returns>
        public static Result<TValue> Fail<TValue>(Error error) => new(false, default, null, error);

        /// <summary>
        /// Creates an empty result with the error object of an result with an unsuccessful outcome.
        /// </summary>
        /// <param name="fail">An empty result whose error object to copy.</param>
        /// <returns>An empty result based on the specified result with an unsuccessful outcome.</returns>
        public static Result<TValue> Fail<TValue>(Result fail)
        {
            if (fail.IsSuccess) throw new ArgumentException("Source result must be unsuccessful");

            return new Result<TValue>(false, default, null, fail.ErrorReason);
        }

        /// <summary>
        /// Creates a new empty result with the error object of the specified empty result.
        /// </summary>
        /// <param name="fail">An empty result whose error object to copy.</param>
        /// <returns>An empty result based on the specified empty result.</returns>
        public static Result<TResult> Fail<TValue, TResult>(Result<TValue> fail)
        {
            if (fail.IsSuccess) throw new ArgumentException("Source result must be empty");

            return new Result<TResult>(false, default, null, fail.ErrorReason);
        }

        /// <summary>
        /// Creates a <see cref="LazyResult"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>When resolved, if the predicate evaluates to false, an empty result is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="error">An error object to attach to the resolved result in case its outcome is unsuccessful.</param>
        /// <returns>A deferred result whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static LazyResult Lazy(Func<bool> predicate, Error error)
        {
            return Lazy(predicate, Reasons.Success.Default, error);
        }

        /// <summary>
        /// Creates a <see cref="LazyResult"/> instance whose outcome depends on the satisfaction of the given predicate.
        /// <para>When resolved, if the predicate evaluates to false, an empty result is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="success">A success object with data describing the successful outcome.</param>
        /// <param name="error">An error object to attach to the resolved result in case its outcome is unsuccessful.</param>
        /// <returns>A deferred result whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static LazyResult Lazy(Func<bool> predicate, Success success, Error error)
        {
            return new(predicate, success, error);
        }

        /// <summary>
        /// Creates a <see cref="LazyResult"/> instance whose outcome depends on the satisfaction of the given predicate task.
        /// <para>When resolved, if the predicate evaluates to false, an empty result is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicateTask">The predicate to satisfy.</param>
        /// <param name="error">An error object to attach to the resolved result in case its outcome is unsuccessful.</param>
        /// <returns>A deferred result whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static LazyResultAsync LazyAsync(Func<Task<bool>> predicateTask, Error error)
        {
            return new(predicateTask, Reasons.Success.Default, error);
        }

        /// <summary>
        /// Creates a <see cref="LazyResult"/> instance whose outcome depends on the satisfaction of the given predicate task.
        /// <para>When resolved, if the predicate evaluates to false, an empty result is returned with the specified error object.</para>
        /// </summary>
        /// <param name="predicateTask">The predicate to satisfy.</param>
        /// <param name="success">A success object with data describing the successful outcome.</param>
        /// <param name="error">An error object to attach to the resolved result in case its outcome is unsuccessful.</param>
        /// <returns>A deferred result whose outcome depends on the satisfaction of the provided predicate.</returns>
        public static LazyResultAsync LazyAsync(Func<Task<bool>> predicateTask, Success success, Error error)
        {
            return new(predicateTask, success, error);
        }

        #endregion
    }
}
