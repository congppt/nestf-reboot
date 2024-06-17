namespace NestF.Application.Interfaces.Services;

public interface IClaimService
{
    T GetClaim<T>(string claimType, T defaultValue);
    string GetIpAddress();
}