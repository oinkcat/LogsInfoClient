using System;
using System.Collections.Generic;
using System.Linq;

namespace LogsInfoClient
{
    /// <summary>
    /// Информация о клиенте, для которого ведется логирование
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование клиента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата регистрации клиента
        /// </summary>
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// Журналы клиента
        /// </summary>
        public IList<LogInfo> Logs { get; set; }

        public ClientInfo() { }

        public ClientInfo(Guid id) => Id = id;

        /// <summary>
        /// Получить лог клиента по его имени
        /// </summary>
        /// <param name="id">Имя запрашиваемого лога</param>
        /// <returns>Запрошенный лог</returns>
        public LogInfo GetLog(string id) => Logs.SingleOrDefault(l => l.Id == id);
    }
}
