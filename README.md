# ⚡ HIT TRACKER

Mike Mentzer'ın "High Intensity Training" (HIT) felsefesine dayalı olarak geliştirilmiş, profesyonel düzeyde bir antrenman takip uygulamasıdır.

## 📖 Proje Hakkında
HIT Tracker, klasik antrenman mantığı yerine Mike Mentzer'ın "kısa, yoğun ve seyrek" prensibini (HIT) temel alır. Sistem; egzersizler için minimum 3 günlük zorunlu dinlenme süresi, sete kadar başarısızlık takibi (failure tracking) ve progressive overload (artan yük) izleme gibi özellikler sunar. Modern mimari ile geliştirilen bu web uygulaması, Google Gemini yapay zeka modeli ile kişiselleştirilmiş programlar oluşturur.

## ✨ Öne Çıkan Özellikler
* **Mentzer Prensipleri:** Egzersizlerde zorunlu dinlenme süresi kontrolü ve tükeniş (failure) takibi[cite: 1].
* **Yapay Zeka Destekli Koç:** Google Gemini 2.5 Flash API ile hedefe, deneyime ve mevcut ekipmana göre özel program oluşturma[cite: 1].
* **Akıllı Beslenme Hesabı:** Mifflin-St Jeor formülü ile bilimsel BMR ve TDEE hesabı, hedefe yönelik otomatik makro dağılımı[cite: 1].
* **Kapsamlı Egzersiz Kataloğu:** Resimli ve detaylı talimatlar içeren 800'den fazla egzersiz kütüphanesi[cite: 1].
* **Güvenli Mimari:** BCrypt şifreleme ve JWT (JSON Web Token) tabanlı yetkilendirme[cite: 1].

## 🛠️ Kullanılan Teknolojiler

| Kategori | Teknolojiler |
| :--- | :--- |
| **Backend Framework** | ASP.NET Core 10 Web API + Razor Pages[cite: 1] |
| **Veritabanı** | SQLite (Entity Framework Core)[cite: 1] |
| **Kimlik Doğrulama** | JWT Bearer Token + BCrypt[cite: 1] |
| **Yapay Zeka** | Google Gemini 2.5 Flash API[cite: 1] |
| **Frontend** | Razor Pages + HTML5 + CSS3 + Vanilla JavaScript[cite: 1] |
| **Test / Dokümantasyon** | xUnit (20 birim test), Scalar (OpenAPI)[cite: 1] |

## 🚀 Kurulum ve Çalıştırma

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları sırasıyla izleyin.

### Gereksinimler
* .NET 10.0 SDK[cite: 1]
* Git
* SQLite destekleyen bir veritabanı görüntüleyici (Opsiyonel)[cite: 1]

### Adım Adım Kurulum

**1. Projeyi Klonlayın**
```bash
git clone [https://github.com/burcpercin/HitTrackerAPI.git](https://github.com/burcpercin/HitTrackerAPI.git)
cd HitTrackerAPI
