using AiCandidate.Dto.Candidates;
using AiCandidate.Dto.Educations;
using AiCandidate.Dto.Experiences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Business.Candidates
{
    public interface ICandidateBusiness<TEntity> : IBusiness<TEntity>
    {
        Task<string> Authenticate(AuthenticateCandidateDto authenticateCandidateDto);
        Task<Dto> CreateCandidate<Dto>(CreateCandidateDto createCandidateDto);
        Task<Dto> UpdateCandidate<Dto>(ulong primaryKey, UpdateCandidateDto updateCandidateDto);
        Task<Dto> AddSkill<Dto>(ulong primaryKey, ulong skillId);
        Task RemoveSkill(ulong primaryKey, ulong skillId);
        Task<Dto> AddEducation<Dto>(ulong primaryKey, CreateEducationDto createEducationDto);
        Task<Dto> UpdateEducation<Dto>(ulong primaryKey, ulong educationId, UpdateEducationDto updateEducationDto);
        Task RemoveEducation(ulong primaryKey, ulong educationId);
        Task<Dto> AddExperience<Dto>(ulong primaryKey, CreateExperienceDto createExperienceDto);
        Task<Dto> UpdateExperience<Dto>(ulong primaryKey, ulong experienceId, UpdateExperienceDto updateExperienceDto);
        Task RemoveExperience(ulong primaryKey, ulong experienceId);
    }
}
