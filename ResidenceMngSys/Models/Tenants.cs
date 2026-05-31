namespace ResidenceMngSys.Models
{
    public class Tenants
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }

        public DateTime? CreatedAt { get; set; }
        public int? IsActive { get; set; }
    }
}
