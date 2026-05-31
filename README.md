# 🏢 ResiEase - Rezidans Yönetim Sistemi

ResiEase, apartman ve rezidans yönetimini dijitalleştiren, yöneticiler ve sakinler için geliştirilmiş kapsamlı bir web tabanlı yönetim sistemidir. ASP.NET Core MVC ve SQL Server kullanılarak geliştirilmiştir.

---

## 🚀 Özellikler

### 👤 Kullanıcı Yönetimi
- Yönetici ve ev sakini rolleri
- Oturum açma / çıkış sistemi
- Profil yönetimi

### 💰 Aidat Yönetimi
- Aylık aidat takibi ve ödeme sistemi
- Yönetici aidat miktarını değiştirdiğinde fazla ödenen tutarın otomatik olarak bir sonraki aya aktarılması
- Gecikmiş aidat bildirimleri
- Ödeme geçmişi görüntüleme

### 📧 E-posta Bildirim Sistemi
- Aidat hatırlatma e-postaları
- Ödeme onay bildirimleri
- Duyuru e-postaları
- Gmail SMTP entegrasyonu ile gerçek zamanlı bildirimler

### 📋 Duyuru Yönetimi
- Yönetici tarafından tüm sakinlere duyuru gönderimi
- Duyuru geçmişi

### 🏠 Daire Yönetimi
- Daire bilgileri ve sakin takibi
- Boş / dolu daire durumu

### ⚙️ Arka Plan İşlemleri
- Hangfire ile zamanlanmış görevler (otomatik hatırlatmalar vb.)

---

## 🛠️ Teknolojiler

| Teknoloji | Kullanım |
|---|---|
| ASP.NET Core 8 MVC | Web framework |
| Entity Framework Core | ORM / Veritabanı yönetimi |
| SQL Server (SQLEXPRESS) | Veritabanı |
| Hangfire | Arka plan görevleri |
| Gmail SMTP | E-posta bildirimleri |
| Bootstrap | Arayüz |

---

## ⚙️ Kurulum

### 1. Gereksinimler
- Visual Studio 2022
- .NET 8 SDK
- SQL Server / SQLEXPRESS
- SSMS (SQL Server Management Studio)

### 2. Repoyu Klonla
```bash
git clone https://github.com/hsnckll/Residence_Management_System.git
```

### 3. Veritabanını Kur
- SSMS'i aç
- `database.sql` dosyasını aç
- Çalıştır (F5) → `ApartmentManagementSystem` veritabanı otomatik oluşur

### 4. appsettings.json Ayarla
`ResidenceMngSys/appsettings.json` dosyasını düzenle:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SENIN_SUNUCU_ADIN\\SQLEXPRESS;Database=ApartmentManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "GMAIL_ADRESIN@gmail.com",
    "SenderName": "Rezidans Yönetim Sistemi",
    "Password": "GMAIL_APP_PASSWORD"
  }
}
```

> **Not:** Gmail App Password oluşturmak için: Google Hesabı → Güvenlik → 2 Adımlı Doğrulama → Uygulama Şifreleri

### 5. Projeyi Çalıştır
- `ResidenceMngSys.sln` dosyasını Visual Studio ile aç
- `Ctrl + F5` ile başlat

---

## 🔑 Test Hesapları

> ⚠️ E-posta bildirimleri gerçek mail adreslerine gönderildiğinden, sistemi test ederken gerçek mail adresleri kullanılması önerilir. Aşağıdaki hesaplar demo amaçlıdır.

| Rol | E-posta | Şifre |
|---|---|---|
| Yönetici | elif@gmail.com | a |
| Ev Sakini | yener@gmail.com | a |

---

## 📁 Proje Yapısı

```
ResidenceMngSys/
├── Controllers/        # MVC Controller'lar
├── Models/             # Veritabanı modelleri
├── Views/              # Razor View'lar
├── Services/           # Email, Tenant servisleri
├── EmailTemplates/     # E-posta şablonları
├── Migrations/         # EF Core migration'ları
├── wwwroot/            # Statik dosyalar (CSS, JS, resimler)
└── appsettings.json    # Uygulama ayarları
database.sql            # Veritabanı kurulum scripti
```

---

## 👨‍💻 Geliştirici

**hsnckll** - Bireysel proje
