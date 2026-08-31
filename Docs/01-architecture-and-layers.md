# 01. Katmanlı Mimari ve Sorumluluklar (Architecture & Layers) 🏗️

DailyDN projesi, **Clean Architecture (Temiz Mimari)** ve **Onion Architecture** ilkeleri temel alınarak katmanlara ayrılmıştır. Bu mimarinin temel amacı; iş kurallarını (business logic) dış etkenlerden (veritabanı, web framework'leri, harici kütüphaneler) izole etmek ve test edilebilir, bakımı kolay, gevşek bağlı (loosely coupled) bir sistem inşa etmektir.

---

## 🧭 Bağımlılık Kuralı (The Dependency Rule)

Clean Architecture'ın altın kuralı: **Bağımlılıklar daima içe doğru (Domain'e doğru) akar.**

```mermaid
graph TD
    API[DailyDN.API<br/>(Presentation Layer)] --> Application[DailyDN.Application<br/>(Use Cases / CQRS)]
    Infrastructure[DailyDN.Infrastructure<br/>(External Concerns)] --> Application
    Infrastructure --> Domain[DailyDN.Domain<br/>(Enterprise Business Rules)]
    Application --> Domain
```

- **Domain:** Sıfır dış bağımlılık. En çekirdek iş mantığı ve kuralları.
- **Application:** Sadece Domain'e bağımlıdır. Use-case'ler, CQRS modelleri, iş akışları.
- **Infrastructure:** Domain ve Application arayüzlerini somutlaştırır (EF Core, Redis, SMTP vb.).
- **API (Presentation):** Dış dünyanın giriş kapısıdır. Application ve Infrastructure'ı DI konteynerinde bir araya getirir.

---

## 📦 Katmanların Detaylı Analizi

### 1. `DailyDN.Domain` (Çekirdek İş Mantığı Katmanı)

Sistemin en merkezindeki katmandır. Veritabanından, UI'dan, harici web API veya kütüphanelerden tamamen bağımsızdır.

