using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace LogsInfoClient
{
    /// <summary>
    /// Клиент службы логирования
    /// </summary>
    public class LogServiceClient
    {
        /// <summary>
        /// Адрес службы логов
        /// </summary>
        public string ServiceAddress { get; }

        /// <summary>
        /// Ключ API для запросов администратора
        /// </summary>
        public string AdminApiToken { get; set; }

        private string BaseApiUrl => $"{ServiceAddress.TrimEnd('/')}/api";

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

            var svcClient = new WebClient();

            string clientInfoUrl = $"{BaseApiUrl}/Clients?token={AdminApiToken}";
            string clientsJson = await svcClient.DownloadStringTaskAsync(clientInfoUrl);

            return JsonConvert.DeserializeObject<List<ClientInfo>>(clientsJson);
        }

        /// <summary>
        /// Получить информацию о логе
        /// </summary>
        /// <param name="client">Клиент службы логирования</param>
        /// <param name="logId">Идентификатор лога</param>
        /// <returns>Информация о логе</returns>
        public async Task<LogInfo> GetLogInfo(ClientInfo client, string logId)
        {
            string logInfoUrl = $"{BaseApiUrl}/Logging/{client.Id}";

            var svcClient = new WebClient();
            string clientLogsJson = await svcClient.DownloadStringTaskAsync(logInfoUrl);

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
            string postUrl = $"{BaseApiUrl}/Logging/{client.Id}/{log.Id}";

            var svcClient = new WebClient();

            svcClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            string messageToPost = String.Concat('"', message, '"');

            try
            {
                string answer = await svcClient.UploadStringTaskAsync(postUrl, messageToPost);
                return bool.Parse(answer);
            }
            catch (WebException)
            {
                return false;
            }
        }

        private void ThrowIfNoAdminToken()
        {
            if (String.IsNullOrWhiteSpace(AdminApiToken))
            {
                throw new ArgumentException(nameof(AdminApiToken));
            }
        }
    }
}
