using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ResidenceMngSys.Services  // Burası olmadığı için Programcs'de çağıramadık. Bunu yazınca çağırabildik.
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var settings = _config.GetSection("EmailSettings");// appsettings.json dosyasındaki EmailSettings bölümünü alır.

            var message = new MimeMessage(); // MimeMessage sınıfı kullanarak boş bir e-posta mesajı oluştururuz.
            message.From.Add(new MailboxAddress(settings["SenderName"],settings["SenderEmail"])); // Gönderen bilgisi
            message.To.Add(new MailboxAddress(toName, toEmail)); // Alıcı bilgisi
            message.Subject = subject; // Maildeki Konu kısmı

            message.Body = new TextPart("html") // Mailin içeriği, html formatında
            {
                Text = htmlBody
            };

            using var client = new SmtpClient(); // SmtpClient sınıfını kullanarak Gmail'e bağlanacağız.
            await client.ConnectAsync(settings["SmtpServer"],int.Parse(settings["SmtpPort"]),SecureSocketOptions.StartTls); 

            await client.AuthenticateAsync(settings["SenderEmail"], settings["Password"]); // Gmail Hesabına giriş yap
            await client.SendAsync(message); //Maili gönder
            await client.DisconnectAsync(true); // Bağlantıyı kapat
        }
    } 
}      