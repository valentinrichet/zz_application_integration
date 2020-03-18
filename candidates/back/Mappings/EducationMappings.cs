using AiCandidate.Dbo;
using AiCandidate.Dto.Educations;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Mappings
{
    public class EducationMappings : Profile
    {
        public EducationMappings()
        {
            CreateMap<Education, EducationDto>().ReverseMap();

            CreateMap<CreateEducationDto, Education>();

            CreateMap<UpdateEducationDto, Education>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
