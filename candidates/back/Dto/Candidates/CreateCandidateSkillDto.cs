using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Dto.Candidates
{
    public class CreateCandidateSkillDto
    {
        [Required]
        public ulong Id { get; set; }
    }
}
