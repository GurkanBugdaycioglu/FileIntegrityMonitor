using System;
using System.IO;
using System.Security.Cryptography;
using FimCore.Interfaces;


namespace FimCore.Services
{
    public class Sha256HashCalculator : IHashCalculator
    {
        public string ComputeHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}