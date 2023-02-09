using System.Net;

namespace SlothApi.Models;

/// <summary>
/// Provides a summary of an api response
/// </summary>
public class ApiResult
{
    /// <summary>
    /// Provides a response message if available
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// True when the response includes a status code indicating success
    /// </summary>
    public bool Success { get; set; }
    
    public HttpStatusCode StatusCode { get; set; }
}

/// <summary>
/// Provides a summary of an api request, including deserialized data if available
/// </summary>
/// <typeparam name="T"></typeparam>
public class ApiResult<T> : ApiResult
{
    public T? Data { get; set; }
}
