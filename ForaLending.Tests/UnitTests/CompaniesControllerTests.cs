using ForaLending.API.CalculationService;
using ForaLending.API.Controllers;
using ForaLending.API.Data;
using ForaLending.API.DTOs;
using ForaLending.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ForaLending.Tests.UnitTests
{
    public class CompaniesControllerTests
    {

        private readonly CalculationService _fundCalculationService;

        public CompaniesControllerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ForaFinancialContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _fundCalculationService = new CalculationService();
        }

        [Fact]
        public async Task GetCompanies_ShouldReturnCompanies()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ForaFinancialContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ForaFinancialContext(dbContextOptions);

            context.Companies.Add(new Company { Id = 1, Name = "Test Company" });
            context.SaveChanges();

            var controller = new CompaniesController(context, _fundCalculationService);

            // Act
            var result = await controller.GetCompanies(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var companies = Assert.IsType<List<CompanyDto>>(okResult.Value);
            Assert.Single(companies);
        }

        [Fact]
        public void CalculateSpecialFundableAmount_ShouldReturnCorrectAmount()
        {
            // Arrange

            var companyRelation = new Company() { Name = "Uber Technologies", };


            var company = new Company
            {
                Name = "Uber Technologies",
                IncomeRecords = new List<IncomeRecord>
                {
                new IncomeRecord { Frame = "CY2018", Val = 1000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2019", Val = 2000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2020", Val = 3000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2021", Val = 4000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2022", Val = 5000000000, Company = companyRelation }
                }
            };


            // Act
            var result = _fundCalculationService.CalculateSpecialFundableAmount(company);

            // Assert
            Assert.Equal(1075500000m * 1.15m, result); // Including the 15% increase for a name starting with a vowel
        }

        [Fact]
        public void CalculateSpecialFundableAmount_ShouldDecrease_If2022IncomeLessThan2021()
        {
            // Arrange
            var companyRelation = new Company() { Name = "Zebra Technologies", };
            var company = new Company
            {
                Name = "Zebra Technologies",
                IncomeRecords = new List<IncomeRecord>
            {
                new IncomeRecord { Frame = "CY2018", Val = 1000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2019", Val = 2000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2020", Val = 3000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2021", Val = 5000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2022", Val = 4000000000, Company = companyRelation }
            }
            };

            // Act
            var result = _fundCalculationService.CalculateSpecialFundableAmount(company);

            // Assert
            Assert.Equal(1075500000m * 0.75m, result); // Including the 25% decrease for lower 2022 income
        }


        [Fact]
        public void CalculateStandardFundableAmount_ShouldReturnZero_IfMissingYears()
        {
            // Arrange
            var companyRelation = new Company() { Name = "Test Company", };
            var company = new Company
            {
                Name = "Test Company",
                IncomeRecords = new List<IncomeRecord>
            {
                new IncomeRecord { Frame = "CY2018", Val = 1000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2019", Val = 2000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2020", Val = 3000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2021", Val = 4000000000, Company = companyRelation }
                // Missing 2022
            }
            };

            // Act
            var result = _fundCalculationService.CalculateStandardFundableAmount(company);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalculateStandardFundableAmount_ShouldReturnZero_IfIncomeNegative()
        {
            // Arrange
            var companyRelation = new Company() { Name = "Test Company", };
            var company = new Company
            {
                Name = "Test Company",
                IncomeRecords = new List<IncomeRecord>
            {
                new IncomeRecord { Frame = "CY2018", Val = 1000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2019", Val = 2000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2020", Val = 3000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2021", Val = -4000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2022", Val = 5000000000, Company = companyRelation }
            }
            };

            // Act
            var result = _fundCalculationService.CalculateStandardFundableAmount(company);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalculateStandardFundableAmount_ShouldReturnZero_IfEmptyIncomeRecords()
        {
            // Arrange
            var company = new Company
            {
                Name = "Test Company",
                IncomeRecords = new List<IncomeRecord>()
            };

            // Act
            var result = _fundCalculationService.CalculateStandardFundableAmount(company);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalculateStandardFundableAmount_ShouldReturnZero_IfIncomeRecordsAllNegative()
        {
            // Arrange
            var companyRelation = new Company() { Name = "Test Company", };
            var company = new Company
            {
                Name = "Test Company",
                IncomeRecords = new List<IncomeRecord>
            {
                new IncomeRecord { Frame = "CY2018", Val = -1000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2019", Val = -2000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2020", Val = -3000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2021", Val = -4000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2022", Val = -5000000000, Company = companyRelation }
            }
            };

            // Act
            var result = _fundCalculationService.CalculateStandardFundableAmount(company);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalculateStandardFundableAmount_ShouldReturnZero_IfIncomeZeroIn2021And2022()
        {
            // Arrange
            var companyRelation = new Company() { Name = "Test Company", };
            var company = new Company
            {
                Name = "Test Company",
                IncomeRecords = new List<IncomeRecord>
            {
                new IncomeRecord { Frame = "CY2018", Val = 1000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2019", Val = 2000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2020", Val = 3000000000, Company = companyRelation },
                new IncomeRecord { Frame = "CY2021", Val = 0, Company = companyRelation },
                new IncomeRecord { Frame = "CY2022", Val = 0, Company = companyRelation }
            }
            };

            // Act
            var result = _fundCalculationService.CalculateStandardFundableAmount(company);

            // Assert
            Assert.Equal(0m, result);
        }
    }
}
