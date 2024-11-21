using System.ComponentModel;

namespace TripifyBackend.DOMAIN.Models;

public class DomainError
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

    public enum StatusCodeEnum
    {
        NotFound,
        BadRequest,
        InternalServerError
    }
}