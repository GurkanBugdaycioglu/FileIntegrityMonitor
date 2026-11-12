using System;
using System.Collections.Generic;
using System.IO;
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
            throw new NotImplementedException();
        }
        public void SaveAll(List<BaselineEntry> entries)
        {
            throw new NotImplementedException();
        }
    }
}