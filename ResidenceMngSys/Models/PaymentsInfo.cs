using System.ComponentModel.DataAnnotations.Schema;


namespace ResidenceMngSys.Models
{
    public class PaymentsInfo : ITenantEntity
    {
        public int Id { get; set; }
        public string IbanNumber { get; set; }
        public string AccountHolder { get; set; }
        public string BankName { get; set; }    
        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property

    }
}
