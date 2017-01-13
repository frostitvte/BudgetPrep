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
    public class GroupPerjawatanTreeHelper
    {
        public string GroupPerjawatanCode { get; set; }
        public string GroupPerjawatanDesc { get; set; }
        public string ParentGroupPerjawatanID { get; set; }
        public string Status { get; set; }
        //public bool IsExpanded { get; set; }
        public int Level { get; set; }
        public int ChildCount { get; set; }
    }

    public class ServiceCodeImport
    {
        public string ServiceCode { get; set; }
        public string ServiceDesc { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public string UpperLevel { get; set; }
    }

    public class ServiceCodeExport
    {
        public string ServiceCode { get; set; }
        public string ServiceDesc { get; set; }
        public string Status { get; set; }
    }

    public partial class GroupPerjawatanSetup : PageHelper
    {
        List<string> SelectedNodes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Session["SelectedNodes"] = null;
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Services Group";

                GetData();
                CreateTreeData();
                LoadDropDown();
            }

        }

        private void GetData()
        {
            try
            {
                List<GroupPerjawatan> data = new GroupPerjawatanBAL().GetGroupPerjawatans().ToList();
                if (data.Count == 0)
                {
                    data = new List<GroupPerjawatan>();
                    data.Add(new GroupPerjawatan() { GroupPerjawatanCode = string.Empty, ParentGroupPerjawatanID = string.Empty });

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

                Session["GroupPerjawatansData"] = data;
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
                SelectedNodes = (List<string>)Session["SelectedNodes"];
                List<GroupPerjawatan> data = (List<GroupPerjawatan>)Session["GroupPerjawatansData"];
                List<GroupPerjawatanTreeHelper> TreeData = new List<GroupPerjawatanTreeHelper>();
                if (data.Count > 0)
                {
                    TreeData = data.Where(x => x.ParentGroupPerjawatanID == string.Empty).OrderBy(x => x.GroupPerjawatanCode).Select(x =>
                            new GroupPerjawatanTreeHelper()
                            {
                                GroupPerjawatanCode = x.GroupPerjawatanCode,
                                GroupPerjawatanDesc = x.GroupPerjawatanDesc,
                                ParentGroupPerjawatanID = x.ParentGroupPerjawatanID,
                                Status = x.Status,
                                Level = 0,
                                ChildCount = data.Where(y => y.ParentGroupPerjawatanID == x.GroupPerjawatanCode).Count()
                            }).ToList();

                    if (SelectedNodes == null || SelectedNodes.Count == 0)
                    {
                        Session["SelectedNodes"] = new List<string>();
                        SelectedNodes = new List<string>();
                    }
                    else
                    {
                        //while(TreeData.Where(x=>x.IsExpanded).Select(x=>x).Count() < SelectedNodes.Count)
                        //{
                        for (int i = 0; i < TreeData.Count; i++)
                        {
                            if (SelectedNodes.Contains(TreeData[i].GroupPerjawatanCode))
                            {
                                //TreeData[i].IsExpanded = true;
                                foreach (GroupPerjawatan sd in data.Where(x => x.ParentGroupPerjawatanID == TreeData[i].GroupPerjawatanCode).OrderByDescending(x => x.GroupPerjawatanCode))
                                {
                                    GroupPerjawatanTreeHelper objSH = new GroupPerjawatanTreeHelper()
                                    {
                                        GroupPerjawatanCode = sd.GroupPerjawatanCode,
                                        GroupPerjawatanDesc = sd.GroupPerjawatanDesc,
                                        ParentGroupPerjawatanID = sd.ParentGroupPerjawatanID,
                                        Status = sd.Status,
                                        Level = TreeData[i].Level + 1,
                                        ChildCount = data.Where(y => y.ParentGroupPerjawatanID == sd.GroupPerjawatanCode).Count()
                                    };
                                    TreeData.Insert(i + 1, objSH);
                                }
                            }
                        }
                        //}
                    }
                }
                Session["GroupPerjawatansTree"] = TreeData;
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
                gvGroupPerjawatans.DataSource = (List<GroupPerjawatanTreeHelper>)Session["GroupPerjawatansTree"];
                gvGroupPerjawatans.DataBind();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void ChangeSeletedNodeStyle(GridViewRow GridViewRow)
        {
            foreach (GridViewRow gvr in gvGroupPerjawatans.Rows)
                gvr.Style["background-color"] = "";
            GridViewRow.Style["background-color"] = "gold";
        }

        private void AssignSelectedNode(string selectedGroupPerjawatanID)
        {
            try
            {
                Session["SelectedGroupPerjawatan"] = ((List<GroupPerjawatan>)Session["GroupPerjawatansData"]).Where(x => x.GroupPerjawatanCode == selectedGroupPerjawatanID).FirstOrDefault();
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

                foreach (GridViewRow gvr in gvGroupPerjawatans.Rows)
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
                GroupPerjawatan objGroupPerjawatan = new GroupPerjawatan();
                objGroupPerjawatan.GroupPerjawatanCode = tbCode.Text.Trim();
                objGroupPerjawatan.GroupPerjawatanDesc = tbDesc.Text.Trim();
                objGroupPerjawatan.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                if ((GroupPerjawatan)Session["SelectedGroupPerjawatan"] == null)
                    objGroupPerjawatan.ParentGroupPerjawatanID = string.Empty;
                else
                    objGroupPerjawatan.ParentGroupPerjawatanID = ((GroupPerjawatan)Session["SelectedGroupPerjawatan"]).GroupPerjawatanCode;

                if (((Helper.PageMode)Session["PageMode"]) == Helper.PageMode.New)
                {
                    if (new GroupPerjawatanBAL().GetGroupPerjawatans().Where(x => x.GroupPerjawatanCode.ToUpper().Trim() == tbCode.Text.ToUpper().Trim()).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "ServiceCode already exists");
                        return;
                    }
                    objGroupPerjawatan.CreatedBy = LoggedInUser.UserID;
                    objGroupPerjawatan.CreatedTimeStamp = DateTime.Now;
                    objGroupPerjawatan.ModifiedBy = LoggedInUser.UserID;
                    objGroupPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    if (new GroupPerjawatanBAL().InsertGroupPerjawatan(objGroupPerjawatan))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Service Group saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Service Group");
                }
                else
                {
                    objGroupPerjawatan.ModifiedBy = LoggedInUser.UserID;
                    objGroupPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    objGroupPerjawatan.GroupPerjawatanCode = ((GroupPerjawatan)Session["SelectedGroupPerjawatan"]).GroupPerjawatanCode;
                    objGroupPerjawatan.ParentGroupPerjawatanID = ((GroupPerjawatan)Session["SelectedGroupPerjawatan"]).ParentGroupPerjawatanID;
                    if (new GroupPerjawatanBAL().UpdateGroupPerjawatan(objGroupPerjawatan))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Service Group updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Service Group");
                }

                ClearPageData();
                Session["SelectedGroupPerjawatan"] = null;
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
            Session["SelectedGroupPerjawatan"] = null;
        }

        //protected void btnAdd_Click(object sender, EventArgs e)
        //{
        //    Session["PageMode"] = Helper.PageMode.New;
        //    EditBox.Visible = true;
        //    tbCode.Enabled = true;
        //}

        private List<GroupPerjawatanTreeHelper> CreateExportData()
        {
            List<GroupPerjawatanTreeHelper> TreeData = new List<GroupPerjawatanTreeHelper>();
            try
            {
                List<GroupPerjawatan> data = (List<GroupPerjawatan>)Session["GroupPerjawatansData"];
                if (data.Count > 0)
                {
                    TreeData = data.Where(x => x.ParentGroupPerjawatanID == string.Empty).OrderBy(x => x.GroupPerjawatanCode).Select(x =>
                            new GroupPerjawatanTreeHelper()
                            {
                                GroupPerjawatanCode = x.GroupPerjawatanCode,
                                GroupPerjawatanDesc = x.GroupPerjawatanDesc,
                                ParentGroupPerjawatanID = x.ParentGroupPerjawatanID,
                                Status = x.Status,
                                Level = 0,
                                ChildCount = data.Where(y => y.ParentGroupPerjawatanID == x.GroupPerjawatanCode).Count()
                            }).ToList();

                    for (int i = 0; i < TreeData.Count; i++)
                    {
                        foreach (GroupPerjawatan sd in data.Where(x => x.ParentGroupPerjawatanID == TreeData[i].GroupPerjawatanCode).OrderByDescending(x => x.GroupPerjawatanCode))
                        {
                            GroupPerjawatanTreeHelper objSH = new GroupPerjawatanTreeHelper()
                            {
                                GroupPerjawatanCode = sd.GroupPerjawatanCode,
                                GroupPerjawatanDesc = sd.GroupPerjawatanDesc,
                                ParentGroupPerjawatanID = sd.ParentGroupPerjawatanID,
                                Status = sd.Status,
                                Level = TreeData[i].Level + 1,
                                ChildCount = data.Where(y => y.ParentGroupPerjawatanID == sd.GroupPerjawatanCode).Count()
                            };
                            TreeData.Insert(i + 1, objSH);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
            return TreeData;
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                DataSet ds = new DataSet();
                ds.Tables.Add(new ReportHelper().ToDataTable<ServiceCodeExport>(
                        CreateExportData()
                            .Select(x => new ServiceCodeExport()
                            {
                                ServiceCode = new Helper().GetLevelString(x.GroupPerjawatanCode, x.Level),
                                ServiceDesc = x.GroupPerjawatanDesc,
                                Status = ((x.Status == "A") ? "Active" : "Inactive")
                            })
                    ));
                new ReportHelper().ToExcel(ds, "ServiceCodes.xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvGroupPerjawatans_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    GroupPerjawatanTreeHelper rowItem = (GroupPerjawatanTreeHelper)e.Row.DataItem;
                    //((Label)e.Row.FindControl("lblIndent")).Width = Unit.Pixel(rowItem.Level * 30);
                    //((Label)e.Row.FindControl("lblDetailCode")).Text = rowItem.DetailCode;
                    int width = rowItem.Level * 30;

                    string strHTML = string.Empty;
                    if (rowItem.ChildCount > 0)
                    {
                        if (SelectedNodes.Contains(rowItem.GroupPerjawatanCode))
                            strHTML = "<label style=\"width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i class=\"fa fa-minus-square pull-right\"></i></label> ";
                        else
                            strHTML = "<label style=\"width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i class=\"fa fa-plus-square pull-right\"></i></label> ";
                    }
                    else
                        strHTML = "<label style=\"width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i></i></label> ";

                    LinkButton btnExpand = ((LinkButton)e.Row.FindControl("btnExpand"));
                    btnExpand.Text = "<div>" + strHTML + rowItem.GroupPerjawatanCode + "</div>";

                    if (rowItem.ParentGroupPerjawatanID != string.Empty)
                        ((LinkButton)e.Row.FindControl("lbAddItem")).Visible = false;
                    if (rowItem.ParentGroupPerjawatanID == string.Empty)
                        ((LinkButton)e.Row.FindControl("lbMakeRoot")).Visible = false;
                    if (rowItem.GroupPerjawatanCode == string.Empty)
                    {
                        ((LinkButton)e.Row.FindControl("btnExpand")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbEit")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbDelete")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbCut")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbPaste")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbMakeRoot")).Visible = false;
                        ((LinkButton)e.Row.FindControl("lbAddChild")).Visible = false;
                    }

                    if (Session["SelectedGroupPerjawatan"] != null && ((GroupPerjawatan)Session["SelectedGroupPerjawatan"]).GroupPerjawatanCode == rowItem.GroupPerjawatanCode)
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

        protected void gvGroupPerjawatans_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                List<GroupPerjawatanTreeHelper> TreeData = (List<GroupPerjawatanTreeHelper>)Session["GroupPerjawatansTree"];
                GridViewRow selectedRow = gvGroupPerjawatans.Rows[Convert.ToInt32(e.CommandArgument)];
                string GroupPerjawatan = gvGroupPerjawatans.DataKeys[selectedRow.RowIndex]["GroupPerjawatanCode"].ToString();
                if (e.CommandName == "Expand")
                {
                    SelectedNodes = (List<string>)Session["SelectedNodes"];
                    if (!SelectedNodes.Contains(GroupPerjawatan))
                    {
                        if (TreeData.Where(x => x.GroupPerjawatanCode == GroupPerjawatan).FirstOrDefault().ChildCount > 0)
                            SelectedNodes.Add(GroupPerjawatan);
                    }
                    else
                    {
                        SelectedNodes.Remove(GroupPerjawatan);
                    }
                    CreateTreeData();
                }
                else if (e.CommandName == "AddItem")
                {
                    //ChangeSeletedNodeStyle(selectedRow);
                    Session["PageMode"] = Helper.PageMode.New;
                    EditBox.Visible = true;
                    tbCode.Enabled = true;
                }
                else if (e.CommandName == "AddChild")
                {
                    ClearPageData();
                    ChangeSeletedNodeStyle(selectedRow);
                    AssignSelectedNode(GroupPerjawatan);
                    Session["PageMode"] = Helper.PageMode.New;
                    EditBox.Visible = true;
                    tbCode.Enabled = true;
                }
                else if (e.CommandName == "MakeRoot")
                {
                    ChangeSeletedNodeStyle(selectedRow);
                    AssignSelectedNode(GroupPerjawatan);
                    MakeRoot();
                    ClearPageData();
                }
                else if (e.CommandName == "CmdEdit")
                {
                    ChangeSeletedNodeStyle(selectedRow);
                    AssignSelectedNode(GroupPerjawatan);
                    CommandEdit();
                    Session["PageMode"] = Helper.PageMode.Edit;
                    EditBox.Visible = true;
                    tbCode.Enabled = false;
                }
                else if (e.CommandName == "CmdDelete")
                {
                    AssignSelectedNode(GroupPerjawatan);
                    GroupPerjawatan objGroupPerjawatan = (GroupPerjawatan)Session["SelectedGroupPerjawatan"];
                    objGroupPerjawatan.Status = "D";
                    objGroupPerjawatan.ModifiedBy = LoggedInUser.UserID;
                    objGroupPerjawatan.ModifiedTimeStamp = DateTime.Now;
                    if (new GroupPerjawatanBAL().UpdateGroupPerjawatan(objGroupPerjawatan))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Service Group updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Service Group");
                    ClearPageData();
                    Session["SelectedGroupPerjawatan"] = null;
                    GetData();
                    CreateTreeData();
                    //Session["PageMode"] = Helper.PageMode.Edit;
                }
                else if (e.CommandName == "CmdCut")
                {
                    ChangeSeletedNodeStyle(selectedRow);
                    AssignSelectedNode(GroupPerjawatan);
                }
                else if (e.CommandName == "CmdPaste")
                {
                    CommandPaste(GroupPerjawatan);
                    GetData();
                    CreateTreeData();
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void CommandPaste(string ParentGroupPerjawatanID)
        {
            try
            {
                GroupPerjawatan cutGroupPerjawatan = (GroupPerjawatan)Session["SelectedGroupPerjawatan"];

                List<GroupPerjawatan> data = (List<GroupPerjawatan>)Session["GroupPerjawatansData"];
                GroupPerjawatan parent = new GroupPerjawatan() { ParentGroupPerjawatanID = ParentGroupPerjawatanID };
                do
                {
                    parent = data.Where(x => x.GroupPerjawatanCode == parent.ParentGroupPerjawatanID).FirstOrDefault();
                    if (parent == null || parent.GroupPerjawatanCode == cutGroupPerjawatan.GroupPerjawatanCode)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "You can not paste a parent under its child");
                        return;
                    }
                } while (parent.ParentGroupPerjawatanID != string.Empty);

                cutGroupPerjawatan.ParentGroupPerjawatanID = ParentGroupPerjawatanID;
                new GroupPerjawatanBAL().UpdateGroupPerjawatan(cutGroupPerjawatan);
                SelectedNodes = (List<string>)Session["SelectedNodes"];
                if (!SelectedNodes.Contains(ParentGroupPerjawatanID))
                {
                    if (((List<GroupPerjawatanTreeHelper>)Session["GroupPerjawatansTree"]).Where(x => x.GroupPerjawatanCode == ParentGroupPerjawatanID).FirstOrDefault().ChildCount > 0)
                        SelectedNodes.Add(ParentGroupPerjawatanID);
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
                GroupPerjawatan cutGroupPerjawatan = (GroupPerjawatan)Session["SelectedGroupPerjawatan"];
                cutGroupPerjawatan.ParentGroupPerjawatanID = string.Empty;
                new GroupPerjawatanBAL().UpdateGroupPerjawatan(cutGroupPerjawatan);
                SelectedNodes = (List<string>)Session["SelectedNodes"];
                if (!SelectedNodes.Contains(cutGroupPerjawatan.GroupPerjawatanCode))
                {
                    if (((List<GroupPerjawatanTreeHelper>)Session["GroupPerjawatansTree"]).Where(x => x.GroupPerjawatanCode == cutGroupPerjawatan.GroupPerjawatanCode).FirstOrDefault().ChildCount > 0)
                        SelectedNodes.Add(cutGroupPerjawatan.GroupPerjawatanCode);
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
                GroupPerjawatan objGroupPerjawatan = (GroupPerjawatan)Session["SelectedGroupPerjawatan"];
                tbCode.Text = objGroupPerjawatan.GroupPerjawatanCode;
                tbDesc.Text = objGroupPerjawatan.GroupPerjawatanDesc;
                ddlStatus.SelectedIndex = -1;
                ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objGroupPerjawatan.Status))).Selected = true;
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

        protected void btnSample_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                new ReportHelper().DownloadSample<ServiceCodeImport>(ref Response);
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

                    if (Path.GetExtension(FileName) == ".csv")
                    {
                        DataSet ds = new ReportHelper().ExcelToDataSet(FullFileName, Path.GetExtension(FileName));
                        dt = new ReportHelper().Validate<ServiceCodeImport>(ds, "", ref lstErrors);

                        if (lstErrors.Count == 0)
                        {
                            try
                            {
                                List<GroupPerjawatan> data = (List<GroupPerjawatan>)Session["GroupPerjawatansData"];
                                if (data.Where(x => x.GroupPerjawatanCode == "").ToString().Count() > 0)
                                {
                                    data = null;
                                }
                                else { return; }

                                data = new ReportHelper().DataTableToList<ServiceCodeImport>(dt).Select(x => new GroupPerjawatan()
                                {
                                    GroupPerjawatanCode = Convert.ChangeType(x.ServiceCode, typeof(string)).ToString(),
                                    GroupPerjawatanDesc = x.ServiceDesc,
                                    Status = ((x.Status == "Active") ? "A" : "D"),
                                    ParentGroupPerjawatanID = ((Convert.ChangeType(x.UpperLevel, typeof(string)).ToString() == "0") ? "" :
                                        Convert.ChangeType(x.UpperLevel, typeof(string)).ToString())
                                }).ToList();

                                if (data.Count > 0)
                                {
                                    foreach (GroupPerjawatan item in data)
                                    {
                                        GroupPerjawatan objGroupPerjawatan = new GroupPerjawatan();

                                        objGroupPerjawatan.GroupPerjawatanCode = item.GroupPerjawatanCode;
                                        objGroupPerjawatan.GroupPerjawatanDesc = item.GroupPerjawatanDesc;
                                        objGroupPerjawatan.Status = item.Status;
                                        objGroupPerjawatan.ParentGroupPerjawatanID = item.ParentGroupPerjawatanID;

                                        if (new GroupPerjawatanBAL().GetGroupPerjawatans().Where(x => x.GroupPerjawatanCode.ToUpper().Trim() == tbCode.Text.ToUpper().Trim()).Count() > 0)
                                        {
                                            ((SiteMaster)this.Master).ShowMessage("Failure", "ServiceCode already exists");
                                            return;
                                        }
                                        objGroupPerjawatan.CreatedBy = LoggedInUser.UserID;
                                        objGroupPerjawatan.CreatedTimeStamp = DateTime.Now;
                                        objGroupPerjawatan.ModifiedBy = LoggedInUser.UserID;
                                        objGroupPerjawatan.ModifiedTimeStamp = DateTime.Now;

                                        if (new GroupPerjawatanBAL().InsertGroupPerjawatan(objGroupPerjawatan))
                                            ((SiteMaster)this.Master).ShowMessage("Success", "Service Group uploaded successfully");
                                        else
                                            ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while uploading Service Group");
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