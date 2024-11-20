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
        ValidationError
    }

    public enum FieldTypeEnum
    {
        Id,
        Latitude,
        Longitude,
        TripDuration,
        Coordinates,
        MaxDistance,
        Categories
    }

    public enum StatusCodeEnum
    {
        NotFound,
        BadRequest,
    }
}