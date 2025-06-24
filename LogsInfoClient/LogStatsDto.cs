using System;
using System.Collections.Generic;
using System.Text;

namespace LogsInfoClient
{
    /// <summary>
    /// Объект передачи данных о статистике лога
    /// </summary>
    internal class LogStatsDto
    {
        public string LogId { get; set; }

        public int EntriesCount { get; set; }

        public DateTime LastEntryDate { get; set; }
    }
}
