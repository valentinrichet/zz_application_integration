using AiCandidate.Dto.Educations;
using AiCandidate.Dto.Experiences;
using AiCandidate.Dto.Skills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Dto.Candidates
{
    public class CandidateDto
    {
        [Required]
        public ulong Id { get; set; }
        [Required]
        [EmailAddress]
        public string Mail { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public ICollection<SkillDto> Skills { get; set; }
        [Required]
        public ICollection<EducationDto> Educations { get; set; }
        [Required]
        public ICollection<ExperienceDto> Experiences { get; set; }
    }
}
