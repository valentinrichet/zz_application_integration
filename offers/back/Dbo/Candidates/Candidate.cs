using System;
using System.Collections.Generic;

namespace AiOffer.Dbo.Candidates
{
    public partial class Candidate
    {
        public Candidate()
        {
            CandidateSkill = new HashSet<CandidateSkill>();
            Education = new HashSet<Education>();
            Experience = new HashSet<Experience>();
        }

        public ulong Id { get; set; }
        public string Mail { get; set; }
        public string HashedPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CandidateSkill> CandidateSkill { get; set; }
        public virtual ICollection<Education> Education { get; set; }
        public virtual ICollection<Experience> Experience { get; set; }
    }
}
