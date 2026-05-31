using ResidenceMngSys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ResidenceMngSys.Controllers
{
    
    public class AccountController : Controller
    {       
                
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }



        [HttpGet]
        public IActionResult Login()
        {
            var viewmodel = new UserViewModel();

            viewmodel.generalSettings = _db.GeneralSettings.IgnoreQueryFilters().FirstOrDefault();

            return View(viewmodel);
        }

        

        [HttpPost]

        public IActionResult Login(string role, string username, string password, Wallet wallet)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Lütfen kullanıcı adı ve şifre giriniz.";
                return View();
            }


            // 2. Rol kontrolü ve yönlendirme
            // Formdan gelen 'role' değeri "admin" ise Admin'e, değilse Resident'a gönderir.
            var user = _db.Users.IgnoreQueryFilters().Include(x=>x.Tenant).FirstOrDefault(u => u.Mail == username && u.Password == password);

            var girisyapankisinincuzdanı=_db.Wallet.IgnoreQueryFilters().FirstOrDefault(x=>x.Apartment_Id == user.Apartment_Id);

           
            


            if (user == null)
            {
                TempData["Error"] = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }

            var userHasRole = (from ur in _db.UserRoles.IgnoreQueryFilters()
                               join r in _db.Roles on ur.Roles_Id equals r.Id
                               where ur.User_Id == user.Id && r.RoleName == role
                               select r).Any();
            //var userdaire = _db.Users.Include(u => u.Apartment).FirstOrDefault(u => u.Id == user.Id);

            if (!userHasRole)
            {
                TempData["Error"] = "Bu alan için yetkiniz bulunmamaktadır.";
                return View();
            }



            if (user.Tenant.IsActive == 0)
            {
                TempData["Error"] = "Hesabınız aktif değildir. Lütfen yönetici ile iletişime geçiniz.";
                return View();
            }






            HttpContext.Session.SetInt32("ApartmentId", user.Apartment_Id ?? 0);
            HttpContext.Session.SetInt32("TenantId", user.TenantId); // TenantId null olabilir, bu yüzden operator kullanarak 0 atıyoruz.

            if (role.ToLower() == "admin")
            {
               
                return RedirectToAction("Index", "Admin");
            }
            else if (role.ToLower() == "resident")
            {
                if (girisyapankisinincuzdanı == null)
                {
                    TempData["Error"] = "Cüzdanınız bulunamadı. Lütfen yönetici ile iletişime geçiniz.";
                    return View();
                }

                return RedirectToAction("Index", "Resident");
            }

            return RedirectToAction("Index", "Home");



        }









        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }



    }
}
