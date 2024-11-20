using TripifyBackend.DOMAIN.Models;

namespace TripifyBackend.APPLICATION.Services;

public static class ValidationMethods
{
    public static bool IsLatAndLonValid(string lat, string lon, List<DomainError> errors)
    {
        var isLatValid = double.TryParse(lat, out var latDouble);
        var isLonValid = double.TryParse(lon, out var lonDouble);

        if (!isLatValid || !isLonValid)
        {
            errors.Add(new DomainError
            {
                StatusCode = DomainError.StatusCodeEnum.BadRequest,
                Detail = "Latitude and Longitude are invalid.",
                Field = DomainError.FieldTypeEnum.Coordinates,
                Type = DomainError.ErrorTypeEnum.ValidationError
            });

            return false;
        }

        if (!IsValidLatitude(latDouble) && !IsValidLongitude(lonDouble))
        {
            errors.Add( new DomainError
            {
                StatusCode = DomainError.StatusCodeEnum.BadRequest,
                Detail = "Latitude and Longitude are invalid.",
                Field = DomainError.FieldTypeEnum.Coordinates,
                Type = DomainError.ErrorTypeEnum.ValidationError
            });

            return false;
        }

        return true;
    }
    
    private static bool IsValidLatitude(double latitude)
    {
        return latitude is >= -90 and <= 90;
    }

    private static bool IsValidLongitude(double longitude)
    {
        return longitude is >= -180 and <= 180;
    }

    public static bool ValidateTripDuration(int tripDuration, List<DomainError> errors)
    {
        if (tripDuration >= 1) return true;
        errors.Add( new DomainError
        {
            StatusCode = DomainError.StatusCodeEnum.BadRequest,
            Detail = "Trip hours are invalid",
            Field = DomainError.FieldTypeEnum.TripDuration,
            Type = DomainError.ErrorTypeEnum.ValidationError
        });
            
        return false;

    }
    
    public static bool ValidateMaxDistance(double maxDistance, List<DomainError> errors)
    {
        if (!(maxDistance < 0.0)) return true;
        errors.Add( new DomainError
        {
            StatusCode = DomainError.StatusCodeEnum.BadRequest,
            Detail = "maxDistance is invalid",
            Field = DomainError.FieldTypeEnum.MaxDistance,
            Type = DomainError.ErrorTypeEnum.ValidationError
        });
            
        return false;
    }
    
    public static bool ValidateCategories(List<string> categories, List<DomainError> errors)
    {
        foreach (var category in categories)
        {
            if (Guid.TryParse(category, out var guid)) continue;
            errors.Add( new DomainError
            {
                StatusCode = DomainError.StatusCodeEnum.BadRequest,
                Detail = "Badly formatted category id was found",
                Field = DomainError.FieldTypeEnum.Categories,
                Type = DomainError.ErrorTypeEnum.ValidationError
            });
            
            return false;
        }
        
        return true;
    }
}
