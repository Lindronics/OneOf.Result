using System;
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
        var response = FakeApi.GetResource(resource)
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
        var response = await FakeApi.GetResourceAsync(resource)
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
        var successRes = new FakeResource(1);
        var defaultRes = new FakeResource(5);

        FakeApi.GetResource(successRes).UnwrapOr(defaultRes).Should().Be(successRes);
        FakeApi.GetResource(successRes).UnwrapOrDefault().Should().Be(successRes);
        FakeApi.GetResource(successRes).UnwrapErrOrDefault().Should().Be(null);
        FakeApi.GetResource(successRes).UnwrapOrElse(() => defaultRes).Should().Be(successRes);
        Assert.Throws<InvalidOperationException>(() => FakeApi.GetResource(new FakeResource(1)).UnwrapErr());

        FakeApi.GetResource(null).UnwrapOr(defaultRes).Should().Be(defaultRes);
        FakeApi.GetResource(null).UnwrapOrElse(() => defaultRes).Should().Be(defaultRes);
        FakeApi.GetResource(null).UnwrapOrDefault().Should().Be(null);
        FakeApi.GetResource(null).UnwrapErrOrDefault().Should().Be(new ApiError());
        Assert.Throws<InvalidOperationException>(() => FakeApi.GetResource(null).Unwrap());
    }

    [Fact]
    public async Task TestUnwrapsAsync()
    {
        var successRes = new FakeResource(1);
        var defaultRes = new FakeResource(5);

        (await FakeApi.GetResourceAsync(successRes).UnwrapOr(defaultRes)).Should().Be(successRes);
        (await FakeApi.GetResourceAsync(successRes).UnwrapOrDefault()).Should().Be(successRes);
        (await FakeApi.GetResourceAsync(successRes).UnwrapErrOrDefault()).Should().Be(null);
        (await FakeApi.GetResourceAsync(successRes).UnwrapOrElse(() => defaultRes)).Should().Be(successRes);
        (await FakeApi.GetResourceAsync(successRes).UnwrapOrElseAsync(() => Task.FromResult(defaultRes)))
            .Should().Be(successRes);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            FakeApi.GetResourceAsync(new FakeResource(1)).UnwrapErr());

        (await FakeApi.GetResourceAsync(null).UnwrapOr(defaultRes)).Should().Be(defaultRes);
        (await FakeApi.GetResourceAsync(null).UnwrapOrDefault()).Should().Be(null);
        (await FakeApi.GetResourceAsync(null).UnwrapErrOrDefault()).Should().Be(new ApiError());
        (await FakeApi.GetResourceAsync(null).UnwrapOrElse(() => defaultRes)).Should().Be(defaultRes);
        (await FakeApi.GetResourceAsync(null).UnwrapOrElseAsync(() => Task.FromResult(defaultRes)))
            .Should().Be(defaultRes);
        await Assert.ThrowsAsync<InvalidOperationException>(() => FakeApi.GetResourceAsync(null).Unwrap());
    }
}