using AiOffer.Dto.Companies;
using AiOffer.Dto.Skills;
using AiOffer.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Dto.Offers
{
    public class OfferDto
    {
        [Required]
        public ulong Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string Level { get; set; }
        [Required]
        public string Type { get; set; }
        public decimal? Wage { get; set; }
        [Required]
        public CompanyDto Company { get; set; }
        [Required]
        public ICollection<SkillDto> Skills { get; set; }
    }
}
