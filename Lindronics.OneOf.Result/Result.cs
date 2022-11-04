using OneOf;
using OneOf.Types;

namespace Lindronics.OneOf.Result;

public static class Result
{
    public static Success<TOk> Ok<TOk>(TOk _) => new(_);
    public static Error<TErr> Error<TErr>(TErr _) => new(_);
}

public class Result<TOk, TErr> : OneOfBase<TOk, TErr>
{
    protected Result(OneOf<TOk, TErr> input) : base(input)
    {
    }

    public static implicit operator Result<TOk, TErr>(TOk _) => new(_);
    public static implicit operator Result<TOk, TErr>(TErr _) => new(_);

    public bool IsOk => IsT0;
    public bool IsError => IsT1;

    /// <summary>
    /// Applies <paramref name="f"/> to the enclosed Ok value, if present.
    /// Leaves the Err value untouched.
    /// </summary>
    /// <param name="f">A function mapping <typeparamref name="TOk"/> to <typeparamref name="TNew"/></param>
    /// <typeparam name="TNew">The type of the new Ok value.</typeparam>
    /// <returns>A new Result of the new Ok value and the existing error value.</returns>
    public Result<TNew, TErr> Map<TNew>(Func<TOk, TNew> f)
        => Match<Result<TNew, TErr>>(
            ok => f(ok),
            err => err
        );

    /// <summary>
    /// Async overload for <see cref="Map{TNew}"/>
    /// </summary>
    public async Task<Result<TNew, TErr>> MapAsync<TNew>(Func<TOk, Task<TNew>> f)
        => await Match<Task<Result<TNew, TErr>>>(
            async ok => await f(ok),
            async err => await Task.FromResult(err)
        );

    /// <summary>
    /// Applies <paramref name="f"/> to the enclosed Err value, if present.
    /// Leaves the Ok value untouched.
    /// </summary>
    /// <param name="f">A function mapping <typeparamref name="TErr"/> to <typeparamref name="TNew"/></param>
    /// <typeparam name="TNew">The type of the new Err value.</typeparam>
    /// <returns>A new Result of the existing Ok value and the new error value.</returns>
    public Result<TOk, TNew> MapErr<TNew>(Func<TErr, TNew> f)
        => Match<Result<TOk, TNew>>(
            ok => ok,
            err => f(err)
        );

    /// <summary>
    /// Async overload for <see cref="MapErr{TNew}"/>
    /// </summary>
    public async Task<Result<TOk, TNew>> MapErrAsync<TNew>(Func<TErr, Task<TNew>> f)
        => await Match<Task<Result<TOk, TNew>>>(
            async ok => await Task.FromResult(ok),
            async err => await f(err)
        );

    /// <summary>
    /// Applies <paramref name="f"/> to the enclosed Ok value, if present.
    /// Leaves the Err value untouched.
    /// </summary>
    /// <param name="f">A function operating on <typeparamref name="TOk"/>,
    /// yielding a new Result of the new Ok value and an error value of the type <typeparamref name="TErr"/></param>
    /// <typeparam name="TNew">The type of the new Ok value.</typeparam>
    /// <returns>A new Result of the existing Ok value and either the existing or the new error value.</returns>
    public Result<TNew, TErr> AndThen<TNew>(Func<TOk, Result<TNew, TErr>> f)
        => Match(
            f,
            err => err
        );

    /// <summary>
    /// Like <see cref="AndThen{TNew}"/>, but <paramref name="f"/> does not use the enclosed Ok value.
    /// </summary>
    public Result<TOk, TErr> AndThen(Func<Result<TOk, TErr>> f) => AndThen(_ => f());

    /// <summary>
    /// Async overload for <see cref="AndThen{TNew}"/>
    /// </summary>
    public async Task<Result<TNew, TErr>> AndThenAsync<TNew>(Func<TOk, Task<Result<TNew, TErr>>> f)
        => await Match(
            f,
            err => Task.FromResult(new Result<TNew, TErr>(err))
        );

    /// <summary>
    /// Async overload for <see cref="AndThen{TNew}"/>
    /// </summary>
    public async Task<Result<TNew, TErr>> AndThenAsync<TNew>(Func<Task<Result<TNew, TErr>>> f) =>
        await AndThenAsync(_ => f());

    /// <summary>
    /// Attempts to return the enclosed Ok value. May throw an exception if not present.
    /// </summary>
    public TOk Unwrap() => AsT0;

    /// <summary>
    /// Attempts to return the enclosed Err value. May throw an exception if not present.
    /// </summary>
    public TErr UnwrapErr() => AsT1;

    /// <summary>
    /// Returns the enclosed Ok value, or default if not present.
    /// </summary>
    public TOk? UnwrapOrDefault()
        => Match<TOk?>(
            ok => ok,
            err => default
        );

    /// <summary>
    /// Returns the enclosed Err value, or default if not present.
    /// </summary>
    public TErr? UnwrapErrOrDefault()
        => Match<TErr?>(
            ok => default,
            err => err
        );

    /// <summary>
    /// Returns the enclosed Err value, or a specified default if not present.
    /// <param name="defaultValue">The default value to return if the Ok value is not present.</param>
    /// </summary>
    public TOk UnwrapOr(TOk defaultValue)
        => Match(
            ok => ok,
            err => defaultValue
        );

    /// <summary>
    /// Returns the enclosed Err value. If not present, computes default from a function.
    /// <param name="f">The function to compute the default value from.</param>
    /// </summary>
    public TOk UnwrapOrElse(Func<TOk> f)
        => Match(
            ok => ok,
            err => f()
        );

    /// <summary>
    /// Async overload for <see cref="UnwrapOrElse"/>
    /// </summary>
    public async Task<TOk> UnwrapOrElseAsync(Func<Task<TOk>> f)
        => await Match(
            Task.FromResult,
            err => f()
        );

    /// <summary>
    /// Returns the enclosed Err value. If not present, computes default from a function.
    /// <param name="other"></param>
    /// </summary>
    public Result<TNew, TErr> And<TNew>(Result<TNew, TErr> other)
        => Match(
            ok => other,
            err => err
        );
}

public class Result<TOk> : Result<TOk, None>
{
    protected Result(OneOf<TOk, None> input) : base(input)
    {
    }
}