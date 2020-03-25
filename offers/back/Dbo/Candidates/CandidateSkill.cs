using System;
using System.Collections.Generic;

namespace AiOffer.Dbo.Candidates
{
    public partial class CandidateSkill
    {
        public ulong Candidate { get; set; }
        public ulong Skill { get; set; }

        public virtual Candidate CandidateNavigation { get; set; }
        public virtual Skill SkillNavigation { get; set; }
    }
}
