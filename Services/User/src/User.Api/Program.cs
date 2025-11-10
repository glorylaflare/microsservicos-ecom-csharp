using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Observability.Middlewares;
using BuildingBlocks.SharedKernel.Config;
using BuildingBlocks.Infra.Extensions;
using CorrelationId;
using CorrelationId.DependencyInjection;
using User.Api.Extensions;
using User.Application.Commands;
using User.Domain.Interfaces;
using User.Infra.Data.Context;
using User.Infra.Data.Repositories;
using User.Application.Interfaces;
using User.Infra.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddCustomLogging();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(CreateUserCommand).Assembly));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserReadService, UserReadService>();
builder.Services.AddValidators();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<WriteDbContext>();
builder.Services.AddDbContext<ReadDbContext>();

var app = builder.Build();

await app.AddMigrateDatabase<WriteDbContext>();

app.UseMiddleware<ErrorHandleMiddleware>();
app.UseCorrelationId();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
