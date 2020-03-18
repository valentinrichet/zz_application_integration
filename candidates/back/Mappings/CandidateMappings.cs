using AiCandidate.Dbo;
using AiCandidate.Dto.Candidates;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Mappings
{
    public class CandidateMappings : Profile
    {
        public CandidateMappings()
        {
            CreateMap<Candidate, CandidateDto>()
                .ForMember(
                    x => x.Skills,
                    opt => opt.MapFrom(c => c.CandidateSkill.Select(candidateSkill => candidateSkill.SkillNavigation))
                )
                .ForMember(
                    x => x.Educations,
                    opt => opt.MapFrom(c => c.Education)
                )
                .ForMember(
                    x => x.Experiences,
                    opt => opt.MapFrom(c => c.Experience)
                );

            CreateMap<CreateCandidateDto, Candidate>();

            CreateMap<UpdateCandidateDto, Candidate>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
