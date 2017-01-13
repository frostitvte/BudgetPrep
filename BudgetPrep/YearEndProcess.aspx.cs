using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using BudgetPrep.Classes;
using OBBAL;
using OBDAL;

namespace BudgetPrep
{
    public partial class YearEndProcess : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Year End";

                GetData();
                Session["YearEndPageMode"] = Helper.PageMode.New;
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
                if (ddlStatus.SelectedValue == "D")
                    ArchiveBudget();

                if ((Helper.PageMode)Session["YearEndPageMode"] == Helper.PageMode.New)
                {
                    if (new YearEndBAL().GetYearEnds()
                        .Where(x => x.BudgetType == ddlBudgetType.SelectedValue
                        && x.BudgetYear == Convert.ToInt32(ddlBudgetYear.SelectedValue)).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Year End already exists");
                        return;
                    }

                    YearEnd objYearEnd = new YearEnd();
                    objYearEnd.BudgetType = ddlBudgetType.SelectedValue;
                    objYearEnd.BudgetYear = Convert.ToInt32(ddlBudgetYear.SelectedValue);
                    objYearEnd.Status = ddlStatus.SelectedValue;
                    objYearEnd.CreatedBy = LoggedInUser.UserID;
                    objYearEnd.CreatedTimeStamp = DateTime.Now;
                    objYearEnd.ModifiedBy = LoggedInUser.UserID;
                    objYearEnd.ModifiedTimeStamp = DateTime.Now;

                    if (new YearEndBAL().InsertYearEnd(objYearEnd))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Year End Processed successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while processing Year End");
                }
                else if ((Helper.PageMode)Session["YearEndPageMode"] == Helper.PageMode.Edit)
                {
                    YearEnd objYearEnd = (YearEnd)Session["SelectedYearEnd"];

                    YearEnd pm = new YearEndBAL().GetYearEnds()
                        .Where(x => x.BudgetType == ddlBudgetType.SelectedValue
                        && x.BudgetYear == Convert.ToInt32(ddlBudgetYear.SelectedValue)).FirstOrDefault();
                    if (pm != null)
                    {
                        if (pm.YearEndID != objYearEnd.YearEndID)
                        {
                            ((SiteMaster)this.Master).ShowMessage("Failure", "Year End already exists");
                            return;
                        }
                    }

                    objYearEnd.BudgetType = ddlBudgetType.SelectedValue;
                    objYearEnd.BudgetYear = Convert.ToInt32(ddlBudgetYear.SelectedValue);
                    objYearEnd.Status = ddlStatus.SelectedValue;
                    objYearEnd.ModifiedBy = LoggedInUser.UserID;
                    objYearEnd.ModifiedTimeStamp = DateTime.Now;

                    if (new YearEndBAL().UpdateYearEnd(objYearEnd))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Year End Processed successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while processing Year End");
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
                ds.Tables.Add(new ReportHelper().ToDataTable<YearEnd>(
                        ((List<YearEnd>)Session["YearEndData"])
                            .Select(x => new YearEnd()
                            {
                                YearEndID = x.YearEndID,
                                BudgetType = x.BudgetType,
                                BudgetYear = x.BudgetYear,
                                Status = ((x.Status == "A") ? "Active" : "Processed")
                            })
                            .OrderBy(x => x.BudgetYear).ThenBy(x => x.BudgetType).ThenByDescending(x => x.Status)
                    ));
                new ReportHelper().ToExcel(ds, "YearEnds.xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvYearEnd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();
                    GridViewRow selectedRow = gvYearEnd.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    int yearendid = Convert.ToInt32(gvYearEnd.DataKeys[selectedRow.RowIndex]["YearEndID"]);
                    YearEnd objYearEnd = ((List<YearEnd>)Session["YearEndData"]).Where(x => x.YearEndID == yearendid).FirstOrDefault();

                    Session["SelectedYearEnd"] = objYearEnd;

                    ddlBudgetType.SelectedIndex = -1;
                    ddlBudgetType.Items.FindByText(objYearEnd.BudgetType.ToString()).Selected = true;
                    ddlBudgetYear.SelectedIndex = -1;
                    ddlBudgetYear.Items.FindByValue(objYearEnd.BudgetYear.ToString()).Selected = true;
                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(objYearEnd.Status).Selected = true;
                    ddlStatus.Enabled = (objYearEnd.Status == "A");

                    ChangePageMode(Helper.PageMode.Edit);
                    EditBox.Visible = true;
                }
                else if (e.CommandName == "Download")
                {
                    GridViewRow selectedRow = gvYearEnd.Rows[Convert.ToInt32(e.CommandArgument)];
                    int yearendid = Convert.ToInt32(gvYearEnd.DataKeys[selectedRow.RowIndex]["YearEndID"]);
                    YearEnd objYearEnd = ((List<YearEnd>)Session["YearEndData"]).Where(x => x.YearEndID == yearendid).FirstOrDefault();

                    string FolderPath = WebConfigurationManager.AppSettings["BudgetArchiveFolderPath"];
                    string FileName = objYearEnd.BudgetYear + "_" + objYearEnd.BudgetType + ".xls";

                    if (File.Exists(FolderPath + FileName))
                    {
                        System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                        response.ClearContent();
                        response.Clear();
                        response.ContentType = "application/vnd.ms-excel";
                        response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ";");
                        response.TransmitFile(FolderPath + FileName);
                        response.Flush();
                        response.End();
                    }
                    else
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "File not found : " + FolderPath + FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void gvYearEnd_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    YearEnd rowItem = (YearEnd)e.Row.DataItem;

                    LinkButton lb = ((LinkButton)e.Row.FindControl("btnStatus"));
                    Label lbl = ((Label)e.Row.FindControl("lblStatus"));

                    lb.Text = rowItem.Status;
                    lb.Visible = (rowItem.Status == "Processed");
                    lbl.Text = rowItem.Status;
                    lbl.Visible = !(rowItem.Status == "Processed");

                    ScriptManager.GetCurrent(this).RegisterPostBackControl(lb);
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
                List<YearEnd> data = new YearEndBAL().GetYearEnds();
                Session["YearEndData"] = data;
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
                gvYearEnd.DataSource = ((List<YearEnd>)Session["YearEndData"])
                                        .Select(x => new YearEnd()
                                        {
                                            YearEndID = x.YearEndID,
                                            BudgetType = x.BudgetType,
                                            BudgetYear = x.BudgetYear,
                                            Status = ((x.Status == "A") ? "Active" : "Processed")
                                        }).OrderBy(x => x.BudgetYear).ThenBy(x => x.BudgetType).ThenByDescending(x => x.Status).ToList();
                gvYearEnd.DataBind();
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
                for (int i = DateTime.Now.Year; i <= DateTime.Now.Year + 10; i++)
                {
                    ddlBudgetYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
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
                    ddlBudgetType.Enabled = true;
                    ddlBudgetYear.Enabled = true;
                    ddlStatus.Enabled = true;
                    break;
                case Helper.PageMode.Edit:
                    ddlBudgetType.Enabled = false;
                    ddlBudgetYear.Enabled = false;
                    break;
            }
            Session["YearEndPageMode"] = pagemode;
        }

