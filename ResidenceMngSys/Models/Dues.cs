using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class Dues : ITenantEntity
    {
        [Column("İd")]
        public int Id { get; set; }

        [Column("Apartment_İd")]
        public int Apartment_Id { get; set; } 

        public int Year {  get; set; }
        public string Months {  get; set; }
        public decimal DuesPrice {  get; set; }
        public decimal RemainingDebt { get; set; } 
        [Column("İsPaid")]
        public int IsPaid {  get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? PaymentStartDate {  get; set; }
        public DateTime? PaymentEndDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        [ForeignKey("Apartment_Id")]

        public Apartment Apartment { get; set; }
        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property

    }
}
