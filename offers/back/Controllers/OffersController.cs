using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiOffer.Dbo;
using AiOffer.Repositories;
using Microsoft.AspNetCore.Authorization;
using AiOffer.Exceptions;
using AiCompany.Services.JwtGenerator;
using Microsoft.Extensions.Logging;
using AiOffer.Dbo.Offers;
using AiOffer.Business.Offers;
using AiOffer.Dto.Offers;

namespace AiOffer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : AuthorizedController
    {
        private readonly IOfferBusiness<Offer> _business;

        public OffersController(IOfferBusiness<Offer> business, ILogger<OffersController> logger) : base(logger)
        {
            _business = business;
        }

        /// <summary>
        /// Get Offers
        /// </summary>
        /// <returns>Offers</returns>
        /// <response code="200">Returns multiple Offers</response>
        /// <response code="400">If something wrong happened</response>
        /// <response code="401">If unauthorized</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OfferDto>> GetOffers([FromQuery] string search, [FromQuery] int page)
        {
            try
            {
                IEnumerable<OfferDto> offersDto = await _business.GetOffers(search, page);
                return Ok(offersDto);
            }
            catch (ConditionFailedException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Get an Offer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Offer with given Id</returns>
        /// <response code="200">Returns the Offer with given Id</response>
        /// <response code="400">If something wrong happened</response>
        /// <response code="401">If unauthorized</response>
        /// <response code="404">If the Offer does not exist</response> 
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OfferDto>> GetOffer(ulong id)
        {
            try
            {
                OfferDto offerDto = await _business.GetOffer(id);

                if (offerDto == null)
                {
                    return NotFound($"An offer with id \"{id}\" was not found.");
                }

                return Ok(offerDto);
            }
            catch (ConditionFailedException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Create an offer
        /// </summary>
        /// <param name="createOfferDto"></param>
        /// <returns>Created Offer</returns>
        /// <response code="201">Returns the Created Offer</response>
        /// <response code="400">If something wrong happened</response>
        /// <response code="401">If unauthorized</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> PostOffer(CreateOfferDto createOfferDto)
        {
            try
            {
                if (!IsCompany(createOfferDto.IdCompany))
                {
                    return Unauthorized("You can not create this offer with this company.");
                }

                OfferDto offerDto = await _business.CreateOffer(createOfferDto);
                return Created(offerDto.Id.ToString(), offerDto);
            }
            catch (ConditionFailedException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Update an Offer by Id
        /// </summary>
        /// <remarks>
        /// You must be the authenticated as the company to use this route
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="updateOfferDto"></param>
        /// <returns>Updated Offer with given Id</returns>
        /// <response code="200">Returns the Updated Offer with given Id</response>
        /// <response code="400">If something went wrong</response>
        /// <response code="401">If unauthorized</response>
        /// <response code="404">If the Offer does not exist</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutOffer(ulong id, UpdateOfferDto updateOfferDto)
        {
            try
            {
                OfferDto offerDto = await _business.UpdateOffer(id, UserId, updateOfferDto);

                if (offerDto == null)
                {
                    return NotFound($"An offer with id \"{id}\" was not found.");
                }

                return Ok(offerDto);
            }
            catch (AuthenticationFailedException)
            {
                return Unauthorized("You can not update this offer.");
            }
            catch (ConditionFailedException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Delete an Offer by Id
        /// </summary>
        /// <remarks>
        /// You must be the authenticated as the company to use this route
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Updated Offer with given Id</returns>
        /// <response code="204">Returns No Content</response>
        /// <response code="401">If unauthorized</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteOffer(ulong id)
        {
            await _business.Delete(id);
            return NoContent();
        }
    }
}
