using System.Text;
using Backend_API.Implements.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NestF.Application.Interfaces.Services;
using NestF.Infrastructure.Constants;
using Quartz.AspNetCore;
using SilkierQuartz;
using SilkierQuartz.Authorization;

namespace Backend_API;

public static class DepsInject
{
    public static void AddWebService(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors(options =>
            options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options =>
        //     {
        //         options.RequireHttpsMetadata = false;
        //         options.SaveToken = true;
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuer = true,
        //             ValidateAudience = true,
        //             ValidateLifetime = true,
        //             ValidateIssuerSigningKey = true,
        //             ValidIssuer = config["Jwt:Issuer"],
        //             ValidAudience = config["Jwt:Audience"],
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        //         };
        //     });
        //services.AddScoped<AccessCheckMiddleware>();
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.AddScheme<FirebaseTokenHandler>(JwtBearerDefaults.AuthenticationScheme, "FirebaseScheme");
        }).AddJwtBearer(DefaultConstants.STAFF_SCHEME, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
            };
        });
        var firebaseApp = FirebaseApp.DefaultInstance;
        firebaseApp ??= FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(config["Firebase"]),
        });
        services.AddSingleton(firebaseApp);
        services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });
        services.AddSingleton(new SilkierQuartzOptions { VirtualPathRoot = "/quartz", UseLocalTime = true });
        services.AddSingleton(new SilkierQuartzAuthenticationOptions
            { AccessRequirement = SilkierQuartzAuthenticationOptions.SimpleAccessRequirement.AllowAnonymous });
        services.AddAuthorization(opts => opts.AddPolicy("SilkierQuartz",
            builder => builder.AddRequirements(
                new SilkierQuartzDefaultAuthorizationRequirement(SilkierQuartzAuthenticationOptions
                    .SimpleAccessRequirement.AllowAnonymous))));
        services.AddScoped<IAuthorizationHandler, SilkierQuartzDefaultAuthorizationHandler>();
        services.AddHttpContextAccessor();
        services.AddSingleton<IClaimService, ClaimService>();
        services.AddControllers();
    }
}