using System.Net;

namespace PetMinder.Client.Services;

public class SearchSitterServiceResult<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; } 
}