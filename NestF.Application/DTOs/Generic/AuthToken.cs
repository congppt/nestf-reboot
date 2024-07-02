namespace NestF.Application.DTOs.Generic;
#pragma warning disable CS8618
public class AuthToken
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}