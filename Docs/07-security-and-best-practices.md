# 07. Güvenlik, Hata Yönetimi ve Best Practices 🔐

DailyDN mimarisi, OWASP standartları ve kurumsal güvenlik pratikleri göz önüne alınarak tasarlanmıştır. Bu dokümanda projedeki güvenlik katmanları, hassas veri koruma teknikleri ve kodlama prensipleri açıklanmaktadır.

---

## 🛡️ 1. Claim-Based (İzin Tabanlı) Yetkilendirme

Geleneksel rol tabanlı (`[Authorize(Roles = "Admin")]`) kontrol yerine sistemde **İnce Taneli İzin Tabanlı (Fine-Grained Claim-Based)** yetkilendirme uygulanmıştır.

### Neden Bu Yaklaşım?
Roller sabit olabilir ancak bir role hangi izinlerin verildiği dinamik olarak değişebilir. `[Authorized("PostAdd")]` filtresi, kullanıcının rolünden bağımsız olarak o eylemi yapma yetkisinin olup olmadığını denetler.

```csharp
[HttpPost]
[Authorized("PostAdd")] // Claim kontrolü
public async Task<IActionResult> Add([FromBody] AddPostCommand command) { ... }
```

### Yetkilendirme Akışı:
1. İstemci JWT Access Token gönderir.
2. `AuthenticatedUserMiddleware` JWT içerisindeki `Permissions` tipindeki claim'leri okur ve Scoped `IAuthenticatedUser.Claims` listesine atar.
3. `AuthorizedAttribute` bu listede `requiredClaim` ("PostAdd") olup olmadığını kontrol eder.
4. Yetki yoksa `403 Forbidden` (`FailCode.InvalidClaim`) istisnası fırlatılır.

---

## 🔑 2. Şifre Güvenliği ve Hashleme

- Kullanıcı şifreleri veritabanında asla düz metin (plain-text) saklanmaz.
- Microsoft ASP.NET Core Identity'nin `IPasswordHasher<User>` servisi kullanılır.
- Bu algoritma arkada **PBKDF2 (HMAC-SHA256)** ile rastgele üretilen tuz (salt) değerini kullanarak şifreyi 10.000+ iterasyonla hash'ler.
- `VerifyHashedPassword` fonksiyonu sabit zamanlı (constant-time) karşılaştırma yaparak Zamanlama Saldırılarını (Timing Attacks) engeller.

---

## 🔄 3. Güvenli Token Rotasyonu ve Yeniden Kullanım Koruması (Anti-Replay)

Refresh token mekanizmasında karşılaşılan en büyük risk çalınan token'ların süresiz kullanılmasıdır. DailyDN bu riski şu şekilde çözer:

1. **Hash'leyerek Saklama:** İstemciye düz metin token verilir; veritabanında `SHA-256` ile hash'lenmiş hali (`RefreshTokenHash`) tutulur. Veritabanı sızsa bile token'lar kullanılamaz.
2. **Tek Kullanımlık Token (Token Rotation):** Bir refresh token kullanıldığı anda `session.Revoke()` ile iptal edilir ve hemen yeni bir token çifti üretilir.
3. **Cihaz / IP Bağlama:** Her oturum açılışında IP adresi ve `User-Agent` bilgisi kaydedilir.

---

## 🙈 4. Hassas Veri Maskeleme (`[DoNotLog]` Niteliği)

Loglama sırasında kullanıcıların şifreleri, kredi kartı veya OTP kodları gibi kritik verilerin log dosyalarına veya Graylog'a sızması GDPR/KVKK açısından büyük bir suçtur.

DailyDN bu durumu MediatR `LoggingBehavior` içinde `[DoNotLog]` niteliği ile çözer:

```csharp
public record LoginCommand(
    string Email,
    [property: DoNotLog] string Password // Loglanmaz!
) : ICommand<Result>;
```

`LoggingBehavior` reflection ile nesnenin özelliklerini okurken `[DoNotLog]` etiketine sahip alanları otomatik olarak log payload'undan çıkartır.

---

## 🧱 5. Hata Maskeleme ve Bilgi Sızıntısının Önlenmesi

Saldırganlar veritabanı yapısını veya sunucu teknolojisini anlamak için hata mesajlarını (Stack Trace) tetiklemeye çalışır.

`ErrorHandlerMiddleware` ile:
- `ValidationException` -> Hangi input'un neden geçersiz olduğu açıkça belirtilir (`400 Bad Request`).
- `ApiAuthenticationException` -> Standart `FailCode` dönülür (`401/403`).
- **Beklenmeyen Sistem Hataları (NullReference, SqlException vb.):**
  - Hatanın tam Stack Trace'i yalnızca sunucu tarafında Serilog/Graylog'a yazılır.
  - İstemciye dönen yanıt:
    ```json
    {
      "StatusCode": 500,
      "Message": "An unexpected error occurred. Please contact support."
    }
    ```

---

## ⚡ 6. Dayanıklılık ve Hizmet Kesintisi Önleme (Polly Circuit Breaker)

Redis önbellek sunucusu çöktüğünde veya aşırı yüklendiğinde, arka arkaya gelen yüzlerce istek veritabanını kilitleyebilir (Cascading Failure).

`RedisCacheService` üzerinde tanımlı Polly politikası:
- Hata durumunda 2 kez dener (Retry).
- Hata devam ederse devreyi açar (Circuit Breaker) ve Redis'i 15 dakika dinlendirir.
- İstekleri bekletmeden doğrudan veritabanına yönlendirir (Fallback - Cache Bypass).

---

## 🧵 7. Dağıtık İzleme (Correlation ID)

Her HTTP isteğine `CorrelationIdMiddleware` tarafından benzersiz bir `X-Correlation-Id` atanır:
- İstemci bu ID ile kendi isteğini takip edebilir.
- LogContext'e push edildiği için Console, Dosya ve Graylog üzerindeki binlerce log satırı arasında tek bir isteğin tüm adımları saniyeler içinde filtrelenebilir.

---

## 🏆 8. Kodlama Standartları ve Prensipler

- **Rich Domain Model:** Entity'ler içindeki setter'lar `private` tutulmuş; durum değişiklikleri (`UpdateName()`, `SetOtp()`, `VerifyEmailToken()`) Domain metodları üzerinden kapsüllenmiştir (Encapsulation).
- **Result Pattern:** Metodlar kontrolsüz exception fırlatmak yerine başarı/başarısızlık durumlarını `Result`, `Result<T>` nesneleri ile açıkça döner.
- **Fail-Fast Doğrulama:** MediatR `ValidationBehavior` sayesinde geçersiz istekler handler'a veya veritabanına ulaşmadan anında reddedilir.
- **Tek Sorumluluk Prensibi (SRP):** CQRS yapısıyla her Command ve Query yalnızca tek bir işi yapar.
