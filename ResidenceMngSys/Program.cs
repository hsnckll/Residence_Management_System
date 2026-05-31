using ResidenceMngSys.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Hangfire; // YENÝ
using ResidenceMngSys.Services; // YENÝ



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// YENÝ - EmailService kaydý
builder.Services.AddScoped<EmailService>();

// YENÝ - Email Templatelerini okumak için email template service kaydý
builder.Services.AddScoped<EmailTemplateService>();


// YENÝ - TenantService kaydý
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();


// YENÝ - Hangfire kaydý
builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();



// Dosya yükleme limitleri
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

var app = builder.Build();

// Hata loglama, Hata oluþtuðunda hata nerede oldu nasýl ve neyden dolayý oldu onlarýn bilgilerini tutar.
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    File.AppendAllText("crash_log.txt",
        $"{DateTime.Now}: {e.ExceptionObject}\n\n");
};
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

// YENÝ - Hangfire dashboard
app.UseHangfireDashboard("/hangfire");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();