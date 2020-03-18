using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCandidate.Business;
using AiCandidate.Dbo;
using AiCandidate.Dto.Skills;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AiCandidate.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : AuthorizedController
    {
        private readonly IBusiness<Skill> _business;

        public SkillsController(IBusiness<Skill> business, ILogger<SkillsController> logger) : base(logger)
        {
            _business = business;
        }

        /// <summary>
        /// Get All Skills
        /// </summary>
        /// <returns>All skills</returns>
        /// <response code="200">Returns all skills</response>        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<SkillDto>>> GetSkills()
        {
            ICollection<SkillDto> skillsDto = await _business.GetAll<SkillDto>();
            return Ok(skillsDto);
        }

        /// <summary>
        /// Get a Skill by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Skill with given Id</returns>
        /// <response code="200">Returns the Skill with given Id</response>
        /// <response code="404">If the Skill does not exist</response> 
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SkillDto>> GetSkill(ulong id)
        {
            SkillDto skillDto = await _business.GetFirstOrDefault<SkillDto>((skill => skill.Id == id));

            if (skillDto == null)
            {
                return NotFound($"A skill with id \"{id}\" was not found.");
            }

            return Ok(skillDto);
        }
    }
}