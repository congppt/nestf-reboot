using Backend_API;
using NestF.Infrastructure;
using SilkierQuartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseIIS();
builder.Host.ConfigureSilkierQuartzHost();
// builder.WebHost.UseSentry(opt =>
// {
//     opt.Dsn = builder.Configuration["Sentry:Dsn"];
//     opt.SendDefaultPii = true;
//     opt.MaxRequestBodySize = RequestSize.Always;
//     opt.MinimumBreadcrumbLevel = LogLevel.Debug;
//     opt.MinimumEventLevel = LogLevel.Error;
//     opt.AttachStacktrace = true;
//     opt.TracesSampleRate = 1;
//     opt.DiagnosticLevel = SentryLevel.Error;
// });
builder.Services.AddInfra(builder.Configuration);
builder.Services.AddWebService(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();