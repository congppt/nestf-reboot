namespace NestF.Application.DTOs.Account;
#pragma warning disable CS8618
public class StaffBasicInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsMale { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}