using AutoMapper;
using AiCompany.Dbo;
using AiCompany.Repositories;
using AiCompany.Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCompany.Dto.Companies;
using AiCompany.Services.JwtGenerator;
using AiCompany.Services.PasswordHasher;
using AiCompany.Exceptions;
using Microsoft.Extensions.Logging;

namespace AiCompany.Business.Companies
{
    public class DbCompanyBusiness : Business<Company, IRepository<Company>>, ICompanyBusiness<Company>
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public DbCompanyBusiness(IRepository<Company> repository, IMapper mapper, ILogger<ICompanyBusiness<Company>> logger, IJwtGenerator jwtGenerator,  IPasswordHasher passwordHasher) : base(repository, mapper, logger)
        {
            _jwtGenerator = jwtGenerator;
            _passwordHasher = passwordHasher;
        }

        public Task<string> Authenticate(AuthenticateCompanyDto authenticateCompanyDto)
        {
            Company company = null;

            try
            {
                company = _repository.GetAll().Where(company => company.Mail == authenticateCompanyDto.Mail).Single();
            }
            finally
            {
                if (company == null || !_passwordHasher.Check(authenticateCompanyDto.Password, company.HashedPassword))
                {
                    throw new AuthenticationFailedException();
                }
            }

            string token = _jwtGenerator.GenerateToken(company.Id);
            return Task.FromResult(token);
        }

        public async Task<Dto> CreateCompany<Dto>(CreateCompanyDto createCompanyDto)
        {
            Company company = _mapper.Map<Company>(createCompanyDto);
            company.HashedPassword = _passwordHasher.Hash(createCompanyDto.Password);
            company = await _repository.Add(company);
            Dto mappedCompany = _mapper.Map<Dto>(company);
            return mappedCompany;
        }

        public async Task<Dto> UpdateCompany<Dto>(ulong primaryKey, UpdateCompanyDto updateCompanyDto)
        {
            Company company = await _repository.Get(primaryKey);
            _mapper.Map(updateCompanyDto, company);

            if(updateCompanyDto.Password != null)
            {
                company.HashedPassword = _passwordHasher.Hash(updateCompanyDto.Password);
            }

            company = await _repository.Update(company);
            Dto mappedCompany = _mapper.Map<Dto>(company);
            return mappedCompany;
        }
    }
}
