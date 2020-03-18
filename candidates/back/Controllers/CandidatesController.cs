using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiCandidate.Dbo;
using AiCandidate.Business.Candidates;
using AiCandidate.Dto.Candidates;
using Microsoft.AspNetCore.Authorization;
using AiCandidate.Exceptions;
using Microsoft.Extensions.Logging;
using AiCandidate.Dto.Skills;
using AiCandidate.Dto.Educations;
using AiCandidate.Dto.Experiences;

namespace AiCandidate.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : AuthorizedController
    {
        private readonly ICandidateBusiness<Candidate> _business;

        public CandidatesController(ICandidateBusiness<Candidate> business, ILogger<CandidatesController> logger) : base(logger)
        {
            _business = business;
        }

        /// <summary>
        /// Authenticate candidate
        /// </summary>
        /// <param name="authenticateCandidateDto"></param>
        /// <returns>JWT Token</returns>
        /// <response code="200">Returns the JWT Token</response>
        /// <response code="401">If authentication failed</response> 
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Authenticate(AuthenticateCandidateDto authenticateCandidateDto)
        {
            try
            {
                string token = await _business.Authenticate(authenticateCandidateDto);
                return Ok(token);
            }
            catch (AuthenticationFailedException exception)
            {
                return Unauthorized(exception.Message);
            }
        }

        /// <summary>
        /// Get a Candidate by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Candidate with given Id</returns>
        /// <response code="200">Returns the Candidate with given Id</response>
        /// <response code="401">If unauthorized</response>
        /// <response code="404">If the Candidate does not exist</response> 
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CandidateDto>> GetCandidate(ulong id)
        {
            CandidateDto candidateDto = await _business.GetFirstOrDefault<CandidateDto>((candidate => candidate.Id == id));

            if (candidateDto == null)
            {
                return NotFound($"A candidate with id \"{id}\" was not found.");
            }

            return Ok(candidateDto);
        }

        /// <summary>
        /// Create a Candidate
        /// </summary>
        /// <param name="createCandidateDto"></param>
        /// <returns>Created Candidate</returns>
        /// <response code="201">Returns the Created Candidate</response>
        /// <response code="400">If a candidate with the same mail already exists</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CandidateDto>> PostCandidate(CreateCandidateDto createCandidateDto)
        {
            try
            {
                CandidateDto candidateDto = await _business.CreateCandidate<CandidateDto>(createCandidateDto);
                return Created(candidateDto.Id.ToString(), candidateDto);
            }
            catch (DbUpdateException)
            {
                return BadRequest($"A candidate with the mail \"{createCandidateDto.Mail}\" already exists.");
            }
        }

        /// <summary>
        /// Update a Candidate by Id
        /// </summary>
        /// <remarks>
        /// You must be the authenticated as the candidate to use this route
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="updateCandidateDto"></param>
        /// <returns>Updated Candidate with given Id</returns>
        /// <response code="200">Returns the Updated Candidate with given Id</response>
        /// <response code="400">If the Candidate does not exist</response>
        /// <response code="401">If unauthorized</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CandidateDto>> PutCandidate(ulong id, UpdateCandidateDto updateCandidateDto)
        {
            if (!IsUser(id))
            {
                return Unauthorized("You can not update this candidate.");
            }

            try
            {
                CandidateDto candidateDto = await _business.UpdateCandidate<CandidateDto>(id, updateCandidateDto);
                return Ok(candidateDto);
            }
            catch (ArgumentNullException)
            {
                return BadRequest($"A candidate with id \"{id}\" was not found.");
            }
        }

        /// <summary>
        /// Add a Skill to a Candidate
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createCandidateSkillDto"></param>
        /// <returns>Added Skill to the Candidate</returns>
        /// <response code="200">Returns Added Skill to the Candidate</response>
        /// <response code="401">If unauthorized</response> 
        /// <response code="404">If the Candidate or Skill does not exist</response> 
        [HttpPost("{id}/skills")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SkillDto>> AddSkill(ulong id, CreateCandidateSkillDto createCandidateSkillDto)
        {
            if (!IsUser(id))
            {
                return Unauthorized("You can not update this candidate.");
            }

            SkillDto skillDto;

            try
            {
                skillDto = await _business.AddSkill<SkillDto>(id, createCandidateSkillDto.Id);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

            return Ok(skillDto);
        }

        /// <summary>
        /// Remove a Skill from a Candidate
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="skillId"></param>
        /// <response code="204">Returns No Content</response>
        /// <response code="401">If unauthorized</response> 
        /// <response code="404">If the Candidate does not exist</response> 
        [HttpDelete("{candidateId}/skills/{skillId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveSkill(ulong candidateId, ulong skillId)
        {
            if (!IsUser(candidateId))
            {
                return Unauthorized("You can not update this candidate.");
            }

            try
            {
                await _business.RemoveSkill(candidateId, skillId);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Add an Education to a Candidate
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createEducationDto"></param>
        /// <returns>Added Education to the Candidate</returns>
        /// <response code="200">Returns Added Education to the Candidate</response>
        /// <response code="401">If unauthorized</response> 
        /// <response code="404">If the Candidate does not exist</response> 
        [HttpPost("{id}/educations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EducationDto>> AddEducation(ulong id, CreateEducationDto createEducationDto)
        {
            if (!IsUser(id))
            {
                return Unauthorized("You can not update this candidate.");
            }

            EducationDto educationDto;

            try
            {
                educationDto = await _business.AddEducation<EducationDto>(id, createEducationDto);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

            return Ok(educationDto);
        }

        /// <summary>
        /// Update an Education from a Candidate
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="educationId"></param>
        /// <param name="updateEducationDto"></param>
        /// <returns>Updated Education from a Candidate</returns>
        /// <response code="200">Returns Updated Education from a Candidate</response>
        /// <response code="401">If unauthorized</response> 
        /// <response code="404">If the Candidate or Education does not exist</response> 
        [HttpPut("{candidateId}/educations/{educationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EducationDto>> UpdateEducation(ulong candidateId, ulong educationId, UpdateEducationDto updateEducationDto)
        {
            if (!IsUser(candidateId))
            {
                return Unauthorized("You can not update this candidate.");
            }

            EducationDto educationDto;

            try
            {
                educationDto = await _business.UpdateEducation<EducationDto>(candidateId, educationId, updateEducationDto);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

            return Ok(educationDto);
        }

        /// <summary>
        /// Remove an Education from a Candidate
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="educationId"></param>
        /// <response code="204">Returns No Content</response>
        /// <response code="401">If unauthorized</response> 
        /// <response code="404">If the Candidate does not exist</response> 
        [HttpDelete("{candidateId}/educations/{educationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveEducation(ulong candidateId, ulong educationId)
        {
            if (!IsUser(candidateId))
            {
                return Unauthorized("You can not update this candidate.");
            }

            try
            {
                await _business.RemoveEducation(candidateId, educationId);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Add an Experience to a Candidate
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createExperienceDto"></param>
        /// <returns>Added Experience to the Candidate</returns>
        /// <response code="200">Returns Added Experience to the Candidate</response>
        /// <response code="401">If unauthorized</response> 
        /// <response code="404">If the Candidate does not exist</response> 
        [HttpPost("{id}/experiences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExperienceDto>> AddExperience(ulong id, CreateExperienceDto createExperienceDto)
        {
            if (!IsUser(id))
            {
                return Unauthorized("You can not update this candidate.");
            }

            ExperienceDto experienceDto;

            try
            {
                experienceDto = await _business.AddExperience<ExperienceDto>(id, createExperienceDto);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

            return Ok(experienceDto);
        }

        /// <summary>
        /// Update an Experience from a Candidate
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="experienceId"></param>
        /// <param name="updateExperienceDto"></param>
        /// <returns>Updated Experience from a Candidate</returns>
        /// <response code="200">Returns Experience Education from a Candidate</response>
        /// <response code="401">If unauthorized</response> 
        /// <response code="404">If the Candidate or Experience does not exist</response> 
        [HttpPut("{candidateId}/experiences/{experienceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExperienceDto>> UpdateEducation(ulong candidateId, ulong experienceId, UpdateExperienceDto updateExperienceDto)
        {
            if (!IsUser(candidateId))
            {
                return Unauthorized("You can not update this candidate.");
            }

            ExperienceDto experienceDto;

            try
            {
                experienceDto = await _business.UpdateExperience<ExperienceDto>(candidateId, experienceId, updateExperienceDto);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

            return Ok(experienceDto);
        }

        /// <summary>
        /// Remove an Experience from a Candidate
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="experienceId"></param>
        /// <response code="204">Returns No Content</response>
        /// <response code="401">If unauthorized</response> 
        /// <response code="404">If the Candidate does not exist</response> 
        [HttpDelete("{candidateId}/experiences/{experienceId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveExperience(ulong candidateId, ulong experienceId)
        {
            if (!IsUser(candidateId))
            {
                return Unauthorized("You can not update this candidate.");
            }

            try
            {
                await _business.RemoveExperience(candidateId, experienceId);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

            return NoContent();
        }
    }
}
