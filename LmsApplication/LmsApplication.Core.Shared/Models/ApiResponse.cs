namespace LmsApplication.Core.Shared.Models;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public static class ApiResponseHelper
{
    public static ApiResponse Success(object? data = null)
    {
        return new() { Success = true, Data = data };
    }

    public static ApiResponse Error(string message)
    {
        return new() { Success = false, Message = message };
    }
}
