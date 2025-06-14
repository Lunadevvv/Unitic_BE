using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;

using Unitic_BE.Services.Interfaces;

namespace Unitic_BE.Services
{
    public class GoogleService : IGoogleService
    {
        public void ConfigureGoogleService(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            services
            .AddAuthentication()
            .AddGoogleOpenIdConnect("GoogleConnect", options =>
            {
                options.ClientId = configuration["Google:ClientId"];
                options.ClientSecret = configuration["Google:ClientSecret"];
                options.SaveTokens = true; // Lưu token để sử dụng sau này
                options.Scope.Add("email");
                options.Scope.Add("profile");
            });
        }
    }
}
