using AiCompany.Dbo;
using AiCompany.Dto.Companies;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCompany.Mappings
{
    public class CompanyMappings : Profile
    {
        public CompanyMappings()
        {
            CreateMap<Company, CompanyDto>();

            CreateMap<CreateCompanyDto, Company>();

            CreateMap<UpdateCompanyDto, Company>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
