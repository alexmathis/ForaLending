using ForaLending.API;
using ForaLending.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http;
using ForaLending.API.EdgarService;
using ForaLending.API.CalculationService;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Add services to the container.
// Get connection string
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ForaFinancialContext>(options =>
options.UseNpgsql(conn));

builder.Services.AddHttpClient<EdgarService>(client =>
{
    client.BaseAddress = new Uri("https://data.sec.gov/");
    client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.34.0");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
});
builder.Services.AddScoped<ICalculationService, CalculationService>();
builder.Services.AddScoped<EdgarService>();
builder.Services.AddHostedService<EdgarStartUpService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
  
}

app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
