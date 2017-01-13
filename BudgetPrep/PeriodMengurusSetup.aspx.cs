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
    public class PeriodMengurusGridHelper
    {
        public int PeriodMengurusID { get; set; }
        public int FieldMengurusID { get; set; }
        public int MengurusYear { get; set; }
        public string FieldMengurus { get; set; }
        public string Status { get; set; }
    }

    public class PeriodMengurusExport
    {
        public int MengurusYear { get; set; }
        public string Field { get; set; }
        public string Status { get; set; }
    }

    public partial class PeriodMengurusSetup : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Period Mengurus";

                GetData();
                Session["PeriodMenguruPageMode"] = Helper.PageMode.New;
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
                if ((Helper.PageMode)Session["PeriodMenguruPageMode"] == Helper.PageMode.New)
                {
                    if (new PeriodMengurusBAL().GetPeriodMengurus()
                        .Where(x => x.FieldMengurusID == Convert.ToInt32("27")
                        && x.MengurusYear == Convert.ToInt32(ddlYear.SelectedValue)).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Period Mengurus already exists");
                        return;
                    }

                    PeriodMenguru objPeriodMenguru = new PeriodMenguru();
                    objPeriodMenguru.FieldMengurusID = Convert.ToInt32("27");
                    objPeriodMenguru.MengurusYear = Convert.ToInt32(ddlYear.SelectedValue);
                    objPeriodMenguru.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objPeriodMenguru.CreatedBy = LoggedInUser.UserID;
                    objPeriodMenguru.CreatedTimeStamp = DateTime.Now;
                    objPeriodMenguru.ModifiedBy = LoggedInUser.UserID;
                    objPeriodMenguru.ModifiedTimeStamp = DateTime.Now;

                    if (new PeriodMengurusBAL().InsertPeriodMenguru(objPeriodMenguru))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Period Mengurus saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Period Mengurus");
                }
                else if ((Helper.PageMode)Session["PeriodMenguruPageMode"] == Helper.PageMode.Edit)
                {
                    PeriodMenguru objPeriodMenguru = (PeriodMenguru)Session["SelectedPeriodMenguru"];

                    PeriodMenguru pm = new PeriodMengurusBAL().GetPeriodMengurus().Where(x => x.FieldMengurusID == Convert.ToInt32("27")
                        && x.MengurusYear == Convert.ToInt32(ddlYear.SelectedValue)).FirstOrDefault();
                    if (pm != null)
                    {
                        if (pm.PeriodMengurusID != objPeriodMenguru.PeriodMengurusID)
                        {
                            ((SiteMaster)this.Master).ShowMessage("Failure", "Period Mengurus already exists");
                            return;
                        }
                    }

                    objPeriodMenguru.FieldMengurusID = Convert.ToInt32("27");
                    objPeriodMenguru.MengurusYear = Convert.ToInt32(ddlYear.SelectedValue);
                    objPeriodMenguru.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objPeriodMenguru.ModifiedBy = LoggedInUser.UserID;
                    objPeriodMenguru.ModifiedTimeStamp = DateTime.Now;

                    if (new PeriodMengurusBAL().UpdatePeriodMenguru(objPeriodMenguru))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Period Mengurus updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Period Mengurus");
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
                ds.Tables.Add(new ReportHelper().ToDataTable<PeriodMengurusExport>(
                        ((List<PeriodMengurusGridHelper>)Session["PeriodMenguruData"])
                            .Select(x => new PeriodMengurusExport()
                            {
                                MengurusYear = Convert.ToInt32(x.MengurusYear),
                                Field = x.FieldMengurus,
                                Status = ((x.Status == "A") ? "Active" : "Inactive")
                            })
                            .OrderBy(x => x.MengurusYear).ThenBy(x => x.Field)
                    ));
                new ReportHelper().ToExcel(ds, "PeriodMengurus.xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvPeriodMenguruSetup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();
                    GridViewRow selectedRow = gvPeriodMenguruSetup.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    PeriodMenguru objPeriodMenguru = new PeriodMenguru();
                    objPeriodMenguru.PeriodMengurusID = Convert.ToInt32(gvPeriodMenguruSetup.DataKeys[selectedRow.RowIndex]["PeriodMengurusID"]);
                    objPeriodMenguru.FieldMengurusID = Convert.ToInt32(gvPeriodMenguruSetup.DataKeys[selectedRow.RowIndex]["FieldMengurusID"]);
                    objPeriodMenguru.MengurusYear = Convert.ToInt32(selectedRow.Cells[1].Text);
                    objPeriodMenguru.Status = selectedRow.Cells[2].Text;

                    Session["SelectedPeriodMenguru"] = objPeriodMenguru;

                    //ddlFieldMengurus.SelectedIndex = -1;
                    //ddlFieldMengurus.Items.FindByValue(objPeriodMenguru.FieldMengurusID.ToString()).Selected = true;
                    ddlYear.SelectedIndex = -1;
                    ddlYear.Items.FindByValue(objPeriodMenguru.MengurusYear.ToString()).Selected = true;
                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objPeriodMenguru.Status))).Selected = true;

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
                List<PeriodMengurusGridHelper> data = new PeriodMengurusBAL().GetPeriodMengurus()
                    .Select(x => new PeriodMengurusGridHelper()
                    {
                        PeriodMengurusID = x.PeriodMengurusID,
                        FieldMengurusID = x.FieldMengurusID,
                        FieldMengurus = x.FieldMenguru.FieldMengurusDesc,
                        MengurusYear = x.MengurusYear,
                        Status = x.Status
                    }).ToList();

                //data = data.ForEach(x => x.Status = new Helper().GetItemStatusEnumName(Convert.ToChar(x.Status)));
                Session["PeriodMenguruData"] = data;
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
                gvPeriodMenguruSetup.DataSource = (List<PeriodMengurusGridHelper>)Session["PeriodMenguruData"];
                gvPeriodMenguruSetup.DataBind();
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

                List<FieldMenguru> obj = new FieldMenguruBAL().GetFieldMengurus().Where(x => x.FieldMengurusID == 27).ToList();

                foreach (var item in obj)
                { 
                    tbFieldMengurus.Text = string.Format("{0}", item.FieldMengurusDesc);
                }

                //ddlFieldMengurus.DataSource = new FieldMenguruBAL().GetFieldMengurus().Where(x => x.Status == "A").ToList();
                //ddlFieldMengurus.DataTextField = "FieldMengurusDesc";
                //ddlFieldMengurus.DataValueField = "FieldMengurusID";
                //ddlFieldMengurus.DataBind();
                //ddlFieldMengurus.Enabled = false;
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
            Session["PeriodMenguruPageMode"] = pagemode;
        }

        private void ClearPageData()
        {
            try
            {
                //ddlFieldMengurus.SelectedIndex = 0;
                ddlYear.SelectedIndex = 0;
                ddlStatus.SelectedIndex = 0;

                foreach (GridViewRow gvr in gvPeriodMenguruSetup.Rows)
                    gvr.Style["background-color"] = "";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }
    }
}