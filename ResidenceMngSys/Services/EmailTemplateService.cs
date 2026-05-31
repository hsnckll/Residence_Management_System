using ResidenceMngSys.Models;

namespace ResidenceMngSys.Services
{
    public class EmailTemplateService
    {
        private readonly string _templatePath;
        private readonly AppDbContext _db;
        private readonly ITenantService _tenantService;
        public EmailTemplateService(IWebHostEnvironment env, AppDbContext db, ITenantService tenantService)
        {
            _templatePath = Path.Combine(env.ContentRootPath, "EmailTemplates");
            _db = db;
            _tenantService = tenantService;
        }

        public string LoadTemplate(string templateName, Dictionary<string, string> placeholders)
        {
            // 1. Ana şablonu oku
            var basePath = Path.Combine(_templatePath, "BaseTemplate.cshtml");
            var baseHtml = File.ReadAllText(basePath);

            // 2. İçerik şablonunu oku
            var contentPath = Path.Combine(_templatePath, templateName + ".cshtml");
            var contentHtml = File.ReadAllText(contentPath);

            // 3. Ana şablonun içine içeriği yerleştir
            var fullHtml = baseHtml.Replace("{{CONTENT}}", contentHtml);

            var tenantId = _tenantService.GetTenantId();
            var tenant = _db.Tenants.FirstOrDefault(x => x.Id == tenantId);
            fullHtml = fullHtml.Replace("{{TenantName}}", tenant?.Name ?? "ResiEase");



            foreach (var item in placeholders)
            {
                fullHtml = fullHtml.Replace("{{" + item.Key + "}}", item.Value);
            }

            return fullHtml;
        }
    }
}
