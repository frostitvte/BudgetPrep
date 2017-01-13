using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using BudgetPrep.Classes;
using OBBAL;
using OBDAL;

namespace BudgetPrep
{
    public class SegmentDetailTreeHelper
    {
        public int SegmentDetailID { get; set; }
        public int SegmentID { get; set; }
        public string DetailCode { get; set; }
        public string DetailDesc { get; set; }
        public int ParentDetailID { get; set; }
        public string Status { get; set; }
        public int Level { get; set; }
        //public bool IsExpanded { get; set; }
        public int ChildCount { get; set; }
    }

    public class SegmentDetailImport
    {
        public string DetailCode { get; set; }
        public string DetailDesc { get; set; }
        public string Status { get; set; }
    }

    public class SegmentDetailExport
    {
        public string SegmentName { get; set; }
        public string DetailCode { get; set; }
        public string DetailDesc { get; set; }
        public string Status { get; set; }
    }

    public partial class SegmentDetails : PageHelper
    {
        Segment selectedSegment;
        List<int> SelectedNodes;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                selectedSegment = (Segment)Session["SelectedSegment"];
                if (selectedSegment == null)
                {
                    Response.Redirect("~/SegmentSetup.aspx");
                }

                RegExpValidatorCode.ValidationExpression = "^[\\s\\S]{1," + selectedSegment.ShapeFormat.Length.ToString() + "}$";

