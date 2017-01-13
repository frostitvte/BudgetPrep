using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BudgetPrep.Classes;
using OBDAL;
using OBBAL;

namespace BudgetPrep
{
    public partial class MailInBox : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Inbox";

                BindGrid();
            }
        }

        private void BindGrid()
        {
            gvMails.DataSource = new EventLogBAL().GetInboxList(LoggedInUser);
            gvMails.DataBind();
        }
        
        protected void gvMails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Details")
            {
                foreach (GridViewRow gvr in gvMails.Rows)
                    gvr.Style["background-color"] = "";

                GridViewRow selectedRow = gvMails.Rows[Convert.ToInt32(e.CommandArgument)];
                selectedRow.Style["background-color"] = "gold";
                string Title = gvMails.DataKeys[selectedRow.RowIndex]["Title"].ToString();
                string Object = gvMails.DataKeys[selectedRow.RowIndex]["Object"].ToString();

                if (LoggedInUser.JuncUserRoles.First().RoleID == 1)
                {
                    gvDetail.DataSource = new EventLogBAL().GetMailDetails(Title, Object, LoggedInUser);
                }
                else
                {
                    gvDetail.DataSource = new EventLogBAL().GetMailDetails(Title, Object, LoggedInUser);
                }
                gvDetail.DataBind();
            }
        }
    }
}