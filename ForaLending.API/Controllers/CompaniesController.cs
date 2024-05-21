using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForaLending.API.Data;
using ForaLending.API.Models;
using System.Text.Json.Serialization;

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

        [NonAction]
        public decimal CalculateStandardFundableAmount(Company company)
        {
            var incomeRecords = company?.IncomeRecords?.Where(r => r.Frame?.StartsWith("CY") == true).ToList();
           
            var requiredYears = new[] { "2018", "2019", "2020", "2021", "2022" };
            var incomeYears = incomeRecords?.Select(r => r?.Frame?[2..]).ToList();

            var income2021 = incomeRecords?.FirstOrDefault(r => r.Frame == "CY2021")?.Val ?? 0;
            var income2022 = incomeRecords?.FirstOrDefault(r => r.Frame == "CY2022")?.Val ?? 0;

            var  highestIncome = (incomeRecords?.Count ?? 0) > 0 ? incomeRecords?.Max(r => r.Val) ??  0 : 0;

            var standardFundableAmount = (incomeYears != null && incomeRecords != null && requiredYears.All(year => incomeYears.Contains(year)) && income2021 > 0 && income2022 > 0)
                ? (highestIncome >= 10_000_000_000 ? highestIncome * 0.1233m : highestIncome * 0.2151m)
                : 0;

            return Math.Round(standardFundableAmount, 2);
        }

        [NonAction]
        public decimal CalculateSpecialFundableAmount(Company company)
        {
            var standardAmount = CalculateStandardFundableAmount(company);
            var specialAmount = standardAmount;

            if (!string.IsNullOrEmpty(company?.Name) && "AEIOU".Contains(char.ToUpper(company.Name[0])))
            {
                specialAmount += standardAmount * 0.15m;
            }

            var incomeRecords = company?.IncomeRecords?.Where(r => r.Frame?.StartsWith("CY") == true).ToList();
            if (incomeRecords == null || incomeRecords.Count == 0) return Math.Round(specialAmount, 2);

            var income2021 = incomeRecords.FirstOrDefault(r => r.Frame == "CY2021")?.Val ?? 0;
            var income2022 = incomeRecords.FirstOrDefault(r => r.Frame == "CY2022")?.Val ?? 0;

            if (income2022 < income2021)
            {
                specialAmount -= standardAmount * 0.25m;
            }

            return Math.Round(specialAmount, 2);
        }
    }

    public class CompanyDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal StandardFundableAmount { get; set; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal SpecialFundableAmount { get; set; }
    }
}
