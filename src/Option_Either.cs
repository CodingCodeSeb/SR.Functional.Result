namespace Ultimately
{
    using Reasons;

    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Wraps an optional value that may or may not exist depending on a predetermined set of business rules.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be wrapped.</typeparam>
    [Serializable]
    public struct Option<TValue>
    {
        /// <summary>
        /// Checks if a value is present.
        /// </summary>
        public bool HasValue { get; }

        internal TValue Value { get; }
        internal Success Success { get; }
        internal Error Error { get; }

        internal Option(bool hasValue, TValue value, Success success, Error error)
        {
            HasValue = hasValue;
            Value = value;
            Success = success;
            Error = error;
        }


        /// <summary>
        /// Converts the current optional into an enumerable with one or zero elements.
        /// </summary>
        /// <returns>A corresponding enumerable.</returns>
        public IEnumerable<TValue> ToEnumerable()
        {
            if (HasValue)
            {
                yield return Value;
            }
        }

        /// <summary>
        /// Returns an enumerator for the optional.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<(TValue Value, Success Success)> GetEnumerator()
        {
            if (HasValue)
            {
                yield return (Value, Success);
            }
        }

        /// <summary>
        /// Determines if the current optional contains a specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>A boolean indicating whether or not the value was found.</returns>
        public bool Contains(TValue value)
        {
            if (HasValue)
            {
                if (Value == null)
                {
                    return value == null;
                }

                return Value.Equals(value);
            }

            return false;
        }

        /// <summary>
        /// Determines if the current optional contains a value satisfying a specified predicate.
        /// </summary>
        /// <param name="predicate">A predicate to test the optional value against.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public bool Exists(Predicate<TValue> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return HasValue && predicate(Value);
        }

        /// <summary>
        /// Returns the existing value if present, or otherwise an alternative value.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public TValue ValueOr(TValue alternative) => HasValue ? Value : alternative;

        /// <summary>
        /// Returns the existing value if present, or otherwise an alternative value.
        /// </summary>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public TValue ValueOr(Func<TValue> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            return HasValue ? Value : alternativeFactory();
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public Option<TValue> Or(TValue alternative) => HasValue ? this : Optional.Some(alternative);

        /// <summary>
        /// Uses an alternative value if no existing value is present and attaches the specified success object.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative optional value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public Option<TValue> Or(TValue alternative, Success success) => HasValue ? this : Optional.Some(alternative, success);

        /// <summary>
        /// Uses an alternative value if no existing value is present.
        /// </summary>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public Option<TValue> Or(Func<TValue> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            return HasValue ? this : Optional.Some(alternativeFactory());
        }

        /// <summary>
        /// Uses an alternative value if no existing value is present and attaches the specified success object.
        /// </summary>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative optional value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public Option<TValue> Or(Func<TValue> alternativeFactory, Success success)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

            return HasValue ? this : Optional.Some(alternativeFactory(), success);
        }

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="alternativeOption">The alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Option<TValue> Else(Option<TValue> alternativeOption) => HasValue ? this : alternativeOption;

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="alternativeOptionFactory">A factory function to create an alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Option<TValue> Else(Func<Option<TValue>> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

            return HasValue ? this : alternativeOptionFactory();
        }

        /// <summary>
        /// Evaluates a specified function, based on whether a value is present or not.
        /// </summary>
        /// <param name="some">The function to evaluate if the value is present.</param>
        /// <param name="none">The function to evaluate if the value is missing.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<TValue, TResult> some, Func<Error, TResult> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            return HasValue ? some(Value) : none(Error);
        }

        /// <summary>
        /// Evaluates a specified function, based on whether a value is present or not.
        /// </summary>
        /// <param name="some">The function to evaluate if the value is present.</param>
        /// <param name="none">The function to evaluate if the value is missing.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<TValue, Success, TResult> some, Func<Error, TResult> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            return HasValue ? some(Value, Success) : none(Error);
        }

        /// <summary>
        /// Evaluates a specified action, based on whether a value is present or not.
        /// </summary>
        /// <param name="some">The action to evaluate if the value is present.</param>
        /// <param name="none">The action to evaluate if the value is missing.</param>
        public void Match(Action<TValue> some, Action<Error> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (HasValue)
            {
                some(Value);
            }
            else
            {
                none(Error);
            }
        }

        /// <summary>
        /// Evaluates a specified action, based on whether a value is present or not.
        /// </summary>
        /// <param name="some">The action to evaluate if the value is present.</param>
        /// <param name="none">The action to evaluate if the value is missing.</param>
        public void Match(Action<TValue, Success> some, Action<Error> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (HasValue)
            {
                some(Value, Success);
            }
            else
            {
                none(Error);
            }
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="some">The action to evaluate if the value is present.</param>
        public void MatchSome(Action<TValue> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));

            if (HasValue)
            {
                some(Value);
            }
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="some">The action to evaluate if the value is present.</param>
        public void MatchSome(Action<TValue, Success> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));

            if (HasValue)
            {
                some(Value, Success);
            }
        }

        /// <summary>
        /// Evaluates a specified action if no value is present.
        /// </summary>
        /// <param name="none">The action to evaluate if the value is missing.</param>
        public void MatchNone(Action<Error> none)
        {
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (!HasValue)
            {
                none(Error);
            }
        }

        /// <summary>
        /// Transforms the inner value of an optional. If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public Option<TResult> Map<TResult>(Func<TValue, TResult> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Func<Error, Option<TResult>> errorFunc = Optional.None<TResult>;

            if (childError != null)
            {
                errorFunc = e => Optional.None<TResult>(childError.CausedBy(e));
            }

            return Match(
                some: v => Optional.Some(mapping(v)),
                none: errorFunc
            );
        }

        /// <summary>
        /// Transforms the inner value of an optional. If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public Option<TResult> Map<TResult>(Func<TValue, Success, TResult> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Func<Error, Option<TResult>> errorFunc = Optional.None<TResult>;

            if (childError != null)
            {
                errorFunc = e => Optional.None<TResult>(childError.CausedBy(e));
            }

            return Match(
                some: (v, s) => Optional.Some(mapping(v, s)),
                none: errorFunc
            );
        }

        /// <summary>
        /// Transforms the inner value of an optional into another optional. The result is flattened, and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public Option<TResult> FlatMap<TResult>(Func<TValue, Option<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Option<TResult> result;

            if (HasValue)
            {
                result = mapping(Value);

                result.MatchNone(e =>
                {
                    if (childError != null)
                    {
                        result = Optional.None<TResult>(childError.CausedBy(e));
                    }
                });
            }
            else
            {
                result = Optional.None<TResult>(childError != null ? childError.CausedBy(Error) : Error);
            }

            return result;
        }

        /// <summary>
        /// Transforms the inner value of an optional into another optional. The result is flattened, and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public Option<TResult> FlatMap<TResult>(Func<TValue, Success, Option<TResult>> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Option<TResult> result;

            if (HasValue)
            {
                result = mapping(Value, Success);

                result.MatchNone(e =>
                {
                    if (childError != null)
                    {
                        result = Optional.None<TResult>(childError.CausedBy(e));
                    }
                });
            }
            else
            {
                result = Optional.None<TResult>(childError != null ? childError.CausedBy(Error) : Error);
            }

            return result;
        }

        /// <summary>
        /// If the current optional is empty, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para>
        /// </summary>
        /// <param name="childError">The error object to attach the empty optional's error object to.</param>
        public Option<TValue> FlatMapNone(Error childError)
        {
            if (childError == null) throw new ArgumentNullException(nameof(childError));

            if (!HasValue)
            {
                return Optional.None<TValue>(childError.CausedBy(this));
            }

            return this;
        }

        /// <summary>
        /// Empties an optional and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the optional was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The filtered optional.</returns>
        public Option<TValue> Filter(Predicate<TValue> predicate, Error predicateFailure, Error childError = null)
        {
            return Filter(predicate, v => predicateFailure, childError);
        }

        /// <summary>
        /// Empties an optional and attaches an error object if the specified predicate is not satisfied.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
        /// <param name="childError">An error object describing that the predicate potentially failed to execute because the optional was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The filtered optional.</returns>
        public Option<TValue> Filter(Predicate<TValue> predicate, Func<TValue, Error> predicateFailure, Error childError = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var (value, _) in this)
            {
                return predicate(value) ? this : Optional.None<TValue>(predicateFailure(value));
            }

            return Optional.None<TValue>(childError?.CausedBy(Error) ?? Error);
        }

        /// <summary>
        /// Empties an optional and attaches an error object if the value is null.
        /// </summary>
        /// <param name="error">An error object with data describing why the optional is missing its value.</param>
        /// <returns>The filtered optional.</returns>
        public Option<TValue> NotNull(Error error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            return HasValue && Value == null ? Optional.None<TValue>(error) : this;
        }


        /// <summary>
        /// Returns a string that represents the current optional.
        /// </summary>
        /// <returns>A string that represents the current optional.</returns>
        public override string ToString()
        {
            if (HasValue)
            {
                return $"Some({Value}{(Success.Message != "" || Success.Metadata.Count > 0 ? $" | {Success}" : "")})";
            }

            return $"None{(Error != null ? $"(Error={Error})" : "")}";
        }
    }
}
