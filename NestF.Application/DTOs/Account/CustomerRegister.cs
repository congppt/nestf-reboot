namespace NestF.Application.DTOs.Account;
#pragma warning disable CS8618
public class CustomerRegister
{
    public string Name { get; set; }
    public bool IsMale { get; set; }
    public string? DefaultAddress { get; set; }
}