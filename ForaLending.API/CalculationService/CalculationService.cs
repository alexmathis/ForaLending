using ForaLending.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForaLending.API.CalculationService
{
    public class CalculationService: ICalculationService
    {
      
        public decimal CalculateStandardFundableAmount(Company company)
        {
            var incomeRecords = company?.IncomeRecords?.Where(r => r.Frame?.StartsWith("CY") == true).ToList();

            var requiredYears = new[] { "2018", "2019", "2020", "2021", "2022" };
            var incomeYears = incomeRecords?.Select(r => r?.Frame?[2..]).ToList();

            var income2021 = incomeRecords?.FirstOrDefault(r => r.Frame == "CY2021")?.Val ?? 0;
            var income2022 = incomeRecords?.FirstOrDefault(r => r.Frame == "CY2022")?.Val ?? 0;

            var highestIncome = (incomeRecords?.Count ?? 0) > 0 ? incomeRecords?.Max(r => r.Val) ?? 0 : 0;

            var standardFundableAmount = (incomeYears != null && incomeRecords != null && requiredYears.All(year => incomeYears.Contains(year)) && income2021 > 0 && income2022 > 0)
                ? (highestIncome >= 10_000_000_000 ? highestIncome * 0.1233m : highestIncome * 0.2151m)
                : 0;

            return Math.Round(standardFundableAmount, 2);
        }

    
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

}
