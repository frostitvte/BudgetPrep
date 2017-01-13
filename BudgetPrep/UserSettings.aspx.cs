using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BudgetPrep.Classes;
using OBBAL;
using OBDAL;
using OBSecurity;

namespace BudgetPrep
{
    public partial class UserSettings : System.Web.UI.Page
    {
        MasterUser AuthUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthUser = (MasterUser)Session["UserData"];
            if (Session["UserData"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "User";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Settings";
                tbUserName.Text = AuthUser.UserName;
                tbGroupName.Text = AuthUser.MasterGroup.GroupName;
                tbEmail.Text = AuthUser.UserEmail;

                tbSecQuestion.Text = AuthUser.SecQuestion;
                tbSecAnswer.Text = (AuthUser.SecAnswer != null && AuthUser.SecAnswer != string.Empty) ? Security.Decrypt(AuthUser.SecAnswer) : string.Empty;

                if (AuthUser.SecQuestion == null|| AuthUser.SecAnswer == null)
                    lblSecMsg.Visible = true;
                else if (AuthUser.SecQuestion.Trim() == string.Empty || AuthUser.SecAnswer.Trim() == string.Empty)
                    lblSecMsg.Visible = true;
            }
        }

        protected void btnResetPwd_Click(object sender, EventArgs e)
        {
            MasterUser user = new UserBAL().GetValidUser(AuthUser.UserName, tbOldPassword.Text.Trim());
            if (user == null)
            {
                ((SiteMaster)this.Master).ShowMessage("Failure", "Wrong Password");
                return;
            }
            if (new UserBAL().ResetPassword(AuthUser.UserName, tbNewPassword.Text.Trim()))
            {
                bool mail = MailHelper.PasswordChangedMail(AuthUser.UserEmail);
                AuthUser.Passcode = Security.Encrypt(tbNewPassword.Text.Trim());
                Session["UserData"] = AuthUser;
                ((SiteMaster)this.Master).ShowMessage("Success", "User password changed successfully, " + ((mail) ? "Mail Sent" : "Error Sending Mail"));
            }
            else
            {
                ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while changing User password");
            }
        }

        protected void btnSaveSecurityInfo_Click(object sender, EventArgs e)
        {
            if (new UserBAL().SaveSecurityInfo(AuthUser.UserName, tbSecQuestion.Text.Trim(), tbSecAnswer.Text.Trim()))
            {
                AuthUser.SecQuestion = tbSecQuestion.Text.Trim();
                AuthUser.SecAnswer = Security.Encrypt(tbSecAnswer.Text.Trim());
                Session["UserData"] = AuthUser;
                lblSecMsg.Visible = false;
                ((SiteMaster)this.Master).ShowMessage("Success", "User Security information changed successfully");
            }
            else
            {
                ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while changing User Security information");
            }
        }
    }
}