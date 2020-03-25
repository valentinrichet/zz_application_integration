using System;
using System.Collections.Generic;

namespace AiOffer.Dbo.Companies
{
    public partial class Company
    {
        public ulong Id { get; set; }
        public string Mail { get; set; }
        public string HashedPassword { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }
}
