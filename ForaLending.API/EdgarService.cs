using ForaFinacial.API.Models;
using ForaLending.API.Data;
using ForaLending.API.Models;
using System.Text.Json;

namespace ForaLending.API;

public class EdgarService
{
    private readonly HttpClient _httpClient;
    private readonly ForaFinancialContext _context;

    public EdgarService(HttpClient httpClient, ForaFinancialContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task FetchAndSaveDataAsync(IEnumerable<int> ciks)
    {
        foreach (var cik in ciks)
        {
            var response = await _httpClient.GetAsync($"https://data.sec.gov/api/xbrl/companyfacts/CIK{cik:D10}.json");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var companyInfo = JsonSerializer.Deserialize<EdgarCompanyInfo>(content);

                if (companyInfo != null)
                {
                    var company = new Company
                    {
                        Name = companyInfo.EntityName,
                    };

                    // Create the income records and set the Company reference
                    company.IncomeRecords = companyInfo.Facts.UsGaap.NetIncomeLoss.Units.Usd
                        .Where(u => u.Form == "10-K" && u.Frame.StartsWith("CY"))
                        .Select(u => new IncomeRecord
                        {
                            Form = u.Form,
                            Frame = u.Frame,
                            Val = u.Val,
                            Company = company // Set the Company reference
                        }).ToList();

                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
