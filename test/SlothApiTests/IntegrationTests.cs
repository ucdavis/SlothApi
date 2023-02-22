using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using RestEase;
using Shouldly;
using SlothApi.Models;
using SlothApi.Services;
using SlothApi.Services.Internal;
using System.Net;

namespace SlothApiTests;

public class IntegrationTests
{
    [Fact(Skip = "for debug purposes only")]
    public async void ResponseShoudReturnBadRequest()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<IntegrationTests>()
            .Build();
        var apiKey = configuration.GetValue<string>("ApiKey");
        var baseUrl = configuration.GetValue<string>("BaseUrl");
        var slothClient = new SlothApiClient(new SlothApiClientOptions
        {
            BaseUrl = baseUrl,
            ApiKey = apiKey
        });

        var model = new CreateTransactionViewModel
        {
            MerchantTrackingNumber = "[a tracking number]",
            MerchantTrackingUrl = "[a url]",
            AutoApprove = false,
            ValidateFinancialSegmentStrings = false,
            ProcessorTrackingNumber = "[a tracking number]",
            KfsTrackingNumber = "This tracking number is too long and should fail",
            Source = "[a source]",
            SourceType = "[a source type (ie: Income)]",
            Description = "[a description]",
            Transfers = new List<CreateTransferViewModel>
            {
                // Must be at leaset one Debit and one Credit. Debits and credits must balance.
                new CreateTransferViewModel
                {
                    Amount                 = 100.00M,
                    FinancialSegmentString = "[a financial segment string]",
                    Description            = "Moving some money",
                    Direction              = Transfer.CreditDebit.Debit,
                    AccountingDate         = DateTime.UtcNow.Date 
                },
                new CreateTransferViewModel
                {
                    Amount                 = 100.00M,
                    FinancialSegmentString = "[a financial segment string]",
                    Description            = "Moving some money",
                    Direction              = Transfer.CreditDebit.Credit,
                    AccountingDate         = DateTime.UtcNow.Date
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

        // Act
        var result = await slothClient.CreateTransaction(model);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Bad Request: {\r\n  \"KfsTrackingNumber\": [\r\n    \"The field KfsTrackingNumber must be a string or array type with a maximum length of '10'.\"\r\n  ]\r\n}");
    }
}
