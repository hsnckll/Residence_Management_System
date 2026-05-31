namespace ResidenceMngSys.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int ApartmentId { get; set; }
        public string Block { get; set; }
        public decimal Balance { get; set; }
        public string FloorNo { get; set; }
        public string ApartmentNo { get; set; }
        public string SquareMeters { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Password { get; set; }
        public int IsActive { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public List<Dues> DuesList { get; set; }
        public decimal TotalDebt { get; set; }
        public Wallet? wallet { get; set; }
        public Dues? due { get; set; }
        public Apartment? apartment { get; set; }
        public BalanceRequests? balancerequests { get; set; }
        public User? user { get; set; }
        public List<User> UserList { get; set; } = new List<User>();
        public List<BalanceRequests>? RequestsList { get; set; }
        public User SelectedUserforUptade { get; set; } = new User();

        public decimal? totalToplananAidat { get; set; }

        /// <summary>
        /// Admin kısmı için
        /// </summary>
        public int totaluser { get; set; }
        public decimal totalborc { get; set; }
        public int bekleyenonaylar { get; set; }

        /////
        /// DUES İÇİN ///
        ///

        public List<string> MevcutAylar { get; set; } = new List<string>();
        public decimal SeciliAidatTutari { get; set; }
        public decimal SeciliKalanBorc { get; set; }
        public List<Dues> SeciliAyinKayitlari { get; set; } = new List<Dues>();
        public string AktifFiltre { get; set; } // Hangi ayın filtrelendiğini tutar

        public decimal odenentotalaidat { get; set; }
        public List<Dues> aidatıodeyenkisiler { get; set; }

        // yuzdelik olarak gösterme

        public double yuzdelik { get; set; }


        // duyurular 
        public List<Announcements> duyurular { get; set; }


        // şikayetler için

        public List<FaultAndRequest> sikayetler { get; set; }
        public List<FaultAndRequest> oneriler { get; set; }

        // genel ayarlar için
        public GeneralSettings generalSettings { get; set; }

        // ödeme bilgileri için
        public PaymentsInfo paymentsInfo { get; set; }

        // borcu olan sakinler index sayfası için

        public List<Dues> borcuolansakinler { get; set; }

        public List<Dues> EnSonOdenenAidatlar { get; set; }

        public List<FaultAndRequest> ensonSikayetler { get; set; }

        // Residens - ındex sayfasındaki talepler için

        public List<BalanceRequests> bakiyetalepleri { get; set; }

        // Daire Yonetimi

        public List<Apartment> ApartmanlarListesi { get; set; }

        public Apartment? guncellenecekApartman { get; set; }

        // Kullanıcı Ekleme

        public string? SelectedBlokName { get; set; } // sayfa yenilenince viewda seçilen bloğun gözükmesine yarar

        public string? SelectedFloorNo { get; set; }

        public string? SelectedApartmentNo { get; set; }
        public string? OdemeYontemi { get; set; }
        public List<Apartment>? BlogunKatları { get; set; }

        public List<Apartment>? KatınDaireleri { get; set; }

        // Kullanıcı Guncelleme

        public User guncellenecekKullanici { get; set; }


        // Aidat oluşturmak için

        public List<string> olusturulmusAylar { get; set; } = new List<string>();

        public int hedefYıl { get; set; }

        public int dbdekiMaxYıl { get; set; }

        // Aidat duzenlemek için

        public List<Dues> duzenlenmekIstenenAidat { get; set; }

        public Tenants tenant { get; set; }
    }
}
