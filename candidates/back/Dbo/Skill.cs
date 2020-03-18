using System;
using System.Collections.Generic;

namespace AiCandidate.Dbo
{
    public partial class Skill
    {
        public Skill()
        {
            CandidateSkill = new HashSet<CandidateSkill>();
        }

        public ulong Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public virtual ICollection<CandidateSkill> CandidateSkill { get; set; }
    }
}
