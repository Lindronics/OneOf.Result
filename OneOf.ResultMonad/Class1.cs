using OneOf.Types;

namespace result;
public class Class1
{
    public static void Main()
    {
        var a1 = A1();
        var a11 = a1.Map(Convert1);
        var a12 = a11.MapErr(e => CustomError.CustomA);
        var a13 = a12.AndThen(Convert2);

        var a2 = A1().Map(Convert1).MapErr(e => CustomError.CustomA).AndThen(Convert2);

        var a3 = A1().Map(Convert1).MapErr(e => CustomError.CustomA).AndThen(Convert2).Map(x => x.Colour).ToNullable();
        a3 ??= "asf";
    }

    private record Apple(string Colour);

    private record Fruit(string? Colour);

    private enum CustomError
    {
        CustomA,
        CustomB,
    }

    private static Result<Apple, None> A1() => new Apple("blue");

    private static Result<Apple, None> A2() => new Success<Apple>(new Apple("blue"));

    private static Result<Apple, None> A3() => Result<Apple, None>.Ok(new Apple("green"));

    // private static Result<Apple> B1() => (Result<Apple>)new Apple("blue");

    private static Fruit Convert1(Apple a) => new(a.Colour);

    private static Result<Apple, CustomError> Convert2(Fruit a) => a.Colour is not null ? new Apple(a.Colour) : CustomError.CustomB;
}