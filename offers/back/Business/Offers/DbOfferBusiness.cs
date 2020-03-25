using AiOffer.Dbo.Candidates;
using AiOffer.Dbo.Companies;
using AiOffer.Dbo.Offers;
using AiOffer.Dto.Companies;
using AiOffer.Dto.Offers;
using AiOffer.Dto.Skills;
using AiOffer.Exceptions;
using AiOffer.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Business.Offers
{
    public class DbOfferBusiness : Business<Offer, IRepository<Offer>>, IOfferBusiness<Offer>
    {
        private readonly IBusiness<Company> _companyBusiness;
        private readonly IBusiness<Candidate> _candidateBusiness;
        private readonly IBusiness<Skill> _skillBusiness;

        public DbOfferBusiness(IRepository<Offer> repository, IBusiness<Company> companyBusiness, IBusiness<Candidate> candidateBusiness, IBusiness<Skill> skillBusiness, IMapper mapper, ILogger<IOfferBusiness<Offer>> logger) : base(repository, mapper, logger)
        {
            _companyBusiness = companyBusiness;
            _candidateBusiness = candidateBusiness;
            _skillBusiness = skillBusiness;
        }

        public async Task<OfferDto> CreateOffer(CreateOfferDto createOfferDto)
        {
            Offer offer = _mapper.Map<Offer>(createOfferDto);

            // Get and check company
            CompanyDto offerCompany = await _companyBusiness.GetFirstOrDefault<CompanyDto>(company => company.Id == createOfferDto.IdCompany) ?? throw new ConditionFailedException("Company was not found."); ;

            // Get and check skills
            ICollection<SkillDto> offerSkills = await _skillBusiness.Get<SkillDto>(skill => createOfferDto.Skills.Contains(skill.Id));
            if (createOfferDto.Skills.Count != offerSkills.Count)
            {
                throw new ConditionFailedException("One or multiple skills do not exist.");
            }

            offer = await _repository.Add(offer);

            foreach (SkillDto skillDto in offerSkills)
            {
                offer.OfferSkill.Add(new OfferSkill { Offer = offer.Id, Skill = skillDto.Id });
            }

            offer = await _repository.Update(offer);

            OfferDto mappedOffer = _mapper.Map<OfferDto>(offer);
            mappedOffer.Skills = offerSkills;
            mappedOffer.Company = offerCompany;

            return mappedOffer;
        }

        public async Task<OfferDto> GetOffer(ulong offerId)
        {
            Offer offer = await _repository.GetAll().Include(o => o.OfferSkill).Where(o => o.Id == offerId).FirstOrDefaultAsync();

            if (offer == null)
            {
                return null;
            }

            OfferDto mappedOffer = _mapper.Map<OfferDto>(offer);

            // Get and check company
            CompanyDto offerCompany = await _companyBusiness.GetFirstOrDefault<CompanyDto>(company => company.Id == offer.IdCompany) ?? throw new ConditionFailedException("Company was not found."); ;

            // Get and check skills
            ICollection<SkillDto> offerSkills = await _skillBusiness.Get<SkillDto>(skill => offer.OfferSkill.Select(os => os.Skill).Contains(skill.Id));
            if (offer.OfferSkill.Count != offerSkills.Count)
            {
                throw new ConditionFailedException("One or multiple skills do not exist.");
            }

            mappedOffer.Skills = offerSkills;
            mappedOffer.Company = offerCompany;

            return mappedOffer;
        }

        public async Task<IEnumerable<OfferDto>> GetOffers(string search, int page)
        {
            int limit = 20;
            page = page > 1 ? page : 1;
            ICollection<Offer> offers = await _repository.GetAll().Include(o => o.OfferSkill).Where(o => search == null ? true : (o.Title.Contains(search) || o.Description.Contains(search))).Skip(limit * (page - 1)).Take(limit).ToListAsync();

            ICollection<ulong> companiesId = new HashSet<ulong>();
            ICollection<ulong> skillsId = new HashSet<ulong>();
            foreach (Offer offer in offers)
            {
                companiesId.Add(offer.IdCompany);
                foreach (OfferSkill offerSkill in offer.OfferSkill)
                {
                    skillsId.Add(offerSkill.Skill);
                }
            }

            IDictionary<ulong, CompanyDto> companiesDto = (await _companyBusiness.Get<CompanyDto>(c => companiesId.Contains(c.Id))).ToDictionary(c => c.Id);
            if (companiesId.Count != companiesDto.Count)
            {
                throw new ConditionFailedException("One or multiple companies do not exist.");
            }

            IDictionary<ulong, SkillDto> skillsDto = (await _skillBusiness.Get<SkillDto>(s => skillsId.Contains(s.Id))).ToDictionary(s => s.Id);
            if (skillsId.Count != skillsDto.Count)
            {
                throw new ConditionFailedException("One or multiple skills do not exist.");
            }

            ICollection<OfferDto> mappedOffers = new List<OfferDto>();
            foreach (Offer offer in offers)
            {
                OfferDto mappedOffer = _mapper.Map<OfferDto>(offer);
                mappedOffer.Company = companiesDto[offer.IdCompany];
                mappedOffer.Skills = offer.OfferSkill.Select(os => skillsDto[os.Skill]).ToList();
                mappedOffers.Add(mappedOffer);
            }

            return mappedOffers;
        }

        public async Task<OfferDto> UpdateOffer(ulong offerId, string companyId, UpdateOfferDto updateOfferDto)
        {
            Offer offer = await _repository.GetAll().Include(o => o.OfferSkill).Where(o => o.Id == offerId).FirstOrDefaultAsync();

            if (offer == null)
            {
                return null;
            }

            if(offer.IdCompany.ToString() != companyId)
            {
                throw new AuthenticationFailedException("You are not authorized to update this offer.");
            }

            _mapper.Map(updateOfferDto, offer);

            // Get and check company
            CompanyDto offerCompany = await _companyBusiness.GetFirstOrDefault<CompanyDto>(company => company.Id == offer.IdCompany) ?? throw new ConditionFailedException("Company was not found.");

            // Get and check skills
            ICollection<SkillDto> offerSkills = null;
            if (updateOfferDto.Skills != null)
            {
                offer.OfferSkill.Clear();
                offerSkills = await _skillBusiness.Get<SkillDto>(skill => updateOfferDto.Skills.Contains(skill.Id));

                if (updateOfferDto.Skills.Count != offerSkills.Count)
                {
                    throw new ConditionFailedException("One or multiple skills do not exist.");
                }

                foreach (SkillDto skillDto in offerSkills)
                {
                    offer.OfferSkill.Add(new OfferSkill { Offer = offer.Id, Skill = skillDto.Id });
                }
            }
            else
            {
                offerSkills = await _skillBusiness.Get<SkillDto>(skill => offer.OfferSkill.Select(os => os.Skill).Contains(skill.Id));
            }

            offer = await _repository.Update(offer);

            OfferDto mappedOffer = _mapper.Map<OfferDto>(offer);
            mappedOffer.Skills = offerSkills;
            mappedOffer.Company = offerCompany;

            return mappedOffer;
        }
    }
}
