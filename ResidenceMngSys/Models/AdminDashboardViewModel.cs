namespace ResidenceMngSys.Models
{
    public class AdminDashboardViewModel
    {
        public UserViewModel CurrentUser { get; set; }
        public int ToplamSakin { get; set; }
        public decimal ToplamTahsilat { get; set; }
        public decimal BeklenenBorc { get; set; }
        public int BekleyenOnaylar { get; set; }
    }
}
