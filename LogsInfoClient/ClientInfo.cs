using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
