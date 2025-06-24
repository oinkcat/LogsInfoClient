using System;
using System.Collections.Generic;
using System.Text;

namespace LogsInfoClient
{
    /// <summary>
    /// Информация о журнале клиента
    /// </summary>
    public class LogInfo
    {
        /// <summary>
        /// Идентификатор журнала
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Число сообщений
        /// </summary>
        public int EntriesCount { get; set; }

        /// <summary>
        /// Дата последнего сообщения
        /// </summary>
        public DateTime LastEntryDate { get; set; }

        /// <summary>
        /// Загружены ли подробности
        /// </summary>
        public bool DetailsLoaded => LastEntryDate > DateTime.MinValue;

        /// <summary>
        /// Создать из статистической информации, переданной от службы
        /// </summary>
        /// <param name="dto">Информация, полученная от службы логов</param>
        /// <returns>Информация о логе</returns>
        internal static LogInfo CreateFromDto(LogStatsDto dto) => new LogInfo(dto.LogId)
        {
            EntriesCount = dto.EntriesCount,
            LastEntryDate = dto.LastEntryDate
        };

        public LogInfo() { }

        public LogInfo(string id) => Id = id;
    }
}
