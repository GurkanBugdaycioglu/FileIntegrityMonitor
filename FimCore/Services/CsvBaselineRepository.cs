using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FimCore.Interfaces;
using FimCore.Models;

namespace FimCore.Services
{
    public class CsvBaselineRepository : IBaselineRepository
    {
        private readonly string _filePath;
        public CsvBaselineRepository(string filePath)
        {
            _filePath = filePath;
        }
        public List<BaselineEntry> LoadAll()
        {
            var entries = new List<BaselineEntry>();

            if (!File.Exists(_filePath))
            {
                return entries;
            }

            var lines = File.ReadAllLines(_filePath, Encoding.UTF8);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                int lastCommaIndex = lines[i].LastIndexOf(',');
                if (lastCommaIndex <= 0) continue;

                string path = lines[i].Substring(0, lastCommaIndex).Trim('"');
                string hash = lines[i].Substring(lastCommaIndex + 1).Trim('"');

                entries.Add(new BaselineEntry
                {
                    FilePath = path,
                    Sha256Hash = hash
                });
            }

            return entries;
        }
        public void SaveAll(List<BaselineEntry> entries)
        {
            var lines = new List<string> { "path,hash" };

            foreach (var entry in entries)
            {
                lines.Add($"{entry.FilePath},{entry.Sha256Hash}");
            }

            File.WriteAllLines(_filePath, lines, new UTF8Encoding(false));
        }
    }
}