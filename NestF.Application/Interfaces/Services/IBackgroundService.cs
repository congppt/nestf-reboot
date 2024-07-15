namespace NestF.Application.Interfaces.Services;

public interface IBackgroundService
{
    Task EnqueueSendPasswordMailJobAsync(int accountId);
}