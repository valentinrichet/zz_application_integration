using AiOffer.Dbo.Candidates;
using AiOffer.Dto.Skills;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Mappings
{
    public class SkillMappings : Profile
    {
        public SkillMappings()
        {
            CreateMap<Skill, SkillDto>();
        }
    }
}
