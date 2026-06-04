# HIT Tracker

Mike Mentzer'ın High Intensity Training felsefesi üzerine kurulu bir antrenman takip uygulaması. .NET 10 + Razor Pages + SQLite + Gemini AI ile geliştirilmiştir.

---

## Özellikler

- 800+ egzersizden oluşan katalog (resimli, kas grubuna göre filtrelenebilir)
- Kişisel program oluşturma ve yönetme
- Workout takibi (set / rep / ağırlık)
- Kalori ve makro hesabı (BMR + TDEE, Mifflin-St Jeor formülü)
- Gemini AI destekli kişisel program önerisi
- JWT tabanlı güvenli kullanıcı yönetimi
- Mentzer prensiplerine uygun minimum dinlenme süresi kontrolü

---

## Sayfalar

| Sayfa | URL | Açıklama |
|-------|-----|----------|
| Ana Sayfa (Dashboard) | `/` | Günlük alıntı, istatistikler, kalori widget'ı, aktif program |
| Giriş | `/Auth/Login` | E-posta + şifre ile oturum açma |
| Kayıt | `/Auth/Register` | Yeni hesap oluşturma |
| Onboarding | `/Onboarding` | 5 adımlı profil oluşturma: yaş, cinsiyet, boy/kilo, aktivite, hedef |
| Egzersizler | `/Exercises` | 800+ egzersiz kataloğu, arama, kas grubu filtresi, custom egzersiz ekleme |
| Antrenman | `/Training` | Program oluşturma, aktif program yönetimi, workout başlatma, geçmiş |
| AI Coach | `/AI` | Gemini destekli program önerisi: gün, hedef, seviye, ekipman seçimi |
| API Dokümantasyonu | `/scalar/v1` | Scalar tabanlı OpenAPI dokümantasyonu |

---

## Önkoşullar

Projeyi çalıştırmadan önce sisteminde şunlar olmalı:

- .NET 10 SDK → [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
- Git
- Bir kod editörü (Visual Studio, VS Code vb.)
- Gemini API anahtarı → [aistudio.google.com/apikey](https://aistudio.google.com/apikey)

Sürümü doğrulamak için:

```bash
dotnet --version
```

Çıktı `10.0.x` olmalı.

---

## Kurulum

### 1. Projeyi Klonla

```bash
git clone https://github.com/burcpercin/HitTrackerAPI.git
cd HitTrackerAPI
```

### 2. Bağımlılıkları Yükle

```bash
dotnet restore
```

### 3. Konfigürasyon Dosyasını Oluştur

`appsettings.Example.json` dosyasını kopyala ve `appsettings.json` adıyla kaydet:

```bash
# Windows
copy appsettings.Example.json appsettings.json

# Linux / Mac
cp appsettings.Example.json appsettings.json
```

### 4. API Anahtarını Ekle

`appsettings.json` dosyasını aç ve şu alanları doldur:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=hittracker.db"
  },
  "Jwt": {
    "Secret": "kendi-uzun-secret-keyiniz-en-az-32-karakter",
    "ExpiresInDays": 7
  },
  "Gemini": {
    "ApiKey": "AIza..."
  }
}
```

> Önemli: `appsettings.json` dosyası `.gitignore` içindedir. API anahtarını GitHub'a yüklemeyin.

---

## Çalıştırma

```bash
dotnet run
```

Çıktı:

```
Now listening on: http://localhost:5210
```

Tarayıcıda aç:

```
http://localhost:5210
```

### İlk Açılış

1. Veritabanı (`hittracker.db`) otomatik oluşturulur
2. 800+ egzersiz seed edilir (1-2 dakika sürebilir)
3. `/Auth/Register` adresinden kayıt ol
4. Onboarding adımlarını tamamla
5. Dashboard hazır

---

## Test Çalıştırma

```bash
dotnet test
```

Beklenen çıktı: `Passed: 20, Failed: 0`

Testler 4 ana servisi kapsar:

- CalorieServiceTests (6 test) — BMR, TDEE, makro hesabı
- AuthServiceTests (5 test) — Kayıt, giriş, şifre doğrulama
- ExerciseServiceTests (5 test) — Custom egzersiz CRUD
- SessionServiceTests (4 test) — Antrenman başlatma, kas yenilenme

---

## API Dokümantasyonu

Uygulama çalışırken Scalar dokümantasyonuna erişebilirsin:

```
http://localhost:5210/scalar/v1
```

---

## Klasör Yapısı

```
HitTrackerAPI/
├── Controllers/      REST API endpoint'leri
├── DTOs/             Veri transfer objeleri + validasyon
├── Data/             EF Core DbContext
├── Models/           Veritabanı entity'leri
├── Repositories/     Veri erişim katmanı
├── Services/         İş mantığı katmanı
├── Pages/            Razor Pages (UI)
├── wwwroot/          CSS, JS, statik dosyalar
├── exercises_data/   Egzersiz JSON + resimleri
├── appsettings.json  Konfigürasyon (gitignore)
└── Program.cs        Giriş noktası
```

---

## Sık Karşılaşılan Sorunlar

**Port 5210 kullanımda:**

```bash
# Windows
netstat -ano | findstr :5210
taskkill /PID <pid> /F

# Linux / Mac
lsof -i :5210
kill -9 <pid>
```

**Veritabanını sıfırlamak istersen:**

```bash
# Windows
del hittracker.db

# Linux / Mac
rm hittracker.db

dotnet run
```

**Tarayıcı eski JS dosyalarını gösteriyor:**

`Ctrl + Shift + R` ile cache temizle.

---

## Kullanılan Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| Backend | ASP.NET Core 10 |
| Veritabanı | SQLite + Entity Framework Core |
| Auth | JWT + BCrypt |
| Frontend | Razor Pages + Vanilla JavaScript |
| AI | Google Gemini 2.0 Flash |
| Test | xUnit |
| Dokümantasyon | Scalar (OpenAPI) |

---

## Geliştirici

Burç Perçin — [@burcpercin](https://github.com/burcpercin)

---

*"Train hard. Train brief. Train infrequently." — Mike Mentzer*
