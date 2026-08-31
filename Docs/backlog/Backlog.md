# Proje Backlog & Görev Takibi 📋

Bu doküman, DailyDN projesinde gerçekleştirilen geliştirmelerin, hata düzeltmelerinin ve planlanan görevlerin tarih ve saat bazlı takibini sağlayarak **bağlam kaybını (context loss)** önlemek amacıyla tutulmaktadır.

---

## ⏳ Aktif / Sıradaki Görevler (Pending & In Progress)
<!-- Agent veya geliştirici işi yarıda bıraktığında veya sonraki oturuma geçildiğinde buradaki maddelerden devam edilir -->

- [ ] **Görev Adı: [BUG-05] `GenericRepository.cs` `disableTracking` Parametresinin İlk Overload'da Yok Sayılması**
  - **Kategori:** 🔴 Kritik Hata (EF Core Performance / ChangeTracker)
  - **Mevcut Durum:** Beklemede.
  - **Tamamlanması Gerekenler:** `GetPaginatedAsync` ilk overload'u içindeki `query = _dbSet.AsQueryable();` çağrısı `query = query.AsNoTracking();` ile değiştirilecek.
  - **Bağlı Dosyalar:**
    - `src/DailyDN.Infrastructure/Repositories/GenericRepository.cs`

- [ ] **Görev Adı: [BUG-06] `AuthService.VerifyEmailAsync` - Syntax Lekesi ve Geniş `catch` Yakalama**
  - **Kategori:** 🔴 Kritik Hata (Exception Handling / Code Quality)
  - **Mevcut Durum:** Beklemede.
  - **Tamamlanması Gerekenler:** Çift noktalı virgül temizlenecek, `catch (Exception)` yerine domain exception `InvalidOperationException` spesifik yakalanacak.
  - **Bağlı Dosyalar:**
    - `src/DailyDN.Application/Services/Implementations/AuthService.cs`

- [ ] **Görev Adı: [SEC-01] `AuthService.LoginAsync` OTP Response Sızıntısının Önlenmesi**
  - **Kategori:** 🔴 Güvenlik (2FA Bypass Riski)
  - **Mevcut Durum:** Beklemede.
  - **Tamamlanması Gerekenler:** `LoginAsync` sonucunda dönen `Otp` alanı prodüksiyon ortamı için gizlenecek/kaldırılacak.
  - **Bağlı Dosyalar:**
    - `src/DailyDN.Application/Services/Implementations/AuthService.cs`

- [ ] **Görev Adı: [ARCH-01] `Program.cs` Middleware Sıralaması ve Mükerrer `UseAuthorization` Düzeltmesi**
  - **Kategori:** 🟡 Mimari (Middleware Pipeline)
  - **Mevcut Durum:** Beklemede.
  - **Tamamlanması Gerekenler:** `CorrelationIdMiddleware` ve `ErrorHandlerMiddleware` zincirin en başına alınacak, mükerrer `UseAuthorization()` kaldırılacak.
  - **Bağlı Dosyalar:**
    - `src/DailyDN.API/Program.cs`

---

## ✅ Tamamlanan Görevler (Arşiv / Changelog)
<!-- Tamamlanan maddeler tarih ve saat damgasıyla buraya taşınır -->

### 📅 2026-08-31

#### 🕒 03:38:00 (UTC+3)
- [x] **[BUG-04] `UserService.cs` Profile Photo Cache Invalidation Eksikliğinin Giderilmesi**
  - **Branch:** `bugfix/core-fixes`
  - **Commit Mesajı:** `fix(cache): invalidate redis user cache on profile photo update`
  - **Yapılan İşlemler:**
    - `UserService.UpdateProfilePhoto` metoduna veritabanı güncellemesi sonrası `await redis.RemoveAsync($"{CacheKeyPrefix}{userId}");` çağrısı eklendi.
    - Böylece fotoğraf güncellendiğinde Redis'teki eski kullanıcı verisi temizlenerek sonraki isteklerde anında güncel avatarın dönmesi sağlandı.
    - `dotnet test` koşturuldu ve tüm 35 test başarıyla geçti.
  - **Bağlı Dosyalar:**
    - `src/DailyDN.Application/Services/Implementations/UserService.cs`

#### 🕒 03:36:00 (UTC+3)
- [x] **[BUG-03] `DailyDNDbContext` - Eksik DbSet Tanımlamalarının Eklenmesi**
  - **Branch:** `bugfix/core-fixes`
  - **Commit Mesajı:** `fix(efcore): add missing DbSet definitions to DailyDNDbContext`
  - **Yapılan İşlemler:**
    - `DailyDNDbContext` içine eksik olan `DbSet<Post> Posts`, `DbSet<Chat> Chats`, `DbSet<ChatMessage> ChatMessages` ve `DbSet<UserChat> UserChats` DbSet'leri eklendi.
    - Böylece LINQ sorgularında `_context.Posts` ve `_context.Chats` doğrudan erişilebilir hale getirildi.
    - `dotnet test` koşturuldu ve tüm 35 test başarıyla geçti.
  - **Bağlı Dosyalar:**
    - `src/DailyDN.Infrastructure/Contexts/DailyDNDbContext.cs`

