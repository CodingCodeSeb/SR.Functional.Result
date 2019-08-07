Ultimately: [Optional](https://github.com/nlkl/Optional) + [FluentResults](https://github.com/altmann/FluentResults) = ❤
-

![logo](https://github.com/silkfire/Ultimately/blob/master/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/Ultimately.svg)](https://www.nuget.org/packages/Ultimately)

**Ultimately** provides a robust either type with rich error and success models for C#.

## What and Why?

An `Option` is a strongly typed alternative to null values that lets you:

* Avoid those pesky null-reference exceptions
* Signal intent and model your data more explictly
* Cut down on manual null checks and focus on your domain

## Core concepts

The core concept behind Ultimately is derived from two common functional programming constructs – referred to as the *Unit* type and the *Either* type (referred to as `Option` and `Option<TValue>` in Ultimately).

Many functional programming languages disallow null values, as null references can introduce hard-to-find bugs. The Either and the Unit types are both a type-safe alternative to null values.

In general, an `Option`/`Option<TValue>` can be in one of two states: **Some** (representing a *successful* outcome in the case of a Unit optional and the *presence* of a value in the case of an Either optional) and **None** (representing an *unsuccessful* outcome – or the *lack* of a value – respectively). Unlike null, an option type forces the user to check if a value is actually present, thereby mitigating many of the problems of null values.

`Option` and `Option<TValue>` are structs in Ultimately, making it impossible to assign a null value to either of them.

Furthermore, an option type is a lot more explicit than a null value, which can make APIs based on optional values a lot easier to understand. Now, the type signature will indicate if a value can be missing!

The Unit type (`Option`) is a specialized Either type, conceptually the equivalent of `Option<bool>`. The only difference is that instead of containing a true/false value, you can verify the outcome by simply checking the `IsSuccessful` flag. The value itself has otherwise little value.

Finally, Ultimately offers several utility methods that make it easy and convenient to work with both of the above described optional types.

## Rich Reason models

*to be continued*
