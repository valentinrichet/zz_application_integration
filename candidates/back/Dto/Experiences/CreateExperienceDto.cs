using AiCandidate.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Dto.Experiences
{
    public class CreateExperienceDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Comparison("End", ComparisonType.LessThanOrEqualTo, ErrorMessage = "Start must be sooner than End")]
        public DateTime Start { get; set; }
        [Comparison("Start", ComparisonType.GreaterThanOrEqualTo, ErrorMessage = "End must be later than Start")]
        public DateTime? End { get; set; }
    }
}
