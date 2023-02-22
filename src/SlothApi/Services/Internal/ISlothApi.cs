
using RestEase;
using SlothApi.Models;

namespace SlothApi.Services.Internal;

[AllowAnyStatusCode] // Prevent ApiException from being thrown for non-2xx responses
public interface ISlothApi
{
    [Header(SlothApiClient.ApiKeyHeader)]
    string ApiKey { get; set; }

    [Get("transactions")]
    Task<Response<IList<Transaction>>> GetTop1Transactions();

    [Get("transactions/{id}")]
    Task<Response<Transaction>> GetTransactionById([Path] string id);

    [Get("transactions/processor/{id}")]
    Task<Response<Transaction>> GetTransactionByProcessorId([Path] string id);

    [Get("transactions/kfskey/{id}")]
    Task<Response<IList<Transaction>>> GetTransactionsByKfsKey([Path] string id);

    [Get("transactions/validate/{id}")]
    Task<Response<bool>> ValidateFinancialSegmentString([Path] string id);

    [Post("transactions")]
    Task<Response<Transaction>> CreateTransaction([Body] CreateTransactionViewModel transaction);
}