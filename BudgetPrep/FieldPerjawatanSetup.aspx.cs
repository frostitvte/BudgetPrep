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
    public class FieldPerjawatanExport
    {
        public string FieldPerjawatanDesc { get; set; }
        public string Status { get; set; }
    }

    public partial class FieldPerjawatanetup : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Field Perjawatan";

                GetData();
                Session["FieldPerjawatanPageMode"] = Helper.PageMode.New;
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
                if ((Helper.PageMode)Session["FieldPerjawatanPageMode"] == Helper.PageMode.New)
                {
                    if (new FieldPerjawatanBAL().GetFieldPerjawatans().Where(x => x.FieldPerjawatanDesc.ToUpper().Trim() == tbFieldPerjawatanDesc.Text.ToUpper().Trim()).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Field Perjawatan already exists");
                        return;
                    }

                    FieldPerjawatan objFieldPerjawatan = new FieldPerjawatan();
                    //objFieldPerjawatan.FieldPerjawatanID = tbFieldPerjawatanID.Text.Trim();
                    objFieldPerjawatan.FieldPerjawatanDesc = tbFieldPerjawatanDesc.Text.Trim();
                    //objFieldPerjawatan.FieldPerjawatanSDesc = tbFieldPerjawatanSDesc.Text.Trim();
                    objFieldPerjawatan.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objFieldPerjawatan.CreatedBy = LoggedInUser.UserID;
                    objFieldPerjawatan.CreatedTimeStamp = DateTime.Now;
                    objFieldPerjawatan.ModifiedBy = LoggedInUser.UserID;
                    objFieldPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    if(new FieldPerjawatanBAL().InsertFieldPerjawatan(objFieldPerjawatan))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Field Perjawatan saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Field Perjawatan");
                }   
                else if ((Helper.PageMode)Session["FieldPerjawatanPageMode"] == Helper.PageMode.Edit)
                {
                    FieldPerjawatan objFieldPerjawatan = (FieldPerjawatan)Session["SelectedFieldPerjawatan"];
                    //objFieldPerjawatan.FieldPerjawatanID = tbFieldPerjawatanID.Text.Trim();
                    objFieldPerjawatan.FieldPerjawatanDesc = tbFieldPerjawatanDesc.Text.Trim();
                    //objFieldPerjawatan.FieldPerjawatanSDesc = tbFieldPerjawatanSDesc.Text.Trim();
                    objFieldPerjawatan.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objFieldPerjawatan.ModifiedBy = LoggedInUser.UserID;
                    objFieldPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    if (new FieldPerjawatanBAL().UpdateFieldPerjawatan(objFieldPerjawatan))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Field Perjawatan updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Field Perjawatan");
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
                ds.Tables.Add(new ReportHelper().ToDataTable<FieldPerjawatanExport>(
                        ((List<FieldPerjawatan>)Session["FieldPerjawatanData"])
                            .Select(x => new FieldPerjawatanExport()
                            {
                                FieldPerjawatanDesc = x.FieldPerjawatanDesc,
                                Status = ((x.Status == "A") ? "Active" : "Inactive")
                            })
                            .OrderBy(x => x.FieldPerjawatanDesc)
                    ));
                new ReportHelper().ToExcel(ds, "FieldPerjawatan.xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvFieldPerjawatanetup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();

                    GridViewRow selectedRow = gvFieldPerjawatanetup.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    FieldPerjawatan objFieldPerjawatan = new FieldPerjawatan();
                    objFieldPerjawatan.FieldPerjawatanID = Convert.ToInt32(gvFieldPerjawatanetup.DataKeys[selectedRow.RowIndex]["FieldPerjawatanID"]);
                    objFieldPerjawatan.FieldPerjawatanDesc = selectedRow.Cells[0].Text;
                    //objFieldPerjawatan.FieldPerjawatanDesc = selectedRow.Cells[1].Text;
                    objFieldPerjawatan.Status = selectedRow.Cells[1].Text;

                    Session["SelectedFieldPerjawatan"] = objFieldPerjawatan;

                    tbFieldPerjawatanDesc.Text = objFieldPerjawatan.FieldPerjawatanDesc;
                    //tbFieldPerjawatanSDesc.Text = objFieldPerjawatan.FieldPerjawatanSDesc;
                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objFieldPerjawatan.Status))).Selected = true;

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
                List<FieldPerjawatan> data = new FieldPerjawatanBAL().GetFieldPerjawatans();

                //data = data.ForEach(x => x.Status = new Helper().GetItemStatusEnumName(Convert.ToChar(x.Status)));
                Session["FieldPerjawatanData"] = data;
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
                gvFieldPerjawatanetup.DataSource = (List<FieldPerjawatan>)Session["FieldPerjawatanData"];
                gvFieldPerjawatanetup.DataBind();
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
                        tbFieldPerjawatanDesc.Enabled = true;
                        break;
                    case Helper.PageMode.Edit:
                        tbFieldPerjawatanDesc.Enabled = true;
                        break;
                }
                Session["FieldPerjawatanPageMode"] = pagemode;
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
                tbFieldPerjawatanDesc.Text = string.Empty;
                //tbFieldPerjawatanSDesc.Text = string.Empty;
                ddlStatus.SelectedIndex = 0;

                foreach (GridViewRow gvr in gvFieldPerjawatanetup.Rows)
                    gvr.Style["background-color"] = "";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }
    }
}