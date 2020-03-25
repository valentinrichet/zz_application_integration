using AiOffer.Dbo;
using AiOffer.Dbo.Offers;
using AiOffer.Dto.Offers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Mappings
{
    public class OfferMappings : Profile
    {
        public OfferMappings()
        {
            CreateMap<Offer, OfferDto>();

            CreateMap<CreateOfferDto, Offer>();

            CreateMap<UpdateOfferDto, Offer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
