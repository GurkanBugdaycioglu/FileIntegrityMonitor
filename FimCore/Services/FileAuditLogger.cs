using System;
using System.IO;
using System.Text;
using FimCore.Interfaces;
using FimCore.Models;

namespace FimCore.Services
{
    public class FileAuditLogger : IAuditLogger
    {
        private readonly string _logFilePath;
        private readonly object _lockObj = new object();

        public FileAuditLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void LogEvent(IntegrityEvent integrityEvent)
        {
            string logLine = $"[{integrityEvent.Timestamp:yyyy-MM-dd HH:mm:ss}] [{integrityEvent.EventType.ToUpperInvariant()}] {integrityEvent.FilePath}{Environment.NewLine}";

            lock (_lockObj)
            {
                File.AppendAllText(_logFilePath, logLine, new UTF8Encoding(false));
            }
        }
    }
}