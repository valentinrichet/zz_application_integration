using AiCandidate.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Dto.Educations
{
    public class EducationDto
    {
        [Required]
        public ulong Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Level(ErrorMessage = "Level is not a valid value (L1, L2, L3, M1, M2, D1, D2, +)")]
        public string Level { get; set; }
    }
}