#### 🕒 03:30:00 (UTC+3)
- [x] **[BUG-02] `ApplicationContext.cs` Audit Loglarında `CreatedBy`/`UpdatedBy` `0` Kalma Sorununun Düzeltilmesi**
  - **Branch:** `bugfix/core-fixes`
  - **Base:** `origin/develop` (`919d408`)
  - **Commit Hash:** `ffcf8e4`
  - **Commit Mesajı:** `fix(audit): resolve dynamic current user ID in ApplicationContext audit logs`
  - **Yapılan İşlemler:**
    - `ApplicationContext.cs` içinde constructor parametresinden primitive `int` olarak kopyalanan `private readonly int _currentUser = currentUser.UserId;` alanı kaldırıldı.
    - Yerine `private readonly IAuthenticatedUser _currentUser = currentUser;` referansı tutularak `ApplyAuditInfo()` içerisinde `_currentUser.UserId` değerinin dinamik olarak okunması sağlandı.
    - Böylece middleware çalıştıktan sonra çözümlenen kullanıcı kimliği (UserId) `CreatedBy` ve `UpdatedBy` alanlarına eksiksiz yazıldı.
    - Proje köküne `AGENT.md` çalışma anayasası belgesi eklendi.
    - `dotnet test` koşturuldu ve tüm 35 test başarıyla geçti.
  - **Bağlı Dosyalar:**
    - `src/DailyDN.Infrastructure/Contexts/ApplicationContext.cs`
    - `AGENT.md`

#### 🕒 03:26:00 (UTC+3)
- [x] **[BUG-01] `AuthService.VerifyOtpAsync` - Dizi Sınır Aşımı (`ArgumentOutOfRangeException`) Düzeltmesi**
  - **Branch:** `bugfix/auth-verify-otp-out-of-range`
  - **Base:** `origin/main` / `origin/develop` (`919d408` - En güncel upstream üzerine Rebase edildi)
  - **Commit Hash:** `4b74958`
  - **Commit Mesajı:** `fix(auth): prevent ArgumentOutOfRangeException in VerifyOtpAsync when user not found by guid`
  - **Yapılan İşlemler:**
    - `AuthService.VerifyOtpAsync` içinde `var userList = await uow.Users.GetAsync(u => u.Guid == guid); var user = userList[0];` şeklindeki doğrudan indeks erişimi kaldırıldı.
    - Yerine `var user = await uow.Users.FirstOrDefaultAsync(u => u.Guid == guid);` ve `if (user is null) return null;` kontrolü eklendi.
    - `origin/develop`'tan gelen yeni Value Objects (`Email.Value`, `PhoneNumber.Value`) ve `IHttpContextAccessor` değişiklikleri ile rebase edilerek tam senkronize hale getirildi.
    - `dotnet test` çalıştırıldı ve tüm 35 test başarıyla geçti.
  - **Bağlı Dosyalar:**
    - `src/DailyDN.Application/Services/Implementations/AuthService.cs`
    - `src/DailyDN.Application/Features/Auth/VerifyOtp/VerifyOtpCommandHandler.cs`
    - `src/DailyDN.Tests/Application/Features/Auth/VerifyOtp/VerifyOtpCommandHandlerTests.cs`

#### 🕒 03:14:00 (UTC+3)
- [x] **[DOCS-02] Kod İnceleme, Hata Analizi ve Zafiyet Raporlama Seti Oluşturuldu**
  - **Yapılan İşlemler:** Projenin kaynak kodlarına dokunulmadan statik kod analizi yapıldı ve 4 ayrı kategoride detaylı rapor oluşturuldu:
    1. `Docs/researches/01-critical-bugs-and-runtime-errors.md`
    2. `Docs/researches/02-security-and-vulnerability-assessment.md`
    3. `Docs/researches/03-architectural-and-anti-patterns.md`
    4. `Docs/researches/04-code-smells-and-quality-improvements.md`
    5. `Docs/researches/README.md`
  - **Bağlı Dosyalar:** `Docs/researches/*`

#### 🕒 02:56:00 (UTC+3)
- [x] **[DOCS-01] Proje Mimarisi, Sistemler ve Git Geliştirme Rehberleri Hazırlandı**
  - **Yapılan İşlemler:** Projenin baştan sona mimari analizi yapıldı ve 8 adet kapsamlı rehber dokümanı oluşturuldu (`Docs/01-08` ve `Docs/README.md`).
  - **Bağlı Dosyalar:** `Docs/*.md`
