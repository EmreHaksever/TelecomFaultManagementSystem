Rol ve Hedef:
Sen uzman bir .NET Yazılım Mimarı ve Mentor'sun. Birlikte, büyük bir telekomünikasyon şirketi (örneğin Türk Telekom) mülakatı için Junior/Mid-Level yetkinliklerimi göstereceğim, kurumsal standartlara uygun bir backend projesi geliştireceğiz. Adım adım ilerleyeceğiz, sen bana kodu ve mantığını vereceksin, ben uygulayıp sana geri dönüş yapacağım.

Proje Adı:
Telecom Fault & Ticket Management System (Telekom Arıza, Talep ve Saha Operasyon Yönetim Sistemi)

Proje Senaryosu ve Amacı:
Sistem, internet/fiber/mobil hizmet alan müşterilerin arıza kayıtlarını (ticket) yöneten bir BSS/OSS (Operasyon Destek Sistemi) simülasyonudur. Sistem sadece temel CRUD işlemlerinden ibaret olmayıp; iş kuralları (business logic), zaman yönetimi (SLA), yetkilendirme (RBAC) ve denetim izi (Audit Logging) içerecektir.

1. Teknoloji Yığını (Tech Stack)
Geliştirme Ortamı: Visual Studio 2022

Backend Framework: .NET 8 (ASP.NET Core Web API - Klasik Controller Yapısı)

Veritabanı ve ORM: MS SQL Server & Entity Framework Core (Code-First yaklaşımı)

Arka Plan İşlemleri (Background Jobs): Hangfire veya .NET Worker Service (SLA takibi için)

Kimlik Doğrulama: ASP.NET Core Identity & JWT (JSON Web Token)

Mülakat Odaklı Ekstralar: Docker desteği, Serilog ile loglama, xUnit ile Unit Test yazımı.

2. Mimari Tasarım (N-Tier Architecture / Katmanlı Mimari)
Proje kesinlikle "Spaghetti Code" olmayacak. Şu 4 temel katmana (Class Library) sahip bir çözüm (Solution) kurgulayacağız:

Telecom.Domain: Sistemdeki temel varlıkların (Entity sınıflarının) ve arayüzlerin (Interface) bulunduğu, hiçbir yere bağımlılığı olmayan çekirdek katman.

Telecom.DataAccess: EF Core kurulumunun, DbContext'in ve Repository deseninin uygulandığı veri erişim katmanı.

Telecom.Business: "Controller aptal olmalıdır" (Thin Controller, Fat Service) prensibiyle tüm if-else bloklarının, SLA hesaplamalarının ve iş kurallarının yazıldığı servis katmanı.

Telecom.API: Dışarıya açılan kapı. Sadece DTO (Data Transfer Object) kabul eden, istekleri doğrudan Business katmanına ileten Controller'ların bulunduğu katman.

3. Temel Varlıklar (Entities)
Başlangıç aşamasında şu 3 tabloya odaklanacağız:

AppUser: Sistem kullanıcıları (Id, Ad, Soyad, Email, Rol).

Ticket: Arıza kayıtları (Id, Müşteri No, Başlık, Açıklama, Öncelik Durumu [Low, Medium, High, Critical], Durum [Open, InProgress, Resolved], Oluşturulma Tarihi, SLA Bitiş Tarihi, Atanan Teknisyen).

AuditLog: Kurumsal zorunluluk olan işlem geçmişi (Id, İşlemi Yapan, İşlem Tipi, Etkilenen Ticket, Tarih, Detay).

4. Sistem İşleyişi ve Temel Kurallar (Business Rules)
Rol Yönetimi (RBAC): Admin (tüm sistemi yönetir), Agent (Müşteri temsilcisi - arıza açar), Technician (Saha personeli - sadece kendine atananı görür ve çözer).

SLA (Service Level Agreement) Mantığı: Agent bir arıza kaydı açtığında, seçilen "Öncelik (Priority)" durumuna göre Business katmanı otomatik bir "SLA Bitiş Tarihi" hesaplar (Örn: Critical ise 4 saat, Low ise 48 saat).

Audit (Denetim): Arıza durumu değiştiğinde veya yeni bir kayıt açıldığında bu işlemler AuditLog tablosuna asenkron olarak kaydedilir.

İlk Görev İsteğim:
Lütfen bu proje isterlerini ve mimariyi anladığını onayla. Ardından, Visual Studio 2022'de boş bir "Blank Solution" oluşturduğumu varsayarak; proje katmanlarını (Domain, DataAccess, vb.) birbirine nasıl referanslayacağımı ve Telecom.Domain içerisine Ticket ve AuditLog entity'lerinin C# kodlarını nasıl yazacağımı bana adım adım göstererek projemizi başlatalım.