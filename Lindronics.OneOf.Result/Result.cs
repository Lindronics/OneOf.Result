using OneOf;
using OneOf.Types;

namespace Lindronics.OneOf.Result;

public static class Result
{
    public static Success<TOk> Ok<TOk>(TOk _) => new(_);
    public static Error<TErr> Error<TErr>(TErr _) => new(_);
}

public class Result<TOk, TErr> : OneOfBase<Success<TOk>, Error<TErr>>
{
    protected Result(OneOf<Success<TOk>, Error<TErr>> input) : base(input)
    {
    }

    public static implicit operator Result<TOk, TErr>(TOk _) => new(new Success<TOk>(_));
    public static implicit operator Result<TOk, TErr>(TErr _) => new(new Error<TErr>(_));
    public static implicit operator Result<TOk, TErr>(Success<TOk> _) => new(_);
    public static implicit operator Result<TOk, TErr>(Error<TErr> _) => new(_);

    public bool IsOk => IsT0;
    public bool IsError => IsT1;

    public TOk AsOk => AsT0.Value;
    public TErr AsError => AsT1.Value;

    public Result<TNew, TErr> Map<TNew>(Func<TOk, TNew> f)
        => Match<Result<TNew, TErr>>(
            ok => f(ok.Value),
            err => err
        );

    public async Task<Result<TNew, TErr>> MapAsync<TNew>(Func<TOk, Task<TNew>> f)
        => await Match<Task<Result<TNew, TErr>>>(
            async ok => await f(ok.Value),
            async err => await Task.FromResult(err)
        );

    public Result<TOk, TNew> MapErr<TNew>(Func<TErr, TNew> f)
        => Match<Result<TOk, TNew>>(
            ok => ok,
            err => f(err.Value)
        );

    public async Task<Result<TOk, TNew>> MapErrAsync<TNew>(Func<TErr, Task<TNew>> f)
        => await Match<Task<Result<TOk, TNew>>>(
            async ok => await Task.FromResult(ok),
            async err => await f(err.Value)
        );

    public Result<TNew, TErr> AndThen<TNew>(Func<TOk, Result<TNew, TErr>> f)
        => Match(
            ok => f(ok.Value),
            err => err
        );

    public Result<TOk, TErr> AndThen(Func<Result<TOk, TErr>> f) => AndThen(_ => f());

    public async Task<Result<TNew, TErr>> AndThenAsync<TNew>(Func<TOk, Task<Result<TNew, TErr>>> f)
        => await Match(
            ok => f(ok.Value),
            err => Task.FromResult(new Result<TNew, TErr>(err))
        );

    public async Task<Result<TNew, TErr>> AndThenAsync<TNew>(Func<Task<Result<TNew, TErr>>> f) =>
        await AndThenAsync(_ => f());

    public TOk Unwrap() => AsOk;
    
    public TErr UnwrapErr() => AsError;

    public TOk UnwrapOr(TOk defaultValue)
        => Match(
            ok => ok.Value,
            err => defaultValue
        );

    public TOk UnwrapOrElse(Func<TOk> f)
        => Match(
            ok => ok.Value,
            err => f()
        );

    public async Task<TOk> UnwrapOrElseAsync(Func<Task<TOk>> f)
        => await Match(
            ok => Task.FromResult(ok.Value),
            err => f()
        );
    
    public TOk? UnwrapOrNull()
        => Match<TOk?>(
            ok => ok.Value,
            err => default(TOk)
        );

    // public async Task<Result<TOk, TErr>> UnwrapOrElseAsync(Func<Task<Result<TOk, TErr>>> f)
    //     => await Match(
    //         ok => Task.FromResult(this),
    //         err => f()
    //     );

    public Result<TNew, TErr> And<TNew>(Result<TNew, TErr> other)
        => Match(
            ok => other,
            err => err.Value
        );


}

public class Result<TOk> : Result<TOk, None>
{
    protected Result(OneOf<Success<TOk>, Error<None>> input) : base(input)
    {
    }
}