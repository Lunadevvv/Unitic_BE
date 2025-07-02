using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.Exceptions
{
    public class UniversityNameAlreadyExistsException(string name) : Exception($"University with name: {name} already exists");
}
