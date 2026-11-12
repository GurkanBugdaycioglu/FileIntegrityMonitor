using System;
using System.IO;
using System.Text;
using FimCore.Interfaces;
using FimCore.Services;

namespace FimConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            string baseDir = AppContext.BaseDirectory;
            string baselineFilePath = Path.Combine(baseDir, "baselines.csv");
            string monitoredDir = Path.Combine(baseDir, "MonitoredFiles");

            EnsureSampleEnvironment(monitoredDir);

            IHashCalculator calculator = new Sha256HashCalculator();
            IBaselineRepository repository = new CsvBaselineRepository(baselineFilePath);
            FimEngine engine = new FimEngine(calculator, repository);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("  == FILE INTEGRITY MONITOR - CORE ==  ");
                Console.WriteLine($"İzlenen Klasör : {monitoredDir}");
                Console.WriteLine($"Baseline Yolu  : {baselineFilePath}");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("1. Baseline Oluştur");
                Console.WriteLine("2. Bütünlük Doğrulaması Yap");
                Console.WriteLine("3. Tek Dosya Ekle / Güncelle");
                Console.WriteLine("0. Çıkış");
                Console.Write("\nSeçiminiz: ");

                string choice = Console.ReadLine()?.Trim() ?? "";

                try
                {
                    switch (choice)
                    {
                        case "1":
                            int count = engine.CreateBaseline(monitoredDir);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n[BAŞARILI] Toplam {count} dosya tarandı ve baseline oluşturuldu.");
                            Console.ResetColor();
                            break;

                        case "2":
                            var events = engine.VerifyIntegrity();
                            Console.WriteLine("\n--- DOĞRULAMA SONUÇLARI ---");

                            if (events.Count == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Kayıtlı baseline bulunamadı. Önce 1. seçenek ile baseline oluşturun.");
                            }

                            foreach (var ev in events)
                            {
                                switch (ev.EventType)
                                {
                                    case "Unchanged":
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($"[UNCHANGED] {ev.FilePath}");
                                        break;
                                    case "Modified":
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"[MODIFIED!] {ev.FilePath} ({ev.Timestamp:HH:mm:ss})");
                                        break;
                                    case "Deleted":
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine($"[DELETED!]  {ev.FilePath} ({ev.Timestamp:HH:mm:ss})");
                                        break;
                                }
                            }
                            Console.ResetColor();
                            break;

                        case "3":
                            Console.Write("\nEklenecek/Güncellenecek dosya yolunu girin: ");
                            string targetFile = Console.ReadLine()?.Trim('"').Trim() ?? "";
                            engine.AddOrUpdateFile(targetFile);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[BAŞARILI] Dosya baseline kaydına işlendi.");
                            Console.ResetColor();
                            break;

                        case "0":
                            return;

                        default:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\nGeçersiz seçim yaptınız.");
                            Console.ResetColor();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[HATA] {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }

        private static void EnsureSampleEnvironment(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(Path.Combine(directoryPath, "config.txt"), "123");
                File.WriteAllText(Path.Combine(directoryPath, "secret.txt"), "Secret");
            }
        }
    }
}