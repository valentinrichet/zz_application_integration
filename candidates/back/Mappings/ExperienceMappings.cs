using AiCandidate.Dbo;
using AiCandidate.Dto.Experiences;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Mappings
{
    public class ExperienceMappings : Profile
    {
        public ExperienceMappings()
        {
            CreateMap<Experience, ExperienceDto>().ReverseMap();

            CreateMap<CreateExperienceDto, Experience>();

            CreateMap<UpdateExperienceDto, Experience>();
        }
    }
}
