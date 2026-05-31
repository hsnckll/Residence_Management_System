namespace ResidenceMngSys.Models
{
    public class LoginModel : ITenantEntity
    {
        public string Email {  get; set; }
        public string Password { get; set; }

        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
