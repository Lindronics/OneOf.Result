namespace Lindronics.OneOf.Result;

public static class TaskExtensions
{
    public static async Task<Result<TNew, TErr>> Map<TOk, TErr, TNew>(
        this Task<Result<TOk, TErr>> result,
        Func<TOk, TNew> f)
        => await result.ContinueWith(r => r.Result.Map(f));

    public static async Task<Result<TNew, TErr>> MapAsync<TOk, TErr, TNew>(
        this Task<Result<TOk, TErr>> result,
        Func<TOk, Task<TNew>> f) =>
        await await result.ContinueWith(r => r.Result.MapAsync(f));

    public static async Task<Result<TOk, TNew>> MapErr<TOk, TErr, TNew>(
        this Task<Result<TOk, TErr>> result,
        Func<TErr, TNew> f)
        => await result.ContinueWith(r => r.Result.MapErr(f));

    public static async Task<Result<TOk, TNew>> MapErrAsync<TOk, TErr, TNew>(
        this Task<Result<TOk, TErr>> result,
        Func<TErr, Task<TNew>> f) =>
        await await result.ContinueWith(r => r.Result.MapErrAsync(f));

    public static async Task<Result<TNew, TErr>> AndThen<TOk, TErr, TNew>(this Task<Result<TOk, TErr>> result,
        Func<TOk, Result<TNew, TErr>> f)
        => await result.ContinueWith(r => r.Result.AndThen(f));

    public static async Task<Result<TNew, TErr>> AndThenAsync<TOk, TErr, TNew>(this Task<Result<TOk, TErr>> result,
        Func<TOk, Task<Result<TNew, TErr>>> f)
        => await await result.ContinueWith(r => r.Result.AndThenAsync(f));

    public static async Task<TOk> Unwrap<TOk, TErr>(this Task<Result<TOk, TErr>> result)
        => await result.ContinueWith(r => r.Result.Unwrap());
    
    public static async Task<TErr> UnwrapErr<TOk, TErr>(this Task<Result<TOk, TErr>> result)
        => await result.ContinueWith(r => r.Result.UnwrapErr());

    public static async Task<TOk> UnwrapOr<TOk, TErr>(this Task<Result<TOk, TErr>> result, TOk defaultValue)
        => await result.ContinueWith(r => r.Result.UnwrapOr(defaultValue));

    public static async Task<TOk> UnwrapOrElse<TOk, TErr>(this Task<Result<TOk, TErr>> result, Func<TOk> f)
        => await result.ContinueWith(r => r.Result.UnwrapOrElse(f));

    public static async Task<TOk> UnwrapOrElseAsync<TOk, TErr>(this Task<Result<TOk, TErr>> result, Func<Task<TOk>> f)
        => await await result.ContinueWith(r => r.Result.UnwrapOrElseAsync(f));

    public static async Task<TOk?> UnwrapOrNull<TOk, TErr>(this Task<Result<TOk, TErr>> result)
        => await result.ContinueWith(r => r.Result.UnwrapOrNull());
}