        private void ClearPageData()
        {
            try
            {
                ddlBudgetType.SelectedIndex = 0;
                ddlBudgetYear.SelectedIndex = 0;
                ddlStatus.SelectedIndex = 0;

                foreach (GridViewRow gvr in gvYearEnd.Rows)
                    gvr.Style["background-color"] = "";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void ArchiveBudget()
        {
            try
            {
                //string FolderPath = WebConfigurationManager.AppSettings["BudgetArchiveFolderPath"];
                //string FileName = DateTime.Now.ToString("MMyyyy_dd_HHmmss") + "_Backup.backup";
                //ConnectionStringSettings css = ConfigurationManager.ConnectionStrings["ExcelConString1"];
                //string Result = string.Empty;
                //string EXEPath = "cd /D " + Server.MapPath("~") + "files\\";

                ////new CommonHelperBAL().BackupDB(EXEPath, FolderPath, FileName, ref Result);
                //System.Data.Common.DbConnectionStringBuilder builder = new CommonHelperBAL().GetConnectionStringBuilder();

                ////if (File.Exists(Server.MapPath("~") + "files//" + "DBBackup_" + LoggedInUser.UserID + ".bat"))
                ////{
                ////    File.Delete(Server.MapPath("~") + "files//" + "DBBackup_" + LoggedInUser.UserID + ".bat");
                ////}
                ////File.Create(Server.MapPath("~") + "files//" + "DBBackup_" + LoggedInUser.UserID + ".bat");

                //StreamWriter sw = new StreamWriter(Server.MapPath("~") + "files//" + "DBBackup_" + LoggedInUser.UserID + ".bat");
                //// Do not change lines / spaces b/w words.
                //StringBuilder strSB = new StringBuilder(EXEPath);

                //if (strSB.Length != 0)
                //{
                //    strSB.Append("pg_dump.exe --host " + builder["HOST"].ToString() + " --port " + builder["PORT"].ToString() + " --username " + builder["USER ID"].ToString() + " --format custom --blobs --verbose --file ");
                //    strSB.Append("\"" + FolderPath + FileName + "\"");
                //    strSB.Append(" \"" + builder["DATABASE"].ToString() + "\r\n\r\n");
                //    sw.WriteLine(strSB);
                //    sw.Dispose();
                //    sw.Close();
                //    Process processDB = Process.Start(Server.MapPath("~") + "files//" + "DBBackup_" + LoggedInUser.UserID + ".bat");
                //    do
                //    {//dont perform anything
                //    }
                //    while (!processDB.HasExited);
                //    {
                //        //MessageBox.Show(strDatabaseName + " Successfully Backed up at " + textBox1.Text);
                //    }
                //}
                //else
                //{
                //    //MessageBox.Show("Please Provide the Location to take Backup!");
                //}

                string FolderPath = WebConfigurationManager.AppSettings["BudgetArchiveFolderPath"];

                DataTable dt = new DataTable();
                if (ddlBudgetType.SelectedValue == "Mengurus")
                    dt = GetMengurusExcel();
                else if (ddlBudgetType.SelectedValue == "Perjawatan")
                    dt = GetPerjawatanExcel();
                else if (ddlBudgetType.SelectedValue == "Projek")
                    dt = GetProjekExcel();

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                string ExcelData = new ReportHelper().GetExcelXml(ds, string.Empty);

                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string filename = FolderPath + ddlBudgetYear.SelectedValue + "_" + ddlBudgetType.SelectedValue + ".xls";
                if (File.Exists(filename))
                    File.Delete(filename);

                File.WriteAllText(filename, ExcelData);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private DataTable GetMengurusExcel()
        {
            DataTable dt = new DataTable();

            try
            {
                List<BudgetMengurusYearEnd> data = new BudgetMengurusBAL().BudgetMengurusYearEnd(Convert.ToInt32(ddlBudgetYear.SelectedValue));
                List<string> acccodes = data.Select(x => x.AccountCode).Distinct().OrderBy(x => x).ToList();
                List<string> prefixs = data.Select(x => x.Prefix).Distinct().OrderBy(x => x).ToList();
                List<string> periods = data.Select(x => x.PeriodMengurus).Distinct().OrderBy(x => x).ToList();

                //Start Build DataTable
                DataColumn dc = new DataColumn();
                foreach (Segment seg in new SegmentBAL().GetSegments().OrderBy(x => x.SegmentOrder))
                {
                    dc = new DataColumn(seg.SegmentName);
                    dt.Columns.Add(dc);
                }
                dc = new DataColumn("AccountCode");
                dt.Columns.Add(dc);
                dc = new DataColumn("Objeck");
                dt.Columns.Add(dc);
                dc = new DataColumn("Description");
                dt.Columns.Add(dc);
                foreach (string pe in periods)
                {
                    dc = new DataColumn(pe);
                    dt.Columns.Add(dc);
                }
                //End Build DataTable

                //Start pushing data into DataTable
                foreach (string pr in prefixs)
                {
                    foreach (string ac in acccodes)
                    {
                        List<BudgetMengurusYearEnd> subset = data.Where(x => x.AccountCode == ac && x.Prefix == pr).ToList();
                        if (subset.Count > 0)
                        {
                            int c = 0;
                            DataRow dr = dt.NewRow();
                            foreach (string sgd in subset[0].ListSegmentDetails)
                            {
                                dr[c] = sgd;
                                c++;
                            }
                            dr[c] = ac;
                            c++;
                            dr[c] = pr + "-" + ac;
                            c++;
                            dr[c] = subset[0].Description;
                            c++;
                            foreach (string pe in periods)
                            {
                                BudgetMengurusYearEnd d = subset.Where(x => x.PeriodMengurus == pe).FirstOrDefault();

                                if (d != null)
                                    dr[c] = d.Amount.ToString("F");
                                else
                                    dr[c] = string.Empty;

                                c++;
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                //End pushing data into DataTable

            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }

            return dt;
        }

        private DataTable GetPerjawatanExcel()
        {
            DataTable dt = new DataTable();

            try
            {
                List<BudgetPerjawatanYearEnd> data = new BudgetPerjawatanBAL().BudgetPerjawatanYearEnd(Convert.ToInt32(ddlBudgetYear.SelectedValue));
                List<string> sercodes = data.Select(x => x.GroupPerjawatanCode).Distinct().OrderBy(x => x).ToList();
                List<string> prefixs = data.Select(x => x.Prefix).Distinct().OrderBy(x => x).ToList();
                List<string> periods = data.Select(x => x.PeriodPerjawatan).Distinct().OrderBy(x => x).ToList();

                //Start Build DataTable
                DataColumn dc = new DataColumn();
                foreach (Segment seg in new SegmentBAL().GetSegments().OrderBy(x => x.SegmentOrder))
                {
                    dc = new DataColumn(seg.SegmentName);
                    dt.Columns.Add(dc);
                }
                dc = new DataColumn("ServiceCode");
                dt.Columns.Add(dc);
                dc = new DataColumn("Objeck");
                dt.Columns.Add(dc);
                dc = new DataColumn("Description");
                dt.Columns.Add(dc);
                foreach (string pe in periods)
                {
                    dc = new DataColumn(pe);
                    dt.Columns.Add(dc);
                }
                //End Build DataTable

                //Start pushing data into DataTable
                foreach (string pr in prefixs)
                {
                    foreach (string ac in sercodes)
                    {
                        List<BudgetPerjawatanYearEnd> subset = data.Where(x => x.GroupPerjawatanCode == ac && x.Prefix == pr).ToList();
                        if (subset.Count > 0)
                        {
                            int c = 0;
                            DataRow dr = dt.NewRow();
                            foreach (string sgd in subset[0].ListSegmentDetails)
                            {
                                dr[c] = sgd;
                                c++;
                            }
                            dr[c] = ac;
                            c++;
                            dr[c] = pr + "-" + ac;
                            c++;
                            dr[c] = subset[0].Description;
                            c++;
                            foreach (string pe in periods)
                            {
                                BudgetPerjawatanYearEnd d = subset.Where(x => x.PeriodPerjawatan == pe).FirstOrDefault();

                                if (d != null)
                                    dr[c] = d.Amount.ToString("F");
                                else
                                    dr[c] = string.Empty;

                                c++;
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                //End pushing data into DataTable

            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }

            return dt;
        }

        private DataTable GetProjekExcel()
        {
            DataTable dt = new DataTable();

            try
            {
                List<BudgetProjekYearEnd> data = new BudgetProjekBAL().BudgetProjekYearEnd(Convert.ToInt32(ddlBudgetYear.SelectedValue));
                List<string> acccodes = data.Select(x => x.AccountCode).Distinct().OrderBy(x => x).ToList();
                List<string> prefixs = data.Select(x => x.Prefix).Distinct().OrderBy(x => x).ToList();
                List<string> periods = data.Select(x => x.PeriodMengurus).Distinct().OrderBy(x => x).ToList();

                //Start Build DataTable
                DataColumn dc = new DataColumn();
                foreach (Segment seg in new SegmentBAL().GetSegments().OrderBy(x => x.SegmentOrder))
                {
                    dc = new DataColumn(seg.SegmentName);
                    dt.Columns.Add(dc);
                }
                dc = new DataColumn("AccountCode");
                dt.Columns.Add(dc);
                dc = new DataColumn("Object");
                dt.Columns.Add(dc);
                dc = new DataColumn("Description");
                dt.Columns.Add(dc);
                foreach (string pe in periods)
                {
                    dc = new DataColumn(pe);
                    dt.Columns.Add(dc);
                }
                //End Build DataTable

                //Start pushing data into DataTable
                foreach (string pr in prefixs)
                {
                    foreach (string ac in acccodes)
                    {
                        List<BudgetProjekYearEnd> subset = data.Where(x => x.AccountCode == ac && x.Prefix == pr).ToList();
                        if (subset.Count > 0)
                        {
                            int c = 0;
                            DataRow dr = dt.NewRow();
                            foreach (string sgd in subset[0].ListSegmentDetails)
                            {
                                dr[c] = sgd;
                                c++;
                            }
                            dr[c] = ac;
                            c++;
                            dr[c] = pr + "-" + ac;
                            c++;
                            dr[c] = subset[0].Description;
                            c++;
                            foreach (string pe in periods)
                            {
                                BudgetProjekYearEnd d = subset.Where(x => x.PeriodMengurus == pe).FirstOrDefault();

                                if (d != null)
                                    dr[c] = d.Amount.ToString("F");
                                else
                                    dr[c] = string.Empty;

                                c++;
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                //End pushing data into DataTable

            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }

            return dt;
        }
    }
}