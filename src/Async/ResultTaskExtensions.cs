namespace SR.Functional.Async
{
    using Reasons;

    using System;
    using System.Threading.Tasks;


    public static class ResultTaskExtensions
    {
        /// <summary>
        /// Determines if the result task contains a specified value asynchronously.
        /// </summary>
        /// <param name="resultTask">The result task to check for the specified value.</param>
        /// <param name="value">The value to locate. Set to null to check whether the result's' value is null.</param>
        /// <returns>A boolean indicating whether or not the value was found.</returns>
        public static async Task<bool> ContainsAsync<TValue>(this Task<Result<TValue>> resultTask, TValue value)
        {
            if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));

            var option = await resultTask;

            if (option.IsSuccess)
            {
                if (option.Value == null)
                {
                    return value == null;
                }

                return option.Value.Equals(value);
            }

            return false;
        }

        /// <summary>
        /// Determines if the result contains a value satisfying a specified predicate asynchronously.
        /// </summary>
        /// <param name="option">The result task to test.</param>
        /// <param name="predicate">A predicate to test the result value against.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public static async Task<bool> ExistsAsync<TValue>(this Result<TValue> option, Func<TValue, Task<bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (option.IsSuccess)
            {
                var predicateTask = predicate(option.Value);

                if (predicateTask == null) throw new InvalidOperationException($"{nameof(predicateTask)} must not return a null task");

                return await predicateTask;
            }

            return false;
        }

        /// <summary>
        /// Determines if the current result contains a value satisfying a specified predicate asynchronously.
        /// </summary>
        /// <param name="optionTask">The result task to test.</param>
        /// <param name="predicate">A predicate to test the result value against.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public static async Task<bool> ExistsAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue, bool> predicate)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var option = await optionTask;

            return option.IsSuccess && predicate(option.Value);
        }

        /// <summary>
        /// Determines if the current result task contains a value satisfying a specified predicate asynchronously.
        /// </summary>
        /// <param name="optionTask">The result task to test.</param>
        /// <param name="predicate">A predicate to test the result value against.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public static async Task<bool> ExistsAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue, Task<bool>> predicate)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var option = await optionTask;

            if (option.IsSuccess)
            {
                var predicateTask = predicate(option.Value);

                if (predicateTask == null) throw new InvalidOperationException($"{nameof(predicateTask)} must not return a null task");

                return await predicateTask;
            }

            return false;
        }

        /// <summary>
        /// Returns the specified result task's existing value if present, or otherwise an alternative value.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public static async Task<TValue> ValueOrAsync<TValue>(this Task<Result<TValue>> optionTask, TValue alternative)
        {
            var option = await optionTask;

            return option.IsSuccess ? option.Value : alternative;
        }

        /// <summary>
        /// Returns the specified result's existing value if present, or otherwise an alternative value by calling the specified factory delegate task.
        /// </summary>
        /// <param name="option">The result to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The existing or alternative value.</returns>
        public static async Task<TValue> ValueOrAsync<TValue>(this Result<TValue> option, Func<Task<TValue>> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            if (option.IsSuccess)
            {
                return option.Value;
            }

            var alternativeTask = alternativeFactory();

            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeTask)} must not return a null task");

            return await alternativeTask;
        }

        /// <summary>
        /// Returns the specified result task's existing value if present, or otherwise an alternative value by calling the specified factory delegate.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public static async Task<TValue> ValueOrAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return option.IsSuccess ? option.Value : alternativeFactory();
        }

        /// <summary>
        /// Returns the specified result task's existing value if present, or otherwise an alternative value by calling the specified factory delegate task.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The existing or alternative value.</returns>
        public static async Task<TValue> ValueOrAsync<TValue>(this Task<Result<TValue>> optionTask, Func<Task<TValue>> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            if (option.IsSuccess)
            {
                return option.Value;
            }

            var alternativeTask = alternativeFactory();

            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeTask)} must not return a null task");

            return await alternativeTask;
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="option">The result to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>A new result, containing either the existing or alternative value.</returns>
        public static async Task<Result<TValue>> OrAsync<TValue>(this Result<TValue> option, Func<Task<TValue>> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            if (option.IsSuccess)
            {
                return option;
            }

            var alternativeTask = alternativeFactory();
            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeFactory)} must not return a null task");

            var alternative = await alternativeTask;

            return alternative.Success();
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="option">The result to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative result value.</param>
        /// <returns>A new result, containing either the existing or alternative value.</returns>
        public static async Task<Result<TValue>> OrAsync<TValue>(this Result<TValue> option, Func<Task<TValue>> alternativeFactory, Success success)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            if (option.IsSuccess)
            {
                return option;
            }

            var alternativeTask = alternativeFactory();
            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeFactory)} must not return a null task");

            var alternative = await alternativeTask;

            return alternative.Success(success);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>A new result, containing either the existing or alternative value.</returns>
        public static async Task<Result<TValue>> OrAsync<TValue>(this Task<Result<TValue>> optionTask, TValue alternative)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            var option = await optionTask;

            if (option.IsSuccess)
            {
                return option;
            }

            return alternative.Success();
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternative">The alternative value.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative result value.</param>
        /// <returns>A new result, containing either the existing or alternative value.</returns>
        public static async Task<Result<TValue>> OrAsync<TValue>(this Task<Result<TValue>> optionTask, TValue alternative, Success success)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            var option = await optionTask;

            if (option.IsSuccess)
            {
                return option;
            }

            return alternative.Success(success);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <returns>A new result, containing either the existing or alternative value.</returns>
        public static async Task<Result<TValue>> OrAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return option.Or(alternativeFactory);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative result value.</param>
        /// <returns>A new result, containing either the existing or alternative value.</returns>
        public static async Task<Result<TValue>> OrAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue> alternativeFactory, Success success)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return option.Or(alternativeFactory, success);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>A new result, containing either the existing or alternative value.</returns>
        public static async Task<Result<TValue>> OrAsync<TValue>(this Task<Result<TValue>> optionTask, Func<Task<TValue>> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return await option.OrAsync(alternativeFactory);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative result value.</param>
        /// <returns>A new result, containing either the existing or alternative value.</returns>
        public static async Task<Result<TValue>> OrAsync<TValue>(this Task<Result<TValue>> optionTask, Func<Task<TValue>> alternativeFactory, Success success)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return await option.OrAsync(alternativeFactory, success);
        }

        /// <summary>
        /// Uses an alternative result, if no existing value is present.
        /// </summary>
        /// <param name="option">The result to test.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The alternative result, if no value is present, otherwise itself.</returns>
        public static async Task<Result<TValue>> ElseAsync<TValue>(this Result<TValue> option, Func<Task<Result<TValue>>> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            if (option.IsSuccess) return option;

            var alternativeOptionTask = alternativeFactory();
            if (alternativeOptionTask == null) throw new InvalidOperationException($"{nameof(alternativeFactory)} must not return a null task");

            return await alternativeOptionTask;
        }

        /// <summary>
        /// Uses an alternative result, if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to test.</param>
        /// <param name="alternativeResult">The alternative result.</param>
        /// <returns>The alternative result, if no value is present, otherwise itself.</returns>
        public static async Task<Result<TValue>> ElseAsync<TValue>(this Task<Result<TValue>> optionTask, Result<TValue> alternativeResult)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            var option = await optionTask;

            if (option.IsSuccess)
            {
                return option;
            }

            return alternativeResult;
        }

        /// <summary>
        /// Uses an alternative result, if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to test.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The alternative result, if no value is present, otherwise itself.</returns>
        public static async Task<Result<TValue>> ElseAsync<TValue>(this Task<Result<TValue>> optionTask, Func<Result<TValue>> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return option.Else(alternativeFactory);
        }

        /// <summary>
        /// Uses an alternative result, if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The result task to test.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The alternative result, if no value is present, otherwise itself.</returns>
        public static async Task<Result<TValue>> ElseAsync<TValue>(this Task<Result<TValue>> optionTask, Func<Task<Result<TValue>>> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return await option.ElseAsync(alternativeFactory);
        }

        /// <summary>
        /// Transforms the inner value of an result asynchronously. If the instance is empty, an empty result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Result<TValue> option, Func<TValue, Task<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, _) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var mappedValue = await valueTask;

                return mappedValue.Success();
            }

            return Result.Fail<TResult>(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the inner value of an result asynchronously. If the instance is empty, an empty result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Result<TValue> option, Func<TValue, Success, Task<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, _) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var mappedValue = await valueTask;

                return mappedValue.Success();
            }

            return Result.Fail<TResult>(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the inner value of an result task asynchronously. If the instance is empty, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Task<Result<TValue>> optionTask, Func<TValue, TResult> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.Map(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an result task asynchronously. If the instance is empty, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Task<Result<TValue>> optionTask, Func<TValue, Success, TResult> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.Map(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an result task asynchronously. If the instance is empty, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Task<Result<TValue>> optionTask, Func<TValue, Task<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.MapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an result task asynchronously. If the instance is empty, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Task<Result<TValue>> optionTask, Func<TValue, Success, Task<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.MapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if the original result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TResult>(this Result option, Func<Task<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, success) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var mappedValue = await valueTask;

                return mappedValue.Success(success);
            }

            return Result.Fail<TResult>(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if the original result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TResult>(this Result option, Func<Success, Task<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, success) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var mappedValue = await valueTask;

                return mappedValue.Success(success);
            }

            return Result.Fail<TResult>(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if the original result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TResult>(this Task<Result> optionTask, Func<Task<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.MapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if the original result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TResult>(this Task<Result> optionTask, Func<Success, Task<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.MapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if the original result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TResult>(this Task<Result> optionTask, Func<TResult> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.Map(mapping, childError);
        }

        /// <summary>
        /// Transforms the result task into an result with a value. The result is flattened, and if the original result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> MapAsync<TResult>(this Task<Result> optionTask, Func<Success, TResult> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.Map(mapping, childError);
        }

        /// <summary>
        /// Transforms the result into another result task asynchronously. The result is flattened, and if either result's outcome is unsuccessful, an unsuccessful result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync(this Result option, Func<Task<Result>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Result result;

            if (option.IsSuccess)
            {
                result = await mapping();

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
                result = Result.Fail(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
            }

            return result;
        }

        /// <summary>
        /// Transforms the result into another result task asynchronously. The result is flattened, and if either result's outcome is unsuccessful, an unsuccessful result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync(this Result option, Func<Success, Task<Result>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Result result;

            if (option.IsSuccess)
            {
                result = await mapping(option.SuccessReason);

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
                result = Result.Fail(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
            }

            return result;
        }

        /// <summary>
        /// Transforms the result task into another result asynchronously. The result is flattened, and if either result's outcome is unsuccessful, an unsuccessful result is returned.
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync(this Task<Result> optionTask, Func<Result> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the result task into another result asynchronously. The result is flattened, and if either result's outcome is unsuccessful, an unsuccessful result is returned.
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync(this Task<Result> optionTask, Func<Success, Result> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the result task into another result task asynchronously. The result is flattened, and if either result's outcome is unsuccessful, an unsuccessful result is returned.
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync(this Task<Result> optionTask, Func<Task<Result>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the result task into another result task asynchronously. The result is flattened, and if either result's outcome is unsuccessful, an unsuccessful result is returned.
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync(this Task<Result> optionTask, Func<Success, Task<Result>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an result into another result asynchronously. The result is flattened, and if either is empty, an empty result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        public static async Task<Result<TResult>> FlatMapAsync<TValue, TResult>(this Result<TValue> option, Func<TValue, Task<Result<TResult>>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.IfFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail<TResult>(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Result.Fail<TResult>(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the inner value of an result into another result asynchronously. The result is flattened, and if either is empty, an empty result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        public static async Task<Result<TResult>> FlatMapAsync<TValue, TResult>(this Result<TValue> option, Func<TValue, Success, Task<Result<TResult>>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.IfFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail<TResult>(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Result.Fail<TResult>(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the inner value of an result task into another result asynchronously. The result is flattened, and if either is empty, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        public static async Task<Result<TResult>> FlatMapAsync<TValue, TResult>(this Task<Result<TValue>> optionTask, Func<TValue, Result<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an result task into another result asynchronously. The result is flattened, and if either is empty, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        public static async Task<Result<TResult>> FlatMapAsync<TValue, TResult>(this Task<Result<TValue>> optionTask, Func<TValue, Success, Result<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an result task into another task result asynchronously. The result is flattened, and if either is empty, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        public static async Task<Result<TResult>> FlatMapAsync<TValue, TResult>(this Task<Result<TValue>> optionTask, Func<TValue, Task<Result<TResult>>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an result task into another task result asynchronously. The result is flattened, and if either is empty, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        public static async Task<Result<TResult>> FlatMapAsync<TValue, TResult>(this Task<Result<TValue>> optionTask, Func<TValue, Success, Task<Result<TResult>>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if either result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> FlatMapAsync<TResult>(this Result option, Func<Task<Result<TResult>>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.IfFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail<TResult>(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Result.Fail<TResult>(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if either result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> FlatMapAsync<TResult>(this Result option, Func<Success, Task<Result<TResult>>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.IfFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail<TResult>(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Result.Fail<TResult>(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the value-result into an result with an outcome. The result is flattened, and if either the source result is empty or the resulting result's outcome is unsuccessful, an result with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync<TValue>(this Result<TValue> option, Func<TValue, Task<Result>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.IfFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Result.Fail(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the value-result into an result with an outcome. The result is flattened, and if either the source result is empty or the resulting result's outcome is unsuccessful, an result with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="option">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync<TValue>(this Result<TValue> option, Func<TValue, Success, Task<Result>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.IfFail(e =>
                {
                    if (childError != null)
                    {
                        result = Result.Fail(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Result.Fail(childError != null ? childError.CausedBy(option.ErrorReason) : option.ErrorReason);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if either result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> FlatMapAsync<TResult>(this Task<Result> optionTask, Func<Result<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if either result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> FlatMapAsync<TResult>(this Task<Result> optionTask, Func<Success, Result<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if either result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> FlatMapAsync<TResult>(this Task<Result> optionTask, Func<Task<Result<TResult>>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the result into an result with a value. The result is flattened, and if either result's outcome is unsuccessful, an empty result is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result<TResult>> FlatMapAsync<TResult>(this Task<Result> optionTask, Func<Success, Task<Result<TResult>>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the value-result into an result with an outcome. The result is flattened, and if either the source result is empty or the resulting result's outcome is unsuccessful, an result with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue, Task<Result>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the value-result into an result with an outcome. The result is flattened, and if either the source result is empty or the resulting result's outcome is unsuccessful, an result with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="optionTask">The result to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The transformed result.</returns>
        public static async Task<Result> FlatMapAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue, Success, Task<Result>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// If the given result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Operation succeeded" (child success reason), Anteceded by: "Sub-operation successful" (antecedent successful result)</para>
        /// </summary>
        /// <param name="optionTask">The result task to source original error from.</param>
        /// <param name="childSuccess">The error object to attach the unsuccessful result's error object to.</param>
        public static async Task<Result> FlatMapSuccessAsync(this Task<Result> optionTask, Success childSuccess)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (childSuccess == null) throw new ArgumentNullException(nameof(childSuccess));

            var option = await optionTask;

            if (option.IsSuccess)
            {
                return Result.Success(childSuccess.AntecededBy(option));
            }

            return option;
        }

        /// <summary>
        /// If the given result is empty, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Created object successfully" (child success reason), Anteceded by: "Property has a value" (antecedent result with a value)</para>
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="childSuccess">The error object to attach the empty result's error object to.</param>
        public static async Task<Result<TValue>> FlatMapSuccessAsync<TValue>(this Task<Result<TValue>> optionTask, Success childSuccess)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (childSuccess == null) throw new ArgumentNullException(nameof(childSuccess));

            var option = await optionTask;

            if (option.IsSuccess)
            {
                return Result.Success(option.Value, childSuccess.AntecededBy(option));
            }

            return option;
        }

        /// <summary>
        /// If the given result's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent result with an unsuccessful outcome)</para>
        /// </summary>
        /// <param name="optionTask">The result task to source original error from.</param>
        /// <param name="childError">The error object to attach the unsuccessful result's error object to.</param>
        public static async Task<Result> FlatMapFailAsync(this Task<Result> optionTask, Error childError)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (childError == null) throw new ArgumentNullException(nameof(childError));

            var option = await optionTask;

            if (!option.IsSuccess)
            {
                return Result.Fail(childError.CausedBy(option));
            }

            return option;
        }

        /// <summary>
        /// If the given result is empty, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para>
        /// </summary>
        /// <param name="optionTask">The result task to transform.</param>
        /// <param name="childError">The error object to attach the empty result's error object to.</param>
        public static async Task<Result<TValue>> FlatMapFailAsync<TValue>(this Task<Result<TValue>> optionTask, Error childError)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (childError == null) throw new ArgumentNullException(nameof(childError));

            var option = await optionTask;

            if (!option.IsSuccess)
            {
                return Result.Fail<TValue>(childError.CausedBy(option));
            }

            return option;
        }

        /// <summary>
        /// Empties an result asynchronously and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="option">The option to filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the result was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The filtered result.</returns>
        public static async Task<Result<TValue>> FilterAsync<TValue>(this Result<TValue> option, Func<TValue, Task<bool>> predicate, Error predicateFailure, Error childError = null)
        {
            return await option.FilterAsync(predicate, v => predicateFailure, childError);
        }

        /// <summary>
        /// Empties an result asynchronously and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="option">The option to filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the result was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The filtered result.</returns>
        public static async Task<Result<TValue>> FilterAsync<TValue>(this Result<TValue> option, Func<TValue, Task<bool>> predicate, Func<TValue, Error> predicateFailure, Error childError = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var (value, _) in option)
            {
                var predicateTask = predicate(value);

                if (predicateTask == null) throw new InvalidOperationException("Predicate must not return a null task");

                return await predicateTask ? option : Result.Fail<TValue>(predicateFailure(value));
            }

            return Result.Fail<TValue>(childError?.CausedBy(option.ErrorReason));
        }

        /// <summary>
        /// Empties an result task asynchronously and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="optionTask">The option to filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the result was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The filtered result.</returns>
        public static async Task<Result<TValue>> FilterAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue, Task<bool>> predicate, Error predicateFailure, Error childError = null)
        {
            return await optionTask.FilterAsync(predicate, v => predicateFailure, childError);
        }

        /// <summary>
        /// Empties an result task asynchronously and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="optionTask">The option to filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the result was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
        /// <returns>The filtered result.</returns>
        public static async Task<Result<TValue>> FilterAsync<TValue>(this Task<Result<TValue>> optionTask, Func<TValue, Task<bool>> predicate, Func<TValue, Error> predicateFailure, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var option = await optionTask;

            foreach (var (value, _) in option)
            {
                var predicateTask = predicate(value);

                if (predicateTask == null) throw new InvalidOperationException("Predicate must not return a null task");

                return await predicateTask ? option : Result.Fail<TValue>(predicateFailure(value));
            }

            return Result.Fail<TValue>(childError?.CausedBy(option.ErrorReason));
        }

        /// <summary>
        /// Empties an result task and attaches an error object if the value is null.
        /// </summary>
        /// <param name="optionTask">The option to filter.</param>
        /// <param name="error">An error object with data describing why the result is missing its value.</param>
        /// <returns>The filtered result.</returns>
        public static async Task<Result<TValue>> NotNullAsync<TValue>(this Task<Result<TValue>> optionTask, Error error)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (error == null) throw new ArgumentNullException(nameof(error));

            var option = await optionTask;

            return option.IsSuccess && option.Value == null ? Result.Fail<TValue>(error) : option;
        }

        /// <summary>
        /// Flattens two nested results into one. The resulting result will be empty if either the inner or outer result is empty.
        /// </summary>
        /// <param name="nestedOptionTask">The nested result task.</param>
        /// <returns>A flattened result.</returns>
        public static async Task<Result<TValue>> FlattenAsync<TValue>(this Task<Result<Result<TValue>>> nestedOptionTask)
        {
            if (nestedOptionTask == null) throw new ArgumentNullException(nameof(nestedOptionTask));

            var option = await nestedOptionTask;

            return option.Flatten();
        }

        /// <summary>
        /// Evaluates a specified action if the result's outcome is succesful.
        /// </summary>
        /// <param name="success">The action to evaluate if the result's outcome is succesful.</param>
        public static async Task<Result> IfSuccessAsync(this Task<Result> result, Action<Success> success)
        {
            var option = await result;
            return option.IfSuccess(success);
        }

        /// <summary>
        /// Evaluates a specified action if outcome is unsuccesful.
        /// </summary>
        /// <param name="fail">The action to evaluate if outcome is unsuccesful.</param>
        public static async Task<Result> IfFailAsync(this Task<Result> result, Action<Error> fail)
        {
            var option = await result;
            return option.IfFail(fail);
        }

        /// <summary>
        /// Evaluates a specified action if the result's outcome is succesful.
        /// </summary>
        /// <param name="success">The action to evaluate if the result's outcome is succesful.</param>
        public static async Task<Result> IfSuccessAsync(this Task<Result> result, Func<Success, Task> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            var option = await result;

            if (option.IsSuccess)
            {
                await success(option.SuccessReason);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if outcome is unsuccesful.
        /// </summary>
        /// <param name="fail">The action to evaluate if outcome is unsuccesful.</param>
        public static async Task<Result> IfFailAsync(this Task<Result> result, Func<Error, Task> fail)
        {
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            var option = await result;

            if (!option.IsSuccess)
            {
                await fail(option.ErrorReason);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if the result's outcome is succesful.
        /// </summary>
        /// <param name="success">The action to evaluate if the result's outcome is succesful.</param>
        public static async Task<Result> IfSuccessAsync(this Result result, Func<Success, Task> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            var option = result;

            if (option.IsSuccess)
            {
                await success(option.SuccessReason);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if outcome is unsuccesful.
        /// </summary>
        /// <param name="fail">The action to evaluate if outcome is unsuccesful.</param>
        public static async Task<Result> IfFailAsync(this Result result, Func<Error, Task> fail)
        {
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            var option = result;

            if (!option.IsSuccess)
            {
                await fail(option.ErrorReason);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="success">The action to evaluate if the value is present.</param>
        public static async Task<Result<TValue>> IfSuccessAsync<TValue>(this Task<Result<TValue>> result, Action<TValue> success)
        {
            var option = await result;
            return option.IfSuccess(success);
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="success">The action to evaluate if the value is present.</param>
        public static async Task<Result<TValue>> IfSuccessAsync<TValue>(this Task<Result<TValue>> result, Action<TValue, Success> success)
        {
            var option = await result;
            return option.IfSuccess(success);
        }

        /// <summary>
        /// Evaluates a specified action if no value is present.
        /// </summary>
        /// <param name="fail">The action to evaluate if the value is missing.</param>
        public static async Task<Result<TValue>> IfFailAsync<TValue>(this Task<Result<TValue>> result, Action<Error> fail)
        {
            var option = await result;
            return option.IfFail(fail);
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="success">The action to evaluate if the value is present.</param>
        public static async Task<Result<TValue>> IfSuccessAsync<TValue>(this Task<Result<TValue>> result, Func<TValue, Task> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            var option = await result;

            if (option.IsSuccess)
            {
                await success(option.Value);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="success">The action to evaluate if the value is present.</param>
        public static async Task<Result<TValue>> IfSuccessAsync<TValue>(this Task<Result<TValue>> result, Func<TValue, Success, Task> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            var option = await result;

            if (option.IsSuccess)
            {
                await success(option.Value, option.SuccessReason);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if no value is present.
        /// </summary>
        /// <param name="fail">The action to evaluate if the value is missing.</param>
        public static async Task<Result<TValue>> IfFailAsync<TValue>(this Task<Result<TValue>> result, Func<Error, Task> fail)
        {
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            var option = await result;

            if (!option.IsSuccess)
            {
                await fail(option.ErrorReason);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="success">The action to evaluate if the value is present.</param>
        public static async Task<Result<TValue>> IfSuccessAsync<TValue>(this Result<TValue> result, Func<TValue, Task> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            var option = result;

            if (option.IsSuccess)
            {
                await success(option.Value);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="success">The action to evaluate if the value is present.</param>
        public static async Task<Result<TValue>> IfSuccessAsync<TValue>(this Result<TValue> result, Func<TValue, Success, Task> success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            var option = result;

            if (option.IsSuccess)
            {
                await success(option.Value, option.SuccessReason);
            }

            return option;
        }

        /// <summary>
        /// Evaluates a specified action if no value is present.
        /// </summary>
        /// <param name="fail">The action to evaluate if the value is missing.</param>
        public static async Task<Result<TValue>> IfFailAsync<TValue>(this Result<TValue> result, Func<Error, Task> fail)
        {
            if (fail == null) throw new ArgumentNullException(nameof(fail));

            var option = result;

            if (!option.IsSuccess)
            {
                await fail(option.ErrorReason);
            }

            return option;
        }
    }
}
