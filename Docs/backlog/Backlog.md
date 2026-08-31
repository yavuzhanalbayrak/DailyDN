# Proje Backlog & Görev Takibi 📋

Bu doküman, DailyDN projesinde aktif olarak yürütülen ve sırada bekleyen görevlerin durumunu takip ederek **bağlam kaybını (context loss)** önlemek amacıyla kullanılır.

> 💡 **Arşivleme Kuralı:** Backlog'un şişmesini engellemek için tamamlanan görevler periyodik olarak [`Docs/backlog/archives/`](file:///c:/Users/Kutbay/Desktop/Muhammed/DailyDN/Docs/backlog/archives/README.md) altına taşınır. Geçmişe dönük inceleme yapılırken en son arşivden ilkine doğru gidilir.

---

## ⏳ Aktif / Yarım Kalan Görevler (In Progress)
<!-- Agent veya geliştirici işi yarıda bıraktığında veya oturum kapandığında buraya durum notu düşülür -->

- *(Şu anda aktif/yarım kalan görev bulunmamaktadır. Tüm kritik buglar başarıyla tamamlanmıştır.)*

---

## 📌 Sırada Bekleyen Görevler (Next Sprint / Priority Backlog)
<!-- Bir sonraki aşamada ele alınacak araştırma ve iyileştirme maddeleri -->

- [ ] **Görev Adı: [SEC-01] `AuthService.LoginAsync` OTP Response Sızıntısının Önlenmesi**
  - **Kategori:** 🔴 Güvenlik (2FA Bypass Riski)
  - **Mevcut Durum:** Beklemede.
  - **Tamamlanması Gerekenler:** `LoginAsync` sonucunda dönen `Otp` alanı prodüksiyon ortamı için gizlenecek/kaldırılacak (`#if DEBUG` veya configuration flag).
  - **Bağlı Dosyalar:** `src/DailyDN.Application/Services/Implementations/AuthService.cs`

- [ ] **Görev Adı: [SEC-02] `FileStorageService.cs` Güvenlik İyileştirmeleri**
  - **Kategori:** 🔴 Güvenlik (Arbitrary File Upload / Path Traversal)
  - **Mevcut Durum:** Beklemede.
  - **Tamamlanması Gerekenler:** Dosya uzantısı whitelist kontrolü, `Path.GetFullPath` doğrulama ve `BaseUrl` HTTP desteği eklenecek.
  - **Bağlı Dosyalar:** `src/DailyDN.Infrastructure/Services/Impl/FileStorageService.cs`

- [ ] **Görev Adı: [ARCH-02] `UserService.cs` Detached Domain Entity Redis Caching Düzeltmesi**
  - **Kategori:** 🟡 Mimari (Cache-Aside / Entity Isolation)
  - **Mevcut Durum:** Beklemede.
  - **Tamamlanması Gerekenler:** Redis'ten okunan DTO'nun doğrudan Domain Entity'ye dönüştürülmesi yerine API katmanına DTO/Response iletilmesi sağlanacak.
  - **Bağlı Dosyalar:** `src/DailyDN.Application/Services/Implementations/UserService.cs`

- [ ] **Görev Adı: [ARCH-03] Polly Circuit Breaker Yapılandırmasının İyileştirilmesi**
  - **Kategori:** 🟡 Mimari (Resilience / Performance)
  - **Mevcut Durum:** Beklemede.
  - **Tamamlanması Gerekenler:** 1 hata ile 15 dk devreyi açan agresif kural, 3-5 hata eşiği ve 30s-1dk dinlenme süresine revize edilecek.
  - **Bağlı Dosyalar:** `src/DailyDN.Infrastructure/ServiceCollectionExtensions.cs`

---

## 🗄️ Tamamlanan Görevler Arşivi (Changelog Archives)
<!-- Backlog'u hafif tutmak için tamamlanan paketler arşiv dosyalarına taşınır (En yeniden en eskiye sıralı) -->

- 📦 **[2026-08-31 03:45:00 UTC+3] Kritik Hatalar ve Runtime Düzeltmeleri (7 Görev)**: [`archives/2026-08-31_03-45_critical-bugs-and-runtime-fixes.md`](file:///c:/Users/Kutbay/Desktop/Muhammed/DailyDN/Docs/backlog/archives/2026-08-31_03-45_critical-bugs-and-runtime-fixes.md)
  - `BUG-01`: VerifyOtpAsync OutOfRange fix
  - `BUG-02`: ApplicationContext CurrentUser Audit fix
  - `BUG-03`: DailyDNDbContext missing DbSets fix
  - `BUG-04`: UserService Profile photo cache invalidation fix
  - `BUG-05`: GenericRepository AsNoTracking fix
  - `BUG-06`: AuthService VerifyEmailAsync exception handling fix
  - `ARCH-01`: Program.cs Middleware pipeline ordering fix
