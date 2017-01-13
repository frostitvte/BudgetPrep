using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBBAL;
using OBDAL;

namespace BudgetPrep.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!Page.IsPostBack)
            {
                System.Web.Security.FormsAuthentication.SignOut();
                Session.Clear();
            }
        }

        protected void LoginUser_Authenticate(object sender, AuthenticateEventArgs e)
        {
            MasterUser user = new UserBAL().GetValidUser(LoginUser.UserName, LoginUser.Password);
            if (user != null)
            {
                Session["UserData"] = user;
                CreateMenu(user); 
                e.Authenticated = true;
                //Response.Redirect("~/Default.aspx");
            }
            else
            {
                e.Authenticated = false;
            }
        }

        private void CreateMenu(MasterUser AuthUser)
        {
            List<PageMenuHelper> lstPages = new List<PageMenuHelper>();
            foreach (JuncUserRole jr in AuthUser.JuncUserRoles)
            {
                foreach (JuncRolePage rp in jr.MasterRole.JuncRolePages.Where(x => x.Status == "A"))
                {
                    if (lstPages.Where(x => x.PageID == rp.PageID).Count() == 0)
                    {
                        lstPages.Add(new PageMenuHelper()
                        {
                            PageID = (int)rp.PageID,
                            PageName = rp.MasterPage.PageName,
                            PagePath = rp.MasterPage.PagePath,
                            ParentPageID = (rp.MasterPage.ParentPageID != null) ? (int)rp.MasterPage.ParentPageID : 0,
                            PageOrder = rp.MasterPage.PageOrder,
                            MenuID = (int)rp.MasterPage.MenuID,
                            MenuName = rp.MasterPage.MasterMenu.MenuName,
                            MenuIcon = rp.MasterPage.MasterMenu.MenuIcon,
                            MenuOrder = rp.MasterPage.MasterMenu.MenuOrder
                        });
                    }
                }
            }
            Session["ListPages"] = lstPages;
        }
    }
}
