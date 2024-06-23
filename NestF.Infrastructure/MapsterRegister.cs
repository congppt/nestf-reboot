using Mapster;
using NestF.Application.DTOs.Product;
using NestF.Domain.Entities;
using NestF.Domain.Enums;

namespace NestF.Infrastructure;

public class MapsterRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductBasicInfo>()
            .Map(dest => dest.ImgPath, src => src.ImgPaths[0], src => src.ImgPaths.Count > 0)
            .Map(dest => dest.IsAvailable, src => src.Status == ProductStatus.Available);
        config.NewConfig<Product, ProductDetailInfo>()
            .Map(dest => dest.IsAvailable, src => src.Status == ProductStatus.Available);
    }
}