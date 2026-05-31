using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ResidenceMngSys.Models
{
    public class Announcements: ITenantEntity
    {
        [Column("İd")]
        public int Id { get; set; }
        public string Title {  get; set; }
        public string Contents {  get; set; }
        public string ImportanceStatus { get; set; }
        public DateTime CreatedAt {  get; set; }
        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
