using OneOf.Types;

namespace OneOf.ResultMonad;

public class Result<TOk, TErr> : OneOfBase<Success<TOk>, Error<TErr>>
{
    protected Result(OneOf<Success<TOk>, Error<TErr>> input) : base(input)
    {
    }

    public static implicit operator Result<TOk, TErr>(TOk _) => new(new Success<TOk>(_));
    public static implicit operator Result<TOk, TErr>(TErr _) => new(new Error<TErr>(_));
    public static implicit operator Result<TOk, TErr>(Success<TOk> _) => new(_);
    public static implicit operator Result<TOk, TErr>(Error<TErr> _) => new(_);

    public static Result<TOk, TErr> Ok(TOk _) => new(new Success<TOk>(_));
    public static Result<TOk, TErr> Error(TErr _) => new(new Error<TErr>(_));

    public bool IsOk => IsT0;
    public bool IsError => IsT1;

    public Result<TOk, TErr> AsOk => AsT0;
    public Result<TOk, TErr> AsError => AsT1;

    /// <summary>
    /// If Ok, transforms <c>TOk</c> into <c>TNew</c> using function <param>f</param>, leaving <c>Err</c> untouched.
    /// </summary>
    public Result<TNew, TErr> Map<TNew>(Func<TOk, TNew> f)
        => Match<Result<TNew, TErr>>(
            ok => f(ok.Value),
            err => err
        );

    /// <summary>
    /// If Ok, calls function <param>f</param>, returning <c>Result<TNew, TErr></c>.
    /// </summary>
    public Result<TNew, TErr> AndThen<TNew>(Func<TOk, Result<TNew, TErr>> f)
        => Match(
            ok => f(ok.Value),
            err => err
        );

    /// <summary>
    /// If Error, transforms <c>TErr</c> into <c>TNew</c> using <param>f</param>.
    /// </summary>
    public Result<TOk, TNew> MapErr<TNew>(Func<TErr, TNew> f)
        => Match<Result<TOk, TNew>>(
            ok => ok,
            err => f(err.Value)
        );

    /// <summary>
    /// If Ok, returns <c>TOk</c>. 
    /// If Error, calls function <param>f</param>, returning <c>TOk</c>.
    /// </summary>
    public TOk OkOr(Func<TOk> f)
        => Match(
            ok => ok.Value,
            err => f()
        );

    /// <summary>
    /// If TOk, returns <c>TOk</c>. 
    /// If Error, returns <param>defaultValue</param>.
    /// </summary>
    public TOk OkOrDefault(TOk defaultValue)
        => Match(
            ok => ok.Value,
            err => defaultValue
        );

    /// <summary>
    /// If Ok, returns <param>other</param>.
    /// If Error, returns <c>TErr</c>;
    /// </summary>
    public Result<TNew, TErr> And<TNew>(Result<TNew, TErr> other)
        => Match(
            ok => other,
            err => err.Value
        );

    /// <summary>
    /// If Ok, returns <c>TOk</c>. 
    /// If Error, returns <c>null</c>;
    /// </summary>
    public TOk? ToNullable()
        => Match<TOk?>(
            ok => ok.Value,
            err => default(TOk)
        );
}

public class Option<TOk> : Result<TOk, None>
{
    protected Option(OneOf<Success<TOk>, Error<None>> input) : base(input)
    {
    }
}