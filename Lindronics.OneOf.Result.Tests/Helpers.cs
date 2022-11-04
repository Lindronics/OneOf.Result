using System.Threading.Tasks;

namespace Lindronics.OneOf.Result.Tests;

public record FakeResource(int Id);

public record ApiError;

public class FakeApi
{
    public static Result<FakeResource, ApiError> GetResource(FakeResource? resource)
        => resource is not null ? resource : new ApiError();

    public static async Task<Result<FakeResource, ApiError>> GetResourceAsync(FakeResource? resource)
        => resource is not null ? await Task.FromResult(resource) : await Task.FromResult(new ApiError());
}

public record ConvertedResource(int Id);

public record ConversionError;

public static class FakeConvertor
{
    public static Result<ConvertedResource, ConversionError> Convert(this FakeResource resource, bool success)
        => success ? new ConvertedResource(resource.Id) : new ConversionError();

    public static async Task<Result<ConvertedResource, ConversionError>> ConvertAsync(
        this FakeResource resource,
        bool success)
        => await Task.FromResult<Result<ConvertedResource, ConversionError>>(success
            ? new ConvertedResource(resource.Id)
            : new ConversionError());
}