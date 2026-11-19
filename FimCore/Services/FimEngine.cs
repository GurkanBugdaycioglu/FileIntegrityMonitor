using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FimCore.Interfaces;
using FimCore.Models;

namespace FimCore.Services
{
    public class FimEngine
    {
        private readonly IHashCalculator _hashCalculator;
        private readonly IBaselineRepository _repository;
        private readonly IAuditLogger _auditLogger;

        public FimEngine(IHashCalculator hashCalculator, IBaselineRepository repository, IAuditLogger auditLogger)
        {
            _hashCalculator = hashCalculator;
            _repository = repository;
            _auditLogger = auditLogger;
        }
        public int CreateBaseline(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                throw new DirectoryNotFoundException($"Hedef klasör bulunamadı: {targetDirectory}");
            }

            string fullDirectoryPath = Path.GetFullPath(targetDirectory);
            var entries = new List<BaselineEntry>();

            foreach (string filePath in Directory.EnumerateFiles(fullDirectoryPath, "*", SearchOption.AllDirectories))
            {
                string hash = _hashCalculator.ComputeHash(filePath);
                entries.Add(new BaselineEntry
                {
                    FilePath = filePath,
                    Sha256Hash = hash
                });
            }

            _repository.SaveAll(entries);
            return entries.Count;
        }
        public void AddOrUpdateFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Dosya bulunamadı: {filePath}");
            }

            string fullPath = Path.GetFullPath(filePath);
            string newHash = _hashCalculator.ComputeHash(fullPath);

            var entries = _repository.LoadAll();
            var existingEntry = entries.FirstOrDefault(e =>
                string.Equals(e.FilePath, fullPath, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                existingEntry.Sha256Hash = newHash;
            }
            else
            {
                entries.Add(new BaselineEntry
                {
                    FilePath = fullPath,
                    Sha256Hash = newHash
                });
            }

            _repository.SaveAll(entries);
        }
        public List<IntegrityEvent> VerifyIntegrity()
        {
            var events = new List<IntegrityEvent>();
            var entries = _repository.LoadAll();

            foreach (var entry in entries)
            {
                if (!File.Exists(entry.FilePath))
                {
                    var deletedEvent = new IntegrityEvent
                    {
                        FilePath = entry.FilePath,
                        EventType = "Deleted",
                        Timestamp = DateTime.Now,
                        OldHash = entry.Sha256Hash,
                        NewHash = null
                    };

                    events.Add(deletedEvent);
                    _auditLogger.LogEvent(deletedEvent);
                    continue;
                }

                string currentHash = _hashCalculator.ComputeHash(entry.FilePath);

                if (string.Equals(currentHash, entry.Sha256Hash, StringComparison.OrdinalIgnoreCase))
                {
                    events.Add(new IntegrityEvent
                    {
                        FilePath = entry.FilePath,
                        EventType = "Unchanged",
                        Timestamp = DateTime.Now,
                        OldHash = entry.Sha256Hash,
                        NewHash = currentHash
                    });
                }
                else
                {
                    var modifiedEvent = new IntegrityEvent
                    {
                        FilePath = entry.FilePath,
                        EventType = "Modified",
                        Timestamp = DateTime.Now,
                        OldHash = entry.Sha256Hash,
                        NewHash = currentHash
                    };

                    events.Add(modifiedEvent);
                    _auditLogger.LogEvent(modifiedEvent);
                }
            }

            return events;
        }
    }
}