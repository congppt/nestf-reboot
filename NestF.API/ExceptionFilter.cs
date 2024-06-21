using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Backend_API;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception.GetType())
        {
            case var t when t == typeof(KeyNotFoundException):
                context.Result = new NotFoundResult();
                return;
            case var t when t == typeof(UnauthorizedAccessException):
                context.Result = new UnauthorizedResult();
                return;
            case var t when t == typeof(DbUpdateException):
                context.Result = new UnprocessableEntityResult();
                return;
            case var t when t == typeof(ArgumentException):
                context.Result = new BadRequestResult();
                return;
        }
    }
}