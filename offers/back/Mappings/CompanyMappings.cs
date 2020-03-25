using AiOffer.Dbo.Companies;
using AiOffer.Dto.Companies;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Mappings
{
    public class CompanyMappings : Profile
    {
        public CompanyMappings()
        {
            CreateMap<Company, CompanyDto>();
        }
    }
}
