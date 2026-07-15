using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using EventManagementService.Domain.Exceptions;

namespace EventManagementService.Presentation.Middlewares;

public class CustomExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;

    public CustomExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<CustomExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleException(httpContext, ex);
        }
    }

    private async Task HandleException(HttpContext httpContext, Exception ex)
    {
        _logger.LogError(
            ex,
            "Unhandled exception. Method={Method}, Path={Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        if (httpContext.Response.HasStarted)
        {
            return;
        }

        var statusCode = MapStatusCode(ex);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var error = new ProblemDetails
        {
            Status = statusCode,
            Detail = ex.Message
        };

        await httpContext.Response.WriteAsJsonAsync(error);
    }

    private static int MapStatusCode(Exception ex)
        => ex switch
        {
            ValidationException _ => StatusCodes.Status400BadRequest,
            ArgumentException _ => StatusCodes.Status400BadRequest,
            EventNotFoundException _ => StatusCodes.Status404NotFound,
            BookingNotFoundException _ => StatusCodes.Status404NotFound,
            NoAvailableSeatsException _ => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
}
