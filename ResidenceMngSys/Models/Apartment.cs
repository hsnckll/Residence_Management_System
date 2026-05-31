using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    
    public class Apartment : ITenantEntity
    {
        [Column("İd")]
        
        public int Id { get; set; }
        public string? BlockNo { get; set; } // Avaible null
        public string FloorNo { get; set; }
        public string ApartmentNo { get; set; }
        public string? Type { get; set; } // Availbe null
        public string SquareMeters { get; set; }
        [Column("İsOccupied")]
        public int IsOccupied { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; } // Navigation Property 

        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
