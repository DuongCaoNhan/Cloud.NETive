using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Catches all unhandled exceptions in the ASP.NET Core pipeline and returns
/// a structured RFC 7807 ProblemDetails JSON response.
/// Register via <see cref="GlobalExceptionMiddlewareExtensions.UseGlobalExceptionHandler"/>.
/// </summary>
public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleAsync(context, ex);
        }
    }

    private Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (status, title) = ex switch
        {
            ArgumentNullException or ArgumentException => (StatusCodes.Status400BadRequest,  "Bad Request"),
            InvalidOperationException                  => (StatusCodes.Status400BadRequest,  "Bad Request"),
            UnauthorizedAccessException                => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _                                         => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode  = status;

        var problem = new
        {
            type    = $"https://httpstatuses.io/{status}",
            title,
            status,
            detail  = env.IsDevelopment() ? ex.Message : (string?)null,
            traceId = ctx.TraceIdentifier
        };

        return ctx.Response.WriteAsJsonAsync(problem);
    }
}
