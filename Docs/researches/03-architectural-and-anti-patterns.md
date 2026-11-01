# 03. Mimari Yanlış Kullanımlar ve Anti-Pattern'ler (Architectural & Anti-Patterns) 🟡

Bu raporda, Clean Architecture, CQRS, Middleware boru hattı ve mikroservis tasarım prensiplerine uymayan yapısal anti-pattern'ler incelenmiştir.

---

## 1. 🔀 `Program.cs` - Hatalı Middleware Sıralaması ve Mükerrer `UseAuthorization`

- **Konum:** `src/DailyDN.API/Program.cs` (Satır 35-51)
- **Mevcut Kod:**
  ```csharp
  app.UseAuthentication();
  app.UseAuthorization(); // ⚠️ 1. ÇAĞRI (Henüz AuthenticatedUserMiddleware çalışmadı!)

  app.UseMiddleware<CorrelationIdMiddleware>(); // ⚠️ Geç çağrıldı
  app.UseMiddleware<ErrorHandlerMiddleware>();  // ⚠️ Authentication hatalarını yakalayamaz
  app.UseMiddleware<AuthenticatedUserMiddleware>();

  if (app.Environment.IsDevelopment())
  {
      app.UseSwagger();
      app.UseSwaggerUI();
  }

  app.UseHttpsRedirection();

  app.UseAuthorization(); // ⚠️ 2. ÇAĞRI (MÜKERRER!)
  ```
- **Anti-Pattern Analizi:**
  1. `CorrelationIdMiddleware` ve `ErrorHandlerMiddleware` zincirin en başında olmalıdır. Aksi takdirde `UseAuthentication` veya `UseHttpsRedirection` sırasında oluşabilecek hatalar yakalanamaz ve Correlation ID loglanamaz.
  2. `app.UseAuthorization()` 2 kez çağrılmıştır.
  3. `AuthenticatedUserMiddleware`, `UseAuthentication`'dan sonra ama `UseAuthorization`'dan önce çalışmalıdır.
- **Doğru Middleware Sıralaması:**
  ```csharp
  app.UseMiddleware<CorrelationIdMiddleware>(); // 1. En başta
  app.UseMiddleware<ErrorHandlerMiddleware>();  // 2. Global try-catch

  app.UseHttpsRedirection();
  app.UseAuthentication();                      // 3. Token'ı ClaimsPrincipal'a çevir
  app.UseMiddleware<AuthenticatedUserMiddleware>(); // 4. Scoped IAuthenticatedUser'a aktar
  app.UseAuthorization();                       // 5. Yetki kontrolü

  app.MapControllers();
  ```

---

## 2. 🧊 Detached Domain Entity'nin Redis'ten Map Edilmesi Anti-Pattern'i

- **Konum:** `src/DailyDN.Application/Services/Implementations/UserService.cs` (Satır 28-34)
- **Mevcut Kod:**
  ```csharp
  var cachedUser = await redis.GetAsync<RedisUserDto>(cacheKey);
  if (cachedUser is not null)
  {
      var response = mapper.Map<User>(cachedUser); // ⚠️ DTO -> Entity Mapping!
      return response;
  }
  ```
- **Anti-Pattern Analizi:**
  Redis'ten okunan DTO nesnesi `mapper.Map<User>` ile bir Domain Entity'ye dönüştürülmektedir. Bu şekilde üretilen `User` nesnesi **EF Core ChangeTracker tarafından izlenmeyen (Detached)** sahte bir entity'dir. Eğer bu metodu çağıran üst katman bu entity üzerinde bir değişiklik yapıp `uow.Users.UpdateAsync(user)` çağırmaya kalkarsa EF Core'da `DbUpdateConcurrencyException` veya ilişki kopmaları (detached graph issues) yaşanır.
- **Doğru Yaklaşım:**
  Servisler dışarıya doğrudan `User` (Entity) yerine amaca özel `UserDto` veya `GetUserResponse` nesneleri dönmeli; Entity sadece veritabanı transaction sınırları içerisinde yönetilmelidir.

---

## 3. ⚡ Polly Circuit Breaker Aşırı Agresif Yapılandırması

- **Konum:** `src/DailyDN.Infrastructure/ServiceCollectionExtensions.cs` (Satır 45-52)
- **Mevcut Kod:**
  ```csharp
  var circuitBreakerPolicy = Policy
      .Handle<RedisConnectionException>()
      .Or<RedisTimeoutException>()
      .Or<RedisServerException>()
      .CircuitBreakerAsync(1, TimeSpan.FromMinutes(15), // ⚠️ 1 hata ile 15 DAKİKA BLOK!
          onBreak: (ex, _) => logger.LogError(ex, "Circuit opened"),
          onReset: () => logger.LogInformation("Circuit closed"),
          onHalfOpen: () => logger.LogWarning("Circuit half-open"));
  ```
- **Anti-Pattern Analizi:**
  Tek bir anlık ağ gecikmesi veya Redis timeout'unda (`exceptionsAllowedBeforeBreaking: 1`), sistem devreyi hemen açmakta ve **15 dakika boyunca** Redis'e hiç uğramadan önbelleği tamamen kapatmaktadır. Bu durum veritabanına ani ve devasa bir sorgu yükünün (Cache Stampede) binmesine yol açar.
- **Önerilen Çözüm:**
  - İzin verilen hata eşiği: En az **3 veya 5 ardışık hata**.
  - Devre açık kalma süresi (Break duration): **30 saniye ile 1 dakika** arası olmalıdır.

---

## 4. 🔁 Mükerrer ve Kullanılmayan Token Rotasyon Kodu

- **Konum:**
  - `DailyDN.Infrastructure/Services/Impl/TokenService.cs` -> `RotateRefreshToken()`
  - `DailyDN.Application/Services/Implementations/AuthService.cs` -> `RefreshTokenAsync()`
- **Anti-Pattern Analizi:**
  `TokenService` içerisinde mükemmel bir şekilde yazılmış `RotateRefreshToken` metodu bulunmasına rağmen, `AuthService.RefreshTokenAsync` bu metodu çağırmamaktadır. Bunun yerine oturum bulma, hashleme, token üretme ve revoke etme kodlarını kendi içinde elle tekrar yazmıştır. Bu durum **DRY (Don't Repeat Yourself)** prensibini ihlal etmekte ve bakım maliyetini artırmaktadır.

---

## 5. 🐢 `LoggingBehavior.cs` İçinde Her İstekte Reflection Taraması

- **Konum:** `src/DailyDN.Application/Behaviors/LoggingBehavior.cs` (Satır 23-26)
- **Mevcut Kod:**
  ```csharp
  var loggableProperties = request.GetType()
      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
      .Where(p => p.GetCustomAttribute<DoNotLogAttribute>() == null)
      .ToDictionary(p => p.Name, p => p.GetValue(request));
  ```
- **Anti-Pattern Analizi:**
  Saniyede binlerce isteğin geldiği yoğun bir API'de her istek için `GetType().GetProperties()` ve `GetCustomAttribute` reflection çağrısı yapmak CPU ve bellek üzerinde ciddi bir ek yük oluşturur.
- **Önerilen Çözüm:**
  Hangi property'lerin loglanabilir olduğu bilgisi tip bazında `ConcurrentDictionary<Type, PropertyInfo[]>` gibi bir static cache içinde bir kez önbelleğe alınmalıdır.
