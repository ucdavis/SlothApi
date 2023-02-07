using System.Net;

namespace Sloth.Api.Client.Models;


public class ApiResult
{
    public string Message { get; set; } = "";
    public bool Success { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}

public class ApiResult<T> : ApiResult
{
    public T Data { get; set; }
}
