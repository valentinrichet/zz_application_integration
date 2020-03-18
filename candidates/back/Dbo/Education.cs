using System;
using System.Collections.Generic;

namespace AiCandidate.Dbo
{
    public partial class Education
    {
        public ulong Id { get; set; }
        public ulong IdCandidate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }

        public virtual Candidate IdCandidateNavigation { get; set; }
    }
}
