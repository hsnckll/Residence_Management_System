using ResidenceMngSys.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
namespace ResidenceMngSys.Controllers

{
    public class ResidentController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ResidentController(AppDbContext db, IWebHostEnvironment hostingEnvironment) : base(db)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index() // controllerdeki fonksiyon ismi view klasöründeki dosya ismi olmalı
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId != null)
            {
                var viewmodel = new UserViewModel();

                var duelist = _db.Dues.Where(x => x.Apartment_Id == apartmentId).ToList();
                var wallet = _db.Wallet.FirstOrDefault(w => w.Apartment_Id == apartmentId);
                var user = _db.Users.Include(x => x.Apartment).FirstOrDefault(x => x.Apartment_Id == apartmentId);
                var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
                viewmodel.wallet = wallet;
                viewmodel.user = user;
                viewmodel.apartment = apartment;
                viewmodel.DuesList = duelist;

                var talepler = _db.BalanceRequests.Where(x => x.Status == "Beklemede").ToList();
                viewmodel.bakiyetalepleri = talepler;
                return View(viewmodel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }



        public IActionResult Payments() // Buranın bağlantılı olması lazım _ResidentLayout.cshtml ile
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId != null)
            {
                var viewmodel = new UserViewModel();

                var duelist = _db.Dues.Where(x => x.Apartment_Id == apartmentId).ToList();
                var wallet = _db.Wallet.FirstOrDefault(w => w.Apartment_Id == apartmentId);
                var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
                var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
                viewmodel.wallet = wallet;
                viewmodel.user = user;
                viewmodel.apartment = apartment;
                viewmodel.DuesList = duelist;

                return View(viewmodel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult odemesayfasi(int id)
        {
            var userviewmodel = new UserViewModel();
            userviewmodel.due = _db.Dues.Where(x => x.Id == id).FirstOrDefault();
            userviewmodel.wallet = _db.Wallet.FirstOrDefault(x => x.Apartment_Id == userviewmodel.due.Apartment_Id);

            return View(userviewmodel);
        }




        [HttpPost]
        public IActionResult odemesayfasipost(int id, decimal amount)
        {
            var userviewmodel = new UserViewModel();
            userviewmodel.due = _db.Dues.Where(x => x.Id == id).FirstOrDefault();
            var aidat = userviewmodel.due;
            userviewmodel.wallet = _db.Wallet.FirstOrDefault(x => x.Apartment_Id == userviewmodel.due.Apartment_Id);
            var cuzdan = userviewmodel.wallet;

            if (amount <= 0) return BadRequest("Geçersiz miktar.");
            if (amount > cuzdan.Balance) return BadRequest("Cüzdan bakiyesi yetersiz.");
            if (amount > aidat.RemainingDebt) amount = aidat.RemainingDebt; // Bu fazla ödeme olmaması için

            cuzdan.Balance -= amount;
            aidat.RemainingDebt -= amount;

            if (aidat.RemainingDebt == 0)
            {
                aidat.IsPaid = 1;
                aidat.PaymentDate = DateTime.Now;

            }


            var hareketler = new WalletTransactions
            {
                Dues_Id = aidat.Id,
                Wallet_Id = cuzdan.Id,
                Amount = amount,
                TransactionType = "Aidat Ödemesi",
                Description = $"{aidat.Months} / {aidat.Year}, {amount} ₺' lik ödeme",
                BalanceAfter = cuzdan.Balance,
                CreatedAt = DateTime.Now,

            };

            _db.WalletTransactions.Add(hareketler);
            _db.SaveChanges();
            return RedirectToAction("Payments");
        }

        public IActionResult ParaYatir()
        {

            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if(apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();
            var wallet = _db.Wallet.FirstOrDefault(w => w.Apartment_Id == apartmentId);
            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            var requests = _db.BalanceRequests.Where(x => x.Apartment_Id == apartmentId).ToList();
            viewmodel.wallet = wallet;
            viewmodel.user = user;
            viewmodel.apartment = apartment;
            viewmodel.RequestsList = requests;

            var odemebilgileri = _db.PaymentsInfo.FirstOrDefault();
            viewmodel.paymentsInfo = odemebilgileri;

            return View(viewmodel);

        }



        [HttpPost]
        public async Task<IActionResult> ParaYatirPost(int amount, string Description, IFormFile ReceiptFile, int id)
        {

            var apartmentId = id;

            string yuklemeyolu = Path.Combine(_hostingEnvironment.WebRootPath, "dekontlar");

            if (!Directory.Exists(yuklemeyolu))
            {
                Directory.CreateDirectory(yuklemeyolu);
            }

            string dosyauzantisi = Path.GetExtension(ReceiptFile.FileName);
            string yenidosyadi = Guid.NewGuid().ToString() + dosyauzantisi;
            string tamdosyayolu = Path.Combine(yuklemeyolu, yenidosyadi);

            //Dosyayı Fiziksel Olarak Kaydetme kısmı
            using (var stream = new FileStream(tamdosyayolu, FileMode.Create))
            {
                await ReceiptFile.CopyToAsync(stream);
            }

            var apartman = _db.Apartment.FirstOrDefault(x => x.Id == id);

            var yatirma = new BalanceRequests
            {
                apartment=apartman,
                Apartment_Id = apartmentId,
                Amount = amount,
                Status = "Beklemede",
                CreatedAt = DateTime.Now,
                Explanation = Description,
                ReceiptImagePath = "/dekontlar/" + yenidosyadi
            };

            _db.BalanceRequests.Add(yatirma);
            await _db.SaveChangesAsync();

            return RedirectToAction("ParaYatir");
        }





        public IActionResult Duyuru() 
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();
            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.user = user;
            viewmodel.apartment = apartment;

            var duyurular = _db.Announcements.ToList();
            viewmodel.duyurular = duyurular;


            return View(viewmodel);
        }




        [HttpGet]
        public IActionResult Sikayet()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();
            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.user = user;
            viewmodel.apartment = apartment;


            var sikayetler = _db.FaultAndRequest.Where(x => x.Type == "Şikayet"&&x.Apartment_Id==apartmentId).ToList();

            viewmodel.sikayetler = sikayetler;

            return View(viewmodel);
        }



        public IActionResult sikayetolustur() {

            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();
            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.user = user;
            viewmodel.apartment = apartment;

            return View(viewmodel);
        }




        [HttpPost]
        public async Task<IActionResult> sikayetolusturpost(string konu, string icerik, string Status, IFormFile ImagePath, int id)
        {
            string dosyaAdi = null;
            string resim = null;

            if (ImagePath != null)
            {
                string yuklemeyolu = Path.Combine(_hostingEnvironment.WebRootPath, "sikayetler");

                if (!Directory.Exists(yuklemeyolu))
                {
                    Directory.CreateDirectory(yuklemeyolu);
                }

                string dosyauzantisi = Path.GetExtension(ImagePath.FileName);
                dosyaAdi = Guid.NewGuid().ToString() + dosyauzantisi;
                string tamdosyayolu = Path.Combine(yuklemeyolu, dosyaAdi);

                //Dosyayı Fiziksel Olarak Kaydetme kısmı
                using (var stream = new FileStream(tamdosyayolu, FileMode.Create))
                {
                    await ImagePath.CopyToAsync(stream);
                }

                resim = "/sikayetler/" + dosyaAdi;
            }

            var sikayetedenkisininapartman = _db.Apartment.FirstOrDefault(x => x.Id == id);
            // where kullanırsan liste bekler, firstordefault tekil bir çıktı verir
            var sikayet = new FaultAndRequest
            {
                Apartment=sikayetedenkisininapartman,
                Apartment_Id = id,
                Type = "Şikayet",
                Topic = konu,
                Details = icerik,
                Status = "Beklemede",
                CreatedAt=DateTime.Now,
                ImagePath = resim
            };

            _db.FaultAndRequest.Add(sikayet);
            await _db.SaveChangesAsync();


            return RedirectToAction("Sikayet");
        }














        [HttpGet]
        public IActionResult Oneri()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();
            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.user = user;
            viewmodel.apartment = apartment;

            var oneriler = _db.FaultAndRequest.Where(x => x.Type == "Öneri").ToList();
            viewmodel.oneriler = oneriler;

            return View(viewmodel);
        }







        public IActionResult oneriolustur()
        {
            var apartmentId = HttpContext.Session.GetInt32("ApartmentId");
            if (apartmentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewmodel = new UserViewModel();
            var user = _db.Users.FirstOrDefault(x => x.Apartment_Id == apartmentId);
            var apartment = _db.Apartment.FirstOrDefault(x => x.Id == apartmentId);
            viewmodel.user = user;
            viewmodel.apartment = apartment;
            return View(viewmodel);

        }




        [HttpPost]
        public async Task<IActionResult> oneriolusturpost(string konu, string icerik, IFormFile ImagePath, int id, string Status)
        {
            string dosyaAdi = null;
            string resim = null;

            if (ImagePath != null)
            {
                string yuklemeyolu = Path.Combine(_hostingEnvironment.WebRootPath, "oneriler");

                if (!Directory.Exists(yuklemeyolu))
                {
                    Directory.CreateDirectory(yuklemeyolu);
                }

                string dosyauzantisi = Path.GetExtension(ImagePath.FileName);
                dosyaAdi = Guid.NewGuid().ToString() + dosyauzantisi;
                string tamdosyayolu = Path.Combine(yuklemeyolu, dosyaAdi);

                //Dosyayı Fiziksel Olarak Kaydetme kısmı
                using (var stream = new FileStream(tamdosyayolu, FileMode.Create))
                {
                    await ImagePath.CopyToAsync(stream);
                }

                resim = "/oneriler/" + dosyaAdi;
            }

            var onerigonderenkisininapartman = _db.Apartment.FirstOrDefault(x => x.Id == id);


            var oneri = new FaultAndRequest
            {
                Apartment = onerigonderenkisininapartman,
                Apartment_Id = id,
                Type = "Öneri",
                Topic = konu,
                Details = icerik,
                Status = "Beklemede",
                CreatedAt = DateTime.Now,
                ImagePath = resim
            };


            _db.FaultAndRequest.Add(oneri);
            await _db.SaveChangesAsync();


            return RedirectToAction("Oneri");
        }
    }
}
