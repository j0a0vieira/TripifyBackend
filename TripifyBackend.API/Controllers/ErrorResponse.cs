using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            ValidationError,
            DatabaseConnection
        }

        public enum FieldTypeEnum
        {
            Id,
            Latitude,
            Longitude,
            TripDuration,
            Coordinates,
            MaxDistance,
            Categories,
            NotApplicable
        }

        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum StatusCodeEnum
        {
            NotFound = 404,
            BadRequest = 400,
            InternalServerError = 500
        }
}