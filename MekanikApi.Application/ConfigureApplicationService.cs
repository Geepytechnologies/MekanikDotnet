using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using MekanikApi.Application.DTOs.Auth.Requests;
using MekanikApi.Application.mapping;
using System.Reflection;

namespace sispay.Application
{
    public static class ConfigureApplicationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<LoginRequestDTOValidator>();
            return services;
        }
    }
}