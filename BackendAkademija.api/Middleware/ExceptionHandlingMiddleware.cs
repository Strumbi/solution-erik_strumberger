using System.Net;
using System.Text.Json;
using FluentValidation;

namespace BackendAkademija.api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validacijska greška: {Errors}", ex.Errors);
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error");
            await HandleUnexpectedExceptionAsync(context);
        }
    }
    
    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException validationException)
    {
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                e => e.Key, 
                e => e.Select(g => g.ErrorMessage).ToArray())
            ?? new Dictionary<string, string[]>();

        var response = new { errors };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
    
    private async Task HandleUnexpectedExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        
        var response = new { error = "An error ocurred" };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}