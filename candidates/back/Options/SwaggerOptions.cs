using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetSport.Options
{
    public class SwaggerOptions
    {
        public class ContactOptions
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public string EndPoint { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ContactOptions Contact { get; set; }
    }
}
