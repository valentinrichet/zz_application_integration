using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Dto.Offers
{
    public class CreateOfferCandidateDto
    {
        [Required]
        public ulong Offer { get; set; }
        [Required]
        public ulong Candidate { get; set; }
    }
}
