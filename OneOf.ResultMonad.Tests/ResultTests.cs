using result;
using Xunit;

namespace OneOf.ResultMonad.Tests;

public class ResultTests
{
    [Fact]
    public void Test1()
    {
        var x = CreateApple("gala")
            .Map(ToFruit)
            .AndThen(ToOrange);

        Assert.IsError

    }

    private record Apple(string name);
    private record Orange(string name);
    private record Fruit(string name);

    private enum CustomError
    {
        ConversionError

    };

    private Result<Apple, CustomError> CreateApple(string name) => new Apple(name);
    private Fruit ToFruit(Apple apple) => new Fruit(apple.name);
    private Result<Orange, CustomError> ToOrange(Fruit fruit) => CustomError.ConversionError;

}
