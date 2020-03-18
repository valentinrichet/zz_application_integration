using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Exceptions
{
    public class ConditionFailedException : Exception
    {
        public ConditionFailedException() : base("The condition was not met.")
        {
        }

        public ConditionFailedException(string message)
            : base(message)
        {
        }

        public ConditionFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
