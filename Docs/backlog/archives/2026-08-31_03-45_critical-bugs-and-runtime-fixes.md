# Backlog Arşivi: 2026-08-31 - Kritik Hatalar ve Runtime Düzeltmeleri 📦

- **Arşiv Tarihi / Saati:** 2026-08-31 03:45:00 (UTC+3)
- **Kapsam:** `Docs/researches/01-critical-bugs-and-runtime-errors.md` ve `03-architectural-and-anti-patterns.md` içindeki kritik çalışma zamanı hatalarının giderilmesi.
- **Çalışılan Dal (Branch):** `bugfix/core-fixes`
- **Genel Test Durumu:** ✅ 35/35 Test Başarılı (`dotnet test`)

---

## 📑 Arşivlenen Görevler ve Değişiklik Günlüğü (Changelog)

### 1. 🕒 03:41:00 (UTC+3) - [ARCH-01] `Program.cs` Middleware Sıralaması ve Mükerrer `UseAuthorization` Düzeltmesi
- **Commit:** `b16ea12` (`fix(api): correct middleware pipeline execution order and remove redundant UseAuthorization`)
- **Bağlı Dosyalar:** `src/DailyDN.API/Program.cs`
- **Açıklama:**
  - `CorrelationIdMiddleware` ve `ErrorHandlerMiddleware` boru hattının en başına taşındı.
  - `UseAuthentication` -> `AuthenticatedUserMiddleware` -> `UseAuthorization` sırası kuruldu.
  - Mükerrer olan 2. `app.UseAuthorization()` çağrısı kaldırıldı.

---

### 2. 🕒 03:40:00 (UTC+3) - [BUG-06] `AuthService.VerifyEmailAsync` - Exception Handling İyileştirmesi
- **Commit:** `044ccbc` (`fix(auth): catch specific InvalidOperationException in VerifyEmailAsync`)
- **Bağlı Dosyalar:** `src/DailyDN.Application/Services/Implementations/AuthService.cs`
- **Açıklama:**
  - `VerifyEmailAsync` içindeki geniş `catch (Exception)` bloğu kaldırılarak `catch (InvalidOperationException ex)` şeklinde spesifik hale getirildi.
  - Veritabanı ve altyapı hatalarının maskelenmesi engellendi.

---

### 3. 🕒 03:39:00 (UTC+3) - [BUG-05] `GenericRepository.cs` `disableTracking` AsNoTracking Atlamasının Düzeltilmesi
- **Commit:** `42c501b` (`fix(repository): properly call AsNoTracking when disableTracking is true in GetPaginatedAsync`)
- **Bağlı Dosyalar:** `src/DailyDN.Infrastructure/Repositories/GenericRepository.cs`
- **Açıklama:**
  - `GetPaginatedAsync` ilk overload'unda `query = _dbSet.AsQueryable();` yerine `query = query.AsNoTracking();` çağrısı sağlandı.
  - Sayfalama sorgularında gereksiz ChangeTracker bellek tüketimi önlendi.

---

### 4. 🕒 03:38:00 (UTC+3) - [BUG-04] `UserService.cs` Profile Photo Cache Invalidation Eksikliğinin Giderilmesi
- **Commit:** `9ab159e` (`fix(cache): invalidate redis user cache on profile photo update`)
- **Bağlı Dosyalar:** `src/DailyDN.Application/Services/Implementations/UserService.cs`
- **Açıklama:**
  - `UpdateProfilePhoto` metoduna `await redis.RemoveAsync($"{CacheKeyPrefix}{userId}");` eklendi.
  - Kullanıcı avatarını güncellediğinde Redis önbelleğindeki eski verinin kalması (Cache Stale) önlendi.

---

### 5. 🕒 03:36:00 (UTC+3) - [BUG-03] `DailyDNDbContext` Eksik DbSet Tanımlamalarının Eklenmesi
- **Commit:** `f056270` (`fix(efcore): add missing DbSet definitions to DailyDNDbContext`)
- **Bağlı Dosyalar:** `src/DailyDN.Infrastructure/Contexts/DailyDNDbContext.cs`
- **Açıklama:**
  - `DailyDNDbContext` içine eksik olan `Posts`, `Chats`, `ChatMessages` ve `UserChats` DbSet tanımları eklendi.
  - LINQ sorgularında doğrudan DbSet erişimi sağlandı.

---

### 6. 🕒 03:30:00 (UTC+3) - [BUG-02] `ApplicationContext.cs` Audit Loglarında `CreatedBy`/`UpdatedBy` `0` Kalma Sorununun Düzeltilmesi
- **Commit:** `e4629e7` (`fix(audit): resolve dynamic current user ID in ApplicationContext audit logs`)
- **Bağlı Dosyalar:** `src/DailyDN.Infrastructure/Contexts/ApplicationContext.cs`, `AGENT.md`
- **Açıklama:**
  - `_currentUser` primitive `int` kopyası yerine `IAuthenticatedUser` referansı tutuldu.
  - `ApplyAuditInfo()` içinde `_currentUser.UserId` dinamik okunarak audit loglarında `0` kalması engellendi.

---

### 7. 🕒 03:26:00 (UTC+3) - [BUG-01] `AuthService.VerifyOtpAsync` Dizi Sınır Aşımı (`ArgumentOutOfRangeException`) Düzeltmesi
- **Commit:** `4b74958` (`fix(auth): prevent ArgumentOutOfRangeException in VerifyOtpAsync when user not found by guid`)
- **Bağlı Dosyalar:** `src/DailyDN.Application/Services/Implementations/AuthService.cs`
- **Açıklama:**
  - `userList[0]` doğrudan indeks erişimi yerine `FirstOrDefaultAsync` ve null kontrolü eklendi.
  - Geçersiz GUID isteklerinde 500 hatası yerine temiz `null` dönmesi sağlandı.
