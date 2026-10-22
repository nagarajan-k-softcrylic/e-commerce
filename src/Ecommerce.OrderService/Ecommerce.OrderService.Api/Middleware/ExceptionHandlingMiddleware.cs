using System.Net;
using System.Text.Json;
using Ecommerce.OrderService.Domain.Exceptions;
using FluentValidation;

namespace Ecommerce.OrderService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        object response;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                var errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                response = new { title = "Validation Failed", status = (int)statusCode, errors };
                _logger.LogWarning("Validation failed: {Errors}", errors);
                break;

            case OrderDomainException domainException:
                statusCode = HttpStatusCode.BadRequest;
                response = new { title = "Domain Rule Violation", status = (int)statusCode, detail = domainException.Message };
                _logger.LogWarning("Domain exception: {Message}", domainException.Message);
                break;

            case KeyNotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                response = new { title = "Not Found", status = (int)statusCode, detail = notFoundException.Message };
                break;

            default:
                _logger.LogError(exception, "Unhandled server error occurred: {Message}", exception.Message);
                response = new { title = "Internal Server Error", status = (int)statusCode, detail = "An unexpected error occurred. Please try again later." };
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}

