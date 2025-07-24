using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

public class EmailService
{
    public static void Send(string to, string subject, string body)
    {
        string fromEmail = "ttttrucs1511@gmail.com";
        string appPassword = "zmfq bysw zydp wgut";

        // Tạo đối tượng email
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(fromEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        // Gửi email bằng SmtpClient của MailKit
        using (var smtp = new SmtpClient())
        {
            // Kết nối đến server SMTP của Gmail
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            // Đăng nhập bằng email và mật khẩu ứng dụng
            smtp.Authenticate(fromEmail, appPassword);

            // Gửi email
            smtp.Send(email);

            // Ngắt kết nối
            smtp.Disconnect(true);
        }
    }
}