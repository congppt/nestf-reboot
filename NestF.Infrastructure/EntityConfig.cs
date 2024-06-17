using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NestF.Domain.Entities;

namespace NestF.Infrastructure;

public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsMany(o => o.Traces, b => b.ToJson());
    }
}

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(new Category { Id = 1, Name = "Tổ yến thô" }, new Category { Id = 2, Name = "Tổ yến tinh chế" },
            new Category { Id = 3, Name = "Yến vụn" }, new Category { Id = 4, Name = "Yến viên baby" });
    }
}