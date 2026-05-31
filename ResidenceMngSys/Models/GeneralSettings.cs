using System.ComponentModel.DataAnnotations.Schema;


namespace ResidenceMngSys.Models
{
    public class GeneralSettings : ITenantEntity
    {
        public int Id { get; set; }
        public string ResidenceName { get; set; }
        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
