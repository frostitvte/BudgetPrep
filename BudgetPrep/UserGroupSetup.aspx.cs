using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BudgetPrep.Classes;
using OBBAL;
using OBDAL;

namespace BudgetPrep
{
    public class GroupExport
    {
        public string GroupName { get; set; }
        public string Status { get; set; }
    }

    public partial class UserGroupSetup : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "User Group";

                GetData();
                Session["MasterGroupPageMode"] = Helper.PageMode.New;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ChangePageMode(Helper.PageMode.New);
            ClearPageData();
            EditBox.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if ((Helper.PageMode)Session["MasterGroupPageMode"] == Helper.PageMode.New)
                {
                    if (new UserGroupBAL().GetMasterGroups().Where(x => x.GroupName.ToUpper().Trim() == tbGroupName.Text.ToUpper().Trim()).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Group already exists");
                        return;
                    }
                    MasterGroup objMasterGroup = new MasterGroup();
                    objMasterGroup.GroupName = tbGroupName.Text.Trim();
                    objMasterGroup.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objMasterGroup.CreatedBy = LoggedInUser.UserID;
                    objMasterGroup.CreatedTimeStamp = DateTime.Now;
                    objMasterGroup.ModifiedBy = LoggedInUser.UserID;
                    objMasterGroup.ModifiedTimeStamp = DateTime.Now;

                    if(new UserGroupBAL().InsertMasterGroup(objMasterGroup))
                        ((SiteMaster)this.Master).ShowMessage("Success", "User Group saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving User Group");
                }
                else if ((Helper.PageMode)Session["MasterGroupPageMode"] == Helper.PageMode.Edit)
                {
                    MasterGroup objMasterGroup = (MasterGroup)Session["SelectedMasterGroup"];
                    objMasterGroup.GroupName = tbGroupName.Text.Trim();
                    objMasterGroup.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objMasterGroup.ModifiedBy = LoggedInUser.UserID;
                    objMasterGroup.ModifiedTimeStamp = DateTime.Now;

                    if(new UserGroupBAL().UpdateMasterGroup(objMasterGroup))
                        ((SiteMaster)this.Master).ShowMessage("Success", "User Group updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating User Group");
                }

                GetData();
                ClearPageData();
                EditBox.Visible = false;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ChangePageMode(Helper.PageMode.New);
            ClearPageData();
            EditBox.Visible = false;
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                DataSet ds = new DataSet();
                ds.Tables.Add(new ReportHelper().ToDataTable<GroupExport>(
                        ((List<MasterGroup>)Session["MasterGroupData"])
                            .Select(x => new GroupExport()
                            {
                                GroupName = x.GroupName,
                                Status = ((x.Status == "A") ? "Active" : "Inactive")
                            })
                            .OrderBy(x => x.GroupName)
                    ));
                new ReportHelper().ToExcel(ds, "Group.xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvUserGroup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();

                    GridViewRow selectedRow = gvUserGroup.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    MasterGroup objMasterGroup = new MasterGroup();
                    objMasterGroup.GroupID = Convert.ToInt32(gvUserGroup.DataKeys[selectedRow.RowIndex]["GroupID"]);
                    objMasterGroup.GroupName = selectedRow.Cells[0].Text;
                    objMasterGroup.Status = selectedRow.Cells[1].Text;

                    Session["SelectedMasterGroup"] = objMasterGroup;

                    tbGroupName.Text = objMasterGroup.GroupName;
                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objMasterGroup.Status))).Selected = true;

                    ChangePageMode(Helper.PageMode.Edit);
                    EditBox.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void GetData()
        {
            try
            {
                List<MasterGroup> data = new UserGroupBAL().GetMasterGroups();

                Session["MasterGroupData"] = data;
                BindGrid();
                LoadDropDown();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void BindGrid()
        {
            try
            {
                gvUserGroup.DataSource = (List<MasterGroup>)Session["MasterGroupData"];
                gvUserGroup.DataBind();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void LoadDropDown()
        {
            try
            {
                ddlStatus.DataSource = Enum.GetValues(typeof(Helper.ItemStatus));
                ddlStatus.DataBind();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void ChangePageMode(Helper.PageMode pagemode)
        {
            switch (pagemode)
            {
                case Helper.PageMode.New:
                    tbGroupName.Enabled = true;
                    break;
                case Helper.PageMode.Edit:
                    tbGroupName.Enabled = false;
                    break;
            }
            Session["MasterGroupPageMode"] = pagemode;
        }

        private void ClearPageData()
        {
            try
            {
                tbGroupName.Text = string.Empty;
                ddlStatus.SelectedIndex = 0;

                foreach (GridViewRow gvr in gvUserGroup.Rows)
                    gvr.Style["background-color"] = "";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }
    }
}