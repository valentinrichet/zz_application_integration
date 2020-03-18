using AiCandidate.Dbo;
using AiCandidate.Dto.Skills;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Mappings
{
    public class SkillMappings : Profile
    {
        public SkillMappings()
        {
            CreateMap<Skill, SkillDto>().ReverseMap();
        }
    }
}
