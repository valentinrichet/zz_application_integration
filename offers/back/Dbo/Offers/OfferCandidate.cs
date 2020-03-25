using System;
using System.Collections.Generic;

namespace AiOffer.Dbo.Offers
{
    public partial class OfferCandidate
    {
        public ulong Offer { get; set; }
        public ulong Candidate { get; set; }

        public virtual Offer OfferNavigation { get; set; }
    }
}
