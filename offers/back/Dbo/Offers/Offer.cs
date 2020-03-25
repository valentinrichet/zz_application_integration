using System;
using System.Collections.Generic;

namespace AiOffer.Dbo.Offers
{
    public partial class Offer
    {
        public Offer()
        {
            OfferCandidate = new HashSet<OfferCandidate>();
            OfferSkill = new HashSet<OfferSkill>();
        }

        public ulong Id { get; set; }
        public ulong IdCompany { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public decimal? Wage { get; set; }

        public virtual ICollection<OfferCandidate> OfferCandidate { get; set; }
        public virtual ICollection<OfferSkill> OfferSkill { get; set; }
    }
}
