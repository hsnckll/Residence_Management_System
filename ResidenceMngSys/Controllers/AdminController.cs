using ResidenceMngSys.Models;
using ResidenceMngSys.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Packaging.Core;
namespace ResidenceMngSys.Controllers
{
    public class AdminController : BaseController
    {
        private readonly EmailService _emailService;
        private readonly EmailTemplateService _templateService;

        public AdminController(AppDbContext db, EmailService emailService, EmailTemplateService templateService) : base(db)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public IActionResult Index()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();

            var totaluser = _db.Users.Count();
            viewmodel.totaluser = totaluser;

            var totalborc = _db.Dues.Sum(x => x.RemainingDebt);
            viewmodel.totalborc = totalborc;

            var bekleyenonaysayisi = _db.BalanceRequests.Where(x => x.Status == "Beklemede").Count();
            viewmodel.bekleyenonaylar = bekleyenonaysayisi;

            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            viewmodel.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.apartment = apartment;


            var toplamAidatTutarı = _db.Dues.Sum(x => x.DuesPrice);
            var kalanBorclar = _db.Dues.Sum(x => x.RemainingDebt);

            viewmodel.totalToplananAidat = (toplamAidatTutarı - kalanBorclar);


            var borcuolanlar = _db.Dues.Where(x => x.IsPaid == 0).Include(x => x.Apartment.User).GroupBy(x => x.Apartment.User).
                Select(a => new Dues
                {
                    Apartment = a.First().Apartment,
                    RemainingDebt = a.Sum(s => s.RemainingDebt),
                    IsPaid = 0,
                }).ToList();

            viewmodel.borcuolansakinler = borcuolanlar;

            var ensonodenenaidatlar = _db.Dues.Where(x => x.IsPaid == 1).OrderByDescending(x => x.PaymentDate).Include(x => x.Apartment.User).Take(4).ToList();
            viewmodel.EnSonOdenenAidatlar = ensonodenenaidatlar;


            var ensongonderilensikayetler = _db.FaultAndRequest.Where(x => x.Type == "Şikayet").
                OrderByDescending(x => x.CreatedAt).Include(x => x.Apartment).ThenInclude(x => x.User).ToList();

            viewmodel.ensonSikayetler = ensongonderilensikayetler;


            // Grafik için sorgular

            var currentYear = DateTime.Now.Year;

            var allDues = _db.Dues
                .Where(x => x.Year == currentYear)
                .GroupBy(x => x.Months)
                .Select(g => new { Month = g.First().Months, Total = g.Sum(s => s.DuesPrice) })
                .ToList();

            var paidDues = _db.Dues
                .Where(x => x.IsPaid == 1 && x.Year == currentYear)
                .GroupBy(x => x.Months)
                .Select(g => new { Month = g.First().Months, Total = g.Sum(s => s.DuesPrice) })
                .ToList();

            var turkishMonths = new List<string> { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

            var chartData = turkishMonths.Select((monthName, index) => new {
                Month = monthName,
                Generated = allDues.FirstOrDefault(x => x.Month == monthName)?.Total ?? 0,
                Collected = paidDues.FirstOrDefault(x => x.Month == monthName)?.Total ?? 0
            }).ToList();

            ViewBag.ChartLabels = Newtonsoft.Json.JsonConvert.SerializeObject(
                chartData.Select(x => x.Month.Substring(0, 3))
            );
            ViewBag.ChartGenerated = Newtonsoft.Json.JsonConvert.SerializeObject(chartData.Select(x => x.Generated));
            ViewBag.ChartCollected = Newtonsoft.Json.JsonConvert.SerializeObject(chartData.Select(x => x.Collected));


            return View("Index", viewmodel);
        }



        public IActionResult Residents()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewmodel = new UserViewModel();


            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            viewmodel.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.apartment = apartment;

            viewmodel.UserList = _db.Users.Include(u => u.Apartment).ToList();
            ViewBag.Apartment = _db.Apartment.ToList();
            return View(viewmodel);
        }



        public IActionResult aidatolusturma()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewmodel = new UserViewModel();


            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            viewmodel.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.apartment = apartment;

            int hedefYıl;
            var dbdekimaxyil = _db.Dues.Any() ? _db.Dues.Max(x => x.Year) : DateTime.Now.Year;
            viewmodel.dbdekiMaxYıl = dbdekimaxyil;

            var dbdekiEnBuyukYilinAidatlari = _db.Dues.Where(x => x.Year == dbdekimaxyil).Select(x => x.Months).Distinct().ToList();

            if (dbdekiEnBuyukYilinAidatlari.Contains("Aralık")) // Dbdeki en buyuk yılın aidatlarının içinde Aralık ayının Aidatı var mı diye sorguluyor eğer sonuç True çıkarsa if'in içine giriyor
            {
                hedefYıl = dbdekimaxyil + 1;

                ViewBag.ExistingMonths = new List<string>();

            }
            else
            {
                hedefYıl = dbdekimaxyil;
                ViewBag.ExistingMonths = dbdekiEnBuyukYilinAidatlari;
            }


