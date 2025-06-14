using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Google.Apis.Auth.AspNetCore3;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Unitic API",
        Version = "v1",
        Description = "API for Unitic application"
    });
});
builder.Services.AddControllers();

//SQL Server
builder.Services.AddDbContext<UniticDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})
.AddGoogleOpenIdConnect("GoogleOpenIdConnect", options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    options.SaveTokens = true;
    options.Scope.Add("email");
    options.Scope.Add("profile");
});


// Configure Google authentication
//builder.Services.ConfigureGoogleService(builder.Configuration); 

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
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Unitic API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

//authentication and authorization
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.MapGet("/auth/google", async context =>
{
    await context.ChallengeAsync("GoogleOpenIdConnect", new AuthenticationProperties
    {
        RedirectUri = "/signin-google" // What your backend handles after login
    });
});


app.Run();