                if (!Page.IsPostBack)
                {
                    Session["SelectedNodes"] = null;
                    ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                    ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Segment Details - " + selectedSegment.SegmentName;

                    GetData();
                    CreateTreeData();
                    LoadDropDown();
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
                selectedSegment = (Segment)Session["SelectedSegment"];
                List<SegmentDetail> data = new SegmentDetailsBAL().GetSegmentDetails(selectedSegment.SegmentID);
                if (data.Count == 0)
                {
                    data = new List<SegmentDetail>();
                    data.Add(new SegmentDetail() { SegmentID = 0, SegmentDetailID = 0, ParentDetailID = 0 });

                    List<OBDAL.YearUploadSetup> GetData = new YearUploadBAL().GetYearUpload();
                    OBDAL.YearUploadSetup curryear = GetData.Where(x => x.BudgetYear == DateTime.Now.Year).FirstOrDefault();

                    if (curryear.ToString().Count() > 0)
                    {
                        if (GetData.Where(y => y.BudgetYear == curryear.BudgetYear).Select(z => z.Status.Contains("A")).FirstOrDefault())
                        {
                            btnFileUpload.Visible = true;
                            lblMsg.Visible = false;
                        }
                        else
                        {
                            lblMsg.Visible = true;
                        }
                    }
                    else
                    {
                        lblMsg.Visible = true;
                    }

                    btnPrint.Visible = false;
                }
                else
                {
                    btnFileUpload.Visible = false;
                    btnPrint.Visible = true;
                }

                Session["SegmentDetailsData"] = data;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void CreateTreeData()
        {
            try
            {
                SelectedNodes = (List<int>)Session["SelectedNodes"];
                List<SegmentDetail> data = (List<SegmentDetail>)Session["SegmentDetailsData"];
                List<SegmentDetailTreeHelper> TreeData = new List<SegmentDetailTreeHelper>();
                if (data.Count > 0)
                {
                    TreeData = data.Where(x => x.ParentDetailID == 0).OrderBy(x => x.DetailCode).Select(x =>
                            new SegmentDetailTreeHelper()
                            {
                                SegmentDetailID = x.SegmentDetailID,
                                SegmentID = Convert.ToInt32(x.SegmentID),
                                DetailCode = x.DetailCode,
                                DetailDesc = x.DetailDesc,
                                ParentDetailID = Convert.ToInt32(x.ParentDetailID),
                                Status = x.Status,
                                Level = 0,
                                ChildCount = data.Where(y => y.ParentDetailID == x.SegmentDetailID).Count()
                            }).ToList();

                    if (SelectedNodes == null || SelectedNodes.Count == 0)
                    {
                        Session["SelectedNodes"] = new List<int>();
                        SelectedNodes = new List<int>();
                    }
                    else
                    {
                        //while(TreeData.Where(x=>x.IsExpanded).Select(x=>x).Count() < SelectedNodes.Count)
                        //{
                        for (int i = 0; i < TreeData.Count; i++)
                        {
                            if (SelectedNodes.Contains(TreeData[i].SegmentDetailID))
                            {
                                //TreeData[i].IsExpanded = true;
                                foreach (SegmentDetail sd in data.Where(x => x.ParentDetailID == TreeData[i].SegmentDetailID).OrderByDescending(x => x.DetailCode))
                                {
                                    SegmentDetailTreeHelper objSH = new SegmentDetailTreeHelper()
                                        {
                                            SegmentDetailID = sd.SegmentDetailID,
                                            SegmentID = Convert.ToInt32(sd.SegmentID),
                                            DetailCode = sd.DetailCode,
                                            DetailDesc = sd.DetailDesc,
                                            ParentDetailID = Convert.ToInt32(sd.ParentDetailID),
                                            Status = sd.Status,
                                            Level = TreeData[i].Level + 1,
                                            ChildCount = data.Where(y => y.ParentDetailID == sd.SegmentDetailID).Count()
                                        };
                                    TreeData.Insert(i + 1, objSH);
                                }
                            }
                        }
                        //}
                    }
                }
                Session["SegmentDetailsTree"] = TreeData;
                BindGrid();
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
                gvSegmentDetails.DataSource = (List<SegmentDetailTreeHelper>)Session["SegmentDetailsTree"];
                gvSegmentDetails.DataBind();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void ChangeSeletedNodeStyle(GridViewRow GridViewRow)
        {
            try
            {
                foreach (GridViewRow gvr in gvSegmentDetails.Rows)
                    gvr.Style["background-color"] = "";
                GridViewRow.Style["background-color"] = "gold";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void AssignSelectedNode(int selectedSegmentDetailID)
        {
            try
            {
                Session["SelectedSegmentDetail"] = ((List<SegmentDetail>)Session["SegmentDetailsData"]).Where(x => x.SegmentDetailID == selectedSegmentDetailID).FirstOrDefault();
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
                tbCode.Text = string.Empty;
                tbDesc.Text = string.Empty;
                ddlStatus.SelectedIndex = 0;

                foreach (GridViewRow gvr in gvSegmentDetails.Rows)
                    gvr.Style["background-color"] = "";

                EditBox.Visible = false;
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
                SegmentDetail objSegmentDetail = new SegmentDetail();
                objSegmentDetail.SegmentID = ((Segment)Session["SelectedSegment"]).SegmentID;
                objSegmentDetail.DetailCode = tbCode.Text.Trim();
                objSegmentDetail.DetailDesc = tbDesc.Text.Trim();
                objSegmentDetail.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                if ((SegmentDetail)Session["SelectedSegmentDetail"] == null)
                    objSegmentDetail.ParentDetailID = 0;
                else
                    objSegmentDetail.ParentDetailID = ((SegmentDetail)Session["SelectedSegmentDetail"]).SegmentDetailID;

                if (((Helper.PageMode)Session["PageMode"]) == Helper.PageMode.New)
                {
                    if (new SegmentDetailsBAL().GetSegmentDetails(((Segment)Session["SelectedSegment"]).SegmentID)
                        .Where(x => x.DetailCode.ToUpper().Trim() == tbCode.Text.ToUpper().Trim()).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "SegmentDetail already exists");
                        return;
                    }
                    objSegmentDetail.CreatedBy = LoggedInUser.UserID;
                    objSegmentDetail.CreatedTimeStamp = DateTime.Now;
                    objSegmentDetail.ModifiedBy = LoggedInUser.UserID;
                    objSegmentDetail.ModifiedTimeStamp = DateTime.Now;

                    if(new SegmentDetailsBAL().InsertSegmentDetail(objSegmentDetail))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Segment Detail saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Segment Detail");
                }
                else
                {
                    objSegmentDetail.ModifiedBy = LoggedInUser.UserID;
                    objSegmentDetail.ModifiedTimeStamp = DateTime.Now;

                    objSegmentDetail.SegmentDetailID = ((SegmentDetail)Session["SelectedSegmentDetail"]).SegmentDetailID;
                    objSegmentDetail.ParentDetailID = ((SegmentDetail)Session["SelectedSegmentDetail"]).ParentDetailID;
                    if(new SegmentDetailsBAL().UpdateSegmentDetail(objSegmentDetail))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Segment Detail updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Segment Detail");
                }

                ClearPageData();
                Session["SelectedSegmentDetail"] = null;
                GetData();
                CreateTreeData();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPageData();
            Session["SelectedSegmentDetail"] = null;
        }

        //protected void btnAdd_Click(object sender, EventArgs e)
        //{
        //    Session["PageMode"] = Helper.PageMode.New;
        //    EditBox.Visible = true;
        //    tbCode.Enabled = true;
        //}

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                DataSet ds = new DataSet();
                //List<SegmentExport> data = .ToList();
                ds.Tables.Add(new ReportHelper().ToDataTable<SegmentDetailExport>(
                        ((List<SegmentDetail>)Session["SegmentDetailsData"])
                            .Select(x => new SegmentDetailExport()
                            {
                                SegmentName = x.Segment.SegmentName,
                                DetailCode = x.DetailCode,
                                DetailDesc = x.DetailDesc,
                                Status = ((x.Status == "A") ? "Active" : "Inactive")
                            })
                            .OrderBy(x => x.DetailCode)
                    ));
                new ReportHelper().ToExcel(ds, "SegmentDetails.xls", ref Response);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void gvSegmentDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    SegmentDetailTreeHelper rowItem = (SegmentDetailTreeHelper)e.Row.DataItem;
                    //((Label)e.Row.FindControl("lblIndent")).Width = Unit.Pixel(rowItem.Level * 30);
                    //((Label)e.Row.FindControl("lblDetailCode")).Text = rowItem.DetailCode;
                    int width = rowItem.Level * 30;

                    string strHTML = string.Empty;
                    if (rowItem.ChildCount > 0)
                    {
                        if (SelectedNodes.Contains(rowItem.SegmentDetailID))
                            strHTML = "<label style=\"width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i class=\"fa fa-minus-square pull-right\"></i></label> ";
                        else
                            strHTML = "<label style=\"width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i class=\"fa fa-plus-square pull-right\"></i></label> ";
                    }
                    else
                        strHTML = "<label style=\"width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i></i></label> ";

                    LinkButton btnExpand = ((LinkButton)e.Row.FindControl("btnExpand"));
                    btnExpand.Text = "<div>" + strHTML + rowItem.DetailCode + "</div>";

                    if (rowItem.ParentDetailID != 0)
                        ((LinkButton)e.Row.FindControl("lbAddItem")).Visible = false;
                    if (rowItem.ParentDetailID == 0)
                        ((LinkButton)e.Row.FindControl("lbMakeRoot")).Visible = false;
                    if (rowItem.SegmentDetailID == 0)
                    {
                        ((LinkButton)e.Row.FindControl("btnExpand")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbEit")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbDelete")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbCut")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbPaste")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbMakeRoot")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbAddChild")).Visible = false;
                    }

                    if (Session["SelectedSegmentDetail"] != null && ((SegmentDetail)Session["SelectedSegmentDetail"]).SegmentDetailID == rowItem.SegmentDetailID)
                    {
                        e.Row.Style["background-color"] = "gold";
                    }
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void gvSegmentDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                List<SegmentDetailTreeHelper> TreeData = (List<SegmentDetailTreeHelper>)Session["SegmentDetailsTree"];
                GridViewRow selectedRow = gvSegmentDetails.Rows[Convert.ToInt32(e.CommandArgument)];
                int SegmentDetailID = Convert.ToInt32(gvSegmentDetails.DataKeys[selectedRow.RowIndex]["SegmentDetailID"]);
                if (e.CommandName == "Expand")
                {
                    SelectedNodes = (List<int>)Session["SelectedNodes"];
                    if (!SelectedNodes.Contains(SegmentDetailID))
                    {
                        if (TreeData.Where(x => x.SegmentDetailID == SegmentDetailID).FirstOrDefault().ChildCount > 0)
                            SelectedNodes.Add(SegmentDetailID);
                    }
                    else
                    {
                        SelectedNodes.Remove(SegmentDetailID);
                    }
                    CreateTreeData();
                }
                else if (e.CommandName == "AddItem")
                {
                    ChangeSeletedNodeStyle(selectedRow);
                    Session["PageMode"] = Helper.PageMode.New;
                    EditBox.Visible = true;
                    tbCode.Enabled = true;
                }
                else if (e.CommandName == "AddChild")
                {
                    ClearPageData();
                    ChangeSeletedNodeStyle(selectedRow);
                    AssignSelectedNode(SegmentDetailID);
                    Session["PageMode"] = Helper.PageMode.New;
                    EditBox.Visible = true;
                    tbCode.Enabled = true;
                }
                else if (e.CommandName == "MakeRoot")
                {
                    ChangeSeletedNodeStyle(selectedRow);
                    AssignSelectedNode(SegmentDetailID);
                    MakeRoot();
                    ClearPageData();
                }
                else if (e.CommandName == "CmdEdit")
                {
                    ChangeSeletedNodeStyle(selectedRow);
                    AssignSelectedNode(SegmentDetailID);
                    CommandEdit();
                    Session["PageMode"] = Helper.PageMode.Edit;
                    EditBox.Visible = true;
                    tbCode.Enabled = false;
                }
                else if (e.CommandName == "CmdDelete")
                {
                    AssignSelectedNode(SegmentDetailID);
                    SegmentDetail objSegmentDetail = (SegmentDetail)Session["SelectedSegmentDetail"];
                    objSegmentDetail.Status = "D";
                    objSegmentDetail.ModifiedBy = LoggedInUser.UserID;
                    objSegmentDetail.ModifiedTimeStamp = DateTime.Now;
                    if(new SegmentDetailsBAL().UpdateSegmentDetail(objSegmentDetail))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Segment Detail updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Segment Detail");
                    ClearPageData();
                    Session["SelectedSegmentDetail"] = null;
                    GetData();
                    CreateTreeData();
                }
                else if (e.CommandName == "CmdCut")
                {
                    ChangeSeletedNodeStyle(selectedRow);
                    AssignSelectedNode(SegmentDetailID);
                }
                else if (e.CommandName == "CmdPaste")
                {
                    CommandPaste(SegmentDetailID);
                    GetData();
                    CreateTreeData();
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void CommandPaste(int ParentSegmentDetailID)
        {
            try
            {
                SegmentDetail cutSegmentDetail = (SegmentDetail)Session["SelectedSegmentDetail"];

                List<SegmentDetail> data = (List<SegmentDetail>)Session["SegmentDetailsData"];
                SegmentDetail parent = new SegmentDetail() { ParentDetailID = ParentSegmentDetailID };
                do
                {
                    parent = data.Where(x => x.SegmentDetailID == parent.ParentDetailID).FirstOrDefault();
                    if (parent == null || parent.SegmentDetailID == cutSegmentDetail.SegmentDetailID)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "You can not paste a parent under its child");
                        return;
                    }
                } while (parent.ParentDetailID != 0);

                cutSegmentDetail.ParentDetailID = ParentSegmentDetailID;
                new SegmentDetailsBAL().UpdateSegmentDetail(cutSegmentDetail);
                SelectedNodes = (List<int>)Session["SelectedNodes"];
                if (!SelectedNodes.Contains(ParentSegmentDetailID))
                {
                    if (((List<SegmentDetailTreeHelper>)Session["SegmentDetailsTree"]).Where(x => x.SegmentDetailID == ParentSegmentDetailID).FirstOrDefault().ChildCount > 0)
                        SelectedNodes.Add(ParentSegmentDetailID);
                }
                Session["SelectedNodes"] = SelectedNodes;
                GetData();
                CreateTreeData();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void MakeRoot()
        {
            try
            {
                SegmentDetail cutSegmentDetail = (SegmentDetail)Session["SelectedSegmentDetail"];
                cutSegmentDetail.ParentDetailID = 0;
                new SegmentDetailsBAL().UpdateSegmentDetail(cutSegmentDetail);
                SelectedNodes = (List<int>)Session["SelectedNodes"];
                if (!SelectedNodes.Contains(cutSegmentDetail.SegmentDetailID))
                {
                    if (((List<SegmentDetailTreeHelper>)Session["SegmentDetailsTree"]).Where(x => x.SegmentDetailID == cutSegmentDetail.SegmentDetailID).FirstOrDefault().ChildCount > 0)
                        SelectedNodes.Add(cutSegmentDetail.SegmentDetailID);
                }
                Session["SelectedNodes"] = SelectedNodes;
                GetData();
                CreateTreeData();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void CommandEdit()
        {
            try
            {
                SegmentDetail objSegmentDetail = (SegmentDetail)Session["SelectedSegmentDetail"];
                tbCode.Text = objSegmentDetail.DetailCode;
                tbDesc.Text = objSegmentDetail.DetailDesc;
                ddlStatus.SelectedIndex = -1;
                if (objSegmentDetail.Status != null)
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objSegmentDetail.Status))).Selected = true;
                else
                    ddlStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void LoadDropDown()
        {
            ddlStatus.DataSource = Enum.GetValues(typeof(Helper.ItemStatus));
            ddlStatus.DataBind();
        }

        protected void btnSample_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                new ReportHelper().DownloadSample<SegmentDetailImport>(ref Response);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(FileUpload1.PostedFile.FileName))
                {
                    HttpPostedFile objFile = HttpContext.Current.Request.Files[0];

                    string FolderPath = WebConfigurationManager.AppSettings["BudgetUploadFolderPath"];

                    if (!Directory.Exists(FolderPath))
                        Directory.CreateDirectory(FolderPath);

                    string FileName = DateTime.Now.ToString("MMyyyyddHHmmss_") + LoggedInUser.UserID + "_" + System.IO.Path.GetFileName(objFile.FileName);
                    string FullFileName = System.IO.Path.Combine(FolderPath, FileName);
                    Session["FullFileName"] = FullFileName;

                    objFile.SaveAs(FullFileName);

                    List<string> lstErrors = new List<string>();
                    DataTable dt = new DataTable();

                    selectedSegment = (Segment)Session["SelectedSegment"];

                    if (Path.GetExtension(FileName) == ".csv")
                    {
                        DataSet ds = new ReportHelper().ExcelToDataSet(FullFileName, Path.GetExtension(FileName));
                        dt = new ReportHelper().Validate<SegmentDetailImport>(ds, selectedSegment.ShapeFormat, ref lstErrors);

                        if (lstErrors.Count == 0)
                        {
                            try
                            {
                                List<SegmentDetail> data = (List<SegmentDetail>)Session["SegmentDetailsData"];
                                if (data.Where(x => x.SegmentDetailID == 0).Count() > 0)
                                {
                                    data = null;
                                }

                                data = new ReportHelper().DataTableToList<SegmentDetailImport>(dt).Select(x => new SegmentDetail()
                                {
                                    SegmentID = ((Segment)Session["SelectedSegment"]).SegmentID,
                                    DetailCode = x.DetailCode,
                                    DetailDesc = x.DetailDesc,
                                    Status = ((x.Status == "Active") ? "A" : "D")
                                }).ToList();

                                if (data.Count > 0)
                                {
                                    foreach (SegmentDetail item in data)
                                    {
                                        SegmentDetail objSegmentDetail = new SegmentDetail();

                                        objSegmentDetail.SegmentID = item.SegmentID;
                                        objSegmentDetail.DetailCode = item.DetailCode;
                                        objSegmentDetail.DetailDesc = item.DetailDesc;
                                        objSegmentDetail.Status = item.Status;
                                        objSegmentDetail.ParentDetailID = 0;

                                        //checking on DB level
                                        if (new SegmentDetailsBAL().GetSegmentDetails(((Segment)Session["SelectedSegment"]).SegmentID)
                                            .Where(x => x.DetailCode.ToUpper().Trim() == tbCode.Text.ToUpper().Trim()).Count() > 0)
                                        {
                                            ((SiteMaster)this.Master).ShowMessage("Failure", "SegmentDetail already exists");
                                            return;
                                        }

                                        objSegmentDetail.CreatedBy = LoggedInUser.UserID;
                                        objSegmentDetail.CreatedTimeStamp = DateTime.Now;
                                        objSegmentDetail.ModifiedBy = LoggedInUser.UserID;
                                        objSegmentDetail.ModifiedTimeStamp = DateTime.Now;

                                        if (new SegmentDetailsBAL().InsertSegmentDetail(objSegmentDetail))
                                            ((SiteMaster)this.Master).ShowMessage("Success", "SegmentDetail uploaded successfully");
                                        else
                                            ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while uploading SegmentDetail");
                                    }

                                    GetData();
                                    CreateTreeData();
                                }
                            }
                            catch (Exception ex)
                            {
                                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
                            }
                        }
                        else
                        {
                            ((SiteMaster)this.Master).ShowMessage("Error", lstErrors.Aggregate((a, b) => a + "<br/>" + b));
                        }
                    }
                }
                else
                {
                    ((SiteMaster)this.Master).ShowMessage("Error", "Upload required file");
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }
    }
}