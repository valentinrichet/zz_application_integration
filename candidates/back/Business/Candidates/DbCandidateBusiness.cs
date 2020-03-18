using AutoMapper;
using AiCandidate.Dbo;
using AiCandidate.Repositories;
using AiCandidate.Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCandidate.Dto.Candidates;
using AiCandidate.Services.JwtGenerator;
using AiCandidate.Services.PasswordHasher;
using AiCandidate.Exceptions;
using Microsoft.Extensions.Logging;
using AiCandidate.Dto.Educations;
using AiCandidate.Dto.Experiences;

namespace AiCandidate.Business.Candidates
{
    public class DbCandidateBusiness : Business<Candidate, IRepository<Candidate>>, ICandidateBusiness<Candidate>
    {
        private readonly IRepository<Skill> _skillRepository;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public DbCandidateBusiness(IRepository<Candidate> repository, IRepository<Skill> skillRepository, IMapper mapper, ILogger<ICandidateBusiness<Candidate>> logger, IJwtGenerator jwtGenerator, IPasswordHasher passwordHasher) : base(repository, mapper, logger)
        {
            _skillRepository = skillRepository;
            _jwtGenerator = jwtGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<Dto> AddEducation<Dto>(ulong primaryKey, CreateEducationDto createEducationDto)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Id == primaryKey).Include(candidate => candidate.Education).Single();
            }
            finally
            {
                if (candidate == null)
                {
                    throw new Exception($"A candidate with id \"{primaryKey}\" was not found.");
                }
            }

            Education education = _mapper.Map<Education>(createEducationDto);
            education.IdCandidate = candidate.Id;
            education.Level = education.Level.ToUpper();
            candidate.Education.Add(education);

            candidate = await _repository.Update(candidate);

            Dto mappedEducation = _mapper.Map<Dto>(education);

