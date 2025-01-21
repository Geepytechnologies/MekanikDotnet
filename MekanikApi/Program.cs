using DotNetEnv;
using MekanikApi.Api;
using MekanikApi.Api.Filters;
using MekanikApi.Infrastructure;
using MekanikApi.Infrastructure.Seed;
using MekanikApi.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using sispay.Application;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelStateFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    _ = RoleSeeder.Seed(services);
    VehicleBrandSeeder.Seed(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatService>("/chat");

app.Run();
