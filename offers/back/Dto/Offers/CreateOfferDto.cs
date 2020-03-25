using AiOffer.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Dto.Offers
{
    public class CreateOfferDto
    {
        [Required]
        public ulong IdCompany { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Level(ErrorMessage = "Level is not a valid value (TRAINEE, JUNIOR, SENIOR, OTHER)")]
        public string Level { get; set; }
        [Required]
        [Type(ErrorMessage = "Type is not a valid value (PART-TIME, FULL-TIME, OTHER)")]
        public string Type { get; set; }
        public decimal? Wage { get; set; }
        public ICollection<ulong> Skills { get; set; }
    }
}
