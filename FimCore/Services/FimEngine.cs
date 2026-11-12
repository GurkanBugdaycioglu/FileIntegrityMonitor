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

        public FimEngine(IHashCalculator hashCalculator, IBaselineRepository repository)
        {
            _hashCalculator = hashCalculator;
            _repository = repository;
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
                    events.Add(new IntegrityEvent
                    {
                        FilePath = entry.FilePath,
                        EventType = "Deleted",
                        Timestamp = DateTime.Now
                    });
                    continue;
                }

                string currentHash = _hashCalculator.ComputeHash(entry.FilePath);

                if (string.Equals(currentHash, entry.Sha256Hash, StringComparison.OrdinalIgnoreCase))
                {
                    events.Add(new IntegrityEvent
                    {
                        FilePath = entry.FilePath,
                        EventType = "Unchanged",
                        Timestamp = DateTime.Now
                    });
                }
                else
                {
                    events.Add(new IntegrityEvent
                    {
                        FilePath = entry.FilePath,
                        EventType = "Modified",
                        Timestamp = DateTime.Now
                    });
                }
            }

            return events;
        }
    }
}