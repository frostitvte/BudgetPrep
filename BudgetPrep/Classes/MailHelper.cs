using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using OBSecurity;

namespace BudgetPrep.Classes
{
    public class MailHelper
    {
        public static bool NewPasswordMail(string ToMailID, string NewPassword)
        {
            try
            {
                string SuperAdminMail = WebConfigurationManager.AppSettings["SuperAdminMail"];
                string SuperAdminMailPwd = Security.Decrypt(WebConfigurationManager.AppSettings["SuperAdminMailPwd"]);

                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(SuperAdminMail, SuperAdminMailPwd);
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(SuperAdminMail);
                msg.To.Add(new MailAddress(ToMailID));
                msg.Subject = "Reset Password - Budget Prep";
                msg.IsBodyHtml = true;
                msg.Body = string.Format("<html><head></head><body>New Password : <b>" + NewPassword + "</b></body>");
                client.Send(msg);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public static bool PasswordChangedMail(string ToMailID)
        {
            try
            {
                string SuperAdminMail = WebConfigurationManager.AppSettings["SuperAdminMail"];
                string SuperAdminMailPwd = Security.Decrypt(WebConfigurationManager.AppSettings["SuperAdminMailPwd"]);

                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(SuperAdminMail, SuperAdminMailPwd);
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(SuperAdminMail);
                msg.To.Add(new MailAddress(ToMailID));
                msg.Subject = "Password Changed - Budget Prep";
                msg.IsBodyHtml = true;
                msg.Body = string.Format("<html><head></head><body>Your password Changed Successfully...</body>");
                client.Send(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
    }
}