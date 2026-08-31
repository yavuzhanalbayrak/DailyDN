# DailyDN - AI Agent Çalışma Standartları ve Davranış İlkeleri (AGENT.md) 🤖

Bu dosya, **DailyDN** projesinde görev alan tüm AI Agent'ların ve geliştiricilerin çalışma disiplinini, bağlam koruma mekanizmalarını, Git stratejilerini ve proje kurallarını tanımlayan **bağlayıcı çalışma anayasasıdır**.

---

## 🎯 Temel Misyon ve Mimari Kimlik
DailyDN; .NET 8/9, Clean Architecture, CQRS (MediatR), EF Core, Redis (Polly Resilience), Serilog (Graylog) ve zengin Domain modelleri üzerine inşa edilmiştir. Tüm geliştirmeler bu mimari katmanların sınırlarına ve sorumluluklarına tam sadakatle yapılmalıdır.

---

## 📜 Temel Çalışma İlkeleri ve Kısıtlamalar

### 1. 🛑 İzin Verilmedikçe Koda Dokunmama Kuralı (Review vs Execute)
- Kullanıcı yalnızca analiz, inceleme veya araştırma istediğinde **kaynak kod dosyalarında (`src/`) hiçbir değişiklik yapılmaz**.
- Tespit edilen tüm hatalar, güvenlik açıkları ve anti-pattern'ler `Docs/researches/` klasörü altında yapılandırılmış `.md` dosyalarında raporlanır.
- Kaynak kodda değişiklik yapmaya yalnızca kullanıcı açıkça bir görevi (örn: "X hatasını düzelt", "Y özelliğini ekle") talimat verdiğinde başlanır.

---

### 2. 📋 Bağlam Kaybını Önleyen Backlog & Görev Takibi (Context Retention)
- Her yeni göreve başlandığında ve her görev tamamlandığında **`Docs/backlog/Backlog.md`** dosyası güncellenmelidir.
- **Tarih ve Saat Sıralı Takip:** Tamamlanan işler Türkiye Saati (UTC+3) ve zaman damgası (`🕒 SS:DD:SS`) ile arşivlenir.
- **Yarım Kalan İşlerin Devir Teslimi:** Bir görev yarıda bırakıldığında veya oturum kapanma riski olduğunda "Hangi adımda kalındı?", "Sonraki oturumda ne yapılacak?" ve "Bağlı Dosyalar" açıkça Backlog'a not düşülür.

---

### 3. 🌿 Git İş Akışı ve Senkronizasyon Kuralları
- **Tek Bugfix Dalı Stratejisi:** Her hata için ayrı ayrı branch açmak yerine, toplu hata giderme süreçlerinde tek bir **`bugfix/core-fixes`** dalı üzerinden sırayla Conventional Commit'ler atılarak ilerlenir.
- **Daima Güncel Dal Üzerinden Başlama:** Yeni bir işe başlamadan önce mutlaka `git fetch --all` ve `git pull origin develop` yapılarak uzaktaki en güncel commit alınır.
- **Conventional Commits Formatı:** `feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `chore:` standartlarına uyulur (`fix(auth): ...`).
- **Rebase ve Temiz Ağaç:** Dal geride kaldığında `merge` yerine `git rebase origin/develop` tercih edilir; uzak sunucuya push edilirken `--force-with-lease` kullanılır.
- **Doğrudan Push Yasağı:** `main` veya `develop` dallarına doğrudan push atılamaz; sadece PR ile birleştirilir.

---

### 4. 🧪 Test ve Kalite Güvencesi (Quality Assurance)
- Yapılan her kod değişikliğinden sonra mutlaka **`dotnet test`** komutu çalıştırılarak tüm testlerin yeşil olduğu doğrulanmalıdır.
- Yeni bir iş kuralı veya bugfix eklendiğinde ilgili test projesine (`src/DailyDN.Tests/`) birim/entegrasyon testi eklenmeli veya güncellenmelidir.
- Compiler uyarıları (Warnings) ve nullability (`CS8602`, `CS8604` vb.) titizlikle ele alınmalıdır.

---

### 5. 📚 Dokümantasyon Standardı
- Projeye eklenen tüm yeni mimari kararlar `Docs/` klasöründeki ilgili dokümana yansıtılmalı ve `Docs/README.md` indeksi güncel tutulmalıdır.
- Dokümanlarda Mermaid akış diyagramları, net satır referansları ve anlaşılır Türkçe açıklamalar kullanılmalıdır.
