using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
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

        private void ThrowIfNoAdminToken()
        {
            if (String.IsNullOrWhiteSpace(AdminApiToken))
            {
                throw new ArgumentException(nameof(AdminApiToken));
            }
        }
    }
}
