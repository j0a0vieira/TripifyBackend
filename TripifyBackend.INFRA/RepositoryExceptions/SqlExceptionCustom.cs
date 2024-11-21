using TripifyBackend.DOMAIN.Models;

namespace TripifyBackend.INFRA.RepositoryExceptions;

public class SqlExceptionCustom : Exception
{
    public DomainError DomainError { get; set; }
}