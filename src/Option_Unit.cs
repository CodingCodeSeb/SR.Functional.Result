namespace Ultimately
{
    using Reasons;

    using System;
    using System.Collections.Generic;


    /// <summary>
    /// An optional that represents either a successful or an unsuccessful outcome.
    /// </summary>
    public readonly struct Option
    {
        /// <summary>
        /// Checks whether outcome is successful.
        /// </summary>
        public bool IsSuccessful { get; }

        internal Success Success { get; }
        internal Error Error { get; }


        internal Option(bool isSuccessful, Success success, Error error)
        {
            IsSuccessful = isSuccessful;
            Success = success;
            Error = error;
        }


        /// <summary>
        /// Returns an enumerator for the optional.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Success> GetEnumerator()
        {
            if (IsSuccessful)
            {
                yield return Success;
            }
        }

        /// <summary>
        /// Uses an alternative optional, if outcome is unsuccessful.
        /// </summary>
        /// <param name="alternativeOption">The alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Option Else(Option alternativeOption) => IsSuccessful ? this : alternativeOption;

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="alternativeOptionFactory">A factory function to create an alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Option Else(Func<Option> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));
            return IsSuccessful ? this : alternativeOptionFactory();
        }

        /// <summary>
        /// Evaluates a specified function, based on whether the optional's outcome is successful or not.
        /// </summary>
        /// <param name="some">The function to evaluate if the outcome is successful.</param>
        /// <param name="none">The function to evaluate if the outcome is unsuccessful.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<TResult> some, Func<Error, TResult> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            return IsSuccessful ? some() : none(Error);
        }

        /// <summary>
        /// Evaluates a specified function, based on whether the optional's outcome is successful or not.
        /// </summary>
        /// <param name="some">The function to evaluate if the outcome is successful.</param>
        /// <param name="none">The function to evaluate if the outcome is unsuccessful.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<Success, TResult> some, Func<Error, TResult> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            return IsSuccessful ? some(Success) : none(Error);
        }

        /// <summary>
        /// Evaluates a specified action, based on whether the optional's outcome is successful or not.
        /// </summary>
        /// <param name="some">The function to evaluate if the optional's outcome is succesful.</param>
        /// <param name="none">The function to evaluate if the optional's outcome is unsuccesful.</param>
        public void Match(Action some, Action<Error> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (IsSuccessful)
            {
                some();
            }
            else
            {
                none(Error);
            }
        }

        /// <summary>
        /// Evaluates a specified action, based on whether the optional's outcome is successful or not.
        /// </summary>
        /// <param name="some">The function to evaluate if the optional's outcome is succesful.</param>
        /// <param name="none">The function to evaluate if the optional's outcome is unsuccesful.</param>
        public void Match(Action<Success> some, Action<Error> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (IsSuccessful)
            {
                some(Success);
            }
            else
            {
                none(Error);
            }
        }

        /// <summary>
        /// Evaluates a specified action if the optional's outcome is succesful.
        /// </summary>
        /// <param name="some">The action to evaluate if the optional's outcome is succesful.</param>
        public void MatchSome(Action some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));

            if (IsSuccessful)
            {
                some();
            }
        }

        /// <summary>
        /// Evaluates a specified action if the optional's outcome is succesful.
        /// </summary>
        /// <param name="some">The action to evaluate if the optional's outcome is succesful.</param>
        public void MatchSome(Action<Success> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));

            if (IsSuccessful)
            {
                some(Success);
            }
        }

        /// <summary>
        /// Evaluates a specified action if outcome is unsuccesful.
        /// </summary>
        /// <param name="none">The action to evaluate if outcome is unsuccesful.</param>
        public void MatchNone(Action<Error> none)
        {
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (!IsSuccessful)
            {
                none(Error);
            }
        }

        /// <summary>
        /// Transforms the optional into another optional. The result is flattened, and if either optional's outcome is unsuccessful, an unsuccessful optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public Option FlatMap(Func<Option> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Option result;

            if (IsSuccessful)
            {
                result = mapping();

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
                result = Optional.None(childError != null ? childError.CausedBy(this) : Error);
            }

            return result;
        }

        /// <summary>
        /// Transforms the optional into another optional. The result is flattened, and if either optional's outcome is unsuccessful, an unsuccessful optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <param name="childError">If the resulting optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.<para>Example: "Operation failed" (child error), Caused by: "The name property cannot be null" (antecedent empty optional)</para></param>
        /// <returns>The transformed optional.</returns>
        public Option FlatMap(Func<Success, Option> mapping, Error childError = null)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Option result;

            if (IsSuccessful)
            {
                result = mapping(Success);

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
                result = Optional.None(childError != null ? childError.CausedBy(this) : Error);
            }

            return result;
        }

        /// <summary>
        /// If the current optional is successful, sets its success reason as the direct reason to the specified subsequent success reason.
        /// <para>Example: "Operation succeeded" (child success reason), Anteceded by: "Sub-operation successful" (antecedent successful optional)</para>
        /// </summary>
        /// <param name="childSuccess">The success object to attach the succesful optional's success object to.</param>
        public Option FlatMapSome(Success childSuccess)
        {
            if (childSuccess == null) throw new ArgumentNullException(nameof(childSuccess));

            if (!IsSuccessful)
            {
                return Optional.Some(childSuccess.AntecededBy(this));
            }

            return this;
        }

        /// <summary>
        /// If the current optional's outcome is unsuccessful, sets its error as the direct reason to the specified subsequent error.
        /// <para>Example: "Operation failed" (child error), Caused by: "Timed out while connecting to service" (antecedent optional with an unsuccessful outcome)</para>
        /// </summary>
        /// <param name="childError">The error object to attach the unsuccessful optional's error object to.</param>
        public Option FlatMapNone(Error childError)
        {
            if (childError == null) throw new ArgumentNullException(nameof(childError));

            if (!IsSuccessful)
            {
                return Optional.None(childError.CausedBy(this));
            }

            return this;
        }



        /// <summary>
        /// Returns a string that represents the current optional.
        /// </summary>
        /// <returns>A string that represents the current optional.</returns>
        public override string ToString()
        {
            if (IsSuccessful)
            {
                return $"Some({(Success.Message != "" || Success.Metadata.Count > 0 ? Success.ToString() : "")})";
            }

            return $"None{(Error != null ? $"(Error={Error})" : "")}";
        }
    }
}
