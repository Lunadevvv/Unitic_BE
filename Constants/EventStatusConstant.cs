using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.Constants
{
    public static class EventStatusConstant
    {
        public const string Private = "Private";
        public const string Published = "Published";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string SoldOut = "SoldOut";
        public static readonly List<string> AllStatuses = new List<string>
        {
            Private,
            Published,
            InProgress,
            Completed,
            Cancelled,
            SoldOut
        };
    }
}
