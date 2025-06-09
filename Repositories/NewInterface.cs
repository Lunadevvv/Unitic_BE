using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.Repositories
{
    public interface NewInterface
    {
        // Define methods that the implementing class must provide
        Task<IEnumerable<string>> GetValuesAsync();
    }
}