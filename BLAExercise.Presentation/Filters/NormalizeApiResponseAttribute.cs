using BLAExercise.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BLAExercise.Presentation.Filters;

/// <summary>
/// An action filter attribute that normalizes API responses into a consistent format using ApiResponse&lt;T&gt;.
/// </summary>
public class NormalizeApiResponseAttribute : ActionFilterAttribute
{
    private const string SuccessMessage = "Success";

    /// <summary>
    /// Executes before the action result is returned, normalizing the response into a standardized ApiResponse format.
    /// Handles both successful responses and validation errors, adjusting status codes and content accordingly.
    /// </summary>
    /// <param name="context">The context containing the action result to be normalized.</param>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var result = context.Result;

        if (result is ObjectResult objectResult && objectResult.Value != null)
        {
            var statusCode = objectResult.StatusCode ?? 200;
            var responseBody = objectResult.Value;
            var normalizedResponse = new ApiResponse<object>();

            if (statusCode is StatusCodes.Status400BadRequest)
            {
                // Getting the ProblemDetails from the ASP.NET Built-in Middleware
                var problemDetails = (ValidationProblemDetails)responseBody;
                var errors = problemDetails.Errors;
                if (errors is not null)
                {
                    normalizedResponse.StatusCode = statusCode;
                    normalizedResponse.Message = problemDetails.Title;
                    normalizedResponse.Details = errors;
                    normalizedResponse.Data = null;
                }
            }
            else
            {
                normalizedResponse.StatusCode = statusCode;
                normalizedResponse.Data = responseBody;
                normalizedResponse.Message = SuccessMessage;
            }

            context.Result = new ObjectResult(normalizedResponse)
            {
                StatusCode = statusCode,
                DeclaredType = typeof(ApiResponse<object>)
            };
        }
        else if (result is StatusCodeResult statusCodeResult)
        {
            var statusCode = statusCodeResult.StatusCode;

            var normalizedResponse = new ApiResponse<object>
            {
                StatusCode = statusCode,
                Message = SuccessMessage
            };

            context.Result = new ObjectResult(normalizedResponse)
            {
                StatusCode = statusCode,
                DeclaredType = typeof(ApiResponse<object>)
            };
        }

        base.OnResultExecuting(context);
    }
}