using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Requests
{
    public record EventRequest(string Name, string Image, string Description, string Date_Start, string Date_End, int Price, string CateID, int Slot, string Location);
}
