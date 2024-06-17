using NestF.Application.Interfaces.Services;

namespace NestF.Infrastructure.Implements.Services;

public class TimeService : ITimeService
{
    public DateTime Now => DateTime.UtcNow;
}