            viewmodel.hedefYıl = hedefYıl;

            return View(viewmodel);
        }







        // AİDAT OLUŞTURMA
        [HttpPost]
        public async Task<IActionResult> CreateDue(int year, string Months, int DuesPrice, DateTime PaymentStartDate, DateTime PaymentEndDate)
        {
            var apartments = _db.Apartment.Where(x => x.IsOccupied == 1).ToList();
            foreach (var apartment in apartments) // Bütün apartmanlara aidatı ekle demek oluyor. Apartman listesinde geziyor ve aktif bütün apartmanlara ekleme yapıyor
            {
                var due = new Dues
                {
                    Apartment_Id = apartment.Id,
                    Year = year,
                    Months = Months,
                    DuesPrice = DuesPrice,
                    RemainingDebt = (int)DuesPrice,
                    IsPaid = 0,
                    CreatedAt = DateTime.Now,
                    PaymentStartDate = PaymentStartDate,
                    PaymentEndDate = PaymentEndDate
                };
                _db.Add(due);
                _db.SaveChanges();

                ViewBag.ExistingMonths = _db.Dues.Where(x => x.Year == year).Select(x => x.Months).Distinct().ToList();

                var sakin =_db.Users.FirstOrDefault(x => x.Apartment_Id == apartment.Id);
                if (sakin != null && sakin.Mail != null)
                {
                    var body = _templateService.LoadTemplate("AidatOlusturuldu", new Dictionary<string, string>
                    {
                        { "Name", sakin.Name },
                        { "Surname", sakin.Surname },
                        { "Month", Months },
                        { "Year", year.ToString() },
                        { "Amount", DuesPrice.ToString() },
                        { "PaymentEndDate", PaymentEndDate.ToString("dd.MM.yyyy") }
                    });

                     var ArkaPlandaMailGonderme = _emailService.SendEmailAsync(
                        sakin.Mail,
                        sakin.Name + " " + sakin.Surname,
                        "Yeni Aidatınız Oluşturuldu",
                        body
                    );

                }


            }
            return RedirectToAction("Dues");
        }







        [HttpGet]
        public IActionResult Dues(int? yıl, string ay)
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();
            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            viewmodel.user = user;
            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.apartment = apartment;


            // Bu kodda bir filtreleme yok sadece dbdeki tüm aidatları "2026 - Ocak" formatında yazıyor
            // Ama biz şuan buna bir filtreleme ekleyeceğiz çunku bize dues viewinda sadece son 2 yıla ait aidatları göstermesi için.

            var sonikiyıl = DateTime.Now.Year;
            viewmodel.MevcutAylar = _db.Dues.Where(x => x.Year >= sonikiyıl - 1).Select(d => d.Year + " - " + d.Months.Trim()).Distinct().ToList(); //Burada discint benzersiz elemanları getirir. cunku biz her aidatta daire başına aynı aidatan oluşturuyoruz ya onun önüne geçebilmek için 


            int hedefYil;
            string hedefAy;

            // ÖNEMLİ: Burada önce 'ay' null mı kontrol ediyoruz
            if (!string.IsNullOrEmpty(ay))
            {
                hedefYil = yıl.Value;
                hedefAy = ay.Trim();
            }
            else
            {
                // Burası veritabanındaki aidatlar tablosundaki en son veriyi çeker
                var sonKayit = _db.Dues.OrderByDescending(x => x.Year).ThenByDescending(x => x.Months).FirstOrDefault();

                hedefYil = sonKayit?.Year ?? DateTime.Now.Year;
                hedefAy = sonKayit?.Months ?? "Ocak";
            }

            viewmodel.AktifFiltre = hedefYil + " - " + hedefAy;


            // Veritabanından veriyi çekiyoruz
            viewmodel.SeciliAyinKayitlari = _db.Dues.Where(d => d.Year == hedefYil && d.Months.Trim() == hedefAy.Trim()).Include(x => x.Apartment).ThenInclude(x => x.User).ToList();
            // yazdığımız ınclude kısmı kimin ödeyip kimin ödemediğini görmek için.


            // Rakamları modele dolduruyoruz
            viewmodel.SeciliAidatTutari = viewmodel.SeciliAyinKayitlari.FirstOrDefault()?.DuesPrice ?? 0;
            viewmodel.SeciliKalanBorc = viewmodel.SeciliAyinKayitlari.Sum(d => d.RemainingDebt);
            viewmodel.odenentotalaidat = viewmodel.SeciliAyinKayitlari.Sum(x => x.DuesPrice - x.RemainingDebt);
            viewmodel.aidatıodeyenkisiler = viewmodel.SeciliAyinKayitlari;
            //var odenenaidatlar = _db.Dues.Include(x => x.Apartment).ThenInclude(x => x.User).ToList();


            // yüzdelik olarak ne kadar aidat tutarı ödendi onu göstermemiz lazım

            var toplamaidatlar = _db.Dues.Where(d => d.Year == hedefYil && d.Months.Trim() == hedefAy.Trim()).Sum(x => x.DuesPrice);
            var odenenaidatlar = _db.Dues.Where(d => d.Year == hedefYil && d.Months.Trim() == hedefAy.Trim()).Sum(x => x.DuesPrice - x.RemainingDebt);

            double yuzde = 0;
            if (toplamaidatlar > 0)
            {
                yuzde = (double)(odenenaidatlar / toplamaidatlar * 100);
            }

            viewmodel.yuzdelik = yuzde;

            return View(viewmodel);
        }


        [HttpGet]
        public IActionResult aidatDuzenleme(int yıl, string ay)
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewmodel = new UserViewModel();


            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            viewmodel.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.apartment = apartment;

            viewmodel.duzenlenmekIstenenAidat = _db.Dues.Where(x => x.Year == yıl && x.Months == ay).Distinct().ToList();

            return View(viewmodel);
        }



        [HttpPost]
        public async Task<IActionResult> aidatDuzenlemePost(int id, int yil, string ay, int DuesPrice, DateTime PaymentStartDate, DateTime PaymentEndDate)
        {

            var duzenlenmekİstenilenAidat = _db.Dues.Where(x => x.Year == yil && x.Months == ay).Distinct().ToList();
            var FazlaOdemeAktarılacakApartmanlarınWallet = _db.Wallet.Include(x => x.apartment).ToList();

            // Duzenleme İşlemleri
            foreach (var aidat in duzenlenmekİstenilenAidat)
            {
                aidat.Year = yil;
                aidat.Months = ay;
                aidat.PaymentStartDate = PaymentStartDate;
                aidat.PaymentEndDate = PaymentEndDate;


                decimal ödenenAidatMiktarı = (aidat.DuesPrice - aidat.RemainingDebt);

                decimal FazlaOdenenMiktar = ödenenAidatMiktarı - DuesPrice;

                aidat.RemainingDebt += (DuesPrice - aidat.DuesPrice);

                if (FazlaOdenenMiktar > 0)   // aidat.RemainingDebt<= 0  // Yada bu sorgu yerine guncellenen Aidat Miktarı < Kalan tutar. Bu sorgu da olabilir.
                {
                    aidat.IsPaid = 1;
                    aidat.PaymentDate = DateTime.Now;

                    foreach (var fazlaodemecuzdan in FazlaOdemeAktarılacakApartmanlarınWallet.Where(x => x.apartment.Id == aidat.Apartment.Id))
                    {

                        fazlaodemecuzdan.Balance += FazlaOdenenMiktar;
                    }

                    aidat.RemainingDebt = 0;

                    var sakin = _db.Users.FirstOrDefault(x => x.Apartment_Id == aidat.Apartment_Id);

                    var sakinincuzdan=_db.Wallet.FirstOrDefault(x=>x.Apartment_Id==sakin.Apartment.Id);
                    if (sakin != null && sakin.Mail != null)
                    {
                        var body = _templateService.LoadTemplate("FazlaOdemeIadesi", new Dictionary<string, string>
                        {
                            { "Name", sakin.Name },
                            { "Surname", sakin.Surname },
                            { "Month", ay },
                            { "Year", yil.ToString() },
                            { "OldAmount", aidat.DuesPrice.ToString() },
                            { "NewAmount", DuesPrice.ToString() },
                            { "RefundAmount", FazlaOdenenMiktar.ToString() },
                            { "Balance", sakinincuzdan.Balance.ToString() }
                        });

                        await _emailService.SendEmailAsync(
                            sakin.Mail,
                            sakin.Name + " " + sakin.Surname,
                            "Fazla Ödemeniz Bakiyenize Aktarıldı",
                            body
                        );
                    }





                }

                else { aidat.IsPaid = 0; }


                aidat.DuesPrice = DuesPrice; // Bunun altta olması lazım cunku yukarıda olursa remainingdebt işleminde her türlü 0 çıkar. Birisi dbdeki price diğeri bizim girdiğimiz price. Yukarıda olursa dbdeki price'a hiçbir şekilde erişemeyiz.


            }






            _db.SaveChanges();


            return RedirectToAction("Dues");
        }



















        [HttpGet]
        public IActionResult Balances(string durum)
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userview = new UserViewModel();


            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;

            var requests = _db.BalanceRequests.Include(x => x.apartment).ThenInclude(a => a.User).ToList();
            userview.RequestsList = requests;

            ViewData["ActiveMenu"] = "balances";
            return View(userview);
        }






        [HttpPost]
        public async Task<IActionResult> bakiyeislemleri(string islem, int id, int miktar, int requestId)
        {
            if (islem == "onayla")
            {
                var secilenkisinincuzdan = _db.Wallet.FirstOrDefault(x => x.Apartment_Id == id);
                var request = _db.BalanceRequests.FirstOrDefault(x => x.Id == requestId);
                request.Status = "Onaylandı";
                request.ApprovedAt = DateTime.Now;



                secilenkisinincuzdan.Balance += miktar;


                var cuzdanhareketleri = new WalletTransactions
                {
                    Wallet_Id = secilenkisinincuzdan.Id,
                    Amount = miktar,
                    TransactionType = "Bakiye Yüklemesi",
                    Description = request.Explanation,
                    CreatedAt = DateTime.Now,
                    BalanceAfter = secilenkisinincuzdan.Balance

                };

                _db.WalletTransactions.Add(cuzdanhareketleri);


                var istekgonderenkisi = _db.Users.FirstOrDefault(x => x.Apartment_Id == secilenkisinincuzdan.Apartment_Id);

                if (istekgonderenkisi.Mail != null)
                {
                    var body = _templateService.LoadTemplate("Bakiyeonaylandi", new Dictionary<string, string>
                    {
                        { "Name", istekgonderenkisi.Name },
                        { "Surname", istekgonderenkisi.Surname },
                        { "Amount",  miktar.ToString()},
                        { "Date", request.ApprovedAt.ToString() },
                        { "Balance", cuzdanhareketleri.BalanceAfter.ToString() },
                        
                    });

                    await _emailService.SendEmailAsync(
                         istekgonderenkisi.Mail,
                         istekgonderenkisi.Name + " " + istekgonderenkisi.Surname,
                         "Bakiye Talebiniz Onaylandı",
                         body
                     );
                }
            }




            else
            {
                var secilenkisinincuzdan = _db.Wallet.FirstOrDefault(x => x.Apartment_Id == id);
                var request = _db.BalanceRequests.FirstOrDefault(x => x.Id == requestId);
                request.Status = "Reddedildi";

                var cuzdanhareketleri = new WalletTransactions
                {
                    Wallet_Id = secilenkisinincuzdan.Id,
                    Amount = miktar,
                    TransactionType = "Bakiye Yüklemesi",
                    Description = request.Explanation,
                    CreatedAt = DateTime.Now,
                    BalanceAfter = secilenkisinincuzdan.Balance

                };
                _db.WalletTransactions.Add(cuzdanhareketleri);

                var istekgonderenkisi = _db.Users.FirstOrDefault(x => x.Apartment_Id == secilenkisinincuzdan.Apartment_Id);

                if (istekgonderenkisi.Mail != null)
                {
                    var body = _templateService.LoadTemplate("Bakiyereddedildi", new Dictionary<string, string>
                    {
                        { "Name", istekgonderenkisi.Name },
                        { "Surname", istekgonderenkisi.Surname },
                        { "Amount",  miktar.ToString()},
                        { "Explanation", cuzdanhareketleri.Description },
                        { "Date", cuzdanhareketleri.CreatedAt.ToString() },

                    });

                    await _emailService.SendEmailAsync(
                         istekgonderenkisi.Mail,
                         istekgonderenkisi.Name + " " + istekgonderenkisi.Surname,
                         "Bakiye Talebiniz Onaylandı",
                         body
                     );
                }





            }

            _db.SaveChanges();


            return RedirectToAction("Balances");
        }




        [HttpGet]
        public IActionResult Announcements()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userview = new UserViewModel();


            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;

            var duyurular = _db.Announcements.ToList();
            userview.duyurular = duyurular;

            return View(userview);
        }

        [HttpPost]
        public IActionResult duyurukaldır(int id)
        {
            var silinmekistenenduyuru = _db.Announcements.FirstOrDefault(x => x.Id == id);
            _db.Announcements.Remove(silinmekistenenduyuru);
            _db.SaveChanges();

            return RedirectToAction("Announcements");
        }

        [HttpGet]
        public IActionResult duyuruolustur()
        {
            return View();
        }



        [HttpPost]
        public IActionResult duyuruolusturpost(string Oncelik, string Baslik, string Icerik)
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();
            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            viewmodel.user = user;
            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.apartment = apartment;

            var duyuru = new Announcements
            {
                Title = Baslik,
                Contents = Icerik,
                ImportanceStatus = Oncelik,
                CreatedAt = DateTime.Now,

            };

            _db.Announcements.Add(duyuru);
            _db.SaveChanges();

            return RedirectToAction("Announcements");
        }







        [HttpGet]
        public IActionResult Complaints()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userview = new UserViewModel();


            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;


            var sikayetler = _db.FaultAndRequest.Where(x => x.Type == "Şikayet").Include(x => x.Apartment).ThenInclude(x => x.User).ToList();

            userview.sikayetler = sikayetler;



            return View(userview);
        }



        [HttpPost]
        public IActionResult sikayetduzenleme(int sikayetid, string yoneticiyanıtı, string status)
        {
            var seciliSikayet = _db.FaultAndRequest.FirstOrDefault(x => x.Type == "Şikayet" && x.Id == sikayetid);

            seciliSikayet.AdminAnswer = yoneticiyanıtı;
            seciliSikayet.Status = status;
            if (status == "Tamamlandı")
            {
                seciliSikayet.CompletionAt = DateTime.Now;
            }


            _db.SaveChanges();

            return RedirectToAction("Complaints");
        }


        public IActionResult Suggestions()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userview = new UserViewModel();


            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;

            var oneriler = _db.FaultAndRequest.Where(x => x.Type == "Öneri").Include(x => x.Apartment).ThenInclude(x => x.User).ToList();
            userview.oneriler = oneriler;


            return View(userview);
        }





        [HttpPost]
        public IActionResult oneriduzneleme(string status, int OneriId, string yoneticiyanit)
        {
            var secilenoneri = _db.FaultAndRequest.FirstOrDefault(x => x.Id == OneriId);

            secilenoneri.Status = status;
            secilenoneri.AdminAnswer = yoneticiyanit;
            if (secilenoneri.Status == "Tamamlandı")
            {
                secilenoneri.CompletionAt = DateTime.Now;
            }

            _db.SaveChanges();

            return RedirectToAction("Suggestions");
        }




        public IActionResult Settings()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userview = new UserViewModel();

            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;

            var genelayarlar = _db.GeneralSettings.FirstOrDefault();
            userview.generalSettings = genelayarlar;

            var odemebilgileri = _db.PaymentsInfo.FirstOrDefault();
            userview.paymentsInfo = odemebilgileri;

            var tenantId = HttpContext.Session.GetInt32("TenantId");
            userview.tenant = _db.Tenants.FirstOrDefault(x => x.Id == tenantId);


            return View(userview);
        }





        [HttpPost]
        public IActionResult genelAyarlarDuzenleme(string apartmanadi, string ıban, string accountHolder, string bankName, int genelId, int PaymentsId)
        {
            var tenantId = HttpContext.Session.GetInt32("TenantId");

            // view sayfasındaki id hicbir zaman null gelmez cunku parametresi int. İnt değerler doğası gereği hiçbir zaman null dönmezler bir değerleri yoksa sıfır döndürürler.


            if (genelId != null)
            {
                var dbdekigenelAyarlar = _db.GeneralSettings.FirstOrDefault(x => x.Id == genelId);
                dbdekigenelAyarlar.ResidenceName = apartmanadi;
                var tenanttakiName = _db.Tenants.FirstOrDefault(x => x.Id == tenantId);
                tenanttakiName.Name = apartmanadi;
                _db.SaveChanges();
            }
            else
            {
                var yeniad = new GeneralSettings
                {
                    ResidenceName = apartmanadi
                };
                _db.GeneralSettings.Add(yeniad);
                _db.SaveChanges();
            }







            if (PaymentsId == 0)
            {
                var odemeBilgileri = new PaymentsInfo
                {
                    IbanNumber = ıban,
                    AccountHolder = accountHolder,
                    BankName = bankName
                };
                _db.PaymentsInfo.Add(odemeBilgileri);
            }
            else
            {
                var odemebilgileri = _db.PaymentsInfo.FirstOrDefault(x => x.Id == PaymentsId);
                odemebilgileri.IbanNumber = ıban;
                odemebilgileri.AccountHolder = accountHolder;
                odemebilgileri.BankName = bankName;
            }

            _db.SaveChanges();
            return RedirectToAction("Settings");
        }




































        // Burası Duzenlenecek  

        [HttpGet]
        public IActionResult DaireYonetimi()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userview = new UserViewModel();

            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;


            var apartmanlarListesi = _db.Apartment.Include(x => x.User).ToList();
            userview.ApartmanlarListesi = apartmanlarListesi;



            return View(userview);
        }






        // Burası tekil olarak apartman oluşturma
        public IActionResult tekilaptolustur()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userview = new UserViewModel();

            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;

            return View(userview);
        }

        // Tekil olarak apartman oluşturma

        [HttpPost]
        public IActionResult tekilapartmanolusturma(string Blok, string Kat, string DaireNo, string Metrekare, string Tip)
        {

            var tekilapartman = new Apartment
            {
                BlockNo = Blok,
                FloorNo = Kat,
                ApartmentNo = DaireNo,
                Type = Tip,
                SquareMeters = Metrekare,
                IsOccupied = 0,
                CreatedAt = DateTime.Now
            };


            _db.Apartment.Add(tekilapartman);
            _db.SaveChanges();


            return RedirectToAction("DaireYonetimi");
        }
















        // Burası toplu olarak apartman oluşturma
        public IActionResult topluaptolustur()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userview = new UserViewModel();

            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;

            return View(userview);
        }



        [HttpPost]
        public IActionResult topluapartmanolusturma(string Blok, int KatSayisi, int DairePerKat, int BaslangicKat, int NumStyle, string Metrekare, string Tip)
        {

            NumStyle = 1; // Daire numaralandırması 1'den başlıyor bunun mantığı bu

            var daireler = new List<Apartment>();


            for (int x = 0; x < KatSayisi; x++)
            {
                int katno = BaslangicKat + x; // Hangi kattan başlayacağımızı belirliyoruz burada

                for (int y = 1; y <= DairePerKat; y++)
                {
                    daireler.Add(new Apartment
                    {

                        BlockNo = Blok,
                        FloorNo = katno.ToString(),
                        ApartmentNo = NumStyle.ToString(),
                        SquareMeters = Metrekare,
                        Type = Tip,
                        IsOccupied = 0,
                        CreatedAt = DateTime.Now
                    });
                    NumStyle++; // Her daire sonrası numara bir artıyor
                }
            }

            _db.Apartment.AddRange(daireler);
            _db.SaveChanges();

            return RedirectToAction("DaireYonetimi");
        }














        // Buralarda duzenlemek için olacak

        [HttpGet]
        public IActionResult daireduzenleme(int guncellenecekDaire)
        {

            // Bu kısım _AdminLayout.cshtml için gerekli olan kısım
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userview = new UserViewModel();

            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;
            // 

            var secilenDaire = _db.Apartment.Include(x => x.User).FirstOrDefault(x => x.Id == guncellenecekDaire);
            userview.guncellenecekApartman = secilenDaire;

            return View(userview);
        }


        [HttpPost]
        public IActionResult daireduzenlemepost(int id, string Blok, string Kat, string DaireNo, int Durum, string Metrekare, string Tip)
        {
            var dbdekiApartman = _db.Apartment.Include(x => x.User).FirstOrDefault(x => x.Id == id);

            // Guncellenecek kısımlar,
            dbdekiApartman.BlockNo = Blok;
            dbdekiApartman.FloorNo = Kat;
            dbdekiApartman.ApartmentNo = DaireNo;
            dbdekiApartman.IsOccupied = Durum;
            dbdekiApartman.SquareMeters = Metrekare;
            dbdekiApartman.Type = Tip;
            _db.SaveChanges();
            return RedirectToAction("DaireYonetimi");
        }



        [HttpPost]
        public IActionResult dairesilme(int silinecekDaire)
        {
            var dbdekiApartman = _db.Apartment.Include(x => x.User).FirstOrDefault(x => x.Id == silinecekDaire);


            if (dbdekiApartman.IsOccupied == 1)
            {
                TempData["Hata"] = "Daire doludur, silemezsiniz.";
                return RedirectToAction("DaireYonetimi");
            }




            _db.Apartment.Remove(dbdekiApartman);
            _db.SaveChanges();



            return RedirectToAction("DaireYonetimi");
        }
























        // Yukarıdaki yarım kalmış kullanıcı Ekleme, Guncelleme ve Silme kısmını burada duzenliyoruz

        [HttpGet]
        public IActionResult kullaniciekleme(string BlokName, string FloorNo, string ApartmentNo)
        {
            // Bu kısım _AdminLayout.cshtml için gerekli olan kısım
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userview = new UserViewModel();

            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;


            // Blokları Getiriyor
            var bloklar = _db.Apartment.Include(x => x.User).Where(x => x.IsOccupied == 0).ToList();

            userview.ApartmanlarListesi = bloklar;



            // Bloğa ait katları getiriyor. Şuan bir blok var mesela burada. "B" veya "C"
            var secilenblogunkatları = _db.Apartment.Where(x => x.BlockNo == BlokName).ToList();
            userview.BlogunKatları = secilenblogunkatları;
            userview.SelectedBlokName = BlokName;


            // Kata ait daireleri getiriyor
            var secilenkatındaireleri = _db.Apartment.Where(x => x.FloorNo == FloorNo && x.BlockNo == BlokName).ToList();
            userview.KatınDaireleri = secilenkatındaireleri;
            userview.SelectedFloorNo = FloorNo;

            // Daireyi seçiyoruz
            userview.SelectedApartmentNo = ApartmentNo;

            return View(userview);
        }



        [HttpPost]
        public async Task<IActionResult> KullaniciEklemePost(string BlokName, string FloorNo, string ApartmentNo, string Name, string Surname, string Mail, string PhoneNumber, string password, string PaymentPreference)
        {
            var secilenDaire = _db.Apartment.FirstOrDefault(x => x.BlockNo == BlokName && x.FloorNo == FloorNo && x.ApartmentNo == ApartmentNo);
            secilenDaire.IsOccupied = 1;


            //.IgnoreQueryFilters() kullanmamamızın sebebi, Birnevi bizim yaptığımız her sorguda tenantsıd var ya. Bu sorguda onun çalışmasını engelliyor.
            // Onu yazmadan yaparsak sorguyu sorguda otomatikmen where(x=>x.Tenantsıd=1) çalışıyor 
            // Yani sadeve kendi apartmanımıza bakmış oluruz öyle yaparsak o da bizim işimize gelmez bize toplam bütün veritabanındakiler lazım.
            var mailKontrol = _db.Users.IgnoreQueryFilters().Any(x => x.Mail == Mail); // Eğer girilen mailden başka varsa oluşturtmasın
            if (mailKontrol)
            {
                TempData["Error"] = "Bu e-posta adresi zaten kullanılıyor. Lütfen başka bir e-posta girin.";
                return RedirectToAction("kullaniciekleme");
            }


            var kullanici = new User
            {
                Apartment_Id = secilenDaire.Id,
                Name = Name,
                Surname = Surname,
                Mail = Mail,
                PhoneNumber = PhoneNumber,
                Password = password,
                PaymentPreference = PaymentPreference,
                IsActive = 1,
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(kullanici);
            _db.SaveChanges();


            var KullaniciRolü = new UserRoles
            {
                User_Id = kullanici.Id,
                Roles_Id = 1
            };

            var cuzdan = new Wallet
            {
                Apartment_Id = secilenDaire.Id,
                Balance = 0
            };

            _db.Wallet.Add(cuzdan);

            _db.UserRoles.Add(KullaniciRolü);


            var sakin = _db.Users.FirstOrDefault(x => x.Apartment_Id == kullanici.Apartment.Id);
            if (sakin != null && sakin.Mail != null)
            {
                var body = _templateService.LoadTemplate("Hosgeldinmail", new Dictionary<string, string>
                    {
                        { "Name", sakin.Name },
                        { "Surname", sakin.Surname },
                        { "Floor", FloorNo },
                        { "Block", BlokName },
                        { "ApartmentNo", ApartmentNo },
                        { "Mail", Mail },
                        { "Password", password }
                    });

                await _emailService.SendEmailAsync(Mail, Name + " " + Surname, "Hesabınız Oluşturuldu - Hoş Geldiniz", body);

            }








            _db.SaveChanges();

            return RedirectToAction("Residents");
        }





        [HttpPost]
        public IActionResult KullaniciSilme(int id)
        {
            if (_db.UserRoles.Any(x => x.User_Id == id && x.Roles_Id == 2))
            {
                TempData["ErrorMessage"] = "Yönetici hesabı silinemez.";
                return RedirectToAction("Residents");
            }


            else
            {
                var silinmekistenenkullanici = _db.Users.FirstOrDefault(x => x.Id == id);
                var silinmekistenenkullanicininrolleri = _db.UserRoles.FirstOrDefault(x => x.User_Id == id);
                var silinmekistenenkullaniciDaire = _db.Apartment.FirstOrDefault(x => x.User.Id == id);
                var silinmekistenenkullanicinincuzdani = _db.Wallet.FirstOrDefault(x => x.Apartment_Id == silinmekistenenkullaniciDaire.Id);

                var silinmekistenenkullaniciCuzdanhareketleri = _db.WalletTransactions.Where(x => x.Wallet_Id == silinmekistenenkullanicinincuzdani.Id).ToList();
                var silinmekistenenkisininaidatları = _db.Dues.Where(x => x.Apartment_Id == silinmekistenenkullaniciDaire.Id).ToList();
                var silinmekistenenkisininBakiyeİstekleri = _db.BalanceRequests.Where(x => x.Apartment_Id == silinmekistenenkullaniciDaire.Id).ToList();


                silinmekistenenkullaniciDaire.IsOccupied = 0;
                _db.UserRoles.Remove(silinmekistenenkullanicininrolleri);
                _db.Users.Remove(silinmekistenenkullanici);
                _db.Wallet.Remove(silinmekistenenkullanicinincuzdani);
                _db.WalletTransactions.RemoveRange(silinmekistenenkullaniciCuzdanhareketleri);
                _db.Dues.RemoveRange(silinmekistenenkisininaidatları);
                _db.BalanceRequests.RemoveRange(silinmekistenenkisininBakiyeİstekleri);

                _db.SaveChanges();
            }

            return RedirectToAction("Residents");
        }











        [HttpGet]
        public IActionResult KullaniciGuncelleme(int id, string BlokName, string FloorNo, string ApartmentNo)
        {
            // Bu kısım _AdminLayout.cshtml için gerekli olan kısım
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userview = new UserViewModel();

            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            userview.user = user;

            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            userview.apartment = apartment;

            // Guncellenmek İstenen Kullanıcı

            var guncellenmekIstenenKullanici = _db.Users.Include(x => x.Apartment).FirstOrDefault(x => x.Id == id);

            userview.guncellenecekKullanici = guncellenmekIstenenKullanici;



            // Apartman Kısmı
            // Blokları Getiriyor

            var apartmanlar = _db.Apartment.Include(x => x.User).Where(x => x.IsOccupied == 0 /*|| x.Id==guncellenmekIstenenKullanici.Apartment.Id*/).ToList();

            userview.ApartmanlarListesi = apartmanlar;



            // Bloğa ait katları getiriyor
            var secilenblogunkatları = _db.Apartment.Where(x => x.BlockNo == BlokName /*|| x.Id == guncellenmekIstenenKullanici.Apartment.Id*/).ToList();
            userview.BlogunKatları = secilenblogunkatları;
            userview.SelectedBlokName = BlokName ?? guncellenmekIstenenKullanici?.Apartment?.BlockNo;





            // Kata ait daireleri getiriyor
            var secilenkatındaireleri = _db.Apartment.Where(x => x.FloorNo == FloorNo && x.BlockNo == BlokName).ToList();
            userview.KatınDaireleri = secilenkatındaireleri;
            userview.SelectedFloorNo = FloorNo ?? guncellenmekIstenenKullanici?.Apartment?.FloorNo;

            // Daireyi seçiyoruz
            userview.SelectedApartmentNo = ApartmentNo ?? guncellenmekIstenenKullanici?.Apartment?.ApartmentNo;


            return View(userview);
        }


        public IActionResult kullaniciguncellemepost(int id, string BlokName, string FloorNo, string ApartmentNo, string Name, string Surname, string Mail, string PhoneNumber, string password, string PaymentPreference)
        {
            var dbdekiKullanici = _db.Users.Include(x => x.Apartment).FirstOrDefault(x => x.Id == id);

            var yeniDaire = _db.Apartment.FirstOrDefault(x => x.BlockNo == BlokName && x.FloorNo == FloorNo && x.ApartmentNo == ApartmentNo);

            if (yeniDaire != null)
            {
                if (dbdekiKullanici.Apartment_Id != yeniDaire?.Id) //Yani kullanıcı dairesini değiştirmişse.
                {
                    dbdekiKullanici.Apartment_Id = yeniDaire.Id;
                    yeniDaire.IsOccupied = 1;

                    var cuzdan = new Wallet
                    {
                        Apartment_Id = yeniDaire.Id,
                        Balance = 0
                    };

                    _db.Wallet.Add(cuzdan);

                    if (dbdekiKullanici.Apartment != null)
                    {
                        dbdekiKullanici.Apartment.IsOccupied = 0;
                        var eskiaptcuzdan = _db.Wallet.FirstOrDefault(x => x.Apartment_Id == dbdekiKullanici.Apartment.Id);
                        _db.Wallet.Remove(eskiaptcuzdan);

                        var yeniaptcuzdan = new Wallet
                        {
                            Apartment_Id = yeniDaire.Id,
                            Balance = 0
                        };

                        _db.Wallet.Add(yeniaptcuzdan);


                        var dbdekikullanicininmevcutaidatlari = _db.Dues.Where(x => x.Apartment_Id == dbdekiKullanici.Apartment.Id).ToList();

                        foreach (var aidatlar in dbdekikullanicininmevcutaidatlari)
                        {
                            aidatlar.Apartment_Id = yeniDaire.Id;
                        }


                        var dbdekikulanicininmevcutBakiyeİstekleri = _db.BalanceRequests.Where(x => x.Apartment_Id == dbdekiKullanici.Apartment.Id).ToList();

                        foreach (var bakiyeistekleri in dbdekikulanicininmevcutBakiyeİstekleri)
                        {
                            bakiyeistekleri.Apartment_Id = yeniDaire.Id;
                        }

                        var dbdekikullanicininistekvesikayetleri = _db.FaultAndRequest.Where(x => x.Apartment_Id == dbdekiKullanici.Apartment.Id).ToList();

                        foreach (var isteksikayetler in dbdekikullanicininistekvesikayetleri)
                        {
                            isteksikayetler.Apartment_Id = yeniDaire.Id;
                        }

                    }


                    _db.SaveChanges();

                }
            }

            dbdekiKullanici.Apartment_Id = dbdekiKullanici.Apartment_Id;
            dbdekiKullanici.Name = Name;
            dbdekiKullanici.Surname = Surname;
            dbdekiKullanici.Mail = Mail;
            dbdekiKullanici.PhoneNumber = PhoneNumber;
            dbdekiKullanici.Password = password;
            dbdekiKullanici.PaymentPreference = PaymentPreference;

            _db.SaveChanges();


            return RedirectToAction("Residents");
        }


    }
}