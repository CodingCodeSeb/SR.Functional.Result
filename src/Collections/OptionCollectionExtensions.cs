﻿// Note: Several of the below implementations are closely inspired by the corefx source code for FirstOrDefault, etc.

namespace Ultimately.Collections
{
    using Reasons;
    using Utilities;

    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class OptionCollectionExtensions
    {

        /// <summary>
        /// Flattens a sequence of optionals into a sequence containing all inner values.
        /// Empty elements are discarded.
        /// </summary>
        /// <param name="source">The sequence of optionals.</param>
        /// <returns>A flattened sequence of values.</returns>
        public static IEnumerable<TValue> Values<TValue>(this IEnumerable<Option<TValue>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var option in source)
            {
                foreach (var (value, _) in option)
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Flattens a sequence of optionals into a sequence containing all exceptional values.
        /// Non-empty elements and their values are discarded.
        /// </summary>
        /// <param name="source">The sequence of optionals.</param>
        /// <returns>A flattened sequence of exceptional values.</returns>
        public static IEnumerable<Error> Errors<TValue>(this IEnumerable<Option<TValue>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var option in source)
            {
                if (!option.HasValue)
                {
                    yield return option.Error;
                }
            }
        }

        /// <summary>
        /// Returns the value associated with the specified key if such exists.
        /// A dictionary lookup will be used if available, otherwise falling
        /// back to a linear scan of the enumerable.
        /// </summary>
        /// <param name="source">The dictionary or enumerable in which to locate the key.</param>
        /// <param name="key">The key to locate.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An <see cref="Option{TValue}"/> instance containing the associated value if located.</returns>
        public static Option<TValue> GetValueOrNone<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key, Error error = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is IDictionary<TKey, TValue> dictionary)
            {
                return dictionary.TryGetValue(key, out var value) ? value.Some() : Optional.None<TValue>(error ?? Error.Create(MissingReasons.KeyNotFound));
            }

            if (source is IReadOnlyDictionary<TKey, TValue> readOnlyDictionary)
            {
                return readOnlyDictionary.TryGetValue(key, out var value) ? value.Some() : Optional.None<TValue>(error ?? Error.Create(MissingReasons.KeyNotFound));
            }

