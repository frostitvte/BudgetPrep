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
    public class PeriodPerjawatanGridHelper
    {
        public int PeriodPerjawatanID { get; set; }
        public int FieldPerjawatanID { get; set; }
        public int PerjawatanYear { get; set; }
        public string FieldPerjawatan { get; set; }
        public string Status { get; set; }
    }

    public class PeriodPerjawatanExport
    {
        public int PerjawatanYear { get; set; }
        public string Field { get; set; }
        public string Status { get; set; }
    }

    public partial class PeriodPerjawatanSetup : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Period Perjawatan";

                GetData();
                Session["PeriodPerjawatanPageMode"] = Helper.PageMode.New;
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
                if ((Helper.PageMode)Session["PeriodPerjawatanPageMode"] == Helper.PageMode.New)
                {
                    if (new PeriodPerjawatanBAL().GetPeriodPerjawatans()
                           .Where(x => x.FieldPerjawatanID == Convert.ToInt32("21")
                           && x.PerjawatanYear == Convert.ToInt32(ddlYear.SelectedValue)).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Period Perjawatan already exists");
                        return;
                    }

                    PeriodPerjawatan objPeriodPerjawatan = new PeriodPerjawatan();
                    objPeriodPerjawatan.FieldPerjawatanID = Convert.ToInt32("21");
                    objPeriodPerjawatan.PerjawatanYear = Convert.ToInt32(ddlYear.SelectedValue);
                    objPeriodPerjawatan.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objPeriodPerjawatan.CreatedBy = LoggedInUser.UserID;
                    objPeriodPerjawatan.CreatedTimeStamp = DateTime.Now;
                    objPeriodPerjawatan.ModifiedBy = LoggedInUser.UserID;
                    objPeriodPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    if(new PeriodPerjawatanBAL().InsertPeriodPerjawatan(objPeriodPerjawatan))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Period Perjawatan saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Period Perjawatan");
                }
                else if ((Helper.PageMode)Session["PeriodPerjawatanPageMode"] == Helper.PageMode.Edit)
                {
                    PeriodPerjawatan objPeriodPerjawatan = (PeriodPerjawatan)Session["SelectedPeriodPerjawatan"];

                    PeriodPerjawatan pp = new PeriodPerjawatanBAL().GetPeriodPerjawatans()
                           .Where(x => x.FieldPerjawatanID == Convert.ToInt32("21")
                           && x.PerjawatanYear == Convert.ToInt32(ddlYear.SelectedValue)).FirstOrDefault();
                    if (pp != null)
                    {
                        if (pp.PeriodPerjawatanID != objPeriodPerjawatan.PeriodPerjawatanID)
                        {
                            ((SiteMaster)this.Master).ShowMessage("Failure", "Period Perjawatan already exists");
                            return;
                        }
                    }

                    objPeriodPerjawatan.FieldPerjawatanID = Convert.ToInt32("21");
                    objPeriodPerjawatan.PerjawatanYear = Convert.ToInt32(ddlYear.SelectedValue);
                    objPeriodPerjawatan.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objPeriodPerjawatan.ModifiedBy = LoggedInUser.UserID;
                    objPeriodPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    if(new PeriodPerjawatanBAL().UpdatePeriodPerjawatan(objPeriodPerjawatan))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Period Perjawatan updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Period Perjawatan");
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
                ds.Tables.Add(new ReportHelper().ToDataTable<PeriodPerjawatanExport>(
                        ((List<PeriodPerjawatanGridHelper>)Session["PeriodPerjawatanData"])
                            .Select(x => new PeriodPerjawatanExport()
                            {
                                PerjawatanYear = Convert.ToInt32(x.PerjawatanYear),
                                Field = x.FieldPerjawatan,
                                Status = ((x.Status == "A") ? "Active" : "Inactive")
                            })
                            .OrderBy(x => x.PerjawatanYear).ThenBy(x => x.Field)
                    ));
                new ReportHelper().ToExcel(ds, "PeriodPerjawatan.xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvPeriodPerjawatanSetup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();
                    GridViewRow selectedRow = gvPeriodPerjawatanSetup.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    PeriodPerjawatan objPeriodPerjawatan = new PeriodPerjawatan();
                    objPeriodPerjawatan.PeriodPerjawatanID = Convert.ToInt32(gvPeriodPerjawatanSetup.DataKeys[selectedRow.RowIndex]["PeriodPerjawatanID"]);
                    objPeriodPerjawatan.FieldPerjawatanID = Convert.ToInt32(gvPeriodPerjawatanSetup.DataKeys[selectedRow.RowIndex]["FieldPerjawatanID"]);
                    objPeriodPerjawatan.PerjawatanYear = Convert.ToInt32(selectedRow.Cells[1].Text);
                    objPeriodPerjawatan.Status = selectedRow.Cells[2].Text;

                    Session["SelectedPeriodPerjawatan"] = objPeriodPerjawatan;

                    //ddlFieldPerjawatans.SelectedIndex = -1;
                    //ddlFieldPerjawatans.Items.FindByValue(objPeriodPerjawatan.FieldPerjawatanID.ToString()).Selected = true;
                    ddlYear.SelectedIndex = -1;
                    ddlYear.Items.FindByValue(objPeriodPerjawatan.PerjawatanYear.ToString()).Selected = true;
                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objPeriodPerjawatan.Status))).Selected = true;

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
                List<PeriodPerjawatanGridHelper> data = new PeriodPerjawatanBAL().GetPeriodPerjawatans()
                    .Select(x => new PeriodPerjawatanGridHelper()
                    {
                        PeriodPerjawatanID = x.PeriodPerjawatanID,
                        FieldPerjawatanID = x.FieldPerjawatanID,
                        FieldPerjawatan = x.FieldPerjawatan.FieldPerjawatanDesc,
                        PerjawatanYear = x.PerjawatanYear,
                        Status = x.Status
                    }).ToList();

                //data = data.ForEach(x => x.Status = new Helper().GetItemStatusEnumName(Convert.ToChar(x.Status)));
                Session["PeriodPerjawatanData"] = data;
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
                gvPeriodPerjawatanSetup.DataSource = (List<PeriodPerjawatanGridHelper>)Session["PeriodPerjawatanData"];
                gvPeriodPerjawatanSetup.DataBind();
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

                List<FieldPerjawatan> obj = new FieldPerjawatanBAL().GetFieldPerjawatans().Where(x => x.FieldPerjawatanID == 21).ToList();

                foreach (var item in obj)
                {
                    tbFieldPerjawatans.Text = string.Format("{0}", item.FieldPerjawatanDesc);
                }

                //ddlFieldPerjawatans.DataSource = new FieldPerjawatanBAL().GetFieldPerjawatans().Where(x => x.Status == "A").ToList();
                //ddlFieldPerjawatans.DataTextField = "FieldPerjawatanDesc";
                //ddlFieldPerjawatans.DataValueField = "FieldPerjawatanID";
                //ddlFieldPerjawatans.DataBind();
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
                    btnAdd.Enabled = true;
                    break;
                case Helper.PageMode.Edit:
                    btnAdd.Enabled = true;
                    break;
            }
            Session["PeriodPerjawatanPageMode"] = pagemode;
        }

        private void ClearPageData()
        {
            try
            {
                //ddlFieldPerjawatans.SelectedIndex = 0;
                ddlYear.SelectedIndex = 0;
                ddlStatus.SelectedIndex = 0;

                foreach (GridViewRow gvr in gvPeriodPerjawatanSetup.Rows)
                    gvr.Style["background-color"] = "";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }
    }
}