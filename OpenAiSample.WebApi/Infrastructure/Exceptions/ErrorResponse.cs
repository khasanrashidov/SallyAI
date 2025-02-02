using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using static OpenAiSample.WebApi.Constants;

namespace OpenAiSample.WebApi.Infrastructure.Exceptions;

public class ErrorResponse
{
    public string Name { get; }

    public string DebugId { get; }
    public Detail[] Details { get; }

    public string Message { get; }

    public ErrorResponse(Exception exception, string debugId, string name = Errors.Internal)
        : this(exception.Message, debugId)
    {
        Name = name;
    }

    public ErrorResponse(ValidationException exception, string debugId) : this(exception.Message, debugId)
    {
        Name = Errors.Validation;
        Details = exception.Errors.Select(e => new Detail
        {
            Field = e.PropertyName,
            Issue = e.ErrorMessage,
            Value = e.AttemptedValue
        }).ToArray();
    }

    public ErrorResponse(ActionContext actionContext, string debugId)
        : this("One or more validation errors occured", debugId)
    {
        Name = Errors.Validation;

        Details = actionContext.ModelState
            .Where(x => !x.Key.StartsWith('$'))
            .SelectMany(
                x => x.Value.Errors,
                (e, m) => new Detail { Field = e.Key, Issue = m.ErrorMessage, Value = e.Value.AttemptedValue })
            .ToArray();
    }

    private ErrorResponse(string message, string debugId)
    {
        DebugId = debugId;
        Message = message;
    }

    public class Detail
    {
        public string Field { get; set; }

        public object Value { get; set; }

        public string Issue { get; set; }
    }
}
