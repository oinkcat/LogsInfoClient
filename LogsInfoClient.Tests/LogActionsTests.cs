using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace LogsInfoClient.Tests
{
    /// <summary>
    /// “естирование клиента службы логировани€
    /// </summary>
    public class LogActionsTests
    {
        private const string LogServiceUrl = "http://localhost:5000";

        private const string AdminToken = "admin";

        private const string TestClientId = "c19f8597-2a7c-412e-a14b-002aa6d025f0";

        private const string TestLogId = "status";

        /// <summary>
        /// “естирование получени€ информации о клиентах сервиса логов
        /// </summary>
        [Fact]
        public async Task TestGetAllClientsInfo()
        {
            var logsClient = CreateClient();

            var clientsInfo = await logsClient.GetAllClients();

            Assert.NotEmpty(clientsInfo);
        }

        /// <summary>
        /// “естирование получени€ информации о логе
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
        /// “естирование отправки сообщени€ в службу логировани€
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

        /// <summary>
        /// “естирование получени€ сообщений постранично
        /// </summary>
        [Fact]
        public async Task TestRetrieveLogEntriesPaged()
        {
            var logsClient = CreateClient();

            var client = new ClientInfo(Guid.Parse(TestClientId));
            var logInfo = new LogInfo(TestLogId);

            var messages = await logsClient.GetEntries(client, logInfo, 0);

            Assert.NotEmpty(messages);
        }

        /// <summary>
        /// “естирование получени€ последнего сообщени€ в логе
        /// </summary>
        [Fact]
        public async Task TestRetrieveLastMessage()
        {
            var logsClient = CreateClient();

            var client = new ClientInfo(Guid.Parse(TestClientId));
            var logInfo = await logsClient.GetLogInfo(client, TestLogId);

            int lastEntryId = logInfo.EntriesCount;
            var lastEntry = await logsClient.GetEntry(client, logInfo, lastEntryId);

            Assert.NotNull(lastEntry);
            Assert.Equal(lastEntryId, lastEntry.Id);
        }

        /// <summary>
        /// “естирование получени€ последнего сообщени€ в логе новым способом
        /// </summary>
        [Fact]
        public async Task TestRetrieveLastMessageNew()
        {
            var logsClient = CreateClient();

            var client = new ClientInfo(Guid.Parse(TestClientId));
            var logInfo = await logsClient.GetLogInfo(client, TestLogId);

            var lastEntry = await logsClient.GetEntry(client, logInfo);

            Assert.NotNull(lastEntry);
            Assert.Equal(logInfo.EntriesCount, lastEntry.Id);
        }

        /// <summary>
        /// “естирование получени€ содержимого последней страницы новым способом
        /// </summary>
        [Fact]
        public async Task TestRetrieveLastPage()
        {
            var logsClient = CreateClient();

            var client = new ClientInfo(Guid.Parse(TestClientId));
            var logInfo = await logsClient.GetLogInfo(client, TestLogId);

            var lastPageEntries = await logsClient.GetEntries(client, logInfo);

            Assert.NotEmpty(lastPageEntries);
        }

        /// <summary>
        /// “естирование получени€ индекса страницы по дате сообщени€
        /// </summary>
        [Fact]
        public async Task TestGetPageIndexForDate()
        {
            const string TestDateWithMessages = "2023-03-15";
            const string TestDateWithoutMessages = "2099-12-31";

            var logsClient = CreateClient();
            var client = new ClientInfo(Guid.Parse(TestClientId));
            var logInfo = await logsClient.GetLogInfo(client, TestLogId);

            var date1 = DateTime.Parse(TestDateWithMessages);
            int? pageForTestDate1 = await logsClient.GetPageIndexForDate(client, logInfo, date1);

            var date2 = DateTime.Parse(TestDateWithoutMessages);
            int? pageForTestDate2 = await logsClient.GetPageIndexForDate(client, logInfo, date2);

            Assert.True(pageForTestDate1.HasValue);
            Assert.False(pageForTestDate2.HasValue);
        }

        private LogServiceClient CreateClient() => new LogServiceClient(LogServiceUrl)
        {
            AdminApiToken = AdminToken
        };
    }
}
