namespace SR.Functional
{
    using Reasons;

    using System;


    public static class ResultExtensions
    {
        /// <summary>
        /// Wraps an existing value in an <see cref="Result{TValue}"/> instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Result<TValue> Success<TValue>(this TValue value) => Result.Success(value);

        /// <summary>
        /// Wraps an existing value in an <see cref="Result{TValue}"/> instance with a specified success message.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="successMessage">A message describing the reason or origin behind the presence of the optional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Result<TValue> Success<TValue>(this TValue value, string successMessage) => Result.Success(value, successMessage);

        /// <summary>
        /// Wraps an existing value in an <see cref="Result{TValue}"/> instance with a specified success object.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the optional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Result<TValue> Success<TValue>(this TValue value, Success success) => Result.Success(value, success);

        /// <summary>
        /// Creates an <see cref="Result{TValue}"/> instance from a specified value.
        /// <para>If the value does not satisfy the given predicate, an empty optional is returned with the specified error object.</para>
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An optional containing the specified value, if the predicate is satisified.</returns>
        public static Result<TValue> SuccessWhen<TValue>(this TValue value, Predicate<TValue> predicate, Error error)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return predicate(value) ? Result.Success(value) : Result.Fail<TValue>(error);
        }

        /// <summary>
        /// Creates an <see cref="Result{TValue}"/> instance from a specified value and success message.
        /// <para>If the value does not satisfy the given predicate, an empty optional is returned with the specified error message.</para>
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="successMessage">A message describing the reason or origin behind the presence of the optional value.</param>
        /// <param name="errorMessage">A description of why the optional is missing its value if the predicate is not satisfied.</param>
        /// <returns>An optional containing the specified value, if the predicate is satisified.</returns>
        public static Result<TValue> SuccessWhen<TValue>(this TValue value, Predicate<TValue> predicate, string successMessage, string errorMessage)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return predicate(value) ? Result.Success(value, successMessage) : Result.Fail<TValue>(errorMessage);
        }

        /// <summary>
        /// Creates an <see cref="Result{TValue}"/> instance from a specified value and success object.
        /// <para>If the value does not satisfy the given predicate, an empty optional is returned with the specified error object.</para>
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the optional value.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An optional containing the specified value, if the predicate is satisified.</returns>
        public static Result<TValue> SuccessWhen<TValue>(this TValue value, Predicate<TValue> predicate, Success success, Error error)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return predicate(value) ? Result.Success(value, success) : Result.Fail<TValue>(error);
        }

        /// <summary>
        /// Creates an <see cref="Result{TValue}"/> instance from a specified value. 
        /// <para>If the value satisfies the given predicate, an empty optional is returned with the specified error object attached.</para>
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An optional containing the specified value, if the predicate is not satisifed.</returns>
        public static Result<TValue> FailWhen<TValue>(this TValue value, Predicate<TValue> predicate, Error error)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return value.SuccessWhen(val => !predicate(val), error);
        }

        /// <summary>
        /// Creates an <see cref="Result{TValue}"/> instance from a specified value. 
        /// <para>If the value is null, an empty optional is returned with the specified error object attached.</para>
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An optional containing the specified value given that it's not null.</returns>
        public static Result<TValue> SuccessNotNull<TValue>(this TValue value, Error error) => value.SuccessWhen(val => val != null, error);

        /// <summary>
        /// Converts a <see cref="Nullable{T}"/> to an <see cref="Result{TValue}"/> instance with the specified error message if its value is null.
        /// <para>If its value is null, an empty optional is returned with the specified error object attached.</para>
        /// </summary>
        /// <param name="value">The Nullable&lt;T&gt; instance.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>The <see cref="Result{TValue}"/> instance.</returns>
        public static Result<TValue> ToResult<TValue>(this TValue? value, Error error)
            where TValue : struct
            => value.HasValue ? Result.Success(value.Value) : Result.Fail<TValue>(error);

        /// <summary>
        /// Flattens two nested optionals into one. The resulting optional will be empty if either the inner or outer optional is empty.
        /// </summary>
        /// <param name="nestedOption">The nested optional.</param>
        /// <returns>A flattened optional.</returns>
        public static Result<TValue> Flatten<TValue>(this Result<Result<TValue>> nestedOption) =>
            nestedOption.FlatMap(innerOption => innerOption);

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if the original optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static Result<TResult> Map<TResult>(this Result option, Func<TResult> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Func<Error, Result<TResult>> errorFunc = Result.Fail<TResult>;

            if (childError != null)
            {
                errorFunc = e => Result.Fail<TResult>(childError.CausedBy(e));
            }

            return option.Match(
                success: s => Result.Success(mapping(), s),
                fail: errorFunc
            );
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if the original optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static Result<TResult> Map<TResult>(this Result option, Func<Success, TResult> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Func<Error, Result<TResult>> errorFunc = Result.Fail<TResult>;

            if (childError != null)
            {
                errorFunc = e => Result.Fail<TResult>(childError.CausedBy(e));
            }

            return option.Match(
                success: s => Result.Success(mapping(s), s),
                fail: errorFunc
            );
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if either optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static Result<TResult> FlatMap<TResult>(this Result option, Func<Result<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Result<TResult> result;

            if (option.IsSuccessful)
            {
                result = mapping();

                result.MatchFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail<TResult>(childError.CausedBy(e));
                    }
                });
            }
            else
            {
                result = Result.Fail<TResult>(childError != null ? childError.CausedBy(option._Error) : option._Error);
            }

            return result;
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if either optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static Result<TResult> FlatMap<TResult>(this Result option, Func<Success, Result<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Result<TResult> result;

            if (option.IsSuccessful)
            {
                result = mapping(option._Success);

                result.MatchFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail<TResult>(childError.CausedBy(e));
                    }
                });
            }
            else
            {
                result = Result.Fail<TResult>(childError != null ? childError.CausedBy(option._Error) : option._Error);
            }

            return result;
        }

        /// <summary>
        /// Transforms the value-optional into an optional with an outcome. The result is flattened, and if either the source optional is empty or the resulting optional's outcome is unsuccessful, an optional with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static Result FlatMap<TValue>(this Result<TValue> option, Func<TValue, Result> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Result result;

            if (option.HasValue)
            {
                result = mapping(option.Value);

                result.MatchFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail(childError.CausedBy(e));
                    }
                });
            }
            else
            {
                result = Result.Fail(childError != null ? childError.CausedBy(option.Error) : option.Error);
            }

            return result;
        }

        /// <summary>
        /// Transforms the value-optional into an optional with an outcome. The result is flattened, and if either the source optional is empty or the resulting optional's outcome is unsuccessful, an optional with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static Result FlatMap<TValue>(this Result<TValue> option, Func<TValue, Success, Result> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Result result;

            if (option.HasValue)
            {
                result = mapping(option.Value, option.Success);

                result.MatchFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail(childError.CausedBy(e));
                    }
                });
            }
            else
            {
                result = Result.Fail(childError != null ? childError.CausedBy(option.Error) : option.Error);
            }

            return result;
        }
    }
}
