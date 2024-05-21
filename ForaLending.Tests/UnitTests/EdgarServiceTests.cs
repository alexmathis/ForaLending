using ForaLending.API;
using ForaLending.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForaLending.Tests.UnitTests
{
    public class EdgarServiceTests
    {
        [Fact]
        public async Task FetchAndSaveDataAsync_ShouldFetchAndSaveData()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var httpClient = new HttpClient();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var dbContextOptions = new DbContextOptionsBuilder<ForaFinancialContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ForaFinancialContext(dbContextOptions);

            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var scopeMock = new Mock<IServiceScope>();

            scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);
            scopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
            serviceProviderMock.Setup(x => x.GetService(typeof(ForaFinancialContext))).Returns(context);

            var loggerMock = new Mock<ILogger<EdgarService>>();

            var service = new EdgarService(httpClientFactoryMock.Object, scopeFactoryMock.Object, loggerMock.Object);

            // Act
            await service.FetchAndSaveDataAsync(new[] { 18926 });

            // Assert
            var companies = await context.Companies.ToListAsync();
            Assert.NotEmpty(companies);
        }
    }
}
