using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using BudgetPrep.Classes;
using OBBAL;
using OBDAL;
using OBSecurity;

namespace BudgetPrep.Account
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            try
            {
                string secQuestion = new UserBAL().GetSecQuestion(tbUserName.Text.Trim());
                if (secQuestion == null)
                    lblMessage.Text = "Invalid User";
                else if (secQuestion == string.Empty)
                    lblMessage.Text = "Security question was not set. Contact your admin";
                else
                {
                    lblMessage.Text = string.Empty;
                    lblQuestion.Text = secQuestion;
                    pnlQuestion.Visible = true;
                    tbUserName.Enabled = false;
                    btnVerify.Enabled = false;
                    btnVerify.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred";
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myResetMsgModal", "$('#myResetMsgModal').modal();", true);
                MasterUser user = new UserBAL().VerifyAnswer(tbUserName.Text.Trim(), tbAnswer.Text.Trim());
                if (user != null)
                {
                    string newpassword = new Helper().GenerateRandomPassword();
                    if (new UserBAL().ResetPassword(tbUserName.Text.Trim(), newpassword))
                    {
                        bool mail = MailHelper.NewPasswordMail(user.UserEmail, newpassword);
                        lblMessage.Text = "Password sent successfully to your email id : " + new Helper().EmailClipper(user.UserEmail);                     
                    }
                    else
                    {
                        lblMessage.Text = "Error";
                    }
                }
                else
                    lblMessage.Text = "Wrong Answer";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred";
            }
        }
    }
}