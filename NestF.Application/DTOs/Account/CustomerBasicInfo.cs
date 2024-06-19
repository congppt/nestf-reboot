namespace NestF.Application.DTOs.Account;
#pragma warning disable CS8618
public class CustomerBasicInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public bool IsMale { get; set; }
    public int Point { get; set; }
    public string? DefaultAddress { get; set; }
    public bool IsActive { get; set; }
}