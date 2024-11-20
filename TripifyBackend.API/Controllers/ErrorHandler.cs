using Microsoft.AspNetCore.Mvc;
using TripifyBackend.DOMAIN.Models;

namespace TripifyBackend.API.Controllers;

public class ErrorHandler
{
    public static Task<IActionResult> HandleErrorAsync(DomainError domainError)
    {
        IActionResult result;

        switch (domainError.StatusCode)
        {
            case DomainError.StatusCodeEnum.BadRequest:
                result = new BadRequestObjectResult(new ErrorResponse
                {
                    StatusCode = ErrorResponse.StatusCodeEnum.BadRequest,
                    Detail = domainError.Detail,
                    Type = ErrorResponse.ErrorTypeEnum.ValidationError,
                    Field = ErrorResponse.FieldTypeEnum.NotApplicable
                });
                break;
            default:
                result = new ObjectResult(new ErrorResponse
                {
                    StatusCode = ErrorResponse.StatusCodeEnum.InternalError,
                    Detail = "An unexpected error occurred.",
                    Type = ErrorResponse.ErrorTypeEnum.Unknown,
                    Field = ErrorResponse.FieldTypeEnum.NotApplicable
                })
                {
                    StatusCode = 500
                };
                break;
        }

        return Task.FromResult(result);
    }
}
