using System;
using System.Collections.Generic;

namespace AiOffer.Dbo.Offers
{
    public partial class OfferSkill
    {
        public ulong Offer { get; set; }
        public ulong Skill { get; set; }

        public virtual Offer OfferNavigation { get; set; }
    }
}
