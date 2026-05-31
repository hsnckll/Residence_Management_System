namespace ResidenceMngSys.Services
{
    public interface ITenantService
    {
        int GetTenantId();
    }

    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetTenantId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("TenantId") ?? 0;
        }
    }

}
