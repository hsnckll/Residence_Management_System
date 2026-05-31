using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class Resident
    {
        [Column("İd")]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Initials { get; set; }
        public string Block { get; set; }
        public string ApartmentNo { get; set; }
        public string Username { get; set; }
        public decimal Balance { get; set; }
        public decimal Debt { get; set; }
        public DateTime RegisterDate { get; set; }
        public int? TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
