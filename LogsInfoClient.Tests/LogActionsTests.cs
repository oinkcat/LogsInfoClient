using System;
using System.Threading.Tasks;
using Xunit;
using LogsInfoClient.Tests;

namespace LogsInfoClient.Tests
{
    /// <summary>
    /// Тестирование клиента службы логирования
    /// </summary>
    public class LogActionsTests
    {
        private const string LogServiceUrl = "http://myiotlogs.somee.com";

        private const string AdminToken = "admin";

        /// <summary>
        /// Тестирование получения информации о клиентах сервиса логов
        /// </summary>
        [Fact]
        public async Task TestGetAllClientsInfo()
        {
            var logsClient = CreateClient();

            var clientsInfo = await logsClient.GetAllClients();

            Assert.NotEmpty(clientsInfo);
        }

        private LogServiceClient CreateClient() => new LogServiceClient(LogServiceUrl)
        {
            AdminApiToken = AdminToken
        };
    }
}
