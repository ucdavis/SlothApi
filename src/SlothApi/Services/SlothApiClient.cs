using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestEase;
using SlothApi.Models;
using SlothApi.Services.Internal;

namespace SlothApi.Services;

/// <summary>
/// Primary interface for interacting with Sloth Api
/// </summary>
/// <remarks>
/// Responses with no payload are mapped to a <seealso cref="ApiResult" />, while
/// responses with a payload are mapped to a <seealso cref="ApiResult&lt;&gt;" />
/// </remarks>
public interface ISlothApiClient
{
    /// <summary>
    /// Fetch Top 1 <seealso cref="Transaction">
    /// </summary>
    Task<ApiResult<IList<Transaction>>> GetTop1Transactions();

    /// <summary>
    /// Fetch <seealso cref="Transaction"> by <paramref name="transactionId" />
    /// </summary>
    /// <param name="transactionId">Unique identifier of transaction, <seealso cref="Transaction.Id" /></param>
    Task<ApiResult<Transaction>> GetTransactionById(string transactionId);

    /// <summary>
    /// Fetch <seealso cref="Transaction"> by <paramref name="processorTrackingNumber" />
    /// </summary>
    /// <param name="processorTrackingNumber">Tracking number assigned to transaction by third-party processor, <seealso cref="Transaction.ProcessorTrackingNumber" /></param>
    Task<ApiResult<Transaction>> GetTransactionByProcessorId(string processorTrackingNumber);
    
    /// <summary>
    /// Fetch list of <seealso cref="Transaction"> by <paramref name="kfsTrackingNumber" />
    /// </summary>
    /// <param name="kfsTrackingNumber">Tracking number assigned to transactions by KFS, <seealso cref="Transaction.KfsTrackingNumber" /></param>
    Task<ApiResult<IList<Transaction>>> GetTransactionsByKfsTrackingNumber(string kfsTrackingNumber);

    /// <summary>
    /// Request validation of chart of accounts string <paramref name="coaString"/>
    /// </summary>
    Task<ApiResult<bool>> ValidateChartOfAccounts(string coaString);

    /// <summary>
    /// Request creation of <seealso cref="Transaction"> for given <paramref name="transactionViewModel"/>
    /// </summary>
    /// <param name="transaction"></param>
    Task<ApiResult> CreateTransaction(CreateTransactionViewModel transactionViewModel);
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

    public SlothApiClient(SlothApiClientOptions options) : this(RestClient.For<ISlothApi>(options.BaseUrl), Options.Create(options))
    {
    }

    /// <summary>
    /// Fetch Top 1 <seealso cref="Transaction">
    /// </summary>
    public async Task<ApiResult<IList<Transaction>>> GetTop1Transactions()
    {
        using var response = await _slothApi.GetTop1Transactions();
        var result = GetResult(response);
        return result;
    }

    /// <summary>
    /// Fetch <seealso cref="Transaction"> by <paramref name="transactionId" />
    /// </summary>
    /// <param name="transactionId">Unique identifier of transaction, <seealso cref="Transaction.Id" /></param>
    public async Task<ApiResult<Transaction>> GetTransactionById(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            throw new ArgumentException("id required", nameof(transactionId));
        }

        using var response = await _slothApi.GetTransactionById(transactionId);
        var result = GetResult(response);
        return result;
    }

    /// <summary>
    /// Fetch <seealso cref="Transaction"> by <paramref name="processorTrackingNumber" />
    /// </summary>
    /// <param name="processorTrackingNumber">Tracking number assigned to transaction by third-party processor, <seealso cref="Transaction.ProcessorTrackingNumber" /></param>
    public async Task<ApiResult<Transaction>> GetTransactionByProcessorId(string processorTrackingNumber)
    {
        if (string.IsNullOrWhiteSpace(processorTrackingNumber))
        {
            throw new ArgumentException("id required", nameof(processorTrackingNumber));
        }

        using var response = await _slothApi.GetTransactionByProcessorId(processorTrackingNumber);
        var result = GetResult(response);
        return result;
    }

    /// <summary>
    /// Fetch list of <seealso cref="Transaction"> by <paramref name="kfsTrackingNumber" />
    /// </summary>
    /// <param name="kfsTrackingNumber">Tracking number assigned to transactions by KFS, <seealso cref="Transaction.KfsTrackingNumber" /></param>
    public async Task<ApiResult<IList<Transaction>>> GetTransactionsByKfsTrackingNumber(string kfsTrackingNumber)
    {
        if (string.IsNullOrWhiteSpace(kfsTrackingNumber))
        {
            throw new ArgumentException("id required", nameof(kfsTrackingNumber));
        }

        using var response = await _slothApi.GetTransactionsByKfsKey(kfsTrackingNumber);
        var result = GetResult(response);
        return result;
    }

    /// <summary>
    /// Request validation of chart of accounts string <paramref name="coaString"/>
    /// </summary>
    public async Task<ApiResult<bool>> ValidateChartOfAccounts(string coaString)
    {
        if (string.IsNullOrWhiteSpace(coaString))
        {
            throw new ArgumentException("id required", nameof(coaString));
        }

        using var response = await _slothApi.ValidateFinancialSegmentString(coaString);
        var result = GetResult(response);
        return result;
    }

    /// <summary>
    /// Request creation of <seealso cref="Transaction"> for given <paramref name="transactionViewModel"/>
    /// </summary>
    /// <param name="transaction"></param>
    public async Task<ApiResult> CreateTransaction(CreateTransactionViewModel transactionViewModel)
    {
        if (transactionViewModel == null)
        {
            throw new ArgumentNullException(nameof(transactionViewModel));
        }

        using var response = await _slothApi.CreateTransaction(transactionViewModel);
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
            Data = response.ResponseMessage.IsSuccessStatusCode ? response.GetContent() : default
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