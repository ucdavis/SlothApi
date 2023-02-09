# SlothApi

A netstandard2.0 library for sending requests to Sloth API

## Installation

```bash
dotnet add package SlothApi
```

## Using via DI in AspNetCore

### Initialization

```csharp
services.AddSlothApiClient(options => 
{
    // only v2 is supported
    options.BaseUrl = "https://sloth-api-production.azurewebsites.net/v2/";
    options.ApiKey = "[your api key here]";
})

services.AddScoped<MyClass>();
```

### Usage
```csharp
using SlothApi.Models;
using SlothApi.Services;

public class MyClass
{
    private readonly ISlothApiClient _slothApiClient;
    public MyClass(ISlothApiClient slothApiClient)
    {
        this._slothApiClient = slothApiClient;
    }

    public async Task MoveMoney(decimal amount)
    {
        var model = new CreateTransactionViewModel
        {
            MerchantTrackingNumber = "[a tracking number]",
            MerchantTrackingUrl = "[a url]",
            AutoApprove = false,
            ValidateFinancialSegmentStrings = true,
            ProcessorTrackingNumber = "[a tracking number]",
            KfsTrackingNumber = "[a tracking number]",
            Source = "[a source]",
            SourceType = "[a source type (ie: Income)]",
            Description = "[a description]",
            Transfers = new List<CreateTransferViewModel>
            {
                // Must be at leaset one Debit and one Credit. Debits and credits must balance.
                new CreateTransferViewModel
                {
                    Amount                 = amount,
                    FinancialSegmentString = "[a financial segment string]",
                    Description            = "Moving some money",
                    Direction              = Transfer.CreditDebit.Debit,
                    ReferenceId            = "[an optional reference id]",
                    AccountingDate         = DateTime.UtcNow.Date // optional, defaults to today's date
                },
                new CreateTransferViewModel
                {
                    Amount                 = amount,
                    FinancialSegmentString = "[a financial segment string]",
                    Description            = "Moving some money",
                    Direction              = Transfer.CreditDebit.Credit,
                    ReferenceId            = "[an optional reference id]",
                    AccountingDate         = DateTime.UtcNow.Date // optional, defaults to today's date
                }
            },
            Metadata = new List<MetadataEntry>
            {
                new MetadataEntry
                {
                    Name = "[a name]",
                    Value = "[a value]"
                }
            }
        };

        ApiResult result = await _slothApiClient.CreateTransaction(model);
        if (!result.Success)
        {
            throw new Exception($"Api call did not succeed. Returned status: {result.StatusCode} Returned message: {result.Message}");
        }
    }
}
```

## Manual instantiation
```csharp
ISlothApiClient client = new SlothApiClient(new SlothApiClientOptions()
{
    // only v2 is supported
    options.BaseUrl = "https://sloth-api-production.azurewebsites.net/v2/";
    options.ApiKey = "[your api key here]";
});
```
