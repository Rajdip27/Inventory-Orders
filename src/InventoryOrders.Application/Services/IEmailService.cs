using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using InventoryOrders.Application.CommonModel;

namespace InventoryOrders.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage emailMessage);
}
public class EmailService(IConfiguration _config) : IEmailService
{
    public async Task SendEmailAsync(EmailMessage emailMessage)
    {

        try
        {
            if (emailMessage.To.Count == 0)
                throw new ArgumentException("Email must have at least one recipient.");

            using var smtp = new SmtpClient(
                _config["Email:Host"],
                int.Parse(_config["Email:Port"])
            )
            {
                Credentials = new NetworkCredential(
                    _config["Email:Username"],
                    _config["Email:Password"]
                ),
                EnableSsl = bool.Parse(_config["Email:EnableSsl"])
            };

            using var message = new MailMessage
            {
                From = new MailAddress(
                    _config["Email:Username"],
                    _config["Email:FromName"]
                ),
                Subject = emailMessage.Subject,
                IsBodyHtml = !string.IsNullOrEmpty(emailMessage.HtmlFilePath)
            };

            emailMessage.To.ForEach(message.To.Add);
            emailMessage.CC?.ForEach(message.CC.Add);
            emailMessage.BCC?.ForEach(message.Bcc.Add);

            // ✅ HTML BODY FIX
            if (!string.IsNullOrEmpty(emailMessage.HtmlFilePath))
            {
                message.Body = emailMessage.HtmlFilePath; // HTML STRING
            }
            else if (!string.IsNullOrEmpty(emailMessage.Message))
            {
                message.Body = emailMessage.Message;
                message.IsBodyHtml = false;
            }
            else
            {
                message.Body = string.Empty;
            }

            // Attachments
            if (emailMessage.Attachments != null)
            {
                foreach (var attachment in emailMessage.Attachments)
                {
                    var stream = new MemoryStream(attachment.Content);
                    message.Attachments.Add(
                        new Attachment(stream, attachment.FileName, attachment.ContentType)
                    );
                }
            }

            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }
}

//public class TestController : Controller
//{
//    private readonly IEmailService _emailService;

//    public TestController(IEmailService emailService)
//    {
//        _emailService = emailService;
//    }

//    public async Task<IActionResult> SendEmailExample()
//    {
//        // Example 1: HTML email with attachments
//        var htmlEmail = new EmailMessage
//        {
//            To = new List<string> { "to@gmail.com" },
//            CC = new List<string> { "cc@gmail.com" },
//            BCC = null,
//            Subject = "HTML Email Example",
//            HtmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate.html"),
//            Attachments = new List<string> { @"C:\Temp\File1.pdf", @"C:\Temp\File2.txt" }
//        };
//        await _emailService.SendEmailAsync(htmlEmail);

//        // Example 2: Plain message with single attachment
//        var plainEmail = new EmailMessage
//        {
//            To = new List<string> { "to@gmail.com" },
//            Subject = "Plain Message Example",
//            Message = "Hello! This is a plain text email.",
//            Attachments = new List<string> { @"C:\Temp\Document.pdf" }
//        };
//        await _emailService.SendEmailAsync(plainEmail);

//        // Example 3: Plain message without attachment
//        var simpleEmail = new EmailMessage
//        {
//            To = new List<string> { "to@gmail.com" },
//            Subject = "Simple Message",
//            Message = "Just a simple text message."
//        };
//        await _emailService.SendEmailAsync(simpleEmail);

//        return Ok("All emails sent dynamically!");
//    }
//}

