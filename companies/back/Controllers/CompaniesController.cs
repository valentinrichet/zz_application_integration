using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiCompany.Dbo;
using AiCompany.Repositories;
using AiCompany.Business;
using System.ComponentModel.DataAnnotations;
using AiCompany.Business.Companies;
using AiCompany.Dto.Companies;
using Microsoft.AspNetCore.Authorization;
using AiCompany.Exceptions;
using AiCompany.Services.JwtGenerator;
using Microsoft.Extensions.Logging;

namespace AiCompany.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : AuthorizedController
    {
        private readonly ICompanyBusiness<Company> _business;

        public CompaniesController(ICompanyBusiness<Company> business, ILogger<CompaniesController> logger) : base(logger)
        {
            _business = business;
        }

        /// <summary>
        /// Authenticate company
        /// </summary>
        /// <param name="authenticateCompanyDto"></param>
        /// <returns>JWT Token</returns>
        /// <response code="200">Returns the JWT Token</response>
        /// <response code="401">If authentication failed</response> 
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Authenticate(AuthenticateCompanyDto authenticateCompanyDto)
        {
            try
            {
                string token = await _business.Authenticate(authenticateCompanyDto);
                return Ok(token);
            }
            catch (AuthenticationFailedException exception)
            {
                return Unauthorized(exception.Message);
            }
        }

        /// <summary>
        /// Get a Company by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Company with given Id</returns>
        /// <response code="200">Returns the Company with given Id</response>
        /// <response code="404">If the Company does not exist</response> 
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanyDto>> GetCompany(ulong id)
        {
            CompanyDto companyDto = await _business.GetFirstOrDefault<CompanyDto>((company => company.Id == id));

            if (companyDto == null)
            {
                return NotFound($"A company with id \"{id}\" was not found.");
            }

            return Ok(companyDto);
        }

        /// <summary>
        /// Create a Company
        /// </summary>
        /// <param name="createCompanyDto"></param>
        /// <returns>Created Company</returns>
        /// <response code="201">Returns the Created Company</response>
        /// <response code="400">If a company with the same mail already exists</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostCompany(CreateCompanyDto createCompanyDto)
        {
            try
            {
                CompanyDto companyDto = await _business.CreateCompany<CompanyDto>(createCompanyDto);
                return Created(companyDto.Id.ToString(), companyDto);
            } catch(DbUpdateException)
            {
                return BadRequest($"A company with the mail \"{createCompanyDto.Mail}\" already exists.");
            }
        }

        /// <summary>
        /// Update a Company by Id
        /// </summary>
        /// <remarks>
        /// You must be the authenticated as the company to use this route
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="updateCompanyDto"></param>
        /// <returns>Updated Company with given Id</returns>
        /// <response code="200">Returns the Updated Company with given Id</response>
        /// <response code="400">If the Company does not exist</response>
        /// <response code="401">If unauthorized</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutCompany(ulong id, UpdateCompanyDto updateCompanyDto)
        {
            if (!IsUser(id))
            {
                return Unauthorized("You can not update this company.");
            }

            try
            {
                CompanyDto companyDto = await _business.UpdateCompany<CompanyDto>(id, updateCompanyDto);
                return Ok(companyDto);
            }
            catch (ArgumentNullException)
            {
                return BadRequest($"A company with id \"{id}\" was not found.");
            }
        }
    }
}
