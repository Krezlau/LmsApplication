using System.Net;
using FluentValidation;
using LmsApplication.Core.Data.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Api.Shared.Middleware;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        string message;
        switch (exception)
        {
            case UnauthorizedAccessException _:
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                message = "Unauthorized. Please log in first";
                break;
            case KeyNotFoundException _:
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                message = exception.Message is "" ? "Resource not found" : exception.Message;
                break;
            case ArgumentException _:
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                message = exception.Message is "" ? "Bad request" : exception.Message;
                break;
            case ValidationException e:
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                message = e.Message;
                break;
            default:
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                message = "Something went wrong";
                break;
        }
        
        return context.Response.WriteAsJsonAsync(ApiResponseHelper.Error(message));
    }
}
