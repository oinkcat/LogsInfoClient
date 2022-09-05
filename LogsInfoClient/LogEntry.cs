using System;
using System.Collections.Generic;
using System.Text;

namespace LogsInfoClient
{
    /// <summary>
    /// Запись лога
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Временная метка занесения записи
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Данные записи
        /// </summary>
        public string Data { get; set; }
    }
}
