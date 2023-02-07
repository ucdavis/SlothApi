using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestEase;
using Sloth.Api.Client.Models;
using Sloth.Api.Client.Services.Internal;

namespace Sloth.Api.Client.Services;

public interface ISlothApiClient
{
    Task<ApiResult<IList<Transaction>>> GetTop1Transactions();
    Task<ApiResult<Transaction>> GetTransactionById(string id);
    Task<ApiResult<Transaction>> GetTransactionByProcessorId(string id);
    Task<ApiResult<IList<Transaction>>> GetTransactionsByKfsKey(string id);
    Task<ApiResult<bool>> ValidateFinancialSegmentString(string id);
    Task<ApiResult> CreateTransaction(CreateTransactionViewModel transaction);
}

public class SlothApiClient : ISlothApiClient
{
    private readonly ISlothApi _slothApi;

    [ActivatorUtilitiesConstructor]
    public SlothApiClient(ISlothApi slothApi, IOptions<SlothApiClientOptions> options)
    {
        _slothApi = slothApi;
        _slothApi.ApiKey = options.Value.ApiKey;
    }

    public SlothApiClient(IOptions<SlothApiClientOptions> options) : this(RestClient.For<ISlothApi>(options.Value.BaseUrl), options)
    {
    }

    public async Task<ApiResult<IList<Transaction>>> GetTop1Transactions()
    {
        using var response = await _slothApi.GetTop1Transactions();
        var result = GetResult(response);
        return result;
    }

    public async Task<ApiResult<Transaction>> GetTransactionById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("id required", nameof(id));
        }

        using var response = await _slothApi.GetTransactionById(id);
        var result = GetResult(response);
        return result;
    }

    public async Task<ApiResult<Transaction>> GetTransactionByProcessorId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("id required", nameof(id));
        }

        using var response = await  _slothApi.GetTransactionByProcessorId(id);
        var result = GetResult(response);
        return result;
    }

    public async Task<ApiResult<IList<Transaction>>> GetTransactionsByKfsKey(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("id required", nameof(id));
        }

        using var response = await  _slothApi.GetTransactionsByKfsKey(id);
        var result = GetResult(response);
        return result;
    }

    public async Task<ApiResult<bool>> ValidateFinancialSegmentString(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("id required", nameof(id));
        }

        using var response = await  _slothApi.ValidateFinancialSegmentString(id);
        var result = GetResult(response);
        return result;
    }

    public async Task<ApiResult> CreateTransaction(CreateTransactionViewModel transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        using var response = await  _slothApi.CreateTransaction(transaction);
        var result = GetResult(response);
        return result;
    }

    private static ApiResult<T> GetResult<T>(Response<T> response)
    {
        var result = new ApiResult<T>
        {
            StatusCode = response.ResponseMessage.StatusCode,
            Success = response.ResponseMessage.IsSuccessStatusCode,
            Message = response.ResponseMessage.ReasonPhrase ?? "",
            Data  = response.ResponseMessage.IsSuccessStatusCode ? response.GetContent() : default
        };

        return result;
    }

    private static ApiResult GetResult(HttpResponseMessage response)
    {
        var result = new ApiResult
        {
            StatusCode = response.StatusCode,
            Success = response.IsSuccessStatusCode,
            Message = response.ReasonPhrase ?? "",
        };

        return result;
    }
}