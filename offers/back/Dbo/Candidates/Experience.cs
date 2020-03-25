using System;
using System.Collections.Generic;

namespace AiOffer.Dbo.Candidates
{
    public partial class Experience
    {
        public ulong Id { get; set; }
        public ulong IdCandidate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public virtual Candidate IdCandidateNavigation { get; set; }
    }
}
