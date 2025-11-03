using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.SharedKernel.Config;
using Microsoft.EntityFrameworkCore;
using Serilog;
using User.Api.Extensions;
using User.Application.Commands;
using User.Domain.Interfaces;
using User.Infra.Data.Context;
using User.Infra.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(CreateUserCommand).Assembly));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddValidators();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<UserDbContext>();

builder.Services.AddCustomLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
    Log.Information("User Database Migrated Successfully");
}

app.UseAuthorization();

app.MapControllers();

app.Run();
