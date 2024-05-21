using ForaLending.API.Controllers;
using ForaLending.API.Data;
using ForaLending.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForaLending.Tests.UnitTests
{
    public class CompaniesControllerTests
    {
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

            var controller = new CompaniesController(context);

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

            var controller = new CompaniesController(null);

            // Act
            var result = controller.CalculateSpecialFundableAmount(company);

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

            var controller = new CompaniesController(null);

            // Act
            var result = controller.CalculateSpecialFundableAmount(company);

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

            var controller = new CompaniesController(null);

            // Act
            var result = controller.CalculateStandardFundableAmount(company);

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

            var controller = new CompaniesController(null);

            // Act
            var result = controller.CalculateStandardFundableAmount(company);

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

            var controller = new CompaniesController(null);

            // Act
            var result = controller.CalculateStandardFundableAmount(company);

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

            var controller = new CompaniesController(null);

            // Act
            var result = controller.CalculateStandardFundableAmount(company);

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

            var controller = new CompaniesController(null);

            // Act
            var result = controller.CalculateStandardFundableAmount(company);

            // Assert
            Assert.Equal(0m, result);
        }
    }
}
