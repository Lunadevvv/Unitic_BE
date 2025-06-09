using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.Services
{
    public interface IServices
    {
        // Define methods that the implementing class must provide
        Task<IEnumerable<string>> GetValuesAsync();
    }
}