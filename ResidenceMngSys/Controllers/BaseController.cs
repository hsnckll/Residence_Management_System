using ResidenceMngSys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
namespace ResidenceMngSys.Controllers
{
    public class BaseController : Controller
    {
        protected readonly AppDbContext _db;

        public BaseController(AppDbContext db)
        {
            _db = db;
        }

      
        protected UserViewModel GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            // 1. Oturum kontrolü
            if (userId == null) return null;

            var user = _db.Users
                .Include(u => u.Apartment)
                .FirstOrDefault(u => u.Id == userId);

            // 2. Kullanıcı veritabanında var mı kontrolü
            if (user == null) return null;

            var wallet = _db.Wallet
                .FirstOrDefault(w => w.Apartment_Id == user.Apartment_Id);

            var duelist = _db.Dues.Where(x => x.Apartment_Id == user.Apartment_Id).OrderByDescending(d => d.Year).ToList();

            return new UserViewModel
            {
                Name = user.Name,
                ApartmentId = user.Apartment_Id ?? 0, //nullable int oldugu için böyle yapmamız lazım eğer nullsa 0 olsun
                Balance = wallet?.Balance ?? 0m,
                Block = user.Apartment?.BlockNo ?? "Bilinmiyor",     
                FloorNo = user.Apartment?.FloorNo ?? "0",
                ApartmentNo = user.Apartment?.ApartmentNo ?? "0",
                SquareMeters = user.Apartment?.SquareMeters ?? "0",
                Surname =user.Surname,
                CreatedAt=user.CreatedAt,
                Password=user.Password,
                IsActive=user.IsActive ?? 0,
                Mail=user.Mail,
                PhoneNumber=user.PhoneNumber,
                Id=user.Id,

                DuesList=duelist,
                TotalDebt = duelist.Any() ? duelist.Sum(d => d.RemainingDebt) : 0
               
            };

        }

    }
}
