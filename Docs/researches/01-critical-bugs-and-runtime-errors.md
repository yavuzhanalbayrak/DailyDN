# 01. Kritik Hatalar ve Çalışma Zamanı Riskleri (Critical Bugs & Runtime Errors) 🔴

Bu raporda, çalışma zamanında (runtime) istisna (`Exception`) fırlatan, veri kaybına veya tutarsızlığa yol açan kritik kodlama hataları incelenmiştir.

---

## 1. 💥 `AuthService.VerifyOtpAsync` - Dizi Sınır Aşımı (`ArgumentOutOfRangeException`)

- **Konum:** `src/DailyDN.Application/Services/Implementations/AuthService.cs` (Satır 124-125)
- **Mevcut Kod:**
  ```csharp
  var userList = await uow.Users.GetAsync(u => u.Guid == guid);
  var user = userList[0]; // ⚠️ Liste boşsa CRASH!
  ```
- **Hata Analizi:**
  İstemci geçersiz, süresi dolmuş veya veritabanında olmayan bir `guid` ile `/api/v1/auth/otp/verify` endpoint'ine istek attığında `userList` boş (`Count == 0`) döner. `userList[0]` doğrudan ilk elemana erişmeye çalıştığı için `ArgumentOutOfRangeException` fırlatılır ve istemciye anlamsız bir `500 Internal Server Error` döner.
- **Önerilen Çözüm:**
  ```csharp
  var user = await uow.Users.FirstOrDefaultAsync(u => u.Guid == guid);
  if (user is null)
  {
      return null; // veya Result.Failure(new Error("InvalidGuid", "OTP request not found or expired."))
  }
  ```

---

## 2. 🕳️ `ApplicationContext.cs` - Audit Loglarında `CreatedBy` / `UpdatedBy` Değerinin `0` Kalması

- **Konum:** `src/DailyDN.Infrastructure/Contexts/ApplicationContext.cs` (Satır 12)
- **Mevcut Kod:**
  ```csharp
  public abstract class ApplicationContext(DbContextOptions options, ILogger<DailyDNDbContext> logger, IAuthenticatedUser currentUser) : DbContext(options), IApplicationContext
  {
      private readonly ILogger<DailyDNDbContext> _logger = logger;
      // ⚠️ DİKKAT: Constructor anında int olarak kopyalanıyor!
      private readonly int _currentUser = currentUser.UserId; 
  ```
- **Hata Analizi:**
  `IAuthenticatedUser` nesnesi Scoped bir servistir ve içindeki `UserId` değeri `AuthenticatedUserMiddleware` çalıştıktan sonra atanır. Eğer DbContext bu middleware'den önce çözümlenirse (örneğin DI zincirinde erken tetiklenirse), `currentUser.UserId` henüz `0` iken `_currentUser` primitive `int` değişkenine kopyalanır. Sonrasında kullanıcı authenticate olsa bile `_currentUser` alanı `0` kaldığı için tüm veritabanı kayıtlarında `CreatedBy = 0` ve `UpdatedBy = 0` yazılır!
- **Önerilen Çözüm:**
  `int` değerini constructor'da kopyalamak yerine `IAuthenticatedUser` referansını tutmalı ve `ApplyAuditInfo()` içinde dinamik olarak çağırmalısınız:
  ```csharp
  private readonly IAuthenticatedUser _currentUser = currentUser;

  // ApplyAuditInfo() içinde:
  entity.CreatedBy = _currentUser.UserId;
  entity.UpdatedBy = _currentUser.UserId;
  ```

---

## 3. 🧩 `DailyDNDbContext` - Eksik DbSet Tanımlamaları

- **Konum:** `src/DailyDN.Infrastructure/Contexts/DailyDNDbContext.cs` (Satır 11-16)
- **Mevcut Kod:**
  ```csharp
  public DbSet<User> Users { get; set; }
  public DbSet<Claim> Claims { get; set; }
  public DbSet<Role> Roles { get; set; }
  public DbSet<RoleClaim> RoleClaims { get; set; }
  public DbSet<UserRole> UserRoles { get; set; }
  public DbSet<UserSession> UserSessions { get; set; }
  // ⚠️ Post, Chat, ChatMessage, UserChat DbSet'leri YOK!
  ```
