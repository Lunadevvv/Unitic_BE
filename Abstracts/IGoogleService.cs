using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Unitic_BE.Abstracts
{
    public interface IGoogleService
    {
        void ConfigureGoogleService(IServiceCollection services);

        Task<string> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);
    }
}