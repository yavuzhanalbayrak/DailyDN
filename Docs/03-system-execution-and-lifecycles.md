# 03. Sistem Yaşam Döngüsü ve Çalışma Sıraları (Lifecycles & Execution Flow) 🔄

DailyDN projesindeki isteklerin, kimlik doğrulama süreçlerinin, MediatR boru hattının ve hata yönetiminin hangi sıra ile çalıştığını bilmek, sistemin davranışını öngörmek ve hata ayıklamak (debug) için kritiktir.

Bu dokümanda sistemin tüm kritik çalışma sıraları ve akış diyagramları adım adım açıklanmıştır.

---

## 1. 🚀 Uygulama Başlatma (Application Boot / Startup) Sırası

Uygulama `Program.cs` üzerinden ayağa kalkarken aşağıdaki sıra ile yapılandırılır:

```mermaid
graph TD
    A[1. WebApplication.CreateBuilder] --> B[2. Serilog Entegrasyonu<br/>Configuration & Sinks]
    B --> C[3. DI Servis Tescilleri<br/>AddPresentation -> AddApplication -> AddInfrastructure]
    C --> D[4. Options & Settings Bindings<br/>Jwt, Smtp, FileStorage, Redis]
    D --> E[5. WebApplication.Build]
    E --> F[6. Authentication & Authorization Middleware]
    F --> G[7. Custom Middlewares<br/>CorrelationId -> ErrorHandler -> AuthenticatedUser]
    G --> H[8. Swagger / SwaggerUI - Dev Only]
    H --> I[9. MapControllers & MapHealthChecks]
    I --> J[10. app.RunAsync]
```

1. **Host & Serilog Kurulumu:** Serilog yapılandırması `appsettings.json`'dan okunur, Console, File ve Graylog UDP sink'leri aktif edilir.
2. **DI Servis Tescilleri:**
   - `AddPresentation()`: API versiyonlama, MediatR, JWT Bearer yetkilendirme şeması, DbContext, SignalR hub token okuyucu.
   - `AddApplication()`: AutoMapper profilleri, FluentValidation validator'ları, Domain servisleri (`AuthService`, `UserService`, `PostService`, `OtpService`), PasswordHasher.
   - `AddInfrastructure()`: Repository'ler, `UnitOfWork`, `TokenService`, `RedisCacheService`, `Polly AsyncPolicy`, `SmtpMailService`, `FileStorageService`, `FakeSmsProvider`.
3. **Konfigürasyon Bağlamaları (Options Pattern):** `JwtSettings`, `SmtpSettings`, `FileStorageSettings`, `RedisSettings` IOptions arayüzleri ile eşlenir.
4. **HTTP Pipeline Yapılandırması:** Middleware'ler tescil sırasına göre zincire eklenir.

---

## 2. 🌐 Uçtan Uca HTTP İstek Yaşam Döngüsü (Request Lifecycle)

İstemciden gelen korumalı bir HTTP POST isteğinin (örneğin `/api/v1/post` - `AddPostCommand`) sistem içerisindeki tam yolculuğu:

```mermaid
sequenceDiagram
    autonumber
    actor Client as İstemci / Mobil / Web
    participant CorM as CorrelationIdMiddleware
    participant ErrM as ErrorHandlerMiddleware
    participant AuthM as AuthenticatedUserMiddleware
    participant AuthFilter as ASP.NET Auth & [Authorized] Filter
    participant Ctrl as PostController
    participant Med as MediatR Pipeline
    participant LogB as LoggingBehavior
    participant ValB as ValidationBehavior
    participant Hndl as AddPostCommandHandler
    participant Srv as PostService
    participant UoW as UnitOfWork & DbContext
    participant DB as MSSQL Database

    Client->>CorM: HTTP POST /api/v1/post (Bearer Token + Payload)
    Note over CorM: X-Correlation-Id üretilir/okunur,<br/>LogContext'e CorrelationId basılır.
    CorM->>ErrM: next(context)
    Note over ErrM: try-catch bloğu açılır
    ErrM->>AuthM: next(context)
    Note over AuthM: JWT'den UserId, Role ve Claims<br/>Scoped IAuthenticatedUser nesnesine atanır.
    AuthM->>AuthFilter: ASP.NET Pipeline Kontrolü
    Note over AuthFilter: 1. Token geçerli mi?<br/>2. Kullanıcıda "PostAdd" izni var mı?
    AuthFilter->>Ctrl: Yetki onaylandı, Action tetiklenir
    Ctrl->>Med: _mediator.Send(command)
    Med->>LogB: Pipeline Adım 1: Logging
    Note over LogB: Request ismi ve [DoNotLog] hariç<br/>tüm parametreler Serilog'a yazılır.
    LogB->>ValB: Pipeline Adım 2: Validation
    Note over ValB: AddPostCommandValidator koşturulur.<br/>Hata varsa ValidationException fırlatılır!
    ValB->>Hndl: Pipeline Adım 3: Execution
    Hndl->>Srv: postService.AddAsync(post)
    Srv->>UoW: uow.Posts.AddAsync(post)
    Srv->>UoW: uow.SaveChangesAsync()
    Note over UoW: ChangeTracker tetiklenir:<br/>CreatedAt=Now, CreatedBy=UserId,<br/>Soft-Delete/Audit logları yazılır.
    UoW->>DB: INSERT INTO Posts (...)
    DB-->>UoW: Commit Success
    UoW-->>Srv: Success
    Srv-->>Hndl: Success
    Hndl-->>ValB: Result.Success()
    ValB-->>LogB: Result.Success()
    Note over LogB: "Handled AddPostCommand" logu yazılır.
    LogB-->>Med: Result.Success()
    Med-->>Ctrl: Result.Success()
    Ctrl-->>Client: HTTP 200 OK (Result JSON)
```

---

## 3. 🔐 Kimlik Doğrulama ve Güvenlik Akış Sıraları

### A. Kullanıcı Kaydı ve E-posta Doğrulama (Register & Email Verification)

```mermaid
sequenceDiagram
    autonumber
    actor User as Kullanıcı
    participant API as AuthController
    participant Srv as AuthService
    participant UoW as UnitOfWork
    participant Mail as SmtpMailService + Template
    participant DB as MSSQL

    User->>API: POST /api/v1/auth/register (Ad, Soyad, Email, Tel, Şifre)
    API->>Srv: RegisterAsync(...)
    Srv->>UoW: Email ve Telefon var mı kontrol et
    UoW->>DB: SELECT Users WHERE Email OR PhoneNumber
    DB-->>UoW: Kayıt Yok
    Note over Srv: Şifre Identity PasswordHasher ile hash'lenir.<br/>User nesnesi oluşturulur (IsEmailVerified=false).<br/>user.GenerateEmailVerificationToken() çağrılır.
    Srv->>Mail: VerifyEmailTemplate.html yükle + link oluştur ve gönder
    Mail-->>User: E-posta Gönderildi (Doğrulama Linki ile)
    Srv->>UoW: uow.Users.AddAsync(user) + SaveChangesAsync()
    UoW->>DB: INSERT INTO Users
    Srv-->>API: Result.SuccessWithMessage
    API-->>User: HTTP 200 (Kayıt başarılı, lütfen e-postanızı onaylayın)
    
    Note over User,DB: --- E-POSTA ONAY ADIMI ---
    User->>API: POST /api/v1/auth/verify-email (Guid Token)
    API->>Srv: VerifyEmailAsync(token)
    Srv->>UoW: Token'a sahip kullanıcıyı bul
    Note over Srv: user.VerifyEmailToken(token, 24 saat)<br/>IsEmailVerified = true<br/>Token sıfırlanır.
    Srv->>UoW: SaveChangesAsync()
    UoW->>DB: UPDATE Users SET IsEmailVerified=1
    Srv-->>API: Result.Success
    API-->>User: HTTP 200 (E-posta başarıyla doğrulandı)
```

---

### B. İki Adımlı Giriş (2FA) ve SMS OTP Akışı (Login & Verify OTP)

```mermaid
sequenceDiagram
    autonumber
    actor User as Kullanıcı
    participant API as AuthController
    participant Srv as AuthService
    participant OtpSrv as OtpService & SmsService
    participant TokSrv as TokenService
    participant UoW as UnitOfWork
    participant DB as MSSQL

    User->>API: POST /api/v1/auth/login (Email, Password)
    API->>Srv: LoginAsync(email, password)
    Srv->>UoW: Kullanıcıyı getir
    Note over Srv: 1. Kullanıcı var mı?<br/>2. IsEmailVerified == true mu?<br/>3. PasswordHasher.VerifyHashedPassword başarılı mı?
    Srv->>OtpSrv: CreateOtp() -> 6 haneli kod + OtpGuid
    Srv->>OtpSrv: smsService.SendSmsAsync(phone, otp)
    Note over Srv: user.SetOtp(code, guid)<br/>OtpGeneratedAt = UtcNow
    Srv->>UoW: SaveChangesAsync()
    Srv-->>API: Result.Success({ Guid, Otp })
    API-->>User: HTTP 200 { Guid, Otp } (SMS gönderildi)

    Note over User,DB: --- 2. ADIM: OTP DOĞRULAMA ---
    User->>API: POST /api/v1/auth/otp/verify (Guid, Otp)
    API->>Srv: VerifyOtpAsync(guid, otp)
    Srv->>UoW: Guid'e göre kullanıcıyı bul
    Note over Srv: user.IsOtpValid(otp, 1 dakika)<br/>Süre dolmamış ve kod doğru ise devam et.
    Srv->>TokSrv: GenerateTokens(userId, ip, userAgent)
    Note over TokSrv: 1. Kullanıcının Rol ve Claim'lerini çek.<br/>2. JWT Access Token üret (24 saat).<br/>3. Rastgele Refresh Token üret ve SHA-256 ile hashle.<br/>4. UserSessions tablosuna yeni aktif oturum ekle.
    TokSrv->>UoW: UserSessions.AddAsync(...) + SaveChangesAsync()
    Note over Srv: user.Login() -> LastLoginAt güncellenir, IsGuidUsed=true.
    Srv->>UoW: SaveChangesAsync()
    Srv-->>API: TokenResponse (AccessToken, RefreshToken, Expiry)
    API-->>User: HTTP 200 (JWT ve Refresh Token)
```

---

### C. Güvenli Token Rotasyonu (Refresh Token Rotation Flow)

```mermaid
sequenceDiagram
    autonumber
    actor Client as İstemci (Web / Mobil)
    participant API as AuthController
    participant Srv as AuthService
    participant TokSrv as TokenService
    participant UoW as UnitOfWork
    participant DB as MSSQL

    Client->>API: POST /api/v1/auth/refresh-token (RefreshToken)
    API->>Srv: RefreshTokenAsync(refreshToken)
    Note over Srv: Gelen Refresh Token SHA-256 ile hash'lenir.
    Srv->>UoW: UserSessions.FirstOrDefaultAsync(hash)
    Note over Srv: Oturum var mı ve IsActive() (IsRevoked==false && Süresi Dolmamış) mı?
    alt Oturum Geçersiz veya İptal Edilmiş (Revoked)
        Srv-->>API: null / Error("Invalid refresh token")
        API-->>Client: HTTP 400 Bad Request
    else Oturum Geçerli
        Srv->>TokSrv: GenerateTokens(session.UserId, session.IpAddress, session.UserAgent)
        Note over TokSrv: 1. Yeni JWT Access Token üretilir.<br/>2. Yeni Refresh Token üretilir ve hash'lenir.<br/>3. Yeni UserSession kaydı oluşturulur.
        Note over Srv: session.Revoke() -> Eski oturum iptal edilir (Token Reuse engellenir!).
        Srv->>UoW: UserSessions.AddAsync(newSession) + UpdateAsync(oldSession)
        Srv->>UoW: SaveChangesAsync()
        Srv-->>API: TokenResponse (Yeni AccessToken + Yeni RefreshToken)
        API-->>Client: HTTP 200 OK
    end
```

---

## 4. 🗃️ Veritabanı Kayıt ve Denetim (Audit & Soft-Delete) Sırası

Herhangi bir servis veya handler `uow.SaveChangesAsync()` çağırdığında `ApplicationContext` içinde çalışan mekanizma:

```mermaid
graph TD
    A[uow.SaveChangesAsync tetiklendi] --> B[ChangeTracker Entries Taranır]
    B --> C{Entry State Nedir?}
    
    C -->|Added| D[CreatedAt = DateTime.UtcNow<br/>CreatedBy = CurrentUser.UserId<br/>Serilog Info Log Yazılır]
    C -->|Modified| E[UpdatedAt = DateTime.UtcNow<br/>UpdatedBy = CurrentUser.UserId<br/>Serilog Info Log Yazılır]
    C -->|Deleted| F[State = EntityState.Modified'a çevrilir<br/>IsDeleted = true<br/>UpdatedAt = UtcNow, UpdatedBy = UserId<br/>Serilog Soft-Delete Log Yazılır]
    
    D --> G[base.SaveChangesAsync]
    E --> G
    F --> G
    G --> H[Veritabanına SQL Olarak Gönderilir]
```

---

## 5. ⚠️ Hata Yakalama ve Yanıt Dönüşüm Sırası (Error Handling)

Uygulamanın herhangi bir yerinde hata fırlatıldığında `ErrorHandlerMiddleware` tarafından devreye giren sıralama:

```mermaid
graph TD
    Ex[Exception Oluştu] --> EM[ErrorHandlerMiddleware catch bloğu]
    EM --> Type{Exception Tipi Nedir?}

    Type -->|ValidationException| V[HTTP 400 Bad Request<br/>ValidationProblemDetails JSON Formatı]
    Type -->|ApiAuthenticationException| A[HTTP 401 / 403<br/>FailCode ve Hata Mesajı ile Result.Failure JSON]
    Type -->|AuthorizationException| Z[HTTP 403 Forbidden<br/>Result.Failure JSON]
    Type -->|Beklenmeyen Exception| G[Serilog Error Log Yazılır<br/>HTTP 500 Internal Server Error<br/>'An unexpected error occurred' Maskeli Mesaj]
```
