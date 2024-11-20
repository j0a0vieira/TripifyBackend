using Microsoft.AspNetCore.Mvc;

namespace TripifyBackend.API.Controllers;

public  class ErrorResponse
{
        public StatusCodeEnum StatusCode { get; set; }
        public ErrorTypeEnum Type { get; set; }
        public string Detail { get; set; }
        public FieldTypeEnum Field { get; set; }

        public enum ErrorTypeEnum
        {
            Unknown,
            ValidationError
        }

        public enum FieldTypeEnum
        {
            Id,
            Latitude,
            Longitude,
            TripDuration,
            Coordinates,
            NotApplicable
        }

        public enum StatusCodeEnum
        {
            NotFound,
            BadRequest,
            InternalError
        }
}