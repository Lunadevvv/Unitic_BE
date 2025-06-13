using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Unitic_BE.Models;
using Unitic_BE.Profiles;
using Unitic_BE.Repositories;
using Unitic_BE.Repositories.Interfaces;
using Unitic_BE.Services;
using Unitic_BE.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

//SQL Server
builder.Services.AddDbContext<UniticDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            //bật mấy cái này mới check validate 
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, //"exp"
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], //"iss"
            ValidAudience = builder.Configuration["Jwt:Audience"], //"aud"
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) //"key"
        };
    });

//Automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
//User
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
//Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//authentication and authorization
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();