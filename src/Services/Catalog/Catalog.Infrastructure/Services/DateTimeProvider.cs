using Catalog.Application.Common.Interfaces;

namespace Catalog.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
