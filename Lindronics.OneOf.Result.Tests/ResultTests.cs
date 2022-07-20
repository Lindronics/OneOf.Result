using FluentAssertions;
using Xunit;

namespace Lindronics.OneOf.Result.Tests;

public class ResultTests
{
    [Fact]
    public void Test1()
    {
        var x = CreateApple("gala")
            .Map(ToFruit)
            .AndThen(ToOrange);

        x.IsError.Should().BeTrue();
        x.AsError.Should().Be(CustomError.ConversionError);
    }

    private record Apple(string Name);

    private record Orange(string Name);

    private record Fruit(string Name);

    private enum CustomError
    {
        ConversionError
    };

    private static Result<Apple, CustomError> CreateApple(string name) => new Apple(name);
    private static Fruit ToFruit(Apple apple) => new(apple.Name);
    private static Result<Orange, CustomError> ToOrange(Fruit fruit) => CustomError.ConversionError;
}