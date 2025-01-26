using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MekanikApi.Application.Interfaces;
using MekanikApi.Infrastructure.Services;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MekanikApi.Application.DTOs.Common;

namespace MekanikApi.Api
{
    public static class ConfigureService
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mekanik", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer token\""
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                },
            });
            });
            services.AddAuthorizationBuilder()
                .AddPolicy("admin", policy => policy.RequireClaim("Role", "admin"));

            _ = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = Environment.GetEnvironmentVariable("JWTISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("JWTAUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTKEY")))
                };
                options.Events = new JwtBearerEvents()
                {
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsync(JsonSerializer.Serialize(new ApiResponse
                        {
                            StatusCode = StatusCodes.Status403Forbidden,
                            Message = "Forbidden",
                            Result = "Not allowed to access this resource"
                        }));
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        if (string.IsNullOrEmpty(context.Error))
                            context.Error = "Invalid Token";
                        if (string.IsNullOrEmpty(context.ErrorDescription))
                            context.ErrorDescription = "Authorization header missing.";

                        if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() ==
                            typeof(SecurityTokenExpiredException))
                        {
                            var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                            context.Response.Headers.Append("x-token-expired", "true");
                            if (authenticationException is not null)
                            {
                                Console.WriteLine($"The token expired on   {authenticationException.Expires:o}");
                            }

                            context.ErrorDescription =
                                $"Expired Token";
                        }

                        return context.Response.WriteAsync(JsonSerializer.Serialize(new ApiResponse
                        {
                            StatusCode = StatusCodes.Status401Unauthorized,
                            Message = "Invalid Token",
                            Result = context.ErrorDescription
                        }));
                    }
                };
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<FileService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IMechanicService, MechanicService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IServSpecializationService, ServSpecializationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();



            return services;
        }
    }
}