using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForaLending.API.Data;
using ForaLending.API.CalculationService;
using ForaLending.API.DTOs;

namespace ForaLending.API.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ForaFinancialContext _context;
        private readonly ICalculationService _calculationService;

        public CompaniesController(ForaFinancialContext context, ICalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies([FromQuery] char? startsWith)
        {
            var query = _context.Companies.Include(c => c.IncomeRecords).AsQueryable();

            if (startsWith.HasValue)
            {
                query = query.Where(c => c.Name.StartsWith(startsWith.ToString() ?? "", StringComparison.OrdinalIgnoreCase));
            }

            var companies = await query.ToListAsync();
            var companyDtos = companies.Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                StandardFundableAmount = _calculationService.CalculateStandardFundableAmount(c),
                SpecialFundableAmount = _calculationService.CalculateSpecialFundableAmount(c)
            }).ToList();

            return Ok(companyDtos);
        }
    }



