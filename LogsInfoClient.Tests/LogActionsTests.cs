using System;
using System.Threading.Tasks;
using Xunit;

namespace LogsInfoClient.Tests
{
    /// <summary>
    /// Тестирование клиента службы логирования
    /// </summary>
    public class LogActionsTests
    {
        private const string LogServiceUrl = "http://myiotlogs.somee.com";

        private const string AdminToken = "admin";

        private const string TestClientId = "c19f8597-2a7c-412e-a14b-002aa6d025f0";

        private const string TestLogId = "status";

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

        /// <summary>
        /// Тестирование получения информации о логе
        /// </summary>
        [Fact]
        public async Task TestGetLogInfo()
        {
            var logsClient = CreateClient();

            var client = new ClientInfo(Guid.Parse(TestClientId));
            var logInfo = await logsClient.GetLogInfo(client, TestLogId);

            Assert.NotNull(logInfo);
            Assert.Equal(TestLogId, logInfo.Id);
            Assert.True(logInfo.DetailsLoaded);
            Assert.NotEqual(0, logInfo.EntriesCount);
        }

        /// <summary>
        /// Тестирование отправки сообщения в службу логирования
        /// </summary>
        [Fact]
        public async Task TestLogMessage()
        {
            const string MessageToLog = "Test message";

            var logsClient = CreateClient();

            var client = new ClientInfo(Guid.Parse(TestClientId));
            var logInfo = new LogInfo(TestLogId);

            bool logResult = await logsClient.PostLogMessage(client, logInfo, MessageToLog);

            Assert.True(logResult);
        }

        private LogServiceClient CreateClient() => new LogServiceClient(LogServiceUrl)
        {
            AdminApiToken = AdminToken
        };
    }
}
