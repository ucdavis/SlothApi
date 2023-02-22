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

public class SlothApiClientTests
{
    [Fact]
    public async void ApiRequestShouldSucceed()
    {
        // Arrange
        var apiKey = "";
        var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        messageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns((HttpRequestMessage request, CancellationToken token) =>
            {
                apiKey = request.Headers.GetValues(SlothApiClient.ApiKeyHeader).FirstOrDefault();
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("true")
                });
            })
            .Verifiable();

        var httpClient = new HttpClient(messageHandlerMock.Object);

        var slothApi = RestClient.For<ISlothApi>(httpClient);
        var slothClient = new SlothApiClient(slothApi, Options.Create(new SlothApiClientOptions
        {
            BaseUrl = "https://some.sloth.api.net/",
            ApiKey = "123"
        }));

        // Act
        var result = await slothClient.ValidateChartOfAccounts("asdf");

        // Assert
        messageHandlerMock.Verify();
        result.Success.ShouldBeTrue();
        result.Data.ShouldBeTrue();
        apiKey.ShouldBe("123");
    }

    [Fact]
    public async void ShouldHandleUnsuccessfulResponse()
    {
        // Arrange
        var apiKey = "";
        var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        messageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns((HttpRequestMessage request, CancellationToken token) =>
            {
                apiKey = request.Headers.GetValues(SlothApiClient.ApiKeyHeader).FirstOrDefault();
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            })
            .Verifiable();

        var httpClient = new HttpClient(messageHandlerMock.Object);

        var slothApi = RestClient.For<ISlothApi>(httpClient);
        var slothClient = new SlothApiClient(slothApi, Options.Create(new SlothApiClientOptions
        {
            BaseUrl = "https://some.sloth.api.net/",
            ApiKey = "123"
        }));

        // Act
        var result = await slothClient.ValidateChartOfAccounts("asdf");

        // Assert
        messageHandlerMock.Verify();
        result.Success.ShouldBeFalse();
        result.Data.ShouldBeFalse();
        apiKey.ShouldBe("123");
    }
}
