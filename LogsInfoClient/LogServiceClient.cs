using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LogsInfoClient
{
    /// <summary>
    /// Клиент службы логирования
    /// </summary>
    public class LogServiceClient
    {
        private const string EndpointClients = "api/Clients";
        private const string EndpointLogging = "api/Logging";

        private const int DefaultTimeoutSeconds = 5;

        /// <summary>
        /// Адрес службы логов
        /// </summary>
        public string ServiceAddress { get; }

        /// <summary>
        /// Ключ API для запросов администратора
        /// </summary>
        public string AdminApiToken { get; set; }

        /// <summary>
        /// Таймаут запроса
        /// </summary>
        public int RequestTimeoutSeconds { get; set; } = DefaultTimeoutSeconds;

        public LogServiceClient(string serviceUrl)
        {
            ServiceAddress = serviceUrl;
        }

        /// <summary>
        /// Получить информацию о всех клиентах
        /// </summary>
        /// <returns>Список информации обо всех клиентах службы</returns>
        public async Task<List<ClientInfo>> GetAllClients()
        {
            ThrowIfNoAdminToken();

            string clientInfoUri = $"{EndpointClients}?token={AdminApiToken}";

            var svcClient = CreateHttpClient();
            string clientsJson = await svcClient.GetStringAsync(clientInfoUri);

            return JsonConvert.DeserializeObject<List<ClientInfo>>(clientsJson);
        }

        private void ThrowIfNoAdminToken()
        {
            if (String.IsNullOrWhiteSpace(AdminApiToken))
            {
                throw new ArgumentException(nameof(AdminApiToken));
            }
        }

        /// <summary>
        /// Получить индекс страницы, на которой находится сообщение с заданной датой
        /// </summary>
        /// <param name="client">Клиент службы логирования</param>
        /// <param name="log">Лог, информацию из которого получить</param>
        /// <param name="date">Дата для поиска</param>
        /// <returns>Индекс страницы с сообщением за заданную дату</returns>
        public async Task<int?> GetPageIndexForDate(ClientInfo client, LogInfo log, DateTime date)
        {
            string uDate = date.ToString("u").Split()[0];
            string pageIndexUrl = $"{EndpointLogging}/{client.Id}/{log.Id}/pfd/{uDate}";

            var svcClient = CreateHttpClient();
            string pageIndexText = await svcClient.GetStringAsync(pageIndexUrl);

            return !pageIndexText.Equals("-1") ? int.Parse(pageIndexText) : new int?();
        }

        /// <summary>
        /// Получить информацию о логе
        /// </summary>
        /// <param name="client">Клиент службы логирования</param>
        /// <param name="logId">Идентификатор лога</param>
        /// <returns>Информация о логе</returns>
        public async Task<LogInfo> GetLogInfo(ClientInfo client, string logId)
        {
            string logInfoUrl = $"{EndpointLogging}/{client.Id}";

            var svcClient = CreateHttpClient();
            string clientLogsJson = await svcClient.GetStringAsync(logInfoUrl);

            var logStats = JsonConvert.DeserializeObject<LogStatsDto[]>(clientLogsJson)
                .FirstOrDefault(ls => ls.LogId.Equals(logId));

            return (logStats != null)
                ? LogInfo.CreateFromDto(logStats)
                : null;
        }

        /// <summary>
        /// Отправить сообщение в службу логов
        /// </summary>
        /// <param name="client">Клиент службы логирования</param>
        /// <param name="log">Лог, в который отправить сообщение</param>
        /// <param name="message">Сообщение для записи</param>
        /// <returns>Успешность отправки сообщения</returns>
        public async Task<bool> PostLogMessage(ClientInfo client, LogInfo log, string message)
        {
            string postUrl = $"{EndpointLogging}/{client.Id}/{log.Id}";

            var svcClient = CreateHttpClient();
            var jsonContent = new StringContent($"\"{message}\"");
            jsonContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                var answer = await svcClient.PostAsync(postUrl, jsonContent);
                return (answer.StatusCode == HttpStatusCode.OK) &&
                       bool.Parse(await answer.Content.ReadAsStringAsync());
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary>
        /// Получить записи лога постранично
        /// </summary>
        /// <param name="client">Клиент службы логирования</param>
        /// <param name="log">Лог для получения сообщений</param>
        /// <param name="page">Страница для получения сообщений</param>
        /// <returns>Список сообщений на странице</returns>
        public async Task<List<LogEntry>> GetEntries(ClientInfo client, LogInfo log, int page = -1)
        {
            string pageQuerySuffix = (page > -1) ? page.ToString() : String.Empty;
            string messagesUrl = $"{EndpointLogging}/{client.Id}/{log.Id}/p/{pageQuerySuffix}";

            var svcClient = CreateHttpClient();
            string messagesJson = await svcClient.GetStringAsync(messagesUrl);

            return JsonConvert.DeserializeObject<List<LogEntry>>(messagesJson);
        }

        /// <summary>
        /// Получить запись лога по идентификатору
        /// </summary>
        /// <param name="client">Клиент службы логирования</param>
        /// <param name="log">Лог для получения сообщений</param>
        /// <param name="entryId">Идентификатор записи</param>
        /// <returns>Сообщение лога с заданным идентификатором</returns>
        public async Task<LogEntry> GetEntry(ClientInfo client, LogInfo log, int entryId = -1)
        {
            string entryIdSuffix = (entryId > -1) ? entryId.ToString() : String.Empty;
            string messageUrl = $"{EndpointLogging}/{client.Id}/{log.Id}/id/{entryIdSuffix}";

            var svcClient = CreateHttpClient();
            string messageJson = await svcClient.GetStringAsync(messageUrl);

            return JsonConvert.DeserializeObject<LogEntry>(messageJson);
        }

        // Создать клиент HTTP доступа
        private HttpClient CreateHttpClient() => new HttpClient
        {
            BaseAddress = new Uri(ServiceAddress),
            Timeout = TimeSpan.FromSeconds(RequestTimeoutSeconds)
        };
    }
}
