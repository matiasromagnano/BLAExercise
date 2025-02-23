using BLAExercise.API.Models;
using BLAExercise.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BLAExercise.API.Filters;

/// <summary>
/// A custom exception filter that handles specific API exceptions and returns formatted error responses.
/// </summary>
public class ApiExceptionFilter : ExceptionFilterAttribute
{
    /// <summary>
    /// Overrides the default exception handling behavior to process and format API-specific exceptions.
    /// </summary>
    /// <param name="context">The context containing details about the exception and the current request.</param>
    public override void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case NotFoundException notFoundException:
                HandleNotFoundException(context, notFoundException);
                break;
            case BadRequestException badRequestException:
                HandleBadRequestException(context, badRequestException);
                break;
            default:
                HandleOtherExceptions(context);
                break;
        }
    }

    /// <summary>
    /// Handles a NotFoundException by returning a 404 response with details from the exception.
    /// </summary>
    /// <param name="context">The context containing details about the exception and the current request.</param>
    /// <param name="exception">The NotFoundException instance containing specific error details.</param>
    private void HandleNotFoundException(ExceptionContext context, NotFoundException exception)
    {
        context.Result = new ObjectResult(new ApiResponse<NotFoundException>
        {
            StatusCode = exception.StatusCode,
            Message = exception.Message,
            Data = null
        })
        {
            StatusCode = StatusCodes.Status404NotFound
        };

        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Handles a BadRequestException by returning a 400 response with details from the exception.
    /// </summary>
    /// <param name="context">The context containing details about the exception and the current request.</param>
    /// <param name="exception">The BadRequestException instance containing specific error details.</param>
    private void HandleBadRequestException(ExceptionContext context, BadRequestException exception)
    {
        context.Result = new ObjectResult(new ApiResponse<BadRequestException>
        {
            StatusCode = exception.StatusCode,
            Message = exception.Message,
            Data = null
        })
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Handles uncaught or unspecified exceptions by returning a 500 response with a generic error message.
    /// </summary>
    /// <param name="context">The context containing details about the exception and the current request.</param>
    private void HandleOtherExceptions(ExceptionContext context)
    {
        string? message;
        if (context.Exception.InnerException is not null)
        {
            message = context.Exception.InnerException.Message;
        }
        else
        {
            message = context.Exception.Message;
        }

        context.Result = new ObjectResult(new ApiResponse<OtherException>
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = message,
            Data = null
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}