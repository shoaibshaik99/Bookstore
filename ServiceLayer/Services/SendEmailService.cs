using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ModelLayer.Models
{
    public class SendEmailService
    {
        public string SendMail (string ToEmail, string Token)
        {
            string FromEmail = "smdshoaib137@gmail.com";
            MailMessage mailMessage = new MailMessage(FromEmail, ToEmail);
            string mailBody = "The token for reset password is \n\n" + Token;
            mailMessage.Subject = "Reset Password Token";
            mailMessage.Body = mailBody;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com",587);
            NetworkCredential credential = new NetworkCredential(FromEmail  , "naop xouh mglo mmph");

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = credential;

            smtpClient.Send(mailMessage);
            return ToEmail;
        }
    }
}
