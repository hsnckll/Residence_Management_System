using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class BalanceRequests : ITenantEntity
    {
        [Column("İd")]
        public int Id { get; set; }
        [Column("Apartment_İd")]
        public int Apartment_Id {  get; set; }
        public int Amount {  get; set; }
        [Column("ReceiptİmagePath")]
        public string? ReceiptImagePath {  get; set; }
        public string Status {  get; set; }
        public DateTime CreatedAt {  get; set; }
        public DateTime? ApprovedAt {  get; set; }

        public string? Explanation { get; set; }
        [ForeignKey("Apartment_Id")]
        public Apartment apartment { get; set; }

        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
