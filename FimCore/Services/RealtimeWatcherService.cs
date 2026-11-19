using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using FimCore.Interfaces;
using FimCore.Models;

namespace FimCore.Services
{
    public class RealtimeWatcherService : IDisposable
    {
        private readonly IHashCalculator _hashCalculator;
        private readonly IBaselineRepository _repository;
        private readonly IAuditLogger _auditLogger;
        private readonly ConcurrentDictionary<string, DateTime> _lastEventTimes = new();
        private FileSystemWatcher? _watcher;

        public event Action<IntegrityEvent>? OnIntegrityAlert;

        public RealtimeWatcherService(
            IHashCalculator hashCalculator,
            IBaselineRepository repository,
            IAuditLogger auditLogger)
        {
            _hashCalculator = hashCalculator;
            _repository = repository;
            _auditLogger = auditLogger;
        }

        public void StartWatching(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
                throw new DirectoryNotFoundException($"İzlenecek klasör bulunamadı: {targetDirectory}");

            _watcher = new FileSystemWatcher(Path.GetFullPath(targetDirectory))
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _watcher.Created += OnFileCreated;
            _watcher.Changed += OnFileChanged;
            _watcher.Deleted += OnFileDeleted;
            _watcher.Renamed += OnFileRenamed;

            _watcher.EnableRaisingEvents = true;
        }

        public void StopWatching()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        private bool IsDebounced(string fullPath)
        {
            DateTime now = DateTime.UtcNow;
            if (_lastEventTimes.TryGetValue(fullPath, out DateTime lastTime))
            {
                if ((now - lastTime).TotalMilliseconds < 300)
                    return true;
            }
            _lastEventTimes[fullPath] = now;
            return false;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath) || IsDebounced(e.FullPath)) return;

            string? newHash = ComputeHashWithRetry(e.FullPath);

            var alert = new IntegrityEvent
            {
                FilePath = e.FullPath,
                EventType = "Created",
                Timestamp = DateTime.Now,
                OldHash = null,
                NewHash = newHash
            };
            PublishAlert(alert);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath) || IsDebounced(e.FullPath)) return;

            string? currentHash = ComputeHashWithRetry(e.FullPath);
            if (currentHash == null) return;

            var entries = _repository.LoadAll();
            var baselineEntry = entries.FirstOrDefault(x =>
                string.Equals(x.FilePath, e.FullPath, StringComparison.OrdinalIgnoreCase));

            if (baselineEntry != null &&
                !string.Equals(baselineEntry.Sha256Hash, currentHash, StringComparison.OrdinalIgnoreCase))
            {
                var alert = new IntegrityEvent
                {
                    FilePath = e.FullPath,
                    EventType = "Modified",
                    Timestamp = DateTime.Now,
                    OldHash = baselineEntry.Sha256Hash,
                    NewHash = currentHash
                };
                PublishAlert(alert);
            }
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            if (IsDebounced(e.FullPath)) return;

            var entries = _repository.LoadAll();
            var baselineEntry = entries.FirstOrDefault(x =>
                string.Equals(x.FilePath, e.FullPath, StringComparison.OrdinalIgnoreCase));

            var alert = new IntegrityEvent
            {
                FilePath = e.FullPath,
                EventType = "Deleted",
                Timestamp = DateTime.Now,
                OldHash = baselineEntry?.Sha256Hash,
                NewHash = null
            };
            PublishAlert(alert);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (IsDebounced(e.FullPath)) return;

            var entries = _repository.LoadAll();
            var baselineEntry = entries.FirstOrDefault(x =>
                string.Equals(x.FilePath, e.OldFullPath, StringComparison.OrdinalIgnoreCase));

            string? currentHash = Directory.Exists(e.FullPath) ? null : ComputeHashWithRetry(e.FullPath);

            var alert = new IntegrityEvent
            {
                FilePath = $"{e.OldFullPath} -> {e.FullPath}",
                EventType = "Renamed",
                Timestamp = DateTime.Now,
                OldHash = baselineEntry?.Sha256Hash,
                NewHash = currentHash
            };
            PublishAlert(alert);
        }

        private string? ComputeHashWithRetry(string filePath, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    if (!File.Exists(filePath)) return null;
                    return _hashCalculator.ComputeHash(filePath);
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
            }
            return null;
        }

        private void PublishAlert(IntegrityEvent alert)
        {
            _auditLogger.LogEvent(alert);
            OnIntegrityAlert?.Invoke(alert);
        }

        public void Dispose() => StopWatching();
    }
}