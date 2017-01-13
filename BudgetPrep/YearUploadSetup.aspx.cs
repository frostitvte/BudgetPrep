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
    public partial class YearUploadSetup : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Budget Year Upload";

                GetData();
                Session["YearUploadPageMode"] = Helper.PageMode.New;
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
                if ((Helper.PageMode)Session["YearUploadPageMode"] == Helper.PageMode.New)
                {
                    if (new YearUploadBAL().GetYearUpload()
                        .Where(x => x.BudgetYear == Convert.ToInt32(ddlYear.SelectedValue)).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Budget Year already exists");
                        return;
                    }

                    OBDAL.YearUploadSetup objYearUpload = new OBDAL.YearUploadSetup();
                    objYearUpload.BudgetYear = Convert.ToInt32(ddlYear.SelectedValue);
                    objYearUpload.BudgetYearDesc = tbDesc.Text;
                    objYearUpload.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objYearUpload.CreatedBy = LoggedInUser.UserID;
                    objYearUpload.CreatedTimeStamp = DateTime.Now;
                    objYearUpload.ModifiedBy = LoggedInUser.UserID;
                    objYearUpload.ModifiedTimeStamp = DateTime.Now;

                    if (new YearUploadBAL().InsertYearUpload(objYearUpload))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget Year Processed successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while processing Budget Year");
                }
                else if ((Helper.PageMode)Session["YearUploadPageMode"] == Helper.PageMode.Edit)
                {
                    OBDAL.YearUploadSetup objYearUpload = (OBDAL.YearUploadSetup)Session["SelectedYearUpload"];

                    OBDAL.YearUploadSetup pm = new YearUploadBAL().GetYearUpload()
                        .Where(x => x.BudgetYear == Convert.ToInt32(ddlYear.SelectedValue)).FirstOrDefault();

                    if (pm != null)
                    {
                        if (pm.BudgetYearID != objYearUpload.BudgetYearID)
                        {
                            ((SiteMaster)this.Master).ShowMessage("Failure", "Budget Year already exists");
                            return;
                        }
                    }

                    objYearUpload.BudgetYear = Convert.ToInt32(ddlYear.SelectedValue);
                    objYearUpload.BudgetYearDesc = tbDesc.Text;
                    objYearUpload.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objYearUpload.ModifiedBy = LoggedInUser.UserID;
                    objYearUpload.ModifiedTimeStamp = DateTime.Now;

                    if (new YearUploadBAL().UpdateYearUpload(objYearUpload))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget Year updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Budget Year");
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

        protected void gvYearUploadSetup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();
                    GridViewRow selectedRow = gvYearUploadSetup.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    int budgetyearid = Convert.ToInt32(gvYearUploadSetup.DataKeys[selectedRow.RowIndex]["BudgetYearID"]);
                    OBDAL.YearUploadSetup objYearUpload = ((List<OBDAL.YearUploadSetup>)Session["YearUploadData"]).Where(x => x.BudgetYearID == budgetyearid).FirstOrDefault();

                    Session["SelectedYearUpload"] = objYearUpload;

                    ddlYear.SelectedIndex = -1;
                    ddlYear.Items.FindByValue(objYearUpload.BudgetYear.ToString()).Selected = true;

                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objYearUpload.Status))).Selected = true;

                    tbDesc.Text = objYearUpload.BudgetYearDesc;

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
                List<OBDAL.YearUploadSetup> data = new YearUploadBAL().GetYearUpload();
                Session["YearUploadData"] = data;
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
                gvYearUploadSetup.DataSource = ((List<OBDAL.YearUploadSetup>)Session["YearUploadData"])
                                        .Select(x => new OBDAL.YearUploadSetup()
                                        {
                                            BudgetYearID = x.BudgetYearID,
                                            BudgetYear = x.BudgetYear,
                                            BudgetYearDesc = x.BudgetYearDesc,
                                            Status = ((x.Status == "A") ? "A" : "D")
                                        }).OrderBy(x => x.BudgetYear).ThenBy(x => x.BudgetYearDesc).ThenByDescending(x => x.Status).ToList();
                gvYearUploadSetup.DataBind();
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

                for (int i = DateTime.Now.Year; i <= DateTime.Now.Year + 10; i++)
                {
                    ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
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
                    ddlYear.Enabled = true;
                    ddlStatus.Enabled = true;
                    tbDesc.Text = String.Empty;
                    break;
                case Helper.PageMode.Edit:
                    ddlYear.Enabled = false;
                    break;
            }
            Session["YearUploadPageMode"] = pagemode;
        }

        private void ClearPageData()
        {
            try
            {
                ddlYear.SelectedIndex = 0;
                ddlStatus.SelectedIndex = 0;
                tbDesc.Text = String.Empty;

                foreach (GridViewRow gvr in gvYearUploadSetup.Rows)
                    gvr.Style["background-color"] = "";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }
    }
}