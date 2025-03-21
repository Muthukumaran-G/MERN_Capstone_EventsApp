using Microsoft.EntityFrameworkCore;
using AuthenticationService.Data;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionString = $"Data Source={dbHost}; Initial Catalog={dbName};User ID=sa;Password={dbPassword}";
builder.Services.AddDbContext<AuthContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddConsulConfig(builder.Configuration);
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseConsul(builder.Configuration);

app.Run();
