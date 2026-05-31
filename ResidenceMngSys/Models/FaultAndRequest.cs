using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class FaultAndRequest : ITenantEntity
    {
        [Column("İd")]
        public int Id {  get; set; }
        [Column("Apartment_İd")]
        public int Apartment_Id {  get; set; }
        public string Type {  get; set; }
        public string Topic { get; set; }
        public string Details {  get; set; }
        public string Status {  get; set; }
        public string? ImagePath { get; set; }
        public string? AdminAnswer { get; set; }
        public DateTime? CompletionAt { get; set; }

        [ForeignKey("Apartment_Id")]
        public Apartment? Apartment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
