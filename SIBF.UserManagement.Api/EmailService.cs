using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SIBF.UserManagement.Api
{
    public class EmailService
    {
        private String smtpHost;
        private int port;
        private MailAddress fromAddress;
        private String password;

        public EmailService(String smtpHost, int port, String fromAddress, String password)
        {
            this.smtpHost = smtpHost;
            this.port = port;
            this.fromAddress = new MailAddress(fromAddress, "admin");
            this.password = password;
        }

        public void SendMail(String toAddress, String toName, String subject, String body)
        {
            //var fromAddress = new MailAddress("khussain7@gmail.com", "Khizar Hussain");
            var _toAddress = new MailAddress(toAddress, toName);
            //const string fromPassword = "Khizar1984";

            var smtp = new SmtpClient
            {
                Host = smtpHost,//"smtp.gmail.com",
                Port = port,//587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, password)
            };
            using (var message = new MailMessage(fromAddress, _toAddress)
            {                
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
