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
    public class FieldMengurusExport
    {
        public string FieldMengurusDesc { get; set; }
        public string Status { get; set; }
    }

    public partial class FieldMengurusSetup : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Field Mengurus";

                GetData();
                Session["FieldMenguruPageMode"] = Helper.PageMode.New;
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
                if ((Helper.PageMode)Session["FieldMenguruPageMode"] == Helper.PageMode.New)
                {
                    if (new FieldMenguruBAL().GetFieldMengurus().Where(x => x.FieldMengurusDesc.ToUpper().Trim() == tbFieldMengurusDesc.Text.ToUpper().Trim()).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Field Mengurus already exists");
                        return;
                    }

                    FieldMenguru objFieldMenguru = new FieldMenguru();
                    //objFieldMenguru.FieldMengurusID = tbFieldMengurusID.Text.Trim();
                    objFieldMenguru.FieldMengurusDesc = tbFieldMengurusDesc.Text.Trim();
                    //objFieldMenguru.FieldMengurusSDesc = tbFieldMengurusSDesc.Text.Trim();
                    objFieldMenguru.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objFieldMenguru.CreatedBy = LoggedInUser.UserID;
                    objFieldMenguru.CreatedTimeStamp = DateTime.Now;
                    objFieldMenguru.ModifiedBy = LoggedInUser.UserID;
                    objFieldMenguru.ModifiedTimeStamp = DateTime.Now;

                    if(new FieldMenguruBAL().InsertFieldMenguru(objFieldMenguru))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Field Mengurus saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Field Mengurus");
                }
                else if ((Helper.PageMode)Session["FieldMenguruPageMode"] == Helper.PageMode.Edit)
                {
                    FieldMenguru objFieldMenguru = (FieldMenguru)Session["SelectedFieldMenguru"];
                    //objFieldMenguru.FieldMengurusID = tbFieldMengurusID.Text.Trim();
                    objFieldMenguru.FieldMengurusDesc = tbFieldMengurusDesc.Text.Trim();
                    //objFieldMenguru.FieldMengurusSDesc = tbFieldMengurusSDesc.Text.Trim();
                    objFieldMenguru.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objFieldMenguru.ModifiedBy = LoggedInUser.UserID;
                    objFieldMenguru.ModifiedTimeStamp = DateTime.Now;

                    if(new FieldMenguruBAL().UpdateFieldMenguru(objFieldMenguru))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Field Mengurus updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Field Mengurus");
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
                //List<SegmentExport> data = .ToList();
                ds.Tables.Add(new ReportHelper().ToDataTable<FieldMengurusExport>(
                        ((List<FieldMenguru>)Session["FieldMenguruData"])
                            .Select(x => new FieldMengurusExport()
                            {
                                FieldMengurusDesc = x.FieldMengurusDesc,
                                Status = ((x.Status == "A") ? "Active" : "Inactive")
                            })
                            .OrderBy(x => x.FieldMengurusDesc)
                    ));
                new ReportHelper().ToExcel(ds, "FieldMengurus.xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvFieldMenguruSetup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();

                    GridViewRow selectedRow = gvFieldMenguruSetup.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    FieldMenguru objFieldMenguru = new FieldMenguru();
                    objFieldMenguru.FieldMengurusID = Convert.ToInt32(gvFieldMenguruSetup.DataKeys[selectedRow.RowIndex]["FieldMengurusID"]);
                    objFieldMenguru.FieldMengurusDesc = selectedRow.Cells[0].Text;
                    //objFieldMenguru.FieldMengurusDesc = selectedRow.Cells[1].Text;
                    objFieldMenguru.Status = selectedRow.Cells[1].Text;

                    Session["SelectedFieldMenguru"] = objFieldMenguru;

                    tbFieldMengurusDesc.Text = objFieldMenguru.FieldMengurusDesc;
                    //tbFieldMengurusSDesc.Text = objFieldMenguru.FieldMengurusSDesc;
                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objFieldMenguru.Status))).Selected = true;

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
                List<FieldMenguru> data = new FieldMenguruBAL().GetFieldMengurus();

                //data = data.ForEach(x => x.Status = new Helper().GetItemStatusEnumName(Convert.ToChar(x.Status)));
                Session["FieldMenguruData"] = data;
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
                gvFieldMenguruSetup.DataSource = (List<FieldMenguru>)Session["FieldMenguruData"];
                gvFieldMenguruSetup.DataBind();
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
            try
            {
                switch (pagemode)
                {
                    case Helper.PageMode.New:
                        tbFieldMengurusDesc.Enabled = true;
                        break;
                    case Helper.PageMode.Edit:
                        tbFieldMengurusDesc.Enabled = false;
                        break;
                }
                Session["FieldMenguruPageMode"] = pagemode;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void ClearPageData()
        {
            try
            {
                tbFieldMengurusDesc.Text = string.Empty;
                //tbFieldMengurusSDesc.Text = string.Empty;
                ddlStatus.SelectedIndex = 0;

                foreach (GridViewRow gvr in gvFieldMenguruSetup.Rows)
                    gvr.Style["background-color"] = "";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }
    }
}