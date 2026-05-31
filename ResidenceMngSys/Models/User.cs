using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class User : ITenantEntity
    {
        [Column("İd")]
        public int Id { get; set; }
        [Column("Apartment_İd")]
        public int? Apartment_Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? PaymentPreference { get; set; }
        [Column("İsActive")]
        public int? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Apartment_Id")]
        public virtual Apartment Apartment { get; set; } // Navigation Property
        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property

    }
}
