﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.Exceptions
{
    public class ObjectNotFoundException(string objectName) : Exception($"{objectName} not found");
}
