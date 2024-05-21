using ForaFinacial.API.Models;
using ForaLending.API.Data;
using ForaLending.API.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace ForaLending.API;

public class EdgarService
{
    private readonly HttpClient _httpClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EdgarService> _logger;


    public EdgarService(IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory, ILogger<EdgarService> logger)
    {
        _httpClient = _httpClient = httpClientFactory.CreateClient("EdgarClient"); ;
        _scopeFactory = scopeFactory;
        _logger = logger;

    }

    public async Task FetchAndSaveDataAsync(IEnumerable<int> ciks)
    {
        const int rateLimit = 10; // 10 requests per second
        const int delay = 1000; // 1000 milliseconds = 1 second

        var invalidCiks = 0;
        var cikList = ciks.ToList();

        for (int i = 0; i < cikList.Count; i += rateLimit)
        {
            var batch = cikList.Skip(i).Take(rateLimit);

            var tasks = batch.Select(cik => ProcessCikAsync(cik));
            var results = await Task.WhenAll(tasks);

            // Count invalid CIKs in this batch
            invalidCiks += results.Count(r => !r);

            if (i + rateLimit < cikList.Count)
            {
                await Task.Delay(delay);
            }
        }

        _logger.LogInformation($"Number of invalid CIKs: {invalidCiks}");
    }

    private async Task<bool> ProcessCikAsync(int cik)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ForaFinancialContext>();

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://data.sec.gov/api/xbrl/companyfacts/CIK{cik:D10}.json");
        request.Headers.UserAgent.ParseAdd("PostmanRuntime/7.34.0");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Failed to fetch data for CIK {cik:D10}: {response.ReasonPhrase}");
            return false;
        }

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var companyInfo = JsonSerializer.Deserialize<EdgarCompanyInfo>(content);

            if (companyInfo == null)
            {
                _logger.LogWarning($"No data found for CIK {cik:D10}");
                return false;
            }

            var company = new Company
            {
                Name = companyInfo.EntityName,
            };

            //Create the income records and set the Company reference
            company.IncomeRecords = companyInfo?.Facts?.UsGaap?.NetIncomeLoss?.Units?.Usd?
                .Where(u => (u.Form ?? "") == "10-K" && (u.Frame ?? "").StartsWith("CY"))
                .Select(u => new IncomeRecord
                {
                    Form = u.Form,
                    Frame = u.Frame,
                    Val = u.Val,
                    Company = company // Set the Company reference
                }).ToList();

            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            _logger.LogWarning($"Deserialization error for {cik} error message {ex.Message}" ?? "");
        }
   

        return true;
    }
}
