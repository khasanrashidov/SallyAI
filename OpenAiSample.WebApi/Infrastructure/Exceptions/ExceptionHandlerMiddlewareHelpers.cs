using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Primitives;
using static System.Net.Mime.MediaTypeNames;

namespace OpenAiSample.WebApi.Infrastructure.Exceptions;

public static class ExceptionHandlerMiddlewareHelpers
{
    private static readonly JsonSerializerOptions ErrorSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private sealed record ErrorResponseInformation(ErrorResponse Response, HttpStatusCode StatusCode);

    public static void UseCustomErrors(this IApplicationBuilder app) => app.Use(WriteResponse);

    // This method is a base point for defining the logic of building ErrorResponse based on exception type.
    // Add additional cases here...
    private static ErrorResponseInformation GetErrorResponse(Exception exception, string debugId) =>
        exception switch
        {
            ValidationException vex => new(new ErrorResponse(vex, debugId), HttpStatusCode.BadRequest),
            _ => new(new ErrorResponse(exception, debugId), HttpStatusCode.InternalServerError)
        };

    private static async Task WriteResponse(HttpContext httpContext, Func<Task> next)
    {
        // Try and retrieve the error from the ExceptionHandler middleware
        IExceptionHandlerFeature exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
        Exception ex = exceptionDetails?.Error;

        if (ex != null)
        {
            var debugId = httpContext.Request.Headers.TryGetValue("request-id", out StringValues values)
                ? values.First()
                : Guid.NewGuid().ToString();

            (ErrorResponse errorResponse, HttpStatusCode statusCode) = GetErrorResponse(ex, debugId);

            httpContext.Response.ContentType = Application.Json;
            httpContext.Response.StatusCode = (int)statusCode;

            var body = JsonSerializer.Serialize(errorResponse, ErrorSerializerOptions);

            await httpContext.Response.WriteAsync(body);
        }
    }
}