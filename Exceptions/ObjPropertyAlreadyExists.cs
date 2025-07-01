using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.Exceptions
{
    public class ObjPropertyAlreadyExists(string obj) : Exception($"{obj} already exists");

}
