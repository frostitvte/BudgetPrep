using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using BudgetPrep.Classes;
using OBBAL;
using OBDAL;

namespace BudgetPrep
{
    public class SegmentExport
    {
        public string SegmentName { get; set; }
        public int SegmentOrder { get; set; }
        public string ShapeFormat { get; set; }
        public string Status { get; set; }
    }

    public class SegmentImport
    {
        public string SegmentName { get; set; }
        public int SegmentOrder { get; set; }
        public string ShapeFormat { get; set; }
        public string DetailCode { get; set; }
        public string DetailDesc { get; set; }
        public string ParentCode { get; set; }

        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }
        
    }

    public partial class SegmentSetup : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Segment";
                tbSegName.Text = string.Empty;
                tbSegFormat.Attributes.Add("value", string.Empty);
                GetData();
                Session["SegmentPageMode"] = Helper.PageMode.New;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ChangePageMode(Helper.PageMode.New);
                ClearPageData();
                EditBox.Visible = true;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if ((Helper.PageMode)Session["SegmentPageMode"] == Helper.PageMode.New)
                {
                    if (new SegmentBAL().GetSegments().Where(x => x.SegmentName.ToUpper().Trim() == tbSegName.Text.ToUpper().Trim()).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Segment already exists");
                        return;
                    }
                    if (new SegmentBAL().GetSegments().Where(x => x.SegmentOrder == Convert.ToInt32(tbSegOrder.Text.Trim())).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Please change Segment order");
                        return;
                    }
                    Segment objSegment = new Segment();
                    objSegment.SegmentName = tbSegName.Text.Trim();
                    objSegment.ShapeFormat = string.Empty.PadRight(tbSegFormat.Text.Trim().Length, '?');
                    objSegment.SegmentOrder = Convert.ToInt32(tbSegOrder.Text.Trim());
                    objSegment.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objSegment.CreatedBy = LoggedInUser.UserID;
                    objSegment.CreatedTimeStamp = DateTime.Now;
                    objSegment.ModifiedBy = LoggedInUser.UserID;
                    objSegment.ModifiedTimeStamp = DateTime.Now;

                    if (new SegmentBAL().InsertSegment(objSegment))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Segment saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Segment");
                }
                else if ((Helper.PageMode)Session["SegmentPageMode"] == Helper.PageMode.Edit)
                {
                    Segment objSegment = (Segment)Session["SelectedSegment"];

                    Segment seg = new SegmentBAL().GetSegments().Where(x => x.SegmentOrder == Convert.ToInt32(tbSegOrder.Text.Trim())).FirstOrDefault();
                    if (seg != null)
                    {
                        if (seg.SegmentID != objSegment.SegmentID)
                        {
                            ((SiteMaster)this.Master).ShowMessage("Failure", "Please change Segment order");
                            return;
                        }
                    }

                    objSegment.SegmentName = tbSegName.Text.Trim();
                    objSegment.ShapeFormat = string.Empty.PadRight(tbSegFormat.Text.Trim().Length, '?');
                    objSegment.SegmentOrder = Convert.ToInt32(tbSegOrder.Text.Trim());
                    objSegment.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objSegment.ModifiedBy = LoggedInUser.UserID;
                    objSegment.ModifiedTimeStamp = DateTime.Now;

                    if (new SegmentBAL().UpdateSegment(objSegment))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Segment updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Segment");
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
            try
            {
                ChangePageMode(Helper.PageMode.New);
                ClearPageData();
                EditBox.Visible = false;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                DataSet ds = new DataSet();
                ds.Tables.Add(new ReportHelper().ToDataTable<SegmentExport>(
                        ((List<Segment>)Session["SegmentData"])
                            .Select(x => new SegmentExport()
                            {
                                SegmentName = x.SegmentName,
                                SegmentOrder = Convert.ToInt32(x.SegmentOrder),
                                ShapeFormat = x.ShapeFormat,
                                Status = ((x.Status == "A") ? "Active" : "Inactive")
                            })
                            .OrderBy(x => x.SegmentName)
                    ));
                new ReportHelper().ToExcel(ds, "Segment.xls", ref Response);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void gvSegmentSetup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();
                    GridViewRow selectedRow = gvSegmentSetup.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    Segment objSegment = new Segment();
                    objSegment.SegmentID = Convert.ToInt32(gvSegmentSetup.DataKeys[selectedRow.RowIndex]["SegmentID"]);
                    objSegment.SegmentName = selectedRow.Cells[0].Text;
                    objSegment.ShapeFormat = selectedRow.Cells[1].Text;
                    objSegment.SegmentOrder = Convert.ToInt32(selectedRow.Cells[2].Text);
                    objSegment.Status = selectedRow.Cells[3].Text;

                    Session["SelectedSegment"] = objSegment;

                    tbSegName.Text = objSegment.SegmentName;
                    tbSegFormat.Attributes.Add("value", objSegment.ShapeFormat);
                    tbSegOrder.Text = objSegment.SegmentOrder.ToString();
                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objSegment.Status))).Selected = true;

                    ChangePageMode(Helper.PageMode.Edit);
                    EditBox.Visible = true;
                }

                if (e.CommandName == "EditDetails")
                {
                    GridViewRow selectedRow = gvSegmentSetup.Rows[Convert.ToInt32(e.CommandArgument)];

                    Segment objSegment = new Segment();
                    objSegment.SegmentID = Convert.ToInt32(gvSegmentSetup.DataKeys[selectedRow.RowIndex]["SegmentID"]);
                    objSegment.SegmentName = selectedRow.Cells[0].Text;
                    objSegment.ShapeFormat = selectedRow.Cells[1].Text;
                    objSegment.SegmentOrder = Convert.ToInt32(selectedRow.Cells[2].Text);
                    objSegment.Status = selectedRow.Cells[3].Text;

                    Session["SelectedSegment"] = objSegment;

                    Response.Redirect("~/SegmentDetails.aspx");
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
                List<Segment> data = new SegmentBAL().GetSegments();

                //data = data.ForEach(x => x.Status = new Helper().GetItemStatusEnumName(Convert.ToChar(x.Status)));
                Session["SegmentData"] = data.OrderBy(x => x.SegmentOrder).ThenBy(x => x.SegmentName).ToList();
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
                //((SiteMaster)this.Master).ChangeLanguage();

                gvSegmentSetup.DataSource = (List<Segment>)Session["SegmentData"];
                gvSegmentSetup.DataBind();
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
                    tbSegName.Enabled = true;
                    break;
                case Helper.PageMode.Edit:
                    tbSegName.Enabled = false;
                    break;
            }
            Session["SegmentPageMode"] = pagemode;
        }

        private void ClearPageData()
        {
            tbSegName.Text = string.Empty;
            tbSegFormat.Text = string.Empty;
            tbSegFormat.Attributes.Add("value", "");
            tbSegOrder.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;

            foreach (GridViewRow gvr in gvSegmentSetup.Rows)
                gvr.Style["background-color"] = "";
        }

        //protected void gvSegmentSetup_SelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    GridViewRow selectedRow = gvSegmentSetup.SelectedRow;
        //    selectedRow.Style["background-color"] = "gold";

        //    Segment objSegment = new Segment();
        //    objSegment.SegmentID = Convert.ToInt32(gvSegmentSetup.DataKeys[selectedRow.RowIndex]["SegmentID"]);
        //    objSegment.SegmentName = selectedRow.Cells[0].Text;
        //    objSegment.ShapeFormat = selectedRow.Cells[1].Text;
        //    objSegment.SegmentOrder = Convert.ToInt32(selectedRow.Cells[2].Text);
        //    objSegment.Status = selectedRow.Cells[3].Text;

        //    Session["SelectedSegment"] = objSegment;

        //    tbSegName.Text = objSegment.SegmentName;
        //    tbSegFormat.Attributes.Add("value", objSegment.ShapeFormat);
        //    tbSegOrder.Text = objSegment.SegmentOrder.ToString();
        //    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objSegment.Status))).Selected = true;

        //    ChangePageMode(Helper.PageMode.Edit);
        //}

        //protected void gvSegmentSetup_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gvSegmentSetup, "Select$" + e.Row.RowIndex);
        //        e.Row.Style["cursor"] = "hand";
        //        e.Row.ToolTip = "Click to select this row.";
        //    }
        //}

        protected void btnMailTest_Click(object sender, EventArgs e)
        {

        }

    }
}

//DataTable dt = new DataTable();
//dt.Columns.Add(new DataColumn("Name"));
//dt.Columns.Add(new DataColumn("Location"));

//DataRow dr = dt.NewRow();
//dr["Name"] = "Pavan";
//dr["Location"] = "Guntur";
//dt.Rows.Add(dr);

//dr = dt.NewRow();
//dr["Name"] = "kumar";
//dr["Location"] = "Vijayawada";
//dt.Rows.Add(dr);

//gvSegmentSetup.DataSource = dt;
//gvSegmentSetup.DataBind();