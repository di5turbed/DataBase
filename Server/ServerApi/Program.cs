using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerApi.Data;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = "Super_Secret_Key_123456789_Esports";

var isTesting = builder.Environment.EnvironmentName == "Testing";

builder.Services.AddDbContext<EsportsContext>(options =>
{
    if (isTesting)
    {
        // Для тестов используем базу в памяти
        options.UseInMemoryDatabase("E2E_Test_Db");
    }
    else
    {
        // Для реальной работы используем PostgreSQL
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }