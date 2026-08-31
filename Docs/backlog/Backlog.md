# Proje Backlog & Görev Takibi 📋

Bu doküman, DailyDN projesinde aktif olarak yürütülen ve sırada bekleyen görevlerin durumunu takip ederek **bağlam kaybını (context loss)** önlemek amacıyla kullanılır.

> 💡 **Arşivleme Kuralı:** Backlog'un şişmesini engellemek için tamamlanan görevler periyodik olarak [`Docs/backlog/archives/`](file:///c:/Users/Kutbay/Desktop/Muhammed/DailyDN/Docs/backlog/archives/README.md) altına taşınır. Geçmişe dönük inceleme yapılırken en son arşivden ilkine doğru gidilir.

---

## ⏳ Aktif / Yarım Kalan Görevler (In Progress)
<!-- Agent veya geliştirici işi yarıda bıraktığında veya oturum kapandığında buraya durum notu düşülür -->

- *(Şu anda aktif/yarım kalan görev bulunmamaktadır. Kritik buglar, güvenlik zafiyetleri ve mimari anti-pattern'ler başarıyla tamamlanmıştır.)*

---

## 📌 Sırada Bekleyen Görevler (Next Sprint: Kod Kalitesi ve Refactoring)
<!-- Docs/researches/04-code-smells-and-quality-improvements.md kapsamındaki maddeler -->

- [ ] **Görev Adı: [QUALITY-01] `BaseResult.cs` & `Result.cs` Başarılı Durumda Null Error Dönülmesi**
  - **Kategori:** 🟢 Kod Kalitesi (Standardizasyon)
  - **Mevcut Durum:** Beklemede (Kullanıcı talimatı bekleniyor).
  - **Bağlı Dosyalar:** `src/DailyDN.Application/Common/Model/BaseResult.cs`

- [ ] **Görev Adı: [QUALITY-02] `ApiResponseFactory.cs` Kullanılmayan Dead Code Temizliği**
  - **Kategori:** 🟢 Kod Kalitesi (Dead Code Removal)
  - **Mevcut Durum:** Beklemede.
  - **Bağlı Dosyalar:** `src/DailyDN.API/Common/ApiResponseFactory.cs`

- [ ] **Görev Adı: [QUALITY-03] `Entity.cs` Protected Parameterless Constructor Eksikliği**
  - **Kategori:** 🟢 Kod Kalitesi (EF Core Best Practices)
  - **Mevcut Durum:** Beklemede.
  - **Bağlı Dosyalar:** `src/DailyDN.Domain/Entities/Entity.cs`

- [ ] **Görev Adı: [QUALITY-04] `GetListPostQueryHandler.cs` Async Await Kullanımı**
  - **Kategori:** 🟢 Kod Kalitesi (Async Best Practices)
  - **Mevcut Durum:** Beklemede.
  - **Bağlı Dosyalar:** `src/DailyDN.Application/Features/Posts/GetList/GetListPostQueryHandler.cs`

---

## 🗄️ Tamamlanan Görevler Arşivi (Changelog Archives)
<!-- En yeniden en eskiye (Ters Kronolojik) sıralı arşiv paketleri -->

- 📦 **[2026-08-31 03:55:00 UTC+3] Güvenlik & Mimari Düzeltmeleri (7 Görev)**: [`archives/2026-08-31_03-55_security-and-architecture-fixes.md`](file:///c:/Users/Kutbay/Desktop/Muhammed/DailyDN/Docs/backlog/archives/2026-08-31_03-55_security-and-architecture-fixes.md)
  - `SEC-01`: AuthService Login OTP sızıntısı önleme (`#if DEBUG`)
  - `SEC-02 & SEC-03`: FileStorageService Whitelist uzantı, Path Traversal önleme ve URL desteği
  - `SEC-04`: AuthorizedAttribute DI ve 401/403 ayrımı
  - `SEC-05`: Program.cs CORS yapılandırması
  - `ARCH-02`: Polly Circuit Breaker eşik optimizasyonu (3 hata / 30s)
  - `ARCH-03`: LoggingBehavior reflection property cache (`ConcurrentDictionary`)
  - `ARCH-04`: AuthService RefreshTokenAsync -> TokenService RotateRefreshToken DRY entegrasyonu

- 📦 **[2026-08-31 03:45:00 UTC+3] Kritik Hatalar ve Runtime Düzeltmeleri (7 Görev)**: [`archives/2026-08-31_03-45_critical-bugs-and-runtime-fixes.md`](file:///c:/Users/Kutbay/Desktop/Muhammed/DailyDN/Docs/backlog/archives/2026-08-31_03-45_critical-bugs-and-runtime-fixes.md)
  - `BUG-01`: VerifyOtpAsync OutOfRange fix
  - `BUG-02`: ApplicationContext CurrentUser Audit fix
  - `BUG-03`: DailyDNDbContext missing DbSets fix
  - `BUG-04`: UserService Profile photo cache invalidation fix
  - `BUG-05`: GenericRepository AsNoTracking fix
  - `BUG-06`: AuthService VerifyEmailAsync exception handling fix
  - `ARCH-01`: Program.cs Middleware pipeline ordering fix
