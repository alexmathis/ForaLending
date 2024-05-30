using ForaLending.API.Data;
using ForaLending.API.EdgarService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ForaLending.Tests.UnitTests
{
    public class EdgarServiceTests
    {
        [Fact]
        public async Task FetchAndSaveDataAsync_ShouldFetchAndSaveData()
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                    {
                        ""entityName"": ""Test Company"",
                        ""facts"": {
                            ""us-gaap"": {
                                ""NetIncomeLoss"": {
                                    ""units"": {
                                        ""USD"": [
                                            { ""frame"": ""CY2021"", ""val"": 1000000000 },
                                            { ""frame"": ""CY2022"", ""val"": 2000000000 }
                                        ]
                                    }
                                }
                            }
                        }
                    }")
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
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
