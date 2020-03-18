using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetSport.Options
{
    public class HashingOptions
    {
        public string Salt { get; set; }
        public int KeySize { get; set; }
        public int Iterations { get; set; }
    }
}
