using Microsoft.Extensions.Configuration;

namespace Unitic_BE.Services.Interfaces
{
    public interface IGoogleService
    {
        void ConfigureGoogleService(IServiceCollection services);
    }
}
