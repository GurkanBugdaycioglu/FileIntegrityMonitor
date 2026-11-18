[English](#english) | [Türkçe](#turkce)

<a id="english"></a>

# File Integrity Monitor Project

## C# and .NET based File Integrity Monitoring application developed with layered architecture!

This project is an application that demonstrates how to develop a File Integrity Monitoring tool from scratch by applying Object-Oriented Programming and layered architecture principles. Each part of the project shows how to perform the following operations:

* Calculate cryptographic hash summaries of files using the SHA-256 algorithm
* Store and update file paths and hash values in a UTF-8 supported baselines.csv file
* Identify changed, deleted, and added files by comparing existing files with stored hash values
* Using FileSystemWatcher to capture create, change, delete, and rename operations in the target folder in real-time
* Logging all detected integrity events with timestamps to the fim_audit.log file

## How to run the project on your computer?

To run the project, you only need to have .NET SDK 8.0 or higher installed on your computer.

1. Clone this repository to your computer: `git clone https://github.com/GurkanBugdaycioglu/FileIntegrityMonitor.git`
2. Navigate into the project folder: `cd FileIntegrityMonitor`
3. Build and run the application: `dotnet run --project FimConsole/FimConsole.csproj`
4. In the opened menu, first press 1 to create the initial baseline record for the sample folder, then press 2 for manual verification or 4 to start real-time monitoring.

## How to adapt it for your own use?

The core business logic of the project is designed as a class library completely independent of the user interface. This layered structure allows you to easily extend the project according to your needs:

- You can change the `monitoredDir` variable in `FimConsole/Program.cs` to monitor any critical folder on your computer.
- You can directly use the `FimCore.dll` reference in your own desktop or web project without touching the existing business logic.

## Roadmap

### Completed Steps
- [x] Starting the project development with C# and .NET 8.0
- [x] Creating the `.sln` solution structure, `FimCore` class library, and `FimConsole` project
- [x] Defining data models (`BaselineEntry`, `IntegrityEvent`) and interfaces (`IHashCalculator`, `IBaselineRepository`, `IAuditLogger`)
- [x] Writing the file hash calculation service with `Sha256HashCalculator`
- [x] Completing the UTF-8 supported CSV reading and writing operations with `CsvBaselineRepository`
- [x] Setting up the folder scanning, single file update, and integrity verification logic in the `FimEngine` class
- [x] Creating the console menu and adding Turkish character support
- [x] Logging detected events to the log file with `FileAuditLogger`
- [x] Adding live folder monitoring, debounce, and retry mechanisms with `RealtimeWatcherService`

### Planned Steps
- [ ] Adding a verification or encryption mechanism to protect the integrity of the baseline file
- [ ] Allowing the monitored folder and settings to be configured externally
- [ ] Developing a desktop user interface using the `FimCore` library
- [ ] Adding live event table and report export features in the desktop interface

---

<a id="turkce"></a>

# Dosya Bütünlüğü İzleyici Projesi

## C# ve .NET kullanılarak katmanlı mimari ile geliştirilmiş çalışan bir Dosya Bütünlüğü İzleme uygulaması!

Bu proje, Nesne Tabanlı Programlama ve katmanlı mimari prensiplerini uygulayarak sıfırdan bir Dosya Bütünlüğü İzleme aracının nasıl geliştirileceğini gösteren bir uygulamadır. Projenin her kısmı şu işlemlerin nasıl yapılacağını göstermektedir:

* SHA-256 algoritması ile dosyaların kriptografik hash özetlerini hesaplamak
* Dosya yollarını ve hash değerlerini UTF-8 destekli baselines.csv dosyasında saklamak ve güncellemek
* Mevcut dosyaları kayıtlı hash değerleri ile karşılaştırarak değişen, silinen ve eklenen dosyaları tespit etmek
* FileSystemWatcher kullanarak hedef klasördeki oluşturma, değiştirme, silme ve yeniden adlandırma işlemlerini gerçek zamanlı yakalamak
* Tespit edilen tüm bütünlük olaylarını zaman damgasıyla fim_audit.log dosyasına kaydetmek

## Projeyi bilgisayarınızda nasıl çalıştırırsınız?

Projeyi çalıştırmak için bilgisayarınızda .NET SDK 8.0 veya üzeri kurulu olması yeterlidir.

1. Bu depoyu bilgisayarınıza klonlayın: `git clone https://github.com/GurkanBugdaycioglu/FileIntegrityMonitor.git`
2. Proje klasörünün içine girin: `cd FileIntegrityMonitor`
3. Uygulamayı derleyin ve başlatın: `dotnet run --project FimConsole/FimConsole.csproj`
4. Açılan menüden önce 1 tuşuna basarak örnek klasör için ilk baseline kaydını oluşturun, ardından 2 ile manuel doğrulama yapın veya 4 ile gerçek zamanlı izlemeyi başlatın.

## Kendi kullanımınız için nasıl uyarlarsınız?

Projenin çekirdek iş mantığı, kullanıcı arayüzünden tamamen bağımsız bir sınıf kütüphanesi olarak tasarlanmıştır. Bu katmanlı yapı sayesinde projeyi kendi ihtiyaçlarınıza göre kolayca genişletebilirsiniz:

- `FimConsole/Program.cs` içerisindeki `monitoredDir` değişkenini değiştirerek bilgisayarınızdaki herhangi bir kritik klasörü izlemeye alabilirsiniz.
- Mevcut iş mantığına dokunmadan `FimCore.dll` referansını kendi masaüstü veya web projenize ekleyerek doğrudan kullanabilirsiniz.

## Yol Haritası

### Tamamlanan Adımlar
- [x] Projenin C# ve .NET 8.0 ile geliştirilmeye başlanması
- [x] `.sln` çözüm yapısının, `FimCore` sınıf kütüphanesinin ve `FimConsole` projesinin oluşturulması
- [x] Veri modelleri (`BaselineEntry`, `IntegrityEvent`) ve arayüzlerin (`IHashCalculator`, `IBaselineRepository`, `IAuditLogger`) tanımlanması
- [x] `Sha256HashCalculator` ile dosya hash hesaplama servisinin yazılması
- [x] `CsvBaselineRepository` ile UTF-8 destekli CSV okuma ve yazma işlemlerinin tamamlanması
- [x] `FimEngine` sınıfında klasör tarama, tek dosya güncelleme ve bütünlük doğrulama mantığının kurulması
- [x] Konsol menüsünün oluşturulması ve Türkçe karakter desteğinin eklenmesi
- [x] `FileAuditLogger` ile tespit edilen olayların log dosyasına kaydedilmesi
- [x] `RealtimeWatcherService` ile canlı klasör izleme, debounce ve retry mekanizmalarının eklenmesi

### Planlanan Adımlar
- [ ] Baseline dosyasının kendi bütünlüğünü korumak için doğrulama veya şifreleme mekanizması eklenmesi
- [ ] İzlenecek klasör ve ayarların dışarıdan yapılandırılabilmesi
- [ ] `FimCore` kütüphanesi kullanılarak masaüstü kullanıcı arayüzünün geliştirilmesi
- [ ] Masaüstü arayüzünde canlı olay tablosu ve rapor dışa aktarma özelliklerinin eklenmesi