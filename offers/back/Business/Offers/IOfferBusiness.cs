using AiOffer.Dto.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Business.Offers
{
    public interface IOfferBusiness<TEntity> : IBusiness<TEntity>
    {
        Task<OfferDto> CreateOffer(CreateOfferDto createOfferDto);
        Task<OfferDto> GetOffer(ulong offerId);
        Task<IEnumerable<OfferDto>> GetOffers(string search, int page);
        Task<OfferDto> UpdateOffer(ulong offerId, string companyId, UpdateOfferDto updateOfferDto);
        

    }
}