            return source
                .FirstOrNone(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key), Error.Create(MissingReasons.KeyNotFoundIndexer))
                .Map(pair => pair.Value);
        }

        /// <summary>
        /// Returns the first element of a sequence if such exists.
        /// </summary>
        /// <param name="source">The sequence to return the first element from.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An <see cref="Option{TValue}"/> instance containing the first element if present.</returns>
        public static Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Error error = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is IList<TSource> list)
            {
                if (list.Count > 0)
                {
                    return list[0].Some();
                }
            }
            else if (source is IReadOnlyList<TSource> readOnlyList)
            {
                if (readOnlyList.Count > 0)
                {
                    return readOnlyList[0].Some();
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        return enumerator.Current.Some();
                    }
                }
            }

            return Optional.None<TSource>(error ?? Error.Create(MissingReasons.CollectionWasEmpty));
        }

        /// <summary>
        /// Returns the first element of a sequence, satisfying a specified predicate, 
        /// if such exists.
        /// </summary>
        /// <param name="source">The sequence to return the first element from.</param>
        /// <param name="predicate">The predicate to filter by.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An <see cref="Option{TValue}"/> instance containing the first element if present.</returns>
        public static Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate, Error error = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var element in source)
            {
                if (predicate(element))
                {
                    return element.Some();
                }
            }

            return Optional.None<TSource>(error ?? Error.Create(MissingReasons.NoElementsFound));
        }

        /// <summary>
        /// Returns the last element of a sequence if such exists.
        /// </summary>
        /// <param name="source">The sequence to return the last element from.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An <see cref="Option{TValue}"/> instance containing the last element if present.</returns>
        public static Option<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Error error = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is IList<TSource> list)
            {
                var count = list.Count;
                if (count > 0)
                {
                    return list[count - 1].Some();
                }
            }
            else if (source is IReadOnlyList<TSource> readOnlyList)
            {
                var count = readOnlyList.Count;
                if (count > 0)
                {
                    return readOnlyList[count - 1].Some();
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        TSource result;
                        do
                        {
                            result = enumerator.Current;
                        }
                        while (enumerator.MoveNext());

                        return result.Some();
                    }
                }
            }

            return Optional.None<TSource>(error ?? Error.Create(MissingReasons.CollectionWasEmpty));
        }

        /// <summary>
        /// Returns the last element of a sequence, satisfying a specified predicate, 
        /// if such exists.
        /// </summary>
        /// <param name="source">The sequence to return the last element from.</param>
        /// <param name="predicate">The predicate to filter by.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An <see cref="Option{TValue}"/> instance containing the last element if present.</returns>
        public static Option<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate, Error error = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (source is IList<TSource> list)
            {
                for (var i = list.Count - 1; i >= 0; --i)
                {
                    var result = list[i];
                    if (predicate(result))
                    {
                        return result.Some();
                    }
                }
            }
            else if (source is IReadOnlyList<TSource> readOnlyList)
            {
                for (var i = readOnlyList.Count - 1; i >= 0; --i)
                {
                    var result = readOnlyList[i];
                    if (predicate(result))
                    {
                        return result.Some();
                    }
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var result = enumerator.Current;
                        if (predicate(result))
                        {
                            while (enumerator.MoveNext())
                            {
                                var element = enumerator.Current;
                                if (predicate(element))
                                {
                                    result = element;
                                }
                            }

                            return result.Some();
                        }
                    }
                }
            }

            return Optional.None<TSource>(error ?? Error.Create(MissingReasons.NoElementsFound));
        }

        /// <summary>
        /// Returns a single element from a sequence, if it exists 
        /// and is the only element in the sequence.
        /// </summary>
        /// <param name="source">The sequence to return the element from.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An <see cref="Option{TValue}"/> instance containing the element if present.</returns>
        public static Option<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source, Error error = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is IList<TSource> list)
            {
                switch (list.Count)
                {
                    case 0: return Optional.None<TSource>(error ?? Error.Create(MissingReasons.CollectionWasEmpty));
                    case 1: return list[0].Some();
                }
            }
            else if (source is IReadOnlyList<TSource> readOnlyList)
            {
                switch (readOnlyList.Count)
                {
                    case 0: return Optional.None<TSource>(error ?? Error.Create(MissingReasons.CollectionWasEmpty));
                    case 1: return readOnlyList[0].Some();
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                    {
                        return Optional.None<TSource>(error ?? Error.Create(MissingReasons.CollectionWasEmpty));
                    }

                    var result = enumerator.Current;
                    if (!enumerator.MoveNext())
                    {
                        return result.Some();
                    }
                }
            }

            return Optional.None<TSource>(error ?? Error.Create(MissingReasons.CollectionWasEmpty));
        }

        /// <summary>
        /// Returns a single element from a sequence, satisfying a specified predicate, 
        /// if it exists and is the only element in the sequence.
        /// </summary>
        /// <param name="source">The sequence to return the element from.</param>
        /// <param name="predicate">The predicate to filter by.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An <see cref="Option{TValue}"/> instance containing the element if present.</returns>
        public static Option<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate, Error error = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var result = enumerator.Current;
                    if (predicate(result))
                    {
                        while (enumerator.MoveNext())
                        {
                            if (predicate(enumerator.Current))
                            {
                                return Optional.None<TSource>(error ?? Error.Create(MissingReasons.NoElementsFound));
                            }
                        }

                        return result.Some();
                    }
                }
            }

            return Optional.None<TSource>(error ?? Error.Create(MissingReasons.NoElementsFound));
        }

        /// <summary>
        /// Returns an element at a specified position in a sequence if such exists.
        /// </summary>
        /// <param name="source">The sequence to return the element from.</param>
        /// <param name="index">The index in the sequence.</param>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>An <see cref="Option{TValue}"/> instance containing the element if found.</returns>
        public static Option<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, int index, Error error = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (index >= 0)
            {
                if (source is IList<TSource> list)
                {
                    if (index < list.Count)
                    {
                        return list[index].Some();
                    }
                }
                else if (source is IReadOnlyList<TSource> readOnlyList)
                {
                    if (index < readOnlyList.Count)
                    {
                        return readOnlyList[index].Some();
                    }
                }
                else
                {
                    using (var enumerator = source.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (index == 0)
                            {
                                return enumerator.Current.Some();
                            }

                            index--;
                        }
                    }
                }
            }

            return Optional.None<TSource>(error ?? Error.Create(MissingReasons.IndexNotFound));
        }

        /// <summary>
        /// Aggregates a sequence of options into one result, short-circuiting as soon as an empty optional is hit.
        /// </summary>
        /// <typeparam name="TValue">The type of the optional value.</typeparam>
        /// <param name="optionsCollection">Sequence of <see cref="Option{T}"/>s to reduce.</param>
        public static Option<TValue> Reduce<TValue>(this IEnumerable<Option<TValue>> optionsCollection)
        {
            return optionsCollection.Aggregate((o1, o2) => o1.FlatMap(_ => o2));
        }

        /// <summary>
        /// Aggregates a sequence of options into one result, short-circuiting as soon as an empty optional is hit.
        /// </summary>
        /// <param name="optionsCollection">Sequence of <see cref="Option"/>s to reduce.</param>
        public static Option Reduce(this IEnumerable<Option> optionsCollection)
        {
            return optionsCollection.Aggregate((o1, o2) => o1.FlatMap(_ => o2));
        }

        /// <summary>
        /// Aggregates a sequence of deferred options into one result, short-circuiting as soon as an empty optional is hit.
        /// </summary>
        /// <param name="lazyOptionsCollection">Sequence of <see cref="LazyOption"/>s to reduce.</param>
        public static Option Reduce(this IEnumerable<LazyOption> lazyOptionsCollection)
        {
            return lazyOptionsCollection.Aggregate(Optional.Some(), (v1, v2) => v1.FlatMap(_ => v2.Resolve()));
        }


        /// <summary>
        /// Transforms each item in a sequence according to the specified option-returning transformation function, short-circuiting as soon as a function iteration results in an empty optional.
        /// </summary>
        /// <typeparam name="TSource">The type of the source item to transform.</typeparam>
        /// <typeparam name="TResult">The type of the resulting optional value.</typeparam>
        /// <param name="source">Sequence of items to transform.</param>
        /// <param name="func">Transformation function to apply to each item of the sequence.</param>
        public static Option<IEnumerable<TResult>> Transform<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> func)
        {
            Option<TResult>? failedTransformation = null;


            var transformationResult = Transform();

            if (failedTransformation.HasValue)
            {
                return Optional.None<IEnumerable<TResult>>(failedTransformation.Value.Error);
            }


            return Optional.Some(transformationResult);



            IEnumerable<TResult> Transform()
            {
                foreach (var item in source)
                {
                    var result = func(item);

                    if (result.HasValue)
                    {
                        yield return result.Value;
                    }
                    else
                    {
                        failedTransformation = result;

                        yield break;
                    }
                }
            }
        }
    }
}
