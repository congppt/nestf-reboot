using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NestF.Domain.Enums;

namespace NestF.Domain.Entities;
#pragma warning disable CS8618
[Index(nameof(Phone), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class Account
{
    public int Id { get; set; }
    [Column(TypeName = "citext")]
    public string Name { get; set; }
    public bool IsMale { get; set; }
    public string? Phone { get; set; }
    [Column(TypeName = "citext")]
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public Role Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Point { get; set; }
    public string? DefaultAddress { get; set; }
    public bool IsActive { get; set; }
    public HashSet<Order> Orders { get; set; }
    public HashSet<Transaction> Transactions { get; set; }
}