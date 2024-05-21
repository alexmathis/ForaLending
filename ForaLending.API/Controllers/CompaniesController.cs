using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForaLending.API.Data;
using ForaLending.API.Models;

namespace ForaLending.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ForaFinancialContext _context;

        public CompaniesController(ForaFinancialContext context)
        {
            _context = context;
        }

        // GET: api/Companies
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
                StandardFundableAmount = CalculateStandardFundableAmount(c),
                SpecialFundableAmount = CalculateSpecialFundableAmount(c)
            }).ToList();

            return Ok(companyDtos);
        }

        private decimal CalculateStandardFundableAmount(Company company)
        {
            var incomeRecords = company?.IncomeRecords?.Where(r => (r.Frame ?? "").StartsWith("CY")).ToList();
            var incomeYears = incomeRecords.Select(r => int.Parse((r.Frame ?? "").Substring(2))).ToList();

            if (!incomeYears.Contains(2018) || !incomeYears.Contains(2019) || !incomeYears.Contains(2020) || !incomeYears.Contains(2021) || !incomeYears.Contains(2022))
            {
                return 0;
            }

            var income2021 = incomeRecords.FirstOrDefault(r => r.Frame == "CY2021")?.Val ?? 0;
            var income2022 = incomeRecords.FirstOrDefault(r => r.Frame == "CY2022")?.Val ?? 0;

            if (income2021 <= 0 || income2022 <= 0)
            {
                return 0;
            }

            var highestIncome = incomeRecords.Max(r => r.Val);

            if (highestIncome >= 10000000000)
            {
                return highestIncome * 0.1233m;
            }

            return highestIncome * 0.2151m;
        }

        private decimal CalculateSpecialFundableAmount(Company company)
        {
            var standardAmount = CalculateStandardFundableAmount(company);
            var specialAmount = standardAmount;

            if ("AEIOU".Contains(char.ToUpper(company.Name[0])))
            {
                specialAmount += standardAmount * 0.15m;
            }

            var income2021 = company.IncomeRecords?.FirstOrDefault(r => r.Frame == "CY2021")?.Val ?? 0;
            var income2022 = company.IncomeRecords?.FirstOrDefault(r => r.Frame == "CY2022")?.Val ?? 0;

            if (income2022 < income2021)
            {
                specialAmount -= standardAmount * 0.25m;
            }

            return specialAmount;
        }
    }

    public class CompanyDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal StandardFundableAmount { get; set; }
        public decimal SpecialFundableAmount { get; set; }
    }


}
