namespace NestF.Application.DTOs.Generic;
#pragma warning disable CS8618
public class Page<T> where T : class
{
    public List<T> Items { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPageCount => TotalCount % PageSize == 0 ? TotalCount / PageSize : TotalCount / PageSize + 1;
    public bool HasPrev => PageIndex > 0;
    public bool HasNext => PageIndex < TotalPageCount - 1;
}