# Backlog Arşivi: 2026-08-31 - Güvenlik & Mimari Düzeltmeleri 🛡️

- **Arşiv Tarihi / Saati:** 2026-08-31 03:55:00 (UTC+3)
- **Kapsam:** `Docs/researches/02-security-and-vulnerability-assessment.md` ve `03-architectural-and-anti-patterns.md` içindeki tüm güvenlik ve mimari maddelerin eksiksiz çözümü.
- **Çalışılan Dal (Branch):** `bugfix/core-fixes`
- **Genel Test Durumu:** ✅ 35/35 Test Başarılı (`dotnet test`)

---

## 📑 Arşivlenen Görevler ve Değişiklik Günlüğü (Changelog)

### 1. 🕒 05:15:00 (UTC+3) - [SEC-06] AutoMapper 14.0.0 Yüksek Öncelikli Güvenlik Açığı (`NU1903` / `GHSA-rvv3-g6hj-g44x`) Yaması
- **Commit:** `fix(deps): upgrade AutoMapper to 16.2.0 to resolve NU1903 vulnerability`
- **Bağlı Dosyalar:** `src/DailyDN.Application/DailyDN.Application.csproj`, `src/DailyDN.Application/ServiceCollectionExtensions.cs`
- **Açıklama:**
  - `AutoMapper` paketi 14.0.0'dan güvenlik açığı kapatılmış en güncel 16.2.0 sürümüne yükseltildi.
  - `ServiceCollectionExtensions.cs` içindeki DI tescili yeni API (`cfg => cfg.AddMaps(...)`) standardına uyarlandı.
  - Derleme ve restore sırasındaki 2 adet `NU1903` güvenlik uyarısı tamamen sıfırlandı.

---

### 2. 🕒 03:54:00 (UTC+3) - [ARCH-04] `AuthService.RefreshTokenAsync` ve `TokenService.RotateRefreshToken` DRY Refactoring
- **Commit:** `0804473` (`refactor(auth): integrate AuthService.RefreshTokenAsync with TokenService.RotateRefreshToken`)
- **Bağlı Dosyalar:** `src/DailyDN.Application/Services/Implementations/AuthService.cs`
- **Açıklama:**
  - `AuthService` içindeki elle yazılmış token hashleme ve session güncelleme kodları kaldırılarak `TokenService.RotateRefreshToken()` çağrısına bağlandı. DRY prensibi sağlandı.

---

### 2. 🕒 03:53:30 (UTC+3) - [ARCH-03] `LoggingBehavior.cs` Reflection Önbelleklemesi (`ConcurrentDictionary` Cache)
- **Commit:** `9f8e6e1` (`perf(logging): cache loggable reflection property info in LoggingBehavior`)
- **Bağlı Dosyalar:** `src/DailyDN.Application/Behaviors/LoggingBehavior.cs`
- **Açıklama:**
  - MediatR boru hattındaki her istekte yapılan `GetType().GetProperties()` taraması static `ConcurrentDictionary<Type, PropertyInfo[]>` ile önbelleğe alındı. CPU yükü minimize edildi.

---

### 3. 🕒 03:53:00 (UTC+3) - [ARCH-02] Polly Circuit Breaker Aşırı Agresif Eşiklerinin İyileştirilmesi
- **Commit:** `d1d73ba` (`fix(resilience): tune Polly Circuit Breaker thresholds to 3 errors and 30 seconds break duration`)
- **Bağlı Dosyalar:** `src/DailyDN.Infrastructure/ServiceCollectionExtensions.cs`
- **Açıklama:**
  - 1 hata ile 15 dakika açılan agresif Circuit Breaker eşiği; 3 ardışık hata ve 30 saniye devre açık kalma süresi olarak optimize edildi.

---

### 4. 🕒 03:52:45 (UTC+3) - [SEC-05] `Program.cs` CORS Yapılandırması ve Pipeline Entegrasyonu
- **Commit:** `4d0dd40` (`feat(api): add CORS configuration and middleware integration`)
- **Bağlı Dosyalar:** `src/DailyDN.API/Program.cs`
- **Açıklama:**
  - `builder.Services.AddCors(...)` ve `app.UseCors("DefaultCorsPolicy")` boru hattına eklenerek web/mobil istemcilerin tarayıcı CORS engeline takılması önlendi.

---

### 5. 🕒 03:52:20 (UTC+3) - [SEC-04] `AuthorizedAttribute.cs` Güvenlik & DI Güçlendirmesi
- **Commit:** `a5ee7f5` (`fix(auth): improve AuthorizedAttribute DI safety and return proper 401/403 responses`)
- **Bağlı Dosyalar:** `src/DailyDN.Application/Common/Attributes/AuthorizedAttribute.cs`
- **Açıklama:**
  - `GetRequiredService<IAuthenticatedUser>()` ile DI güvenliği sağlandı, giriş yapmamış kullanıcılar için 401 Unauthorized, yetkisi olmayan kullanıcılar için 403 Forbidden doğru exception'ı (`AuthorizationException`) fırlatıldı.

---

### 6. 🕒 03:51:50 (UTC+3) - [SEC-02 & SEC-03] `FileStorageService.cs` Path Traversal & Whitelist Uzantı Koruması
- **Commit:** `2d06fe8` (`fix(security): prevent path traversal and arbitrary file upload in FileStorageService`)
- **Bağlı Dosyalar:** `src/DailyDN.Infrastructure/Services/Impl/FileStorageService.cs`
- **Açıklama:**
  - Whitelist uzantı doğrulaması (`.jpg`, `.png`, `.webp` vb.), `Path.GetFullPath` tabanlı Path Traversal güvenliği ve güvenli URL oluşturucu (`BuildFileUrl`) eklendi.

---

### 7. 🕒 03:51:20 (UTC+3) - [SEC-01] `AuthService.LoginAsync` OTP Kodunun API Yanıtında İstemciye Sızmasının Önlenmesi
- **Commit:** `099adcd` (`fix(auth): protect OTP leakage in LoginAsync response for production builds`)
- **Bağlı Dosyalar:** `src/DailyDN.Application/Services/Implementations/AuthService.cs`
- **Açıklama:**
  - `LoginAsync` yanıtındaki `Otp` alanı `#if DEBUG` bloğu altına alınarak Release/Production derlemelerinde 2FA bypass riski tamamen ortadan kaldırıldı.
