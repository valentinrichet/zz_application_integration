using AiCompany.Dto.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCompany.Business.Companies
{
    public interface ICompanyBusiness<TEntity> : IBusiness<TEntity>
    {
        Task<string> Authenticate(AuthenticateCompanyDto authenticateCompanyDto);
        Task<Dto> CreateCompany<Dto>(CreateCompanyDto createCompanyDto);
        Task<Dto> UpdateCompany<Dto>(ulong primaryKey, UpdateCompanyDto updateCompanyDto);
    }
}
