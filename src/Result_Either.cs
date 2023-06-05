namespace SR.Functional;

using Reasons;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Wraps an result value that may or may not exist depending on a predetermined set of business rules.
/// </summary>
/// <typeparam name="TValue">The type of the value to be wrapped.</typeparam>
[Serializable]
public readonly struct Result<TValue>
{
    /// <summary>
    /// Checks if a value is present.
    /// </summary>
    public bool IsSuccess { get; }

    public TValue Value { get; }
    public Success SuccessReason { get; }
    public Error ErrorReason { get; }

    internal Result(bool hasValue, TValue value, Success success, Error error)
    {
        IsSuccess = hasValue;
        Value = value;
        SuccessReason = success;
        ErrorReason = error;
    }

    public static implicit operator Result<TValue>(TValue value)
        => Result.Success(value);

    public static implicit operator Result<TValue>(Error error)
        => Result.Fail<TValue>(error);

    public static implicit operator Result<TValue>(string error)
        => Result.Fail<TValue>(error);

    public static implicit operator Result<TValue>(Exception exception)
        => Result.Fail<TValue>(ExceptionalError.Create(exception));

    /// <summary>
    /// Converts the current result into an enumerable with one or zero elements.
    /// </summary>
    /// <returns>A corresponding enumerable.</returns>
    public IEnumerable<TValue> ToEnumerable()
    {
        if (IsSuccess)
        {
            yield return Value;
        }
    }

    /// <summary>
    /// Returns an enumerator for the result.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<(TValue Value, Success Success)> GetEnumerator()
    {
        if (IsSuccess)
        {
            yield return (Value, SuccessReason);
        }
    }

    /// <summary>
    /// Determines if the current result contains a specified value.
    /// </summary>
    /// <param name="value">The value to locate.</param>
    /// <returns>A boolean indicating whether or not the value was found.</returns>
    public bool Contains(TValue value)
    {
        if (IsSuccess)
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
    /// Determines if the current result contains a value satisfying a specified predicate.
    /// </summary>
    /// <param name="predicate">A predicate to test the result value against.</param>
    /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
    public bool Exists(Predicate<TValue> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return IsSuccess && predicate(Value);
    }

    /// <summary>
    /// Returns the existing value if present, or otherwise an alternative value.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    public TValue ValueOr(TValue alternative) => IsSuccess ? Value : alternative;

    /// <summary>
    /// Returns the existing value if present, or otherwise an alternative value.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    public TValue ValueOr(Func<TValue> alternativeFactory)
    {
        if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

        return IsSuccess ? Value : alternativeFactory();
    }

    /// <summary>
    /// Uses an alternative value if no existing value is present.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <returns>A new result, containing either the existing or alternative value.</returns>
    public Result<TValue> Or(TValue alternative) => IsSuccess ? this : Result.Success(alternative);

    /// <summary>
    /// Uses an alternative value if no existing value is present and attaches the specified success object.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative result value.</param>
    /// <returns>A new result, containing either the existing or alternative value.</returns>
    public Result<TValue> Or(TValue alternative, Success success) => IsSuccess ? this : Result.Success(alternative, success);

    /// <summary>
    /// Uses an alternative value if no existing value is present.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
    /// <returns>A new result, containing either the existing or alternative value.</returns>
    public Result<TValue> Or(Func<TValue> alternativeFactory)
    {
        if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

        return IsSuccess ? this : Result.Success(alternativeFactory());
    }

    /// <summary>
    /// Uses an alternative value if no existing value is present and attaches the specified success object.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
    /// <param name="success">An object with data describing the reason or origin behind the presence of the alternative result value.</param>
    /// <returns>A new result, containing either the existing or alternative value.</returns>
    public Result<TValue> Or(Func<TValue> alternativeFactory, Success success)
    {
        if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));

        return IsSuccess ? this : Result.Success(alternativeFactory(), success);
    }

    /// <summary>
    /// Uses an alternative result, if no existing value is present.
    /// </summary>
    /// <param name="alternativeOption">The alternative result.</param>
    /// <returns>The alternative result, if no value is present, otherwise itself.</returns>
    public Result<TValue> Else(Result<TValue> alternativeOption) => IsSuccess ? this : alternativeOption;

    /// <summary>
    /// Uses an alternative result, if no existing value is present.
    /// </summary>
    /// <param name="alternativeOptionFactory">A factory function to create an alternative result.</param>
    /// <returns>The alternative result, if no value is present, otherwise itself.</returns>
    public Result<TValue> Else(Func<Result<TValue>> alternativeOptionFactory)
    {
        if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));

        return IsSuccess ? this : alternativeOptionFactory();
    }

    /// <summary>
    /// Evaluates a specified function, based on whether a value is present or not.
    /// </summary>
    /// <param name="success">The function to evaluate if the value is present.</param>
    /// <param name="fail">The function to evaluate if the value is missing.</param>
    /// <returns>The result of the evaluated function.</returns>
    public TResult Match<TResult>(Func<TValue, TResult> success, Func<Error, TResult> fail)
    {
        if (success == null) throw new ArgumentNullException(nameof(success));
        if (fail == null) throw new ArgumentNullException(nameof(fail));

        return IsSuccess ? success(Value) : fail(ErrorReason);
    }

    /// <summary>
    /// Evaluates a specified function, based on whether a value is present or not.
    /// </summary>
    /// <param name="success">The function to evaluate if the value is present.</param>
    /// <param name="fail">The function to evaluate if the value is missing.</param>
    /// <returns>The result of the evaluated function.</returns>
    public TResult Match<TResult>(Func<TValue, Success, TResult> success, Func<Error, TResult> fail)
    {
        if (success == null) throw new ArgumentNullException(nameof(success));
        if (fail == null) throw new ArgumentNullException(nameof(fail));

        return IsSuccess ? success(Value, SuccessReason) : fail(ErrorReason);
    }

    /// <summary>
    /// Evaluates a specified action, based on whether a value is present or not.
    /// </summary>
    /// <param name="success">The action to evaluate if the value is present.</param>
    /// <param name="fail">The action to evaluate if the value is missing.</param>
    public void Match(Action<TValue> success, Action<Error> fail)
    {
        if (success == null) throw new ArgumentNullException(nameof(success));
        if (fail == null) throw new ArgumentNullException(nameof(fail));

        if (IsSuccess)
        {
            success(Value);
        }
        else
        {
            fail(ErrorReason);
        }
    }

    /// <summary>
    /// Evaluates a specified action, based on whether a value is present or not.
    /// </summary>
    /// <param name="success">The action to evaluate if the value is present.</param>
    /// <param name="fail">The action to evaluate if the value is missing.</param>
    public void Match(Action<TValue, Success> success, Action<Error> fail)
    {
        if (success == null) throw new ArgumentNullException(nameof(success));
        if (fail == null) throw new ArgumentNullException(nameof(fail));

        if (IsSuccess)
        {
            success(Value, SuccessReason);
        }
        else
        {
            fail(ErrorReason);
        }
    }

    /// <summary>
    /// Evaluates a specified action if a value is present.
    /// </summary>
    /// <param name="success">The action to evaluate if the value is present.</param>
    public Result<TValue> IfSuccess(Action<TValue> success)
    {
        if (success == null) throw new ArgumentNullException(nameof(success));

        if (IsSuccess)
        {
            success(Value);
        }

        return this;
    }

    /// <summary>
    /// Evaluates a specified action if a value is present.
    /// </summary>
    /// <param name="success">The action to evaluate if the value is present.</param>
    public Result<TValue> IfSuccess(Action<TValue, Success> success)
    {
        if (success == null) throw new ArgumentNullException(nameof(success));

        if (IsSuccess)
        {
            success(Value, SuccessReason);
        }

        return this;
    }

    /// <summary>
    /// Evaluates a specified action if no value is present.
    /// </summary>
    /// <param name="fail">The action to evaluate if the value is missing.</param>
    public Result<TValue> IfFail(Action<Error> fail)
    {
        if (fail == null) throw new ArgumentNullException(nameof(fail));

        if (!IsSuccess)
        {
            fail(ErrorReason);
        }

        return this;
    }

    /// <summary>
    /// Transforms the inner value of an result. If the instance is empty, an empty result is returned.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
    /// <returns>The transformed result.</returns>
    public Result<TResult> Map<TResult>(Func<TValue, TResult> mapping, Error childError = null)
    {
        if (mapping == null) throw new ArgumentNullException(nameof(mapping));

        Func<Error, Result<TResult>> errorFunc = Result.Fail<TResult>;

        if (childError != null)
        {
            errorFunc = e => Result.Fail<TResult>(childError.CausedBy(e));
        }

        return Match(
            success: v => Result.Success(mapping(v)),
            fail: errorFunc
        );
    }

    /// <summary>
    /// Transforms the inner value of an result. If the instance is empty, an empty result is returned.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
    /// <returns>The transformed result.</returns>
    public Result<TResult> Map<TResult>(Func<TValue, Success, TResult> mapping, Error childError = null)
    {
        if (mapping == null) throw new ArgumentNullException(nameof(mapping));

        Func<Error, Result<TResult>> errorFunc = Result.Fail<TResult>;

        if (childError != null)
        {
            errorFunc = e => Result.Fail<TResult>(childError.CausedBy(e));
        }

        return Match(
            success: (v, s) => Result.Success(mapping(v, s)),
            fail: errorFunc
        );
    }

    /// <summary>
    /// Transforms the inner value of an result into another result. The result is flattened, and if either is empty, an empty result is returned.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
    /// <returns>The transformed result.</returns>
    public Result<TResult> FlatMap<TResult>(Func<TValue, Result<TResult>> mapping, Error childError = null)
    {
        if (mapping == null) throw new ArgumentNullException(nameof(mapping));

        Result<TResult> result;

        if (IsSuccess)
        {
            result = mapping(Value);

            result.IfFail(e =>
            {
                if (childError != null)
                {
                    result = Result.Fail<TResult>(childError.CausedBy(e));
                }
            });
        }
        else
        {
            result = Result.Fail<TResult>(childError != null ? childError.CausedBy(ErrorReason) : ErrorReason);
        }

        return result;
    }

    /// <summary>
    /// Transforms the inner value of an result into another result. The result is flattened, and if either is empty, an empty result is returned.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <param name="childError">If the resulting result is empty, sets its error as the direct reason to the specified subsequent error.<para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
    /// <returns>The transformed result.</returns>
    public Result<TResult> FlatMap<TResult>(Func<TValue, Success, Result<TResult>> mapping, Error childError = null)
    {
        if (mapping == null) throw new ArgumentNullException(nameof(mapping));

        Result<TResult> result;

        if (IsSuccess)
        {
            result = mapping(Value, SuccessReason);

            result.IfFail(e =>
            {
                if (childError != null)
                {
                    result = Result.Fail<TResult>(childError.CausedBy(e));
                }
            });
        }
        else
        {
            result = Result.Fail<TResult>(childError != null ? childError.CausedBy(ErrorReason) : ErrorReason);
        }

        return result;
    }

    /// <summary>
    /// If the current result has a value, sets its success reason as the direct reason to the specified subsequent success reason.
    /// <para>Example: "Created object successfully" (child success reason), Anteceded by: "Property has a value" (antecedent result with a value)</para>
    /// </summary>
    /// <param name="childSuccess">The success object to attach the non-empty result's success object to.</param>
    public Result<TValue> FlatMapSome(Success childSuccess)
    {
        if (childSuccess == null) throw new ArgumentNullException(nameof(childSuccess));

        if (!IsSuccess)
        {
            return Result.Success(Value, childSuccess.AntecededBy(this));
        }

        return this;
    }

    /// <summary>
    /// If the current result is empty, sets its error as the direct reason to the specified subsequent error.
    /// <para>Example: "Failed to create object" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para>
    /// </summary>
    /// <param name="childError">The error object to attach the empty result's error object to.</param>
    public Result<TValue> FlatMapNone(Error childError)
    {
        if (childError == null) throw new ArgumentNullException(nameof(childError));

        if (!IsSuccess)
        {
            return Result.Fail<TValue>(childError.CausedBy(this));
        }

        return this;
    }

    /// <summary>
    /// Empties an result and attaches an error object if the specified predicate is not satisfied.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
    /// <param name="childError">An error object describing that the predicate potentially failed to execute because the result was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
    /// <returns>The filtered result.</returns>
    public Result<TValue> Filter(Predicate<TValue> predicate, Error predicateFailure, Error childError = null)
    {
        return Filter(predicate, v => predicateFailure, childError);
    }

    /// <summary>
    /// Empties an result and attaches an error object if the specified predicate is not satisfied.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="predicateFailure">An error object describing why the predicate failed.<para>Example: "Value must be greater than 10. Was 2."</para></param>
    /// <param name="childError">An error object describing that the predicate potentially failed to execute because the result was empty.<para>Example: "Predicate never executed" (child error), Caused by: "The name property cannot be null" (antecedent empty result)</para></param>
    /// <returns>The filtered result.</returns>
    public Result<TValue> Filter(Predicate<TValue> predicate, Func<TValue, Error> predicateFailure, Error childError = null)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        foreach (var (value, _) in this)
        {
            return predicate(value) ? this : Result.Fail<TValue>(predicateFailure(value));
        }

        return Result.Fail<TValue>(childError?.CausedBy(ErrorReason) ?? ErrorReason);
    }

    /// <summary>
    /// Empties an result and attaches an error object if the value is null.
    /// </summary>
    /// <param name="error">An error object with data describing why the result is missing its value.</param>
    /// <returns>The filtered result.</returns>
    public Result<TValue> NotNull(Error error)
    {
        if (error == null) throw new ArgumentNullException(nameof(error));

        return IsSuccess && Value == null ? Result.Fail<TValue>(error) : this;
    }


    /// <summary>
    /// Returns a string that represents the current result.
    /// </summary>
    /// <returns>A string that represents the current result.</returns>
    public override string ToString()
    {
        if (IsSuccess)
        {
            string valueString = "";

            if (Value == null) valueString = "null";
            else
            {
                if (Value is ICollection c) valueString = $"Count = {c.Count}";
                else if (typeof(TValue) == typeof(IReadOnlyCollection<>))
                {
                    var valueType = Value.GetType().GetInterfaces().Single(i => i == typeof(IReadOnlyCollection<>));

                    valueString = $"Count = {valueType.GetProperty("Count").GetValue(valueType)}";
                }
                else
                {
                    valueString = Value.ToString();
                }
            }

            return $"Success({valueString}{(SuccessReason.Message != "" || SuccessReason.Metadata.Count > 0 ? $" | {SuccessReason}" : "")})";
        }

        return $"Fail{(ErrorReason != null ? $"(Error={ErrorReason})" : "")}";
    }
}
