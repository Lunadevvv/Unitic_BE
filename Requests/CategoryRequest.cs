using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.Requests
{
    public record CategoryRequest(string Name, bool IsDisabled);
}