- **Amaç:** Projenin iş varlıklarını (Entity), enum'larını, domain kurallarını ve zengin domain modelini barındırmak.
- **İçerik:**
  - `Entities/Entity.cs`: Tüm varlıkların türediği temel sınıf (`Id`, `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `IsDeleted`).
  - `Entities/User.cs`: Zengin domain metodlarına sahip (`SetOtp()`, `IsOtpValid()`, `GeneratePasswordResetToken()`, `VerifyEmailToken()`, `Login()` vb.) kullanıcı modeli.
  - `Entities/UserSession.cs`: Oturum yönetimi, refresh token hash'i, IP/UserAgent ve token rotation durumları.
  - `Entities/Role.cs`, `Claim.cs`, `UserRole.cs`, `RoleClaim.cs`: Rol tabanlı yetki ve yetki-claim ilişkileri.
  - `Entities/Post.cs`, `Chat.cs`, `ChatMessage.cs`, `UserChat.cs`: Sosyal medya gönderileri ve anlık mesajlaşma varlıkları.
  - `Enums/RoleEnum.cs`: Sistem rolleri (`Admin`, `User`, vb.).
- **Bağımlılıklar:** Hiçbir dış proje veya harici paket referansı içermez.

---

### 2. `DailyDN.Application` (İş Akışları ve CQRS Katmanı)

İş senaryolarını (Use-Cases) yöneten katmandır. HTTP, Veritabanı veya UI ile doğrudan ilgilenmez; MediatR ile CQRS (Command Query Responsibility Segregation) modelini uygular.

- **Amaç:** İstekleri doğrulamak, iş kurallarını işletmek, Domain modelleri üzerinde işlem yapmak ve DTO/Response nesneleri üretmek.
- **İçerik:**
  - `Features/`: CQRS dikey dilimleme (Vertical Slice) klasörleri:
    - `Auth/`: `Login`, `Register`, `VerifyOtp`, `RefreshToken`, `ForgotPassword`, `ResetPassword`, `VerifyEmail`.
    - `Posts/`: `Add`, `GetList`.
    - `Users/`: `GetUserById`, `UpdateProfilePhoto`.
    - Her bir feature kendi içinde `Command/Query`, `CommandHandler/QueryHandler`, `Validator` ve `Response` sınıflarını barındırır.
  - `Behaviors/`: MediatR Pipeline adımları:
    - `LoggingBehavior`: İstekleri ve loglanabilir parametreleri otomatik loglar (`[DoNotLog]` niteliğine sahip şifre gibi hassas alanları maskeler).
    - `ValidationBehavior`: Herhangi bir Command/Query çalışmadan önce FluentValidation doğrulamalarını koşturur; hata varsa anında `ValidationException` fırlatır.
  - `Common/`:
    - `Attributes/`: `[Authorized("ClaimName")]`, `[DoNotLog]`.
    - `Model/`: Standart `Result`, `Result<T>`, `PaginatedResult<T>`, `Error` modelleri.
  - `Services/`: Domain seviyesindeki orkestrasyon servislerinin interface ve implementasyonları (`IAuthService`, `IUserService`, `IPostsService`, `IOtpService`).
  - `Profiles/`: AutoMapper eşleme profilleri (`MappingProfile.cs`).
  - `Messaging/`: `ICommand`, `ICommandHandler`, `IQuery`, `IQueryHandler`, `IPaginatedQuery` arayüzleri.
  - `Exceptions/`: `ApiAuthenticationException`, `AuthorizationException`, `FailCode`.
- **Bağımlılıklar:** `DailyDN.Domain`, `MediatR`, `FluentValidation`, `AutoMapper`, `Microsoft.AspNetCore.Identity`.

---

### 3. `DailyDN.Infrastructure` (Dış Dünya ve Altyapı Entegrasyonları)

Application katmanında tanımlanan arayüzlerin (Interface) teknik olarak somutlaştırıldığı (Implementation) katmandır.

- **Amaç:** Veri tabanı erişimi, önbellekleme, e-posta gönderimi, dosya yükleme, SMS ve dış servis entegrasyonlarını gerçekleştirmek.
- **İçerik:**
  - `Contexts/`:
    - `ApplicationContext.cs`: ChangeTracker ile otomatik Audit Log (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`), Soft Delete dönüşümü (`IsDeleted = true`) ve Global Query Filter (`!e.IsDeleted`).
    - `DailyDNDbContext.cs`: Entity konfigürasyonlarını yükleyen, Seed verilerini çağıran somut DbContext.
  - `Repositories/`:
    - `IGenericRepository<T>` ve `GenericRepository<T>`: Sayfalama, filtreleme, include destekli genel repository.
    - `IUserRepository`, `IUserSessionRepository`, `IPostRepository` ve `Impl/` altındaki özel repository'ler.
  - `UnitOfWork/`: `IUnitOfWork` ve `UnitOfWork`: Tek transaction altında çoklu repository yönetimi ve `SaveChangesAsync()`.
  - `Redis/` & `Services/Impl/RedisCacheService.cs`: StackExchange.Redis ile cache mekanizması. **Polly** ile sarmalanmış Retry + Circuit Breaker + Fallback (Cache bypass) dayanıklılık politikası.
  - `Services/Impl/TokenService.cs`: JWT Access Token üretimi (Claim'lerle beraber), Refresh Token üretimi, SHA-256 hash'leme ve token rotation mantığı.
  - `Services/Impl/SmtpMailService.cs` & `MailTemplateService.cs`: Embedded HTML şablonları (`VerifyEmailTemplate.html`, `ResetPasswordTemplate.html`) ile e-posta gönderimi.
  - `Services/Impl/FileStorageService.cs`: Profil fotoğrafları ve medya dosyaları için disk tabanlı depolama yönetimi.
  - `Services/Impl/FakeSmsProvider.cs` & `SmsService.cs`: OTP gönderim altyapısı.
  - `Configurations/`: EF Core `IEntityTypeConfiguration<T>` sınıfları.
  - `Seed/`: Veritabanı ilk kurulumunda varsayılan roller, claimler ve kullanıcılar (`SeedUsers`, `SeedRoles`, vb.).
- **Bağımlılıklar:** `DailyDN.Domain`, `DailyDN.Application`, `Microsoft.EntityFrameworkCore.SqlServer`, `StackExchange.Redis`, `Polly`.

---

### 4. `DailyDN.API` (Sunum ve Giriş Katmanı)

Sistemin HTTP isteklerini karşılayan, istemcilerle konuşan REST API katmanıdır.

- **Amaç:** Endpoint yönlendirmeleri, Controller'lar, Middleware'ler, API Versioning, Swagger ve uygulama başlangıç konfigürasyonunu sağlamak.
- **İçerik:**
  - `Controllers/`:
    - `AuthController`: Login, Register, VerifyOtp, RefreshToken, ForgotPassword, ResetPassword, VerifyEmail.
    - `PostController`: Post ekleme, listeleme (`[Authorized("PostAdd")]`, `[Authorized("PostGet")]`).
    - `UserController`: Kullanıcı getirme (`GetById`), profil fotoğrafı yükleme (`UpdateProfilePhoto`).
  - `Middleware/`:
    - `CorrelationIdMiddleware`: Her HTTP isteğine tekil bir `X-Correlation-Id` atar ve Serilog LogContext'e push eder.
    - `ErrorHandlerMiddleware`: Oluşan tüm istisnaları (`ValidationException`, `ApiAuthenticationException`, `AuthorizationException`, `Exception`) yakalar ve standart hata formatına dönüştürür.
    - `AuthenticatedUserMiddleware`: JWT token'dan kullanıcı ID'si, rolü ve yetki claim'lerini okuyup Scoped `IAuthenticatedUser` nesnesine doldurur.
  - `Program.cs`: Bağımlılıkların tescili, Serilog konfigürasyonu, Middleware pipeline sıralaması.
  - `ServiceCollectionExtensions.cs`: API katmanı tescil uzantıları (`AddPresentation`).
  - `appsettings.*.json`: Ortamlara göre yapılandırma ayarları (Serilog, MSSQL, Redis, JWT, SMTP, FileStorage).
- **Bağımlılıklar:** `DailyDN.Application`, `DailyDN.Infrastructure`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Serilog.AspNetCore`, `Swashbuckle.AspNetCore`, `Microsoft.AspNetCore.Mvc.Versioning`.

---

### 5. `DailyDN.Tests` (Test Katmanı)

Sistemin iş kurallarının ve altyapı servislerinin doğruluğunu garanti altına alan birim ve entegrasyon testlerini barındırır.

- **Amaç:** CQRS Handler'larını, Validator'ları ve kritik altyapı servislerini (örn: `TokenService`) izole olarak test etmek.
- **İçerik:**
  - `Application/Features/Auth/`: Login, Register, VerifyOtp, RefreshToken, ForgotPassword, ResetPassword handler ve validator testleri.
  - `Application/Features/Posts/`: Post handler ve validator testleri.
  - `Application/Features/Users/`: User query/command testleri.
  - `Infrastructure/Services/`: `TokenServiceTests.cs` (JWT üretimi, claims atamaları, refresh token rotasyonu).
- **Bağımlılıklar:** `xunit`, `Moq`, `FluentAssertions`, `DailyDN.Application`, `DailyDN.Infrastructure`.

---

## 📊 Katmanlar Arası Sorumluluk Matrisi

| Sorumluluk / Görev | Domain | Application | Infrastructure | API |
|---|:---:|:---:|:---:|:---:|
| Veritabanı Tablo Yapısı Tanımı | ✅ | ❌ | ✅ (Fluent API) | ❌ |
| İş Kuralları (Entity Methods) | ✅ | ❌ | ❌ | ❌ |
| CQRS Komut ve Sorguları | ❌ | ✅ | ❌ | ❌ |
| FluentValidation Kuralları | ❌ | ✅ | ❌ | ❌ |
| DTO / Response Mapping | ❌ | ✅ | ❌ | ❌ |
| Veritabanı Sorguları & SQL | ❌ | ❌ | ✅ | ❌ |
| Redis Cache & Polly Politikaları | ❌ | ❌ | ✅ | ❌ |
| JWT Üretimi & Token Rotation | ❌ | ❌ | ✅ | ❌ |
| SMTP E-posta & HTML Şablonları | ❌ | ❌ | ✅ | ❌ |
| HTTP Controller Routing & Versioning | ❌ | ❌ | ❌ | ✅ |
| Global Exception Handling & Middlewares | ❌ | ❌ | ❌ | ✅ |
| Swagger & OpenAPI Dökümantasyonu | ❌ | ❌ | ❌ | ✅ |
