using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class Wallet : ITenantEntity
    {
        [Column("İd")]
        public int Id {  get; set; }

        [Column("Apartment_İd")]
        
        public int Apartment_Id {  get; set; }

        public decimal Balance {  get; set; }
        public Apartment? apartment { get; set; }// Forgein key gibi düşün her apartmanın sadece bir cüzdanı vardır

        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
