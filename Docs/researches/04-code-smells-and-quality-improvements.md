# 04. Kod Kalitesi ve İyileştirme Fırsatları (Code Smells & Improvements) 🟢

Bu raporda, kod temizliği (Clean Code), bakım kolaylığı, isimlendirme standartları, performans optimizasyonları ve modern .NET standartlarına uyum fırsatları listelenmiştir.

---

## 1. 📧 `SmtpMailService.cs` - `SmtpClient` ve Socket Exhaustion Riski

- **Konum:** `src/DailyDN.Infrastructure/Services/Impl/SmtpMailService.cs` (Satır 25-30)
- **Mevcut Kod:**
  ```csharp
  using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
  ```
- **İnceleme & İyileştirme:**
  Microsoft'un resmi dokümantasyonunda `System.Net.Mail.SmtpClient` sınıfının modern protokolleri desteklemediği, connection pooling barındırmadığı ve yüksek trafik altında **Socket Exhaustion (Port tükenmesi)** oluşturduğu belirtilmekte; yerine açık kaynaklı kurumsal standart olan **MailKit / MimeKit** kütüphanesi önerilmektedir.

---

## 2. 🕒 `DateTime.Now` vs `DateTime.UtcNow` Zaman Dilimi Uyuşmazlığı

- **Konum:** `src/DailyDN.Infrastructure/Services/Impl/TokenService.cs` (Satır 54)
- **Mevcut Kod:**
  ```csharp
  var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);
  // ...
  return new TokenResponse(jwt, "", expiration, DateTime.Now); // ⚠️ Local Time vs UTC karışıklığı!
  ```
- **İnceleme & İyileştirme:**
  Token'ın bitiş süresi `DateTime.UtcNow` olarak hesaplanırken, `TokenResponse` nesnesi içine `DateTime.Now` (sunucunun yerel saati) verilmektedir. Sunucunun ve istemcinin farklı saat dilimlerinde olması durumunda token geçerlilik kontrollerinde saat farkı karmaşası yaşanır. Sistem genelinde **istisnasız her zaman `DateTime.UtcNow`** kullanılmalıdır.

---

## 3. 🏷️ `TokenResponse` İçinde Kafa Karıştıran İsimlendirme (`RefreshTokenHash`)

- **Konum:** `src/DailyDN.Infrastructure/Models/TokenResponse.cs` ve `TokenService.cs`
- **Mevcut Kod:**
  ```csharp
  token.RefreshTokenHash = rawRefreshToken; // ⚠️ Düz metin token, adı "Hash" olan property'ye atanıyor!
  ```
- **İnceleme & İyileştirme:**
  İstemciye dönülen DTO modelinde özelliğin adı `RefreshTokenHash` olarak tanımlanmıştır. Ancak içine hash değil, istemcinin kullanacağı ham (raw) refresh token konulmaktadır. Bu durum kodu yeni okuyan geliştiriciler için kafa karışıklığı yaratır. Modeldeki alan `RefreshToken` olarak yeniden adlandırılmalıdır.

---

## 4. 🌐 Dil ve Hata Mesajı Tutarsızlığı (Localization)

- **Konumlar:**
  - `AuthorizedAttribute.cs`: `"Uygulama Yöneticisinden Yetki Tanımlaması Yapmasını İsteyeniz."` (Türkçe)
  - `AuthService.cs`: `"Email or password is incorrect."` (İngilizce)
  - `ErrorHandlerMiddleware.cs`: `"Validation Error"`, `"One or more fields are invalid."` (İngilizce)
- **İnceleme & İyileştirme:**
  Hata mesajları iki farklı dilde hardcoded olarak yazılmıştır. Proje genelinde tüm hata mesajları İngilizce olmalı veya merkezi bir `Resources` (IStringLocalizer) yapısı ile çok dilli destek sağlanmalıdır.

---

## 5. 🧹 Controller'larda Temizlenmemiş `using` Direktifleri

- **Konum:** `src/DailyDN.API/Controllers/UserController.cs` (Satır 2-7)
- **Mevcut Kod:**
  ```csharp
  using DailyDN.Application.Features.Auth.ForgotPassword;
  using DailyDN.Application.Features.Auth.Login;
  using DailyDN.Application.Features.Auth.RefreshToken;
  using DailyDN.Application.Features.Auth.Register;
  using DailyDN.Application.Features.Auth.ResetPassword;
  using DailyDN.Application.Features.Auth.VerifyOtp;
  ```
- **İnceleme & İyileştirme:**
  `UserController` içerisinde Auth işlemlerine ait tüm use-case using direktifleri kopyala-yapıştır sonucu unutulmuştur. Bu gereksiz bağımlılıklar temizlenmelidir.

---

## 6. 📄 Swagger `ProducesResponseType` Tip Tanımlarının Eksikliği

- **Konum:** `PostController.cs`, `UserController.cs`, `AuthController.cs`
- **Mevcut Kod:**
  ```csharp
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
  ```
- **İnceleme & İyileştirme:**
  Dönen yanıt gerçekte `Result<GetPostListQueryResponse>` veya `Result<string>` olmasına rağmen Swagger özniteliğinde sadece non-generic `Result` tipi belirtilmiştir. Bu nedenle Swagger UI üzerinde endpoint'lerin döndüğü veri şemaları (Payload) tam olarak görüntülenememektedir.

---

## 7. 🧪 Test Kapsamındaki Eksiklikler

- **Konum:** `src/DailyDN.Tests/`
- **İnceleme & İyileştirme:**
  Auth feature'ları için güzel testler yazılmış olmasına rağmen:
  - `RedisCacheService` Polly Circuit Breaker / Fallback senaryoları,
  - `ErrorHandlerMiddleware` exception handling testleri,
  - `CorrelationIdMiddleware` testleri,
  - `UserService` ve `PostService` entegrasyon senaryoları test edilmemiştir.
