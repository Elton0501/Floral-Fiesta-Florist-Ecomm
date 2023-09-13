using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HelperController : Controller
    {
        // GET: Helper
        public bool templateEmail(string email, string subject, string head, string body)
        {
            bool result = false;
            try
            {
                var mail = ConfigurationManager.AppSettings["email"];
                var pass = ConfigurationManager.AppSettings["pass"];
                var port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                var stp = ConfigurationManager.AppSettings["smtp"];
                var ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["ssl"]);

                string FilePath = HostingEnvironment.MapPath("~/EmailTemplate/") + "mailtemp" + ".html";
                StreamReader reader = new StreamReader(FilePath);
                string textMail = reader.ReadToEnd();
                reader.Close();

                textMail = textMail.Replace("[head]", head.Trim());
                textMail = textMail.Replace("[content]", body.Trim());

                //textMail = textMail.Replace("[phone]", phone.Trim());
                //textMail = textMail.Replace("[contact]", phone.Trim());
                //textMail = textMail.Replace("[address]", address.Trim());

                MailMessage message = new MailMessage();
                message.From = new MailAddress(mail);
                message.To.Add(email);
                message.Subject = subject;
                message.Body = textMail;

                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(stp, port);
                smtp.Credentials = new NetworkCredential(mail, pass);
                smtp.EnableSsl = ssl;
                smtp.Send(message);
                smtp.Dispose();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
    }
}