            return mappedEducation;
        }

        public async Task<Dto> AddExperience<Dto>(ulong primaryKey, CreateExperienceDto createExperienceDto)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Id == primaryKey).Include(candidate => candidate.Experience).Single();
            }
            finally
            {
                if (candidate == null)
                {
                    throw new Exception($"A candidate with id \"{primaryKey}\" was not found.");
                }
            }

            Experience experience = _mapper.Map<Experience>(createExperienceDto);
            experience.IdCandidate = candidate.Id;
            candidate.Experience.Add(experience);

            candidate = await _repository.Update(candidate);

            Dto mappedExperience = _mapper.Map<Dto>(experience);

            return mappedExperience;
        }

        public async Task<Dto> AddSkill<Dto>(ulong primaryKey, ulong skillId)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Id == primaryKey).Include(candidate => candidate.CandidateSkill).ThenInclude(candidateSkill => candidateSkill.SkillNavigation).Single();
            }
            finally
            {
                if (candidate == null)
                {
                    throw new Exception($"A candidate with id \"{primaryKey}\" was not found.");
                }
            }

            Skill skill = await _skillRepository.Get(skillId);
            if (skill == null)
            {
                throw new Exception($"A skill with id \"{skillId}\" was not found.");
            }

            if (!candidate.CandidateSkill.Any(candidateSkill => candidateSkill.Skill == skillId))
            {
                candidate.CandidateSkill.Add(new CandidateSkill { Candidate = primaryKey, Skill = skillId, SkillNavigation = skill });
                candidate = await _repository.Update(candidate);
            }

            Dto mappedSkill = _mapper.Map<Dto>(skill);

            return mappedSkill;
        }

        public Task<string> Authenticate(AuthenticateCandidateDto authenticateCandidateDto)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Mail == authenticateCandidateDto.Mail).Single();
            }
            finally
            {
                if (candidate == null || !_passwordHasher.Check(authenticateCandidateDto.Password, candidate.HashedPassword))
                {
                    throw new AuthenticationFailedException();
                }
            }

            string token = _jwtGenerator.GenerateToken(candidate.Id);
            return Task.FromResult(token);
        }

        public async Task<Dto> CreateCandidate<Dto>(CreateCandidateDto createCandidateDto)
        {
            Candidate candidate = _mapper.Map<Candidate>(createCandidateDto);
            candidate.HashedPassword = _passwordHasher.Hash(createCandidateDto.Password);
            candidate = await _repository.Add(candidate);
            Dto mappedCandidate = _mapper.Map<Dto>(candidate);
            return mappedCandidate;
        }

        public async Task RemoveEducation(ulong primaryKey, ulong educationId)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Id == primaryKey).Include(candidate => candidate.Education).Single();
            }
            finally
            {
                if (candidate == null)
                {
                    throw new Exception($"A candidate with id \"{primaryKey}\" was not found.");
                }
            }

            Education education = candidate.Education.SingleOrDefault(education => education.Id == educationId);

            if (education != null)
            {
                candidate.Education.Remove(education);
                candidate = await _repository.Update(candidate);
            }
        }

        public async Task RemoveExperience(ulong primaryKey, ulong experienceId)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Id == primaryKey).Include(candidate => candidate.Experience).Single();
            }
            finally
            {
                if (candidate == null)
                {
                    throw new Exception($"A candidate with id \"{primaryKey}\" was not found.");
                }
            }

            Experience experience = candidate.Experience.SingleOrDefault(experience => experience.Id == experienceId);

            if (experience != null)
            {
                candidate.Experience.Remove(experience);
                candidate = await _repository.Update(candidate);
            }
        }

        public async Task RemoveSkill(ulong primaryKey, ulong skillId)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Id == primaryKey).Include(candidate => candidate.CandidateSkill).ThenInclude(candidateSkill => candidateSkill.SkillNavigation).Single();
            }
            finally
            {
                if (candidate == null)
                {
                    throw new Exception($"A candidate with id \"{primaryKey}\" was not found.");
                }
            }

            CandidateSkill candidateSkill = candidate.CandidateSkill.SingleOrDefault(candidateSkill => candidateSkill.Skill == skillId);

            if (candidateSkill != null)
            {
                candidate.CandidateSkill.Remove(candidateSkill);
                candidate = await _repository.Update(candidate);
            }
        }

        public async Task<Dto> UpdateCandidate<Dto>(ulong primaryKey, UpdateCandidateDto updateCandidateDto)
        {
            Candidate candidate = await _repository.Get(primaryKey);
            _mapper.Map(updateCandidateDto, candidate);

            if (updateCandidateDto.Password != null)
            {
                candidate.HashedPassword = _passwordHasher.Hash(updateCandidateDto.Password);
            }

            candidate = await _repository.Update(candidate);
            Dto mappedCandidate = _mapper.Map<Dto>(candidate);
            return mappedCandidate;
        }

        public async Task<Dto> UpdateEducation<Dto>(ulong primaryKey, ulong educationId, UpdateEducationDto updateEducationDto)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Id == primaryKey).Include(candidate => candidate.Education).Single();
            }
            finally
            {
                if (candidate == null)
                {
                    throw new Exception($"A candidate with id \"{primaryKey}\" was not found.");
                }
            }

            Education education = candidate.Education.SingleOrDefault(education => education.Id == educationId);

            if(education == null)
            {
                throw new Exception($"An education with id \"{educationId}\" was not found.");
            }

            _mapper.Map(updateEducationDto, education);
            education.Level = education.Level.ToUpper();
            candidate = await _repository.Update(candidate);
            Dto mappedEducation = _mapper.Map<Dto>(education);

            return mappedEducation;
        }

        public async Task<Dto> UpdateExperience<Dto>(ulong primaryKey, ulong experienceId, UpdateExperienceDto updateExperienceDto)
        {
            Candidate candidate = null;

            try
            {
                candidate = _repository.GetAll().Where(candidate => candidate.Id == primaryKey).Include(candidate => candidate.Experience).Single();
            }
            finally
            {
                if (candidate == null)
                {
                    throw new Exception($"A candidate with id \"{primaryKey}\" was not found.");
                }
            }

            Experience experience = candidate.Experience.SingleOrDefault(experience => experience.Id == experienceId);

            if (experience == null)
            {
                throw new Exception($"An experience with id \"{experienceId}\" was not found.");
            }

            _mapper.Map(updateExperienceDto, experience);
            candidate = await _repository.Update(candidate);
            Dto mappedExperience = _mapper.Map<Dto>(experience);

            return mappedExperience;
        }
    }
}
