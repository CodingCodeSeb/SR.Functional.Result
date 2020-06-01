namespace Ultimately.Async
{
    using Reasons;

    using System;
    using System.Threading.Tasks;


    public static class OptionTaskExtensions
    {
        /// <summary>
        /// Determines if the optional task contains a specified value asynchronously.
        /// </summary>
        /// <param name="optionTask">The optional task to check for the specified value.</param>
        /// <param name="value">The value to locate. Set to null to check whether the optional's' value is null.</param>
        /// <returns>A boolean indicating whether or not the value was found.</returns>
        public static async Task<bool> ContainsAsync<TValue>(this Task<Option<TValue>> optionTask, TValue value)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            var option = await optionTask;

            if (option.HasValue)
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
        /// Determines if the optional contains a value satisfying a specified predicate asynchronously.
        /// </summary>
        /// <param name="option">The optional task to test.</param>
        /// <param name="predicate">A predicate to test the optional value against.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public static async Task<bool> ExistsAsync<TValue>(this Option<TValue> option, Func<TValue, Task<bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (option.HasValue)
            {
                var predicateTask = predicate(option.Value);

                if (predicateTask == null) throw new InvalidOperationException($"{nameof(predicateTask)} must not return a null task");

                return await predicateTask;
            }

            return false;
        }

        /// <summary>
        /// Determines if the current optional contains a value satisfying a specified predicate asynchronously.
        /// </summary>
        /// <param name="optionTask">The optional task to test.</param>
        /// <param name="predicate">A predicate to test the optional value against.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public static async Task<bool> ExistsAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue, bool> predicate)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var option = await optionTask;

            return option.HasValue && predicate(option.Value);
        }

        /// <summary>
        /// Determines if the current optional task contains a value satisfying a specified predicate asynchronously.
        /// </summary>
        /// <param name="optionTask">The optional task to test.</param>
        /// <param name="predicate">A predicate to test the optional value against.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public static async Task<bool> ExistsAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue, Task<bool>> predicate)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var option = await optionTask;

            if (option.HasValue)
            {
                var predicateTask = predicate(option.Value);

                if (predicateTask == null) throw new InvalidOperationException($"{nameof(predicateTask)} must not return a null task");

                return await predicateTask;
            }

            return false;
        }

        /// <summary>
        /// Returns the specified optional task's existing value if present, or otherwise an alternative value.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public static async Task<TValue> ValueOrAsync<TValue>(this Task<Option<TValue>> optionTask, TValue alternative)
        {
            var option = await optionTask;

            return option.HasValue ? option.Value : alternative;
        }

        /// <summary>
        /// Returns the specified optional's existing value if present, or otherwise an alternative value by calling the specified factory delegate task.
        /// </summary>
        /// <param name="option">The optional to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The existing or alternative value.</returns>
        public static async Task<TValue> ValueOrAsync<TValue>(this Option<TValue> option, Func<Task<TValue>> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            if (option.HasValue)
            {
                return option.Value;
            }

            var alternativeTask = alternativeFactory();

            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeTask)} must not return a null task");

            return await alternativeTask;
        }

        /// <summary>
        /// Returns the specified optional task's existing value if present, or otherwise an alternative value by calling the specified factory delegate.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public static async Task<TValue> ValueOrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return option.HasValue ? option.Value : alternativeFactory();
        }

        /// <summary>
        /// Returns the specified optional task's existing value if present, or otherwise an alternative value by calling the specified factory delegate task.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The existing or alternative value.</returns>
        public static async Task<TValue> ValueOrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Task<TValue>> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            if (option.HasValue)
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
        /// <param name="option">The optional to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public static async Task<Option<TValue>> OrAsync<TValue>(this Option<TValue> option, Func<Task<TValue>> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            if (option.HasValue)
            {
                return option;
            }

            var alternativeTask = alternativeFactory();
            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeFactory)} must not return a null task");

            var alternative = await alternativeTask;

            return alternative.Some();
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="option">The optional to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative optional value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public static async Task<Option<TValue>> OrAsync<TValue>(this Option<TValue> option, Func<Task<TValue>> alternativeFactory, Success success)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            if (option.HasValue)
            {
                return option;
            }

            var alternativeTask = alternativeFactory();
            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeFactory)} must not return a null task");

            var alternative = await alternativeTask;

            return alternative.Some(success);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, TValue alternative)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            var option = await optionTask;

            if (option.HasValue)
            {
                return option;
            }

            return alternative.Some();
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternative">The alternative value.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative optional value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, TValue alternative, Success success)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            var option = await optionTask;

            if (option.HasValue)
            {
                return option;
            }

            return alternative.Some(success);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return option.Or(alternativeFactory);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative optional value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue> alternativeFactory, Success success)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return option.Or(alternativeFactory, success);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Task<TValue>> alternativeFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return await option.OrAsync(alternativeFactory);
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to attempt to source inner value from.</param>
        /// <param name="alternativeFactory">A factory function to create an alternative value asynchronously.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative optional value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Task<TValue>> alternativeFactory, Success success)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask;

            return await option.OrAsync(alternativeFactory, success);
        }

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="option">The optional to test.</param>
        /// <param name="alternativeOptionFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public static async Task<Option<TValue>> ElseAsync<TValue>(this Option<TValue> option, Func<Task<Option<TValue>>> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            if (option.HasValue) return option;

            var alternativeOptionTask = alternativeOptionFactory();
            if (alternativeOptionTask == null) throw new InvalidOperationException($"{nameof(alternativeOptionFactory)} must not return a null task");

            return await alternativeOptionTask;
        }

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to test.</param>
        /// <param name="alternativeOption">The alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public static async Task<Option<TValue>> ElseAsync<TValue>(this Task<Option<TValue>> optionTask, Option<TValue> alternativeOption)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            var option = await optionTask;

            if (option.HasValue)
            {
                return option;
            }

            return alternativeOption;
        }

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to test.</param>
        /// <param name="alternativeOptionFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public static async Task<Option<TValue>> ElseAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Option<TValue>> alternativeOptionFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            var option = await optionTask;

            return option.Else(alternativeOptionFactory);
        }

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="optionTask">The optional task to test.</param>
        /// <param name="alternativeOptionFactory">A factory function to create an alternative value asynchronously.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public static async Task<Option<TValue>> ElseAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Task<Option<TValue>>> alternativeOptionFactory)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            var option = await optionTask;

            return await option.ElseAsync(alternativeOptionFactory);
        }

        /// <summary>
        /// Transforms the inner value of an optional asynchronously. If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Option<TValue> option, Func<TValue, Task<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, _) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var mappedValue = await valueTask;

                return mappedValue.Some();
            }

            return Optional.None<TResult>(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the inner value of an optional asynchronously. If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Option<TValue> option, Func<TValue, Success, Task<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, _) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var mappedValue = await valueTask;

                return mappedValue.Some();
            }

            return Optional.None<TResult>(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the inner value of an optional task asynchronously. If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, TResult> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.Map(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an optional task asynchronously. If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Success, TResult> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.Map(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an optional task asynchronously. If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Task<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.MapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an optional task asynchronously. If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Success, Task<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.MapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if the original optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TResult>(this Option option, Func<Task<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, success) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var mappedValue = await valueTask;

                return mappedValue.Some(success);
            }

            return Optional.None<TResult>(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if the original optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TResult>(this Option option, Func<Success, Task<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, success) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var mappedValue = await valueTask;

                return mappedValue.Some(success);
            }

            return Optional.None<TResult>(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if the original optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TResult>(this Task<Option> optionTask, Func<Task<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.MapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if the original optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TResult>(this Task<Option> optionTask, Func<Success, Task<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.MapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if the original optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TResult>(this Task<Option> optionTask, Func<TResult> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.Map(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional task into an optional with a value. The result is flattened, and if the original optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> MapAsync<TResult>(this Task<Option> optionTask, Func<Success, TResult> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.Map(mapping, childError);
        }



        /// <summary>
        /// Transforms the optional into another optional task asynchronously. The result is flattened, and if either optional's outcome is unsuccessful, an unsuccessful optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync(this Option option, Func<Task<Option>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Option result;

            if (option.IsSuccessful)
            {
                result = await mapping();

                if (!result.IsSuccessful)
                {
                    if (childError != null)
                    {
                        result = Optional.None(childError.CausedBy(result));
                    }
                }
            }
            else
            {
                result = Optional.None(childError != null ? childError.CausedBy(option.Error) : option.Error);
            }

            return result;
        }

        /// <summary>
        /// Transforms the optional into another optional task asynchronously. The result is flattened, and if either optional's outcome is unsuccessful, an unsuccessful optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync(this Option option, Func<Success, Task<Option>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Option result;

            if (option.IsSuccessful)
            {
                result = await mapping(option.Success);

                if (!result.IsSuccessful)
                {
                    if (childError != null)
                    {
                        result = Optional.None(childError.CausedBy(result));
                    }
                }
            }
            else
            {
                result = Optional.None(childError != null ? childError.CausedBy(option.Error) : option.Error);
            }

            return result;
        }

        /// <summary>
        /// Transforms the optional task into another optional asynchronously. The result is flattened, and if either optional's outcome is unsuccessful, an unsuccessful optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync(this Task<Option> optionTask, Func<Option> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional task into another optional asynchronously. The result is flattened, and if either optional's outcome is unsuccessful, an unsuccessful optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync(this Task<Option> optionTask, Func<Success, Option> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional task into another optional task asynchronously. The result is flattened, and if either optional's outcome is unsuccessful, an unsuccessful optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync(this Task<Option> optionTask, Func<Task<Option>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional task into another optional task asynchronously. The result is flattened, and if either optional's outcome is unsuccessful, an unsuccessful optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync(this Task<Option> optionTask, Func<Success, Task<Option>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an optional into another optional asynchronously. The result is flattened, and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Option<TValue> option, Func<TValue, Task<Option<TResult>>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.MatchNone(e =>
                {
                    if (childError != null)
                    {
                        result = Optional.None<TResult>(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Optional.None<TResult>(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the inner value of an optional into another optional asynchronously. The result is flattened, and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Option<TValue> option, Func<TValue, Success, Task<Option<TResult>>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.MatchNone(e =>
                {
                    if (childError != null)
                    {
                        result = Optional.None<TResult>(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Optional.None<TResult>(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the inner value of an optional task into another optional asynchronously. The result is flattened, and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Option<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an optional task into another optional asynchronously. The result is flattened, and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Success, Option<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an optional task into another task optional asynchronously. The result is flattened, and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Task<Option<TResult>>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the inner value of an optional task into another task optional asynchronously. The result is flattened, and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Success, Task<Option<TResult>>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if either optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> FlatMapAsync<TResult>(this Option option, Func<Task<Option<TResult>>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.MatchNone(e =>
                {
                    if (childError != null)
                    {
                        result = Optional.None<TResult>(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Optional.None<TResult>(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if either optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> FlatMapAsync<TResult>(this Option option, Func<Success, Task<Option<TResult>>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.MatchNone(e =>
                {
                    if (childError != null)
                    {
                        result = Optional.None<TResult>(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Optional.None<TResult>(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the value-optional into an optional with an outcome. The result is flattened, and if either the source optional is empty or the resulting optional's outcome is unsuccessful, an optional with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync<TValue>(this Option<TValue> option, Func<TValue, Task<Option>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.MatchNone(e =>
                {
                    if (childError != null)
                    {
                        result = Optional.None(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Optional.None(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }

        /// <summary>
        /// Transforms the value-optional into an optional with an outcome. The result is flattened, and if either the source optional is empty or the resulting optional's outcome is unsuccessful, an optional with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="option">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync<TValue>(this Option<TValue> option, Func<TValue, Success, Task<Option>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task");

                var result = await resultOptionTask;

                result.MatchNone(e =>
                {
                    if (childError != null)
                    {
                        result = Optional.None(childError.CausedBy(e));
                    }
                });

                return result;
            }

            return Optional.None(childError != null ? childError.CausedBy(option.Error) : option.Error);
        }



        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if either optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> FlatMapAsync<TResult>(this Task<Option> optionTask, Func<Option<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if either optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> FlatMapAsync<TResult>(this Task<Option> optionTask, Func<Success, Option<TResult>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return option.FlatMap(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if either optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> FlatMapAsync<TResult>(this Task<Option> optionTask, Func<Task<Option<TResult>>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the optional into an optional with a value. The result is flattened, and if either optional's outcome is unsuccessful, an empty optional is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option<TResult>> FlatMapAsync<TResult>(this Task<Option> optionTask, Func<Success, Task<Option<TResult>>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// Transforms the value-optional into an optional with an outcome. The result is flattened, and if either the source optional is empty or the resulting optional's outcome is unsuccessful, an optional with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue, Task<Option>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }
        
        /// <summary>
        /// Transforms the value-optional into an optional with an outcome. The result is flattened, and if either the source optional is empty or the resulting optional's outcome is unsuccessful, an optional with an unsuccessful outcome is returned.
        /// </summary>
        /// <param name="optionTask">The optional to transform.</param>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public static async Task<Option> FlatMapAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue, Success, Task<Option>> mapping, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask;

            return await option.FlatMapAsync(mapping, childError);
        }

        /// <summary>
        /// If the given optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Operation succeeded" (child success reason), Anteceded by: "Sub-operation successful" (antecedent successful optional)</para>
        /// </summary>
        /// <param name="optionTask">The optional task to source original error from.</param>
        /// <param name="childSuccess">The error object to attach the unsuccessful optional's error object to.</param>
        public static async Task<Option> FlatMapSomeAsync(this Task<Option> optionTask, Success childSuccess)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (childSuccess == null) throw new ArgumentNullException(nameof(childSuccess));

            var option = await optionTask;

            if (option.IsSuccessful)
            {
                return Optional.Some(childSuccess.AntecededBy(option));
            }

            return option;
        }

        /// <summary>
        /// If the given optional is empty, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Created object successfully" (child success reason), Anteceded by: "Property has a value" (antecedent optional with a value)</para>
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="childSuccess">The error object to attach the empty optional's error object to.</param>
        public static async Task<Option<TValue>> FlatMapSomeAsync<TValue>(this Task<Option<TValue>> optionTask, Success childSuccess)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (childSuccess == null) throw new ArgumentNullException(nameof(childSuccess));

            var option = await optionTask;

            if (option.HasValue)
            {
                return Optional.Some(option.Value, childSuccess.AntecededBy(option));
            }

            return option;
        }

        /// <summary>
        /// If the given optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para>
        /// </summary>
        /// <param name="optionTask">The optional task to source original error from.</param>
        /// <param name="childError">The error object to attach the unsuccessful optional's error object to.</param>
        public static async Task<Option> FlatMapNoneAsync(this Task<Option> optionTask, Error childError)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (childError == null) throw new ArgumentNullException(nameof(childError));

            var option = await optionTask;

            if (!option.IsSuccessful)
            {
                return Optional.None(childError.CausedBy(option));
            }

            return option;
        }

        /// <summary>
        /// If the given optional is empty, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para>
        /// </summary>
        /// <param name="optionTask">The optional task to transform.</param>
        /// <param name="childError">The error object to attach the empty optional's error object to.</param>
        public static async Task<Option<TValue>> FlatMapNoneAsync<TValue>(this Task<Option<TValue>> optionTask, Error childError)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (childError == null) throw new ArgumentNullException(nameof(childError));

            var option = await optionTask;

            if (!option.HasValue)
            {
                return Optional.None<TValue>(childError.CausedBy(option));
            }

            return option;
        }

        /// <summary>
        /// Empties an optional asynchronously and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="option">The option to filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the optional was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The filtered optional.</returns>
        public static async Task<Option<TValue>> FilterAsync<TValue>(this Option<TValue> option, Func<TValue, Task<bool>> predicate, Error predicateFailure, Error childError = null)
        {
            return await option.FilterAsync(predicate, v => predicateFailure, childError);
        }

        /// <summary>
        /// Empties an optional asynchronously and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="option">The option to filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the optional was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The filtered optional.</returns>
        public static async Task<Option<TValue>> FilterAsync<TValue>(this Option<TValue> option, Func<TValue, Task<bool>> predicate, Func<TValue, Error> predicateFailure, Error childError = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var (value, _) in option)
            {
                var predicateTask = predicate(value);

                if (predicateTask == null) throw new InvalidOperationException("Predicate must not return a null task");

                return await predicateTask ? option : Optional.None<TValue>(predicateFailure(value));
            }

            return Optional.None<TValue>(childError?.CausedBy(option.Error));
        }

        /// <summary>
        /// Empties an optional task asynchronously and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="optionTask">The option to filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the optional was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The filtered optional.</returns>
        public static async Task<Option<TValue>> FilterAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue, Task<bool>> predicate, Error predicateFailure, Error childError = null)
        {
            return await optionTask.FilterAsync(predicate, v => predicateFailure, childError);
        }

        /// <summary>
        /// Empties an optional task asynchronously and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="optionTask">The option to filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the optional was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The filtered optional.</returns>
        public static async Task<Option<TValue>> FilterAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue, Task<bool>> predicate, Func<TValue, Error> predicateFailure, Error childError = null)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var option = await optionTask;

            foreach (var (value, _) in option)
            {
                var predicateTask = predicate(value);

                if (predicateTask == null) throw new InvalidOperationException("Predicate must not return a null task");

                return await predicateTask ? option : Optional.None<TValue>(predicateFailure(value));
            }

            return Optional.None<TValue>(childError?.CausedBy(option.Error));
        }

        /// <summary>
        /// Empties an optional task and attaches an error object if the value is null.
        /// </summary>
        /// <param name="optionTask">The option to filter.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>The filtered optional.</returns>
        public static async Task<Option<TValue>> NotNullAsync<TValue>(this Task<Option<TValue>> optionTask, Error error)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (error == null) throw new ArgumentNullException(nameof(error));

            var option = await optionTask;

            return option.HasValue && option.Value == null ? Optional.None<TValue>(error) : option;
        }

        /// <summary>
        /// Flattens two nested optionals into one. The resulting optional will be empty if either the inner or outer optional is empty.
        /// </summary>
        /// <param name="nestedOptionTask">The nested optional task.</param>
        /// <returns>A flattened optional.</returns>
        public static async Task<Option<TValue>> FlattenAsync<TValue>(this Task<Option<Option<TValue>>> nestedOptionTask)
        {
            if (nestedOptionTask == null) throw new ArgumentNullException(nameof(nestedOptionTask));

            var option = await nestedOptionTask;

            return option.Flatten();
        }
    }
}
