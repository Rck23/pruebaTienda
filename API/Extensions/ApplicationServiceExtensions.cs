﻿using API.Helpers;
using API.Helpers.Errors;
using API.Services;
using AspNetCoreRateLimit;
using DATA.UnitOfWork;
using ENTITIES.Entities;
using ENTITIES.Intefaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
       
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin() 
                .AllowAnyMethod() 
                .AllowAnyHeader()); 
        });

    public static void AddAplicacionServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IUserService, UserService>();
    }

    public static void ConfigureRateLimitiong(this IServiceCollection services)
    {

        services.AddMemoryCache();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddInMemoryRateLimiting();

        services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = false;
                options.HttpStatusCode = 429;
                options.RealIpHeader = "X-Real-IP";
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint ="*",
                        Period = "10s",
                        Limit =2
                    }
                };
            });
    }

    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);

            options.AssumeDefaultVersionWhenUnspecified = true;

            options.ApiVersionReader = ApiVersionReader.Combine(
                 new QueryStringApiVersionReader("ver"),
                 new HeaderApiVersionReader("X-Version"));

            options.ReportApiVersions = true;


        });
    }

    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWT>(configuration.GetSection("JWT"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false; // Esto activa y desactiva HTTPS
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                };
            });
    }

    public static void AddValidationErrors(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options => {

            options.InvalidModelStateResponseFactory = ActionContext =>
            {
                var errors = ActionContext.ModelState.Where(u => u.Value.Errors.Count > 0)
                                                        .SelectMany(u=> u.Value.Errors)
                                                        .Select(u => u.ErrorMessage).ToArray();

                var errorResponse = new ApiValidation()
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });
    }
}
