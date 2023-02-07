using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using RestEase;
using Shouldly;
using Sloth.Api.Client.Models;
using Sloth.Api.Client.Services;
using Sloth.Api.Client.Services.Internal;
using System.Net;

namespace Sloth.Api.Client.Test;

public class SlothApiClientTests
{
    [Fact]
    public async void SegmentValidationShouldSucceed()
    {
        // Arrange
        var apiKey = "";
        var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        messageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns((HttpRequestMessage request, CancellationToken token) =>
            {
                apiKey = request.Headers.GetValues("X-Api-Key").FirstOrDefault();
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
        var result = await slothClient.ValidateFinancialSegmentString("asdf");

        // Assert
        messageHandlerMock.Verify();
        result.Success.ShouldBeTrue();
        result.Data.ShouldBeTrue();
        apiKey.ShouldBe("123");
    }

    [Fact]
    public async void SegmentValidationShouldFail()
    {
        // Arrange
        var apiKey = "";
        var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        messageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns((HttpRequestMessage request, CancellationToken token) =>
            {
                apiKey = request.Headers.GetValues("X-Api-Key").FirstOrDefault();
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
        var result = await slothClient.ValidateFinancialSegmentString("asdf");

        // Assert
        messageHandlerMock.Verify();
        result.Success.ShouldBeFalse();
        result.Data.ShouldBeFalse();
        apiKey.ShouldBe("123");
    }
}
