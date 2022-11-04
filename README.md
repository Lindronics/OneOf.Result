# OneOf.Result

[![.NET build, test, release](https://github.com/Lindronics/OneOf.Result/actions/workflows/build_and_release.yml/badge.svg)](https://github.com/Lindronics/OneOf.Result/actions/workflows/build_and_release.yml)

## What?

Adds a new `Result<TOk, TErr>` type built on top of the popular [OneOf](https://github.com/mcintyre321/OneOf) library.

Defines many useful methods and `Task` extensions allowing for chaining operations, similar to the `Result` type in Rust.

## Example usage

```csharp
    Result<Foo, Bar> GetSuccess() => new Foo();
    Result<Foo, Bar> GetError() => new Bar();
    
    async Task<Result<Foo, Bar>> GetSuccessAsync() => await Task.FromResult(new Foo());
    
    Fizz FooToFizz(Foo foo) => new Fizz();
    async Task<Foo> FizzToFooAsync(Fizz fizz) => await Task.FromResult<Foo>(new Foo());
    
    void DoSomething() {
        var fizz = GetSuccess()
            .Map(x => FooToFizz(x))
            .UnwrapOrElse(() => new Fizz());
            
        var buzz = GetError()
            .Map(x => FooToFizz(x)) // no-op
            .MapErr(x => new Buzz())
            .Match(
                ok => {
                    // do something
                },
                err => {
                    // do something
                } 
            );
    }
    
    async Task DoSomethingAsync() {
        var foo = await GetSuccessAsync()
            .Map(x => FooToFizz(x))
            .MapAsync(FizzToFooAsync)
            .Unwrap();
    }
```