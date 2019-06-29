namespace Ultimately.Async
{
    using Reasons;

    using System;
    using System.Threading.Tasks;


    public static class OptionTaskExtensions
    {
        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Option<TValue> option, Func<TValue, Task<TResult>> mapping, bool continueOnCapturedContext = false)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, _) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task.");

                var mappedValue = await valueTask.ConfigureAwait(continueOnCapturedContext);

                return mappedValue.Some();
            }

            return Optional.None<TResult>(option.Error);
        }

        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Option<TValue> option, Func<TValue, Success, Task<TResult>> mapping, bool continueOnCapturedContext = false)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (valueTask, _) in option.Map(mapping))
            {
                if (valueTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task.");

                var mappedValue = await valueTask.ConfigureAwait(continueOnCapturedContext);

                return mappedValue.Some();
            }

            return Optional.None<TResult>(option.Error);
        }

        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, TResult> mapping, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.Map(mapping);
        }

        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Success, TResult> mapping, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.Map(mapping);
        }

        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Task<TResult>> mapping, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return await option.MapAsync(mapping).ConfigureAwait(continueOnCapturedContext);
        }

        public static async Task<Option<TResult>> MapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Success, Task<TResult>> mapping, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return await option.MapAsync(mapping).ConfigureAwait(continueOnCapturedContext);
        }


        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Option<TValue> option, Func<TValue, Task<Option<TResult>>> mapping, bool continueOnCapturedContext = false)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task.");

                return await resultOptionTask.ConfigureAwait(continueOnCapturedContext);
            }

            return Optional.None<TResult>(option.Error);
        }

        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Option<TValue> option, Func<TValue, Success, Task<Option<TResult>>> mapping, bool continueOnCapturedContext = false)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            foreach (var (resultOptionTask, _) in option.Map(mapping))
            {
                if (resultOptionTask == null) throw new InvalidOperationException($"{nameof(mapping)} must not return a null task.");

                return await resultOptionTask.ConfigureAwait(continueOnCapturedContext);
            }

            return Optional.None<TResult>(option.Error);
        }

        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Option<TResult>> mapping, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.FlatMap(mapping);
        }

        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Success, Option<TResult>> mapping, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.FlatMap(mapping);
        }

        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Task<Option<TResult>>> mapping, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return await option.FlatMapAsync(mapping).ConfigureAwait(continueOnCapturedContext);
        }

        public static async Task<Option<TResult>> FlatMapAsync<TValue, TResult>(this Task<Option<TValue>> optionTask, Func<TValue, Success, Task<Option<TResult>>> mapping, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return await option.FlatMapAsync(mapping).ConfigureAwait(continueOnCapturedContext);
        }


        public static async Task<Option<TValue>> FilterAsync<TValue>(this Option<TValue> option, Func<TValue, Task<bool>> predicate, Error error, bool continueOnCapturedContext = false)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var (value, _) in option)
            {
                var predicateTask = predicate(value);

                if (predicateTask == null) throw new InvalidOperationException("Predicate must not return a null task.");

                var condition = await predicateTask.ConfigureAwait(continueOnCapturedContext);

                return condition ? option : Optional.None<TValue>(error);
            }

            return Optional.None<TValue>(error.CausedBy(option.Error));
        }

        public static async Task<Option<TValue>> FilterAsync<TValue>(this Task<Option<TValue>> optionTask, Predicate<TValue> predicate, Error error, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.Filter(predicate, error);
        }


        public static async Task<Option<TValue>> FilterAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue, Task<bool>> predicate, Error error, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return await option.FilterAsync(predicate, error).ConfigureAwait(continueOnCapturedContext);
        }


        public static async Task<Option<TValue>> NotNullAsync<TValue>(this Task<Option<TValue>> optionTask, Error error, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            return await optionTask.FilterAsync(value => value != null, error).ConfigureAwait(continueOnCapturedContext);
        }


        public static async Task<Option<TValue>> OrAsync<TValue>(this Option<TValue> option, Func<Task<TValue>> alternativeFactory, bool continueOnCapturedContext = false)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            foreach (var (_, _) in option)
            {
                return option;
            }

            var alternativeTask = alternativeFactory();
            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeFactory)} must not return a null task.");

            var alternative = await alternativeTask.ConfigureAwait(continueOnCapturedContext);

            return alternative.Some();
        }

        public static async Task<Option<TValue>> OrAsync<TValue>(this Option<TValue> option, Func<Task<TValue>> alternativeFactory, Success success, bool continueOnCapturedContext = false)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            foreach (var (_, _) in option)
            {
                return option;
            }

            var alternativeTask = alternativeFactory();
            if (alternativeTask == null) throw new InvalidOperationException($"{nameof(alternativeFactory)} must not return a null task.");

            var alternative = await alternativeTask.ConfigureAwait(continueOnCapturedContext);

            return alternative.Some(success);
        }

        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue> alternativeFactory, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.Or(alternativeFactory);
        }

        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<TValue> alternativeFactory, Success success, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.Or(alternativeFactory, success);
        }

        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Task<TValue>> alternativeFactory, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return await option.OrAsync(alternativeFactory).ConfigureAwait(continueOnCapturedContext);
        }

        public static async Task<Option<TValue>> OrAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Task<TValue>> alternativeFactory, Success success, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return await option.OrAsync(alternativeFactory, success).ConfigureAwait(continueOnCapturedContext);
        }


        public static async Task<Option<TValue>> ElseAsync<TValue>(this Option<TValue> option, Func<Task<Option<TValue>>> alternativeOptionFactory, bool continueOnCapturedContext = false)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            if (option.HasValue) return option;

            var alternativeOptionTask = alternativeOptionFactory();
            if (alternativeOptionTask == null) throw new InvalidOperationException($"{nameof(alternativeOptionFactory)} must not return a null task.");

            return await alternativeOptionTask.ConfigureAwait(continueOnCapturedContext);
        }

        public static async Task<Option<TValue>> ElseAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Option<TValue>> alternativeOptionFactory, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.Else(alternativeOptionFactory);
        }

        public static async Task<Option<TValue>> ElseAsync<TValue>(this Task<Option<TValue>> optionTask, Func<Task<Option<TValue>>> alternativeOptionFactory, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return await option.ElseAsync(alternativeOptionFactory).ConfigureAwait(continueOnCapturedContext);
        }


        public static async Task<Option<TValue>> FlattenAsync<TValue>(this Task<Option<Option<TValue>>> optionTask, bool continueOnCapturedContext = false)
        {
            if (optionTask == null) throw new ArgumentNullException(nameof(optionTask));

            var option = await optionTask.ConfigureAwait(continueOnCapturedContext);

            return option.Flatten();
        }
    }
}
