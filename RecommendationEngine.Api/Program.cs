// RecommendationEngine.Api/Program.cs
using Microsoft.EntityFrameworkCore;
using RecommendationEngine.Application.Services;
using RecommendationEngine.Infrastructure.Database;
using RecommendationEngine.Infrastructure.S3;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
//var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
//builder.Services.AddDbContext<MySqlDbContext>(options =>
//    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDbContext<MySqlDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mysqlOptions => mysqlOptions.EnableStringComparisonTranslations()); // Enable StringComparison translations
});

// Add Application Services
// Register services
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IS3Service, S3Service>();
builder.Services.AddScoped<IColumnConfigurationService, ColumnConfigurationService>();
builder.Services.AddScoped<ColumnConfigurationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
