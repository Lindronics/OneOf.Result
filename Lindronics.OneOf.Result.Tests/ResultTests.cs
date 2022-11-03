using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Lindronics.OneOf.Result.Tests;

public class ResultTests
{
    public static IEnumerable<object?[]> TestCases => new[]
    {
        new object?[] { new FakeResource(1), true },
        new object?[] { new FakeResource(1), false },
        new object?[] { null, true },
        new object?[] { null, false },
    };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void TestAndThenAndMap(FakeResource? resource, bool conversionSuccess)
    {
        var api = new FakeApi();

        var response = api.GetResource(resource)
            .MapErr(_ => new ConversionError())
            .MapErr(e => e)
            .AndThen(x => x.Convert(conversionSuccess))
            .Map(x => x.Id)
            .Map(x => x);

        var expectedSuccess = resource is not null && conversionSuccess;
        response.IsOk.Should().Be(expectedSuccess);

        if (expectedSuccess)
        {
            response.Unwrap().Should().Be(resource!.Id);
        }
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public async Task TestAndThenAndMapAsync(FakeResource? resource, bool conversionSuccess)
    {
        var api = new FakeApi();

        var response = await api.GetResourceAsync(resource)
            .MapErrAsync(_ => Task.FromResult(new ConversionError()))
            .MapErr(e => e)
            .AndThenAsync(x => x.ConvertAsync(conversionSuccess))
            .MapAsync(x => Task.FromResult(x.Id))
            .Map(x => x);

        var expectedSuccess = resource is not null && conversionSuccess;
        response.IsOk.Should().Be(expectedSuccess);

        if (expectedSuccess)
        {
            response.Unwrap().Should().Be(resource!.Id);
        }
    }

    [Fact]
    public void TestUnwraps()
    {
        var fakeApi = new FakeApi();
        var successRes = new FakeResource(1);
        var defaultRes = new FakeResource(5);

        fakeApi.GetResource(successRes).UnwrapOr(defaultRes).Should().Be(successRes);
        fakeApi.GetResource(successRes).UnwrapOrNull().Should().Be(successRes);
        fakeApi.GetResource(successRes).UnwrapOrElse(() => defaultRes).Should().Be(successRes);

        fakeApi.GetResource(null).UnwrapOr(defaultRes).Should().Be(defaultRes);
        fakeApi.GetResource(null).UnwrapOrElse(() => defaultRes).Should().Be(defaultRes);
        fakeApi.GetResource(null).UnwrapOrNull().Should().Be(null);
    }

    [Fact]
    public async Task TestUnwrapsAsync()
    {
        var fakeApi = new FakeApi();
        var successRes = new FakeResource(1);
        var defaultRes = new FakeResource(5);

        (await fakeApi.GetResourceAsync(successRes).UnwrapOr(defaultRes)).Should().Be(successRes);
        (await fakeApi.GetResourceAsync(successRes).UnwrapOrNull()).Should().Be(successRes);
        (await fakeApi.GetResourceAsync(successRes).UnwrapOrElse(() => defaultRes)).Should().Be(successRes);
        (await fakeApi.GetResourceAsync(successRes).UnwrapOrElseAsync(() => Task.FromResult(defaultRes)))
            .Should().Be(successRes);

        (await fakeApi.GetResourceAsync(null).UnwrapOr(defaultRes)).Should().Be(defaultRes);
        (await fakeApi.GetResourceAsync(null).UnwrapOrNull()).Should().Be(null);
        (await fakeApi.GetResourceAsync(null).UnwrapOrElse(() => defaultRes)).Should().Be(defaultRes);
        (await fakeApi.GetResourceAsync(null).UnwrapOrElseAsync(() => Task.FromResult(defaultRes)))
            .Should().Be(defaultRes);
    }
}