using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using Sloth.Api.Client.Models;
using Sloth.Api.Client.Services.Internal;

namespace Sloth.Api.Client.Services;

public static class ServicesConfiguration
{
    public static void AddSlothApiClient(this IServiceCollection services, Action<SlothApiClientOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var options = new SlothApiClientOptions();
        configure.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new InvalidOperationException($"{nameof(options.ApiKey)} not provided");
        }

        services.AddRestEaseClient<ISlothApi>(options.BaseUrl);
        
        services.AddOptions<SlothApiClientOptions>()
            .Configure(o =>
            {
                o.BaseUrl = options.BaseUrl;
                o.ApiKey = options.ApiKey;
            });
    }
}