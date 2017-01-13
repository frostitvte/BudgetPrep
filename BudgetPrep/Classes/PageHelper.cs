
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using OBDAL;

namespace BudgetPrep.Classes
{
    public class PageHelper : System.Web.UI.Page
    {
        //public MasterUser LoggedInUser = new MasterUser();
        public MasterUser LoggedInUser { get; set; }
        public PageHelper()
        {
            LoggedInUser = new MasterUser();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["UserData"] == null)
            {
                ClearSession();
            }
            else
            {
                LoggedInUser = (MasterUser)Session["UserData"];
                if (LoggedInUser.SecQuestion == null || LoggedInUser.SecAnswer == null)
                    Response.Redirect("~/UserSettings.aspx");
                if (LoggedInUser.SecQuestion.Trim() == string.Empty || LoggedInUser.SecAnswer.Trim() == string.Empty)
                    Response.Redirect("~/UserSettings.aspx");
            }

            List<PageMenuHelper> lstPages = (List<PageMenuHelper>)Session["ListPages"];
            if (!Request.Path.ToUpper().Contains(("Default.aspx").ToUpper()) && !Request.Path.ToUpper().Contains(("MailInBox.aspx").ToUpper()))
            {
                if (lstPages.Where(x => Request.Path.ToUpper().Contains(x.PagePath.Replace("./", "").Trim().ToUpper())).Count() == 0)
                {
                    ClearSession();
                }
            }
        }

        private void ClearSession()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Clear();
            Response.Redirect("~/Account/Login.aspx");
        }
    }
}