- **Hata Analizi:**
  Projede `Post`, `Chat`, `ChatMessage` ve `UserChat` entity'leri, konfigürasyonları ve repository'leri oluşturulmuş olmasına rağmen `DailyDNDbContext` içinde bu tablolara ait `DbSet<T>` özellikleri tanımlanmamıştır. Bu durum geliştiricilerin `_context.Posts` veya `_context.Chats` şeklinde LINQ yazmasını engeller; `_context.Set<Post>()` kullanmak zorunda bırakır.
- **Önerilen Çözüm:**
  ```csharp
  public DbSet<Post> Posts { get; set; }
  public DbSet<Chat> Chats { get; set; }
  public DbSet<ChatMessage> ChatMessages { get; set; }
  public DbSet<UserChat> UserChats { get; set; }
  ```

---

## 4. 🔄 `UserService.cs` - Cache Invalidation (Önbellek Bayatlaması) Eksikliği

- **Konum:** `src/DailyDN.Application/Services/Implementations/UserService.cs` (Satır 55-74)
- **Mevcut Kod:**
  ```csharp
  public async Task<Result<string>> UpdateProfilePhoto(IFormFile file)
  {
      // ...
      user.SetAvatar(photoUrl);
      await uow.Users.UpdateAsync(user);
      await uow.SaveChangesAsync();

      // ⚠️ Redis'teki "user:{id}" önbelleği SİLİNMİYOR!
      return Result.Success(photoUrl);
  }
  ```
- **Hata Analizi:**
  Kullanıcı profil fotoğrafını güncellediğinde veritabanı başarıyla güncellenir. Ancak Redis üzerindeki `user:{userId}` anahtarı silinmez veya güncellenmez. Kullanıcı daha sonra `GetByIdAsync()` çağırdığında sistem Redis'ten 30 dakika boyunca eski profil verisini (eski avatar URL'sini) okur. Kullanıcı fotoğrafının değişmediğini zanneder.
- **Önerilen Çözüm:**
  ```csharp
  await uow.Users.UpdateAsync(user);
  await uow.SaveChangesAsync();
  await redis.RemoveAsync($"{CacheKeyPrefix}{userId}"); // Önbelleği temizle!
  ```

---

## 5. 🔍 `GenericRepository.cs` - `disableTracking` Parametresinin İlk Overload'da Yok Sayılması

- **Konum:** `src/DailyDN.Infrastructure/Repositories/GenericRepository.cs` (Satır 33-34)
- **Mevcut Kod:**
  ```csharp
  public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPaginatedAsync(
      int page,
      int pageSize,
      Expression<Func<T, bool>>? predicate = null,
      string? includeString = null,
      bool disableTracking = true
  )
  {
      IQueryable<T> query = _dbSet;

      if (disableTracking)
          query = _dbSet.AsQueryable(); // ⚠️ AsNoTracking() ÇAĞRILMAMIŞ!
  ```
- **Hata Analizi:**
  Metot imzasında `bool disableTracking = true` parametresi bulunmasına rağmen, koşul bloğunda `query = query.AsNoTracking();` yerine yanlışlıkla `query = _dbSet.AsQueryable();` yazılmıştır. Bu nedenle bu metot çağrıldığında EF Core ChangeTracker nesneleri izlemeye devam eder; bellek tüketimi artar ve performans kaybı oluşur.
- **Önerilen Çözüm:**
  ```csharp
  if (disableTracking)
      query = query.AsNoTracking();
  ```

---

## 6. ⚠️ `AuthService.VerifyEmailAsync` - Syntax Lekesi ve Geniş `catch` Yakalama

- **Konum:** `src/DailyDN.Application/Services/Implementations/AuthService.cs` (Satır 216-222)
- **Mevcut Kod:**
  ```csharp
  await uow.Users.UpdateAsync(user); ; // ⚠️ Çift noktalı virgül
  await uow.SaveChangesAsync();
  return Result.SuccessWithMessage("Email verified successfully.");
  catch (Exception)
  {
      return Result.Failure(new Error("Conflict", "Invalid verification token."));
  }
  ```
- **Hata Analizi:**
  `catch (Exception)` bloğu veritabanı bağlantı kopması (`SqlException`), timeout veya sunucu çökmeleri gibi tüm hataları yutarak kullanıcıya her durumda "Invalid verification token" (Geçersiz token) mesajı dönmektedir. Gerçek bir altyapı hatası durumunda hatanın kök sebebi maskelenmektedir.
- **Önerilen Çözüm:**
  Yalnızca `InvalidOperationException` (Domain kural hatası) yakalanmalı, diğer altyapı hataları `ErrorHandlerMiddleware`'e bırakılmalıdır.
