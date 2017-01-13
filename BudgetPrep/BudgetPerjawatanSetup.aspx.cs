using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBBAL;
using OBDAL;
using BudgetPrep.Classes;

namespace BudgetPrep
{
    public partial class BudgetPerjawatanSetup : PageHelper
    {
        MasterUser AuthUser;
        bool IsPreparer;
        bool IsReviewer;
        bool IsApprover;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                AuthUser = (MasterUser)Session["UserData"];
                if (ViewState["IsPreparer"] == null)
                    ViewState["IsPreparer"] = AuthUser.JuncUserRoles.Where(x => x.Status == "A" && x.RoleID == 2).Count() > 0;
                if (ViewState["IsReviewer"] == null)
                    ViewState["IsReviewer"] = AuthUser.JuncUserRoles.Where(x => x.Status == "A" && x.RoleID == 3).Count() > 0;
                if (ViewState["IsApprover"] == null)
                    ViewState["IsApprover"] = AuthUser.JuncUserRoles.Where(x => x.Status == "A" && x.RoleID == 4).Count() > 0;
                IsPreparer = Convert.ToBoolean(ViewState["IsPreparer"]);
                IsReviewer = Convert.ToBoolean(ViewState["IsReviewer"]);
                IsApprover = Convert.ToBoolean(ViewState["IsApprover"]);

                string innerstr = (IsPreparer) ? "<span><i class=\"fa fa-upload txt-success\"></i></span>Submit"
                        : (IsReviewer ? "<span><i class=\"fa fa-upload txt-success\"></i></span>Review" : "<span><i class=\"fa fa-upload txt-success\"></i></span>Approve");
                btnSubmit.InnerHtml = innerstr;

                ddlBulkDecision.SelectedIndex = -1;
                ddlBulkDecision.SelectedIndex = 0;
                ddlBulkDecision.Enabled = !IsPreparer;

                if (!Page.IsPostBack)
                {
                    Session["SelectedNodes"] = null;
                    ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Budget";
                    ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Perjawatan";

                    Page.Form.Attributes.Add("enctype", "multipart/form-data");
                }
                else
                {
                    //GroupPerjawatansData = (Session["GroupPerjawatansData"] != null) ? (List<GroupPerjawatan>)Session["GroupPerjawatansData"] : new List<GroupPerjawatan>();
                    //PeriodData = (Session["PeriodData"] != null) ? (List<PeriodPerjawatan>)Session["PeriodData"] : new List<PeriodPerjawatan>();
                    //BudgetData = (Session["BudgetData"] != null) ? (List<BudgetPerjawatan>)Session["BudgetData"] : new List<BudgetPerjawatan>();
                    //SelectedNodes = (Session["SelectedNodes"] != null) ? (List<string>)Session["SelectedNodes"] : new List<string>();
                    //TreeData = (Session["GroupPerjawatansTree"] != null) ? (List<GroupPerjawatanTreeHelper>)Session["GroupPerjawatansTree"] : new List<GroupPerjawatanTreeHelper>();
                }
                if (BudgetBox.Visible)
                {
                    BuildGrid();
                    BindGrid();
                }
                if (!Page.IsPostBack)
                {
                    //BuildGrid();
                    //BindGrid();
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
                List<GroupPerjawatan> lstGroupPerjawatan = new GroupPerjawatanBAL().GetGroupPerjawatans().Where(x => x.Status == "A").ToList();
                List<GroupPerjawatan> GroupPerjawatansData = AuthUser.UserPerjawatanWorkflows.Where(x => x.Status == "A").Select(x => x.GroupPerjawatan).ToList();
                List<string> lstprntcodes = GroupPerjawatansData.Select(x => x.ParentGroupPerjawatanID).Distinct().ToList();
                while (lstprntcodes.Count > 0)
                {
                    List<GroupPerjawatan> lstprnts = lstGroupPerjawatan.Where(x => lstprntcodes.Contains(x.GroupPerjawatanCode)).ToList();
                    foreach (GroupPerjawatan o in lstprnts)
                        if (GroupPerjawatansData.Where(x => x.GroupPerjawatanCode == o.GroupPerjawatanCode).Count() == 0)
                            GroupPerjawatansData.Add(o);
                    lstprntcodes = lstprnts.Select(x => x.ParentGroupPerjawatanID).Distinct().ToList();
                }
                Session["GroupPerjawatansData"] = GroupPerjawatansData.OrderBy(x => x.GroupPerjawatanCode).ToList();

                List<int> lstperiod = GetSelectedPeriods();
                List<PeriodPerjawatan> PeriodData = new PeriodPerjawatanBAL().GetPeriodPerjawatans().Where(x => x.Status == "A" && lstperiod.Contains(x.PeriodPerjawatanID))
                    .OrderBy(x => x.PerjawatanYear).ThenBy(x => x.FieldPerjawatan.FieldPerjawatanDesc).ToList();

                List<PeriodPerjawatan> FixedData = ((List<FieldPerjawatan>)Session["FixedFieldPerjawatan"]).Where(x => lstperiod.Contains(x.FieldPerjawatanID))
                   .Select(x => new PeriodPerjawatan
                   {
                       //assign FieldPerjawatanID to PeriodPerjawatanID for use after. FieldPerjawatanID in PeriodPerjawatan will be 0 to indicated its fixed
                       PeriodPerjawatanID = lstperiod.Contains(x.FieldPerjawatanID) ? x.FieldPerjawatanID : 0,
                       PerjawatanYear = DateTime.Now.Year,
                       FieldPerjawatan = new FieldPerjawatan
                       {
                           FieldPerjawatanID = x.FieldPerjawatanID,
                           FieldPerjawatanDesc = x.FieldPerjawatanDesc,
                           Status = x.Status
                       }
                   }).OrderBy(x => x.PerjawatanYear).ThenBy(x => x.FieldPerjawatan.FieldPerjawatanDesc).ToList();

                FixedData.AddRange(PeriodData);
                Session["PeriodData"] = FixedData;

                bool CanEdit = false;
                List<int> LstSegmentDetailIDs = GetSegDetals().Select(x => x.SegmentDetailID).ToList();
                List<BudgetPerjawatan> BudgetData = new BudgetPerjawatanBAL().GetBudgetPerjawatansWithTreeCalc(LstSegmentDetailIDs, ref CanEdit)
                    //List<BudgetPerjawatan> BudgetData = new BudgetPerjawatanBAL().GetBudgetPerjawatans(LstSegmentDetailIDs)
                    .Select(x => new BudgetPerjawatan
                    {
                        BudgetPerjawatanID = x.BudgetPerjawatanID,
                        GroupPerjawatanCode = x.GroupPerjawatanCode,
                        PeriodPerjawatanID = x.PeriodPerjawatanID,
                        Status = x.Status,
                        Remarks = x.Remarks,
                        Amount = (GroupPerjawatansData.Where(y => y.ParentGroupPerjawatanID == x.GroupPerjawatanCode).Count() == 0) ? x.Amount : 0
                    }).ToList();
                Session["BudgetData"] = BudgetData;
                Session["CanEdit"] = CanEdit;

                List<int> BlockedYears = new BudgetPerjawatanBAL().GetBlockedPerjawatanYears();
                Session["BlockedYears"] = BlockedYears;

                List<int> OpenYears = new BudgetPerjawatanBAL().GetOpenPerjawatanYears();
                Session["OpenYears"] = OpenYears;
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
                List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];
                List<GroupPerjawatan> GroupPerjawatansData = (List<GroupPerjawatan>)Session["GroupPerjawatansData"];
                List<GroupPerjawatanTreeHelper> TreeData = new List<GroupPerjawatanTreeHelper>();
                if (GroupPerjawatansData.Count > 0)
                {
                    TreeData = GroupPerjawatansData.Where(x => x.ParentGroupPerjawatanID == string.Empty && x.Status == "A").OrderBy(x => x.GroupPerjawatanCode).Select(x =>
                            new GroupPerjawatanTreeHelper()
                            {
                                GroupPerjawatanCode = x.GroupPerjawatanCode,
                                GroupPerjawatanDesc = x.GroupPerjawatanDesc,
                                ParentGroupPerjawatanID = x.ParentGroupPerjawatanID,
                                Status = x.Status,
                                Level = 0,
                                ChildCount = GroupPerjawatansData.Where(y => y.ParentGroupPerjawatanID == x.GroupPerjawatanCode).Count()
                            }).ToList();

                    if (SelectedNodes == null || SelectedNodes.Count == 0)
                    {
                        Session["SelectedNodes"] = new List<string>();
                        SelectedNodes = new List<string>();
                    }
                    else
                    {
                        for (int i = 0; i < TreeData.Count; i++)
                        {
                            if (SelectedNodes.Contains(TreeData[i].GroupPerjawatanCode))
                            {
                                foreach (GroupPerjawatan sd in GroupPerjawatansData.Where(x => x.ParentGroupPerjawatanID == TreeData[i].GroupPerjawatanCode && x.Status == "A").OrderByDescending(x => x.GroupPerjawatanCode))
                                {
                                    GroupPerjawatanTreeHelper objSH = new GroupPerjawatanTreeHelper()
                                    {
                                        GroupPerjawatanCode = sd.GroupPerjawatanCode,
                                        GroupPerjawatanDesc = sd.GroupPerjawatanDesc,
                                        ParentGroupPerjawatanID = sd.ParentGroupPerjawatanID,
                                        Status = sd.Status,
                                        Level = TreeData[i].Level + 1,
                                        ChildCount = GroupPerjawatansData.Where(y => y.ParentGroupPerjawatanID == sd.GroupPerjawatanCode).Count()
                                    };
                                    TreeData.Insert(i + 1, objSH);
                                }
                            }
                        }
                    }
                }
                Session["GroupPerjawatansTree"] = TreeData;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private void BuildGrid()
        {
            try
            {
                //for (int i = gvGroupPerjawatans.Columns.Count - 1; i > 0; i--)
                //{
                //    gvGroupPerjawatans.Columns.RemoveAt(i);
                //}
                gvGroupPerjawatans.Columns.Clear();
                List<PeriodPerjawatan> PeriodData = (List<PeriodPerjawatan>)Session["PeriodData"];
                List<BudgetPerjawatan> BudgetData = (List<BudgetPerjawatan>)Session["BudgetData"];
                List<GroupPerjawatan> GroupPerjawatansData = (List<GroupPerjawatan>)Session["GroupPerjawatansData"];
                List<string> parentcodes = GroupPerjawatansData.Select(x => x.ParentGroupPerjawatanID).Distinct().ToList();
                List<string> ChildCodes = GroupPerjawatansData.Where(x => !parentcodes.Contains(x.GroupPerjawatanCode)).Select(x => x.GroupPerjawatanCode).ToList();

                TemplateField templateField1 = new TemplateField();
                templateField1.HeaderText = "Object";
                //templateField1.HeaderStyle.CssClass = "treecontainer";
                templateField1.ItemTemplate = new GridViewCustomTemplate(0, "", 0);
                templateField1.FooterText = "Total";
                gvGroupPerjawatans.Columns.Add(templateField1);

                BoundField bf1 = new BoundField();
                bf1.HeaderText = "Description";
                bf1.DataField = "GroupPerjawatanDesc";
                gvGroupPerjawatans.Columns.Add(bf1);

                foreach (PeriodPerjawatan pm in PeriodData)
                {
                    TemplateField templateField = new TemplateField();
                    templateField.HeaderText = pm.PerjawatanYear.ToString() + " " + pm.FieldPerjawatan.FieldPerjawatanDesc;
                    GridViewCustomTemplate objTemp = new GridViewCustomTemplate(1, pm.PerjawatanYear.ToString() + " " + pm.FieldPerjawatan.FieldPerjawatanDesc, pm.PeriodPerjawatanID);
                    objTemp.OnCustomTextChanged += new CustomTextChangedEventHandler(objTemp_OnCustomTextChanged);
                    objTemp.OnCustomClicked += new CustomClickedEventHandler(objTemp_OnCustomClicked);
                    templateField.ItemTemplate = objTemp;
                    templateField.FooterText = BudgetData.Where(x => x.PeriodPerjawatanID == pm.PeriodPerjawatanID && ChildCodes.Contains(x.GroupPerjawatanCode)).Select(x => x.Amount).Sum().ToString("F");
                    gvGroupPerjawatans.Columns.Add(templateField);
                }
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

                gvGroupPerjawatans.DataSource = (List<GroupPerjawatanTreeHelper>)Session["GroupPerjawatansTree"];
                gvGroupPerjawatans.DataBind();
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
                //gvGroupPerjawatans.Visible = false;
                //btnPrint.Visible = false;
                //btnSubmit.Visible = false;
                //btnSearchbox.Visible = false;
                BudgetBox.Visible = false;

                foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
                {
                    TreeView tv = (TreeView)gvr.Cells[0].FindControl("tvSegmentDDL");
                    TextBox tb = (TextBox)gvr.Cells[0].FindControl("tbSegmentDDL");
                    if (tb != null)
                    {
                        if (tv.SelectedNode != null)
                            tv.SelectedNode.Selected = false;
                        tb.Text = string.Empty;
                    }
                }

                for (int i = 0; i < gvPeriod.Rows.Count; i++)
                {
                    string stryear = gvPeriod.Rows[i].Cells[1].Text;
                    ((CheckBox)gvPeriod.Rows[i].Cells[0].FindControl("cbPeriodSelect")).Checked = (stryear == (DateTime.Now.Year + 1).ToString());

                    int PeriodPerjawatanID = Convert.ToInt32(gvPeriod.DataKeys[i]["PeriodPerjawatanID"].ToString());
                    List<FieldPerjawatan> obj = ((List<FieldPerjawatan>)Session["FixedFieldPerjawatan"]).Where(x => x.FieldPerjawatanID == PeriodPerjawatanID).ToList();

                    if (obj.Count() > 0)
                    {
                        ((CheckBox)gvPeriod.Rows[i].Cells[0].FindControl("cbPeriodSelect")).Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //gvGroupPerjawatans.Visible = true;
            //btnPrint.Visible = true;
            //btnSubmit.Visible = true;
            //btnSearchbox.Visible = true;
            BudgetBox.Visible = true;

            //SetBudgetCanEditable();
            GetPrefixAcountCode();
            GetData();

            //bool IsBudgetEditable = Convert.ToBoolean(Session["CanEdit"]);
            //if (!IsBudgetEditable)
            //{
            //    chkKeterangan.Checked = false;
            //    chkPengiraan.Checked = false;
            //}

            CreateTreeData();
            BuildGrid();
            BindGrid();

            EditBox.Visible = false;
        }

        protected void btnSearchbox_Click(object sender, EventArgs e)
        {
            List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];

            if (SelectedNodes.Count == 0)
            {
                btnFileUpload.Disabled = true;
            }
            else
            {
                btnFileUpload.Disabled = false;
            }

            EditBox.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPageData();

            EditBox.Visible = true;
        }

        protected void gvGroupPerjawatans_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];
                    List<BudgetPerjawatan> BudgetData = (List<BudgetPerjawatan>)Session["BudgetData"];
                    List<GroupPerjawatanTreeHelper> TreeData = (List<GroupPerjawatanTreeHelper>)Session["GroupPerjawatansTree"];
                    List<GroupPerjawatan> GroupPerjawatansData = (List<GroupPerjawatan>)Session["GroupPerjawatansData"];
                    List<PeriodPerjawatan> PeriodData = (List<PeriodPerjawatan>)Session["PeriodData"];
                    List<int> BlockedYears = (List<int>)Session["BlockedYears"];
                    List<int> OpenYears = (List<int>)Session["OpenYears"];

                    GroupPerjawatanTreeHelper rowItem = (GroupPerjawatanTreeHelper)e.Row.DataItem;

                    /*Start Account Code logics*/
                    int width = rowItem.Level * (new Helper().IndentPixels);
                    string strHTML = string.Empty;
                    if (rowItem.ChildCount > 0)
                    {
                        if (SelectedNodes.Contains(rowItem.GroupPerjawatanCode))
                            strHTML = "<label style=\"display: inline-block;width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i class=\"fa fa-minus-square pull-right\"></i></label> ";
                        else
                            strHTML = "<label style=\"display: inline-block;width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i class=\"fa fa-plus-square pull-right\"></i></label> ";
                    }
                    else
                        strHTML = "<label style=\"display: inline-block;width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i></i></label> ";

                    LinkButton btnExpand = ((LinkButton)e.Row.Cells[0].FindControl("btnExpand"));
                    //btnExpand.Text = "<div style=\"max-width:200px;overflow:hidden;white-space:nowrap;\">" + strHTML + Session["PrefixAcountCode"].ToString() + rowItem.GroupPerjawatan + "</div>";
                    btnExpand.Text = "<div style=\"max-width:200px;overflow:hidden;white-space:nowrap;\">" + strHTML + rowItem.GroupPerjawatanCode + "</div>";
                    btnExpand.ToolTip = rowItem.GroupPerjawatanDesc;

                    if (Session["SelectedGroupPerjawatan"] != null && ((GroupPerjawatan)Session["SelectedGroupPerjawatan"]).GroupPerjawatanCode == rowItem.GroupPerjawatanCode)
                    {
                        e.Row.Style["background-color"] = "gold";
                    }
                    /*End Account Code logics*/

                    /*Start Buget logics*/
                    int index = 2;
                    //if (chkKeterangan.Checked)
                    //{
                    //    if (rowItem.ChildCount > 0)
                    //        ((TextBox)e.Row.Cells[index].Controls[0]).Visible = false;
                    //    index++;
                    //}
                    //if (chkPengiraan.Checked)
                    //{
                    //    if (rowItem.ChildCount > 0)
                    //        ((TextBox)e.Row.Cells[index].Controls[0]).Visible = false;
                    //    index++;
                    //}

                    bool IsBudgetEditable = Convert.ToBoolean(Session["CanEdit"]);
                    for (int c = index; c < gvGroupPerjawatans.Columns.Count; c++)
                    {
                        //int PeriodPerjawatanID = Convert.ToInt32(((Label)e.Row.Cells[c].FindControl("lbl_PeriodPerjawatanID")).Text);
                        int PeriodPerjawatanID = Convert.ToInt32(((Label)e.Row.Cells[c].Controls[0]).Text);
                        PeriodPerjawatan pm = PeriodData.Where(x => x.PeriodPerjawatanID == PeriodPerjawatanID).FirstOrDefault();
                        BudgetPerjawatan ObjBudgetPerjawatan = BudgetData.Where(x => x.GroupPerjawatanCode == rowItem.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID).FirstOrDefault();
                        e.Row.Cells[c].BackColor = (ObjBudgetPerjawatan != null) ? new Helper().GetColorByStatusValue(Convert.ToChar(ObjBudgetPerjawatan.Status)) : System.Drawing.Color.White;

                        if (rowItem.ChildCount == 0 && IsBudgetEditable)
                        {
                            ((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Text = (ObjBudgetPerjawatan != null) ? ObjBudgetPerjawatan.Amount.ToString() : Convert.ToDecimal(0).ToString("F");
                            ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Text = (ObjBudgetPerjawatan != null) ? ObjBudgetPerjawatan.Amount.ToString() : Convert.ToDecimal(0).ToString("F");
                            ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Text = Server.HtmlDecode("&#9635;");

                            if (IsPreparer)
                            {
                                if (ObjBudgetPerjawatan == null || ObjBudgetPerjawatan.Status == "O" || ObjBudgetPerjawatan.Status == "S" || ObjBudgetPerjawatan.Status == "X" || ObjBudgetPerjawatan.Status == "Y")
                                {
                                    ((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Visible = ((pm.PerjawatanYear > DateTime.Now.Year) && !BlockedYears.Contains(pm.PerjawatanYear) && OpenYears.Contains(pm.PerjawatanYear));
                                    ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Visible = !((pm.PerjawatanYear > DateTime.Now.Year) && !BlockedYears.Contains(pm.PerjawatanYear) && OpenYears.Contains(pm.PerjawatanYear));
                                    ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = false;
                                }
                                else if (ObjBudgetPerjawatan.Status == "P" || ObjBudgetPerjawatan.Status == "R" || ObjBudgetPerjawatan.Status == "A")
                                {
                                    ((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Visible = false;
                                    ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Visible = true;
                                    ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = false;
                                }
                            }
                            if (IsReviewer)
                            {
                                if (ObjBudgetPerjawatan == null || ObjBudgetPerjawatan.Status == "O" || ObjBudgetPerjawatan.Status == "S" || ObjBudgetPerjawatan.Status == "P" || ObjBudgetPerjawatan.Status == "X" || ObjBudgetPerjawatan.Status == "Y")
                                {
                                    ((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Visible = ((pm.PerjawatanYear > DateTime.Now.Year) && !BlockedYears.Contains(pm.PerjawatanYear) && OpenYears.Contains(pm.PerjawatanYear));
                                    ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Visible = !((pm.PerjawatanYear > DateTime.Now.Year) && !BlockedYears.Contains(pm.PerjawatanYear) && OpenYears.Contains(pm.PerjawatanYear));
                                    if (ObjBudgetPerjawatan != null && ObjBudgetPerjawatan.Status == "P")
                                        ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = true;
                                    else
                                        ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = false;
                                    //((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = true;
                                }
                                else if (ObjBudgetPerjawatan.Status == "R" || ObjBudgetPerjawatan.Status == "A")
                                {
                                    ((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Visible = false;
                                    ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Visible = true;
                                    ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = false;
                                }
                            }
                            if (IsApprover)
                            {
                                ((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Visible = false;
                                ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Visible = true;
                                if (ObjBudgetPerjawatan != null && ObjBudgetPerjawatan.Status == "R")
                                    ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = true;
                                else
                                    ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = false;
                            }
                            //((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Text = (ObjBudgetPerjawatan != null) ? ObjBudgetPerjawatan.Amount.ToString() : (0.00).ToString();
                            //((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Visible = true;
                            //((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Visible = false;

                            //((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Text = Server.HtmlDecode("&#9635;");
                            //((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = true;
                        }
                        else
                        {
                            int cnt = 0;
                            decimal amount = 0;

                            List<string> ChildIDs = new List<string>() { rowItem.GroupPerjawatanCode };
                            List<string> RefChildIDs = new List<string>();
                            while (ChildIDs.Count > 0)
                            {
                                RefChildIDs.Clear();
                                foreach (GroupPerjawatan t in GroupPerjawatansData.Where(x => ChildIDs.Contains(x.GroupPerjawatanCode)))  //&& x.ChildCount == 0
                                {
                                    amount = amount + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID).Select(x => x.Amount).Sum();
                                    foreach (string s in GroupPerjawatansData.Where(x => x.ParentGroupPerjawatanID == t.GroupPerjawatanCode).Select(x => x.GroupPerjawatanCode).ToList())
                                        RefChildIDs.Add(s);
                                    if (IsPreparer)
                                    {
                                        cnt = cnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && (x.Status == "S" || x.Status == "X" || x.Status == "Y"))
                                            .Select(x => x.BudgetPerjawatanID).Count();
                                    }
                                    if (IsReviewer)
                                    {
                                        cnt = cnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && (x.Status == "P" || x.Status == "X" || x.Status == "Y"))
                                            .Select(x => x.BudgetPerjawatanID).Count();
                                    }
                                    if (IsApprover)
                                    {
                                        cnt = cnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "R")
                                            .Select(x => x.BudgetPerjawatanID).Count();
                                    }
                                }
                                ChildIDs.Clear();
                                foreach (string s in RefChildIDs)
                                    ChildIDs.Add(s);
                                //ChildIDs = TreeData.Where(x => ChildIDs.Contains(x.ParentGroupPerjawatan) && x.ChildCount > 0).Select(x => x.GroupPerjawatan).ToList();
                            }

                            string strBadge = (IsPreparer) ? string.Empty : ((cnt == 0) ? string.Empty : " <span class=\"badge\">" + cnt.ToString() + "</span>");
                            ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Text = amount.ToString("F") + strBadge;
                            ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Visible = true;
                            ((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Visible = false;
                            ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = false;
                        }
                    }
                    /*End Buget logics*/
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
                //GridViewRow selectedRow = gvGroupPerjawatans.Rows[Convert.ToInt32(e.CommandArgument)];
                if (e.CommandName == "Expand")
                {
                    GridViewRow selectedRow = (GridViewRow)((LinkButton)e.CommandSource).Parent.Parent;
                    string GroupPerjawatanCode = gvGroupPerjawatans.DataKeys[selectedRow.RowIndex]["GroupPerjawatanCode"].ToString();
                    List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];
                    if (!SelectedNodes.Contains(GroupPerjawatanCode))
                    {
                        if (TreeData.Where(x => x.GroupPerjawatanCode == GroupPerjawatanCode).FirstOrDefault().ChildCount > 0)
                            SelectedNodes.Add(GroupPerjawatanCode);
                    }
                    else
                    {
                        SelectedNodes.Remove(GroupPerjawatanCode);
                    }

                    if (SelectedNodes.Count == 0)
                    {
                        btnFileUpload.Disabled = true;
                    }
                    else
                    {
                        btnFileUpload.Disabled = false;
                    }

                    CreateTreeData();
                    BuildGrid();
                    BindGrid();
                }
                else if (e.CommandName == "Page")
                {
                    gvGroupPerjawatans.PageIndex = Convert.ToInt32(e.CommandArgument) - 1;
                    BuildGrid();
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void gvGroupPerjawatans_RowCreated(object sender, GridViewRowEventArgs e) { }

        protected void objTemp_OnCustomTextChanged(object sender, CustomEvenArgs e)
        {
            try
            {
                string strGroupPerjawatanCode = e.Code;
                int PeriodPerjawatanID = e.PeriodID;
                decimal amount = e.Amount;
                string strstatus = string.Empty;

                List<BudgetPerjawatan> BudgetData = (List<BudgetPerjawatan>)Session["BudgetData"];

                //string strGroupPerjawatan = ((GridView)((TextBox)sender).Parent.Parent.Parent.Parent).DataKeys[((GridViewRow)((TextBox)sender).Parent.Parent).RowIndex][0].ToString();
                ////int PeriodPerjawatanID = Convert.ToInt32(((Label)(((TextBox)sender).Parent).FindControl("lbl_PeriodPerjawatanID")).Text);
                //int PeriodPerjawatanID = Convert.ToInt32(((Label)(((TextBox)sender).Parent).Controls[0]).Text);
                BudgetPerjawatan item = BudgetData.Where(x => x.GroupPerjawatanCode == strGroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID).FirstOrDefault();

                if (item != null)
                {
                    strstatus = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                        : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);

                    item.Amount = e.Amount;
                    item.Status = strstatus;

                    BudgetPerjawatan newBudgetPerjawatan = new BudgetPerjawatan();
                    newBudgetPerjawatan.GroupPerjawatanCode = strGroupPerjawatanCode;
                    newBudgetPerjawatan.PeriodPerjawatanID = PeriodPerjawatanID;
                    newBudgetPerjawatan.Amount = amount;
                    newBudgetPerjawatan.Status = strstatus;
                    newBudgetPerjawatan.ModifiedBy = AuthUser.UserID;
                    newBudgetPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    List<int> LstSegmentDetailIDs = ((List<JuncBgtPerjawatanSegDtl>)Session["ListSegmentDetails"]).Select(x => x.SegmentDetailID).ToList();
                    if (new BudgetPerjawatanBAL().UpdateBudgetPerjawatans(newBudgetPerjawatan, LstSegmentDetailIDs))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Budget");
                }
                else
                {
                    BudgetPerjawatan newBudgetPerjawatan = new BudgetPerjawatan();
                    newBudgetPerjawatan.GroupPerjawatanCode = strGroupPerjawatanCode;
                    newBudgetPerjawatan.PeriodPerjawatanID = PeriodPerjawatanID;
                    newBudgetPerjawatan.Amount = e.Amount;
                    newBudgetPerjawatan.Status = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                        : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);
                    newBudgetPerjawatan.CreatedBy = AuthUser.UserID;
                    newBudgetPerjawatan.CreatedTimeStamp = DateTime.Now;
                    newBudgetPerjawatan.ModifiedBy = AuthUser.UserID;
                    newBudgetPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    BudgetData.Add(newBudgetPerjawatan);

                    List<JuncBgtPerjawatanSegDtl> lstBgtSegDtl = (List<JuncBgtPerjawatanSegDtl>)Session["ListSegmentDetails"];
                    foreach (JuncBgtPerjawatanSegDtl obj in lstBgtSegDtl)
                        obj.BudgetPerjawatan = newBudgetPerjawatan;

                    if (new BudgetPerjawatanBAL().InsertBudgetPerjawatans(newBudgetPerjawatan, lstBgtSegDtl))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget saved successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Budget");
                }

                CreateTreeData();
                BuildGrid();
                BindGrid();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void objTemp_OnCustomClicked(object sender, CustomEvenArgs e)
        {
            try
            {
                lblDecisionModalAccount.Text = "Account Code : " + Session["PrefixAcountCode"].ToString() + e.Code;
                lblDecisionModalGroupPerjawatanCode.Text = e.Code;
                lblDecisionModalPeriodID.Text = e.PeriodID.ToString();
                lblDecisionModalPeriod.Text = "Period : " + e.Period;
                lblDecisionModalAmount.Text = "Amount : " + e.Amount.ToString();

                rbldecision.SelectedIndex = 0;
                tbRemarks.Text = string.Empty;

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myDecisionModal", "$('#myDecisionModal').modal();", true);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        //protected void objTemp_OnCustomKetPenTextChanged(object sender, CustomKetPenEvenArgs e)
        //{
        //    GroupPerjawatan objGroupPerjawatan = new GroupPerjawatanBAL().GetGroupPerjawatans().Where(x => x.GroupPerjawatan1.ToUpper().Trim() == e.GroupPerjawatan.ToUpper().Trim()).FirstOrDefault();

        //    if (e.KetorPen == "Keterangan")
        //        objGroupPerjawatan.Keterangan = ((TextBox)sender).Text;
        //    else if (e.KetorPen == "Pengiraan")
        //        objGroupPerjawatan.Pengiraan = ((TextBox)sender).Text;

        //    if (new GroupPerjawatanBAL().UpdateGroupPerjawatan(objGroupPerjawatan))
        //        ((SiteMaster)this.Master).ShowMessage("Success", e.KetorPen + " updated successfully");
        //    else
        //        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating " + e.KetorPen);
        //}

        private List<JuncBgtPerjawatanSegDtl> GetSegDetals()
        {
            List<JuncBgtPerjawatanSegDtl> lst = new List<JuncBgtPerjawatanSegDtl>();
            try
            {
                foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
                {
                    TreeView tv = (TreeView)gvr.Cells[0].FindControl("tvSegmentDDL");
                    if (tv != null)
                    {
                        JuncBgtPerjawatanSegDtl obj = new JuncBgtPerjawatanSegDtl();
                        try
                        {
                            obj.SegmentDetailID = Convert.ToInt32(tv.SelectedValue);
                        }
                        catch (Exception exinner)
                        {
                            return (List<JuncBgtPerjawatanSegDtl>)Session["ListSegmentDetails"];
                        }
                        //obj.BudgetPerjawatan = BudgetPerjawatan;
                        lst.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
            Session["ListSegmentDetails"] = lst;
            return lst;
        }

        private void SetBudgetCanEditable()
        {
            //List<int> segdtlids = new List<int>();
            //try
            //{
            //    foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
            //    {
            //        TreeView tv = (TreeView)gvr.Cells[0].FindControl("tvSegmentDDL");
            //        if (tv != null)
            //        {
            //            segdtlids.Add(Convert.ToInt32(tv.SelectedValue));
            //        }
            //    }
            //    ViewState["IsBudgetEditable"] = new SegmentDetailsBAL().IsBudgetEditable(segdtlids);
            //}
            //catch (Exception ex)
            //{
            //    ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            //}
        }

        private void GetPrefixAcountCode()
        {
            try
            {
                string PrefixAcountCode = string.Empty;
                List<string> PrefixDesc = new List<string>();

                foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
                {
                    TreeView tv = (TreeView)gvr.Cells[0].FindControl("tvSegmentDDL");
                    if (tv != null)
                    {
                        PrefixAcountCode = PrefixAcountCode + tv.SelectedNode.Text.Split('-')[0].Trim() + '-';
                        PrefixDesc.Add(tv.SelectedNode.Text);
                    }
                }
                Session["PrefixAcountCode"] = PrefixAcountCode;
                Session["PrefixDesc"] = PrefixDesc;
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
                List<Segment> lstSegment = new SegmentBAL().GetSegments().Where(x => x.Status == "A").OrderBy(x => x.SegmentOrder).ToList();
                //Session["Segments"] = lstSegment;
                gvSegmentDLLs.DataSource = lstSegment;
                gvSegmentDLLs.DataBind();

                //GridView: Period
                List<PeriodPerjawatan> lstPeriodPerjawatan = new PeriodPerjawatanBAL().GetPeriodPerjawatans().Where(x => x.Status == "A" && x.PerjawatanYear > DateTime.Now.Year)
                    .OrderBy(x => x.PerjawatanYear).ThenBy(x => x.FieldPerjawatan.FieldPerjawatanDesc).ToList();

                List<FieldPerjawatan> obj = new FieldPerjawatanBAL().GetFieldPerjawatans().Where(x => x.Status == "F").ToList();
                Session["FixedFieldPerjawatan"] = obj;

                foreach (FieldPerjawatan item in obj)
                {
                    PeriodPerjawatan pm = new PeriodPerjawatan();

                    pm.PeriodPerjawatanID = item.FieldPerjawatanID;
                    pm.PerjawatanYear = DateTime.Now.Year;
                    pm.FieldPerjawatan = new FieldPerjawatan { FieldPerjawatanDesc = item.FieldPerjawatanDesc };

                    lstPeriodPerjawatan.Add(pm);
                }

                gvPeriod.DataSource = lstPeriodPerjawatan.Select(x => new
                {
                    x.PeriodPerjawatanID,
                    x.PerjawatanYear,
                    x.FieldPerjawatan.FieldPerjawatanDesc
                });
                gvPeriod.DataBind();
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void gvSegmentDLLs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Segment rowItem = (Segment)e.Row.DataItem;
                    //List<SegmentDetail> lstSD = rowItem.SegmentDetails.Where(x => x.Status == "A" && x.ParentDetailID == 0).ToList();
                    List<SegmentDetail> lst = new SegmentDetailsDAL().GetSegmentDetails().ToList();
                    List<SegmentDetail> lstSD = new List<SegmentDetail>();

                    lstSD = AuthUser.UserSegDtlWorkflows.Where(x => x.Status == "A" && x.SegmentDetail.SegmentID == rowItem.SegmentID).Select(x => x.SegmentDetail).ToList();
                    List<int> parntids = lstSD.Select(x => Convert.ToInt32(x.ParentDetailID)).Distinct().ToList();

                    while (parntids.Count > 0)
                    {
                        List<SegmentDetail> lstprnts = lst.Where(x => parntids.Contains(x.SegmentDetailID)).ToList();
                        foreach (SegmentDetail o in lstprnts)
                            lstSD.Add(o);
                        parntids = lstprnts.Select(x => Convert.ToInt32(x.ParentDetailID)).Distinct().ToList();
                    }

                    lstSD = lstSD.OrderBy(x => x.ParentDetailID).ThenBy(x => x.DetailCode).ToList();

                    List<TreeNode> lstTN = new List<TreeNode>();
                    TreeNode tn = new TreeNode();
                    //CreateNode(rowItem.SegmentDetails.ToList(), ref tn, 0);
                    CreateNode(lstSD, ref tn, 0);

                    TreeView tvSegmentDDL = ((TreeView)e.Row.Cells[1].FindControl("tvSegmentDDL"));
                    for (int i = tn.ChildNodes.Count - 1; i >= 0; i--)
                    {
                        tvSegmentDDL.Nodes.Add(tn.ChildNodes[0]);
                    }
                    //tvSegmentDDL.Nodes.AddAt(0, new TreeNode("[Please Select]", "0"));
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void gvPeriod_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string stryear = e.Row.Cells[1].Text;
                    ((CheckBox)e.Row.Cells[0].FindControl("cbPeriodSelect")).Checked = (stryear == (DateTime.Now.Year + 1).ToString());

                    int PeriodPerjawatanID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "PeriodPerjawatanID"));
                    List<FieldPerjawatan> obj = ((List<FieldPerjawatan>)Session["FixedFieldPerjawatan"]).Where(x => x.FieldPerjawatanID == PeriodPerjawatanID).ToList();

                    if (obj.Count() > 0)
                    {
                        ((CheckBox)e.Row.Cells[0].FindControl("cbPeriodSelect")).Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private List<int> GetSelectedPeriods()
        {
            List<int> lst = new List<int>();
            try
            {
                foreach (GridViewRow gvr in gvPeriod.Rows)
                {
                    if (((CheckBox)gvr.Cells[0].FindControl("cbPeriodSelect")).Checked)
                        lst.Add(Convert.ToInt32(gvPeriod.DataKeys[gvr.RowIndex]["PeriodPerjawatanID"].ToString()));
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
            return lst;
        }

        private void CreateNode(List<SegmentDetail> ListSegDtls, ref TreeNode Node, int ParentID)
        {
            try
            {
                //List<TreeNode> TreeNodes = new List<TreeNode>();
                foreach (SegmentDetail sd in ListSegDtls.Where(x => x.Status == "A" && x.ParentDetailID == ParentID))
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = sd.DetailCode + " - " + sd.DetailDesc;
                    tn.Value = sd.SegmentDetailID.ToString();
                    tn.SelectAction = TreeNodeSelectAction.Select;
                    CreateNode(ListSegDtls.ToList(), ref tn, Convert.ToInt32(sd.SegmentDetailID));
                    Node.ChildNodes.Add(tn);
                }
                //return TreeNodes;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void tvSegmentDDL_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            {
                ((TextBox)((GridViewRow)((TreeView)sender).NamingContainer).FindControl("tbSegmentDDL")).Text = ((TreeView)sender).SelectedNode.Text;

                bool CanEdit = true;
                foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
                {
                    TreeView tv = (TreeView)gvr.Cells[0].FindControl("tvSegmentDDL");

                    CanEdit = (tv != null && tv.SelectedNode != null && tv.SelectedNode.ChildNodes.Count == 0 && CanEdit);
                }

                //chkKeterangan.Checked = (CanEdit) ? chkKeterangan.Checked : false;
                //chkPengiraan.Checked = (CanEdit) ? chkPengiraan.Checked : false;
                //chkKeterangan.Enabled = CanEdit;
                //chkPengiraan.Enabled = CanEdit;
                btnSubmit.Visible = CanEdit;

                bool uploadbtn = (IsPreparer) && CanEdit ? true : false;
                btnFileUpload.Visible = uploadbtn;
                btnFileUpload.Disabled = true;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void btnDecisionSave_Click(object sender, EventArgs e)
        {
            try
            {
                string strGroupPerjawatanCode = lblDecisionModalGroupPerjawatanCode.Text; 
                int PeriodPerjawatanID = Convert.ToInt32(lblDecisionModalPeriodID.Text);
                decimal amount = Convert.ToDecimal(lblDecisionModalAmount.Text.Replace("Amount : ", "").Trim());

                List<BudgetPerjawatan> BudgetData = (List<BudgetPerjawatan>)Session["BudgetData"];
                BudgetPerjawatan item = BudgetData.Where(x => x.GroupPerjawatanCode == strGroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID).FirstOrDefault();
                string strstatus = string.Empty;
                if (rbldecision.SelectedIndex == 0)
                {
                    strstatus = (IsReviewer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed)
                        : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Approved);
                }
                else
                {
                    strstatus = (IsReviewer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.ReviewerRejected)
                           : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.ApproverRejected);
                    //amount = 0;
                }
                if (item != null)
                {
                    item.Amount = amount;
                    item.Status = strstatus;
                    item.Remarks = tbRemarks.Text;

                    BudgetPerjawatan newBudgetPerjawatan = new BudgetPerjawatan();
                    newBudgetPerjawatan.GroupPerjawatanCode = strGroupPerjawatanCode;
                    newBudgetPerjawatan.PeriodPerjawatanID = PeriodPerjawatanID;
                    newBudgetPerjawatan.Amount = amount;
                    newBudgetPerjawatan.Status = strstatus;
                    newBudgetPerjawatan.Remarks = tbRemarks.Text;
                    newBudgetPerjawatan.ModifiedBy = AuthUser.UserID;
                    newBudgetPerjawatan.ModifiedTimeStamp = DateTime.Now;

                    List<int> LstSegmentDetailIDs = ((List<JuncBgtPerjawatanSegDtl>)Session["ListSegmentDetails"]).Select(x => x.SegmentDetailID).ToList();
                    //if (new BudgetPerjawatanBAL().UpdateBudgetPerjawatans(newBudgetPerjawatan, LstSegmentDetailIDs)) 
                    //    ((SiteMaster)this.Master).ShowMessage("Success", "Budget updated successfully");
                    //else
                    //    ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Budget");
                    new BudgetPerjawatanBAL().UpdateBudgetPerjawatans(newBudgetPerjawatan, LstSegmentDetailIDs);
                }

                CreateTreeData();
                BuildGrid();
                BindGrid();

                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myDecisionModal", "$('#myDecisionModal').hide();", true);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void btnBulkUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> LstSegmentDetailIDs = ((List<JuncBgtPerjawatanSegDtl>)Session["ListSegmentDetails"]).Select(x => x.SegmentDetailID).ToList();
                List<PeriodPerjawatan> PeriodData = (List<PeriodPerjawatan>)Session["PeriodData"];
                List<int> PeriodIDs = PeriodData.Where(x => x.PerjawatanYear > DateTime.Now.Year).Select(x => x.PeriodPerjawatanID).ToList();
                bool Accept = ddlBulkDecision.Items[0].Selected;
                string FromChar = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                    : (IsReviewer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Prepared) : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);
                string ToChar = (IsPreparer) ?
                    new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Prepared)
                    :
                    (IsReviewer) ?
                        (
                            (Accept) ?
                            new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed)
                            :
                            new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.ReviewerRejected)
                        )
                        :
                        (
                            (Accept) ?
                            new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Approved)
                            :
                            new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.ApproverRejected)
                        );

                if (new BudgetPerjawatanBAL().UpdateMultipleBudgetPerjawatans(LstSegmentDetailIDs, PeriodIDs, FromChar, ToChar, tbBulkRemarks.Text.Trim(), LoggedInUser))
                    ((SiteMaster)this.Master).ShowMessage("Success", "Budget updated successfully");
                else
                    ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Budget");

                bool editboxvisible = EditBox.Visible;
                EditBox.Visible = true;

                GetData();
                CreateTreeData();
                BuildGrid();
                BindGrid();

                EditBox.Visible = editboxvisible;
                tbBulkRemarks.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void gvGroupPerjawatans_PageIndexChanged(object sender, EventArgs e)
        {
            BuildGrid();
            BindGrid();
        }

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
                ds.Tables.Add(GetPerjawatansDataTable());
                string filename = Session["PrefixAcountCode"].ToString().Substring(0, Session["PrefixAcountCode"].ToString().Length - 1);
                new ReportHelper().ToExcel(ds, "BudgetPerjawatan_" + filename + ".xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        private DataTable GetPerjawatansDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                List<GroupPerjawatanTreeHelper> GroupPerjawatans = CreateExportData();
                List<PeriodPerjawatan> PeriodData = (List<PeriodPerjawatan>)Session["PeriodData"];
                List<BudgetPerjawatan> BudgetData = ((List<BudgetPerjawatan>)Session["BudgetData"]).ToList();  //.Where(x => x.Status == "A").ToList();
                List<string> PrefixDesc = (List<string>)Session["PrefixDesc"];

                //Start Build DataTable
                DataColumn dc = new DataColumn();
                foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
                {
                    dc = new DataColumn(gvr.Cells[0].Text);
                    dt.Columns.Add(dc);
                }

                dc = new DataColumn("GroupPerjawatanCode");
                dt.Columns.Add(dc);
                //dc = new DataColumn("Objeck");
                //dt.Columns.Add(dc);
                dc = new DataColumn("Description");
                dt.Columns.Add(dc);
                //if (chkKeterangan.Checked)
                //{
                //    dc = new DataColumn("Keterangan");
                //    dt.Columns.Add(dc);
                //}
                //if (chkPengiraan.Checked)
                //{
                //    dc = new DataColumn("Pengiraan");
                //    dt.Columns.Add(dc);
                //}
                foreach (PeriodPerjawatan pm in PeriodData)
                {
                    int count = dt.Columns.Cast<DataColumn>().Where(x => x.ColumnName == pm.PerjawatanYear.ToString() + "_" + pm.FieldPerjawatan.FieldPerjawatanDesc).Count();
                    string colname = pm.PerjawatanYear.ToString() + "_" + pm.FieldPerjawatan.FieldPerjawatanDesc;
                    colname = (count == 0) ? colname : colname + "_" + (count + 1).ToString();
                    dc = new DataColumn(colname);
                    dt.Columns.Add(dc);
                }
                //End Build DataTable

                //Start pushing data into DataTable
                foreach (GroupPerjawatanTreeHelper acchelp in GroupPerjawatans)
                {
                    int c = 0;
                    DataRow dr = dt.NewRow();

                    foreach (string s in PrefixDesc)
                    {
                        dr[c] = s;
                        c++;
                    }

                    dr[c] = new Helper().GetLevelString(acchelp.GroupPerjawatanCode, acchelp.Level);
                    c++;
                    //dr[c] = Session["PrefixAcountCode"].ToString() + acchelp.GroupPerjawatan;
                    //c++;
                    dr[c] = acchelp.GroupPerjawatanDesc;
                    //if (chkKeterangan.Checked)
                    //{
                    //    c++;
                    //    dr[c] = acchelp.Keterangan;
                    //}
                    //if (chkPengiraan.Checked)
                    //{
                    //    c++;
                    //    dr[c] = acchelp.Pengiraan;
                    //}

                    foreach (PeriodPerjawatan pm in PeriodData)
                    {
                        BudgetPerjawatan ObjBudgetPerjawatan = BudgetData.Where(x => x.GroupPerjawatanCode == acchelp.GroupPerjawatanCode && x.PeriodPerjawatanID == pm.PeriodPerjawatanID).FirstOrDefault();
                        decimal amount = Convert.ToDecimal(0);
                        if (acchelp.ChildCount == 0)
                        {
                            amount = (ObjBudgetPerjawatan != null) ? ObjBudgetPerjawatan.Amount : Convert.ToDecimal(0);
                        }
                        else
                        {
                            List<string> ChildIDs = new List<string>() { acchelp.GroupPerjawatanCode };
                            List<string> RefChildIDs = new List<string>();
                            while (ChildIDs.Count > 0)
                            {
                                RefChildIDs.Clear();
                                foreach (GroupPerjawatanTreeHelper t in GroupPerjawatans.Where(x => ChildIDs.Contains(x.GroupPerjawatanCode)))
                                {
                                    amount = amount + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == pm.PeriodPerjawatanID).Select(x => x.Amount).Sum();
                                    foreach (string s in GroupPerjawatans.Where(x => x.ParentGroupPerjawatanID == t.GroupPerjawatanCode).Select(x => x.GroupPerjawatanCode).ToList())
                                        RefChildIDs.Add(s);
                                }
                                ChildIDs.Clear();
                                foreach (string s in RefChildIDs)
                                    ChildIDs.Add(s);
                            }
                        }

                        c++;
                        dr[c] = amount.ToString("F");
                    }
                    dt.Rows.Add(dr);
                }
                //End pushing data into DataTable
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
            return dt;
        }

        protected void chkMedan_CheckedChanged(object sender, EventArgs e)
        {
            //chkKeterangan.Checked = (chkKeterangan.Enabled && chkMedan.Checked);
            //chkPengiraan.Checked = (chkPengiraan.Enabled && chkMedan.Checked);

            for (int i = 0; i < gvPeriod.Rows.Count; i++)
            {
                string stryear = gvPeriod.Rows[i].Cells[1].Text;
                ((CheckBox)gvPeriod.Rows[i].Cells[0].FindControl("cbPeriodSelect")).Checked = (chkMedan.Checked || (stryear == (DateTime.Now.Year + 1).ToString()));
            }
        }

        protected void btnSample_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;

                DataSet ds = new DataSet();
                ds.Tables.Add(BudgetPerjawatanImport());

                new ReportHelper().ToExcel(ds, "BudgetPerjawatanSample.xls", ref Response);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected DataTable BudgetPerjawatanImport()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn();

            List<PeriodPerjawatan> PeriodData = (List<PeriodPerjawatan>)Session["PeriodData"];

            dc = new DataColumn("GroupPerjawatanCode");
            dt.Columns.Add(dc);
            dc = new DataColumn("Description");
            dt.Columns.Add(dc);

            foreach (PeriodPerjawatan pm in PeriodData)
            {
                int count = dt.Columns.Cast<DataColumn>().Where(x => x.ColumnName == pm.PerjawatanYear.ToString() + "_" + pm.FieldPerjawatan.FieldPerjawatanDesc).Count();
                string colname = pm.PerjawatanYear.ToString() + "_" + pm.FieldPerjawatan.FieldPerjawatanDesc;
                colname = (count == 0) ? colname : colname + "_" + (count + 1).ToString();
                dc = new DataColumn(colname);
                dt.Columns.Add(dc);
            }
            //End Build DataTable
            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);

            return dt;
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
                        dt = new ReportHelper().ValidateBudgetImport<BudgetPerjawatanSetup>(ds, gvGroupPerjawatans, ref lstErrors);

                        if (lstErrors.Count == 0)
                        {
                            List<GroupPerjawatan> GroupPerjawatansData = ((List<GroupPerjawatan>)Session["GroupPerjawatansData"]).Where(x =>
                                !String.IsNullOrEmpty(x.ParentGroupPerjawatanID)).ToList();

                            foreach (DataRow row in dt.Rows)
                            {
                                if (GroupPerjawatansData.Where(x => x.GroupPerjawatanCode == Convert.ToString(row["ServiceCode"])).Select(y => y.GroupPerjawatanCode).Count() > 0)
                                {
                                    if (MatchingData(row))
                                    {
                                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget updated successfully");
                                    }
                                    else
                                    {
                                        ((SiteMaster)this.Master).ShowMessage("Upload Failed", "Please check your file");
                                    }
                                }
                                else
                                {
                                    ((SiteMaster)this.Master).ShowMessage("Failure", "AccountCode not match");
                                }
                            }

                            GetData();
                            CreateTreeData();
                            BuildGrid();
                            BindGrid();
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

        public bool MatchingData(DataRow dt)
        {
            bool res = false;

            List<PeriodPerjawatan> PeriodData = (List<PeriodPerjawatan>)Session["PeriodData"];
            List<BudgetPerjawatan> BudgetData = (List<BudgetPerjawatan>)Session["BudgetData"];
            List<int> BlockedYears = (List<int>)Session["BlockedYears"];
            List<int> OpenYears = (List<int>)Session["OpenYears"];
            List<string> ag = (List<string>)Session["BudgetCol"];

            foreach (string dipohon in ag)
            {
                if (dt.Table.Columns.Contains(dipohon))
                {
                    PeriodPerjawatan pd = PeriodData.Where(x => (x.PerjawatanYear.ToString() + " " + x.FieldPerjawatan.FieldPerjawatanDesc).Contains(dipohon)).FirstOrDefault();

                    if (pd != null)
                    {
                        List<BudgetPerjawatan> bp = BudgetData.Where(x => x.PeriodPerjawatanID == pd.PeriodPerjawatanID && x.GroupPerjawatanCode == Convert.ToString(dt["ServiceCode"])).ToList();

                        if (bp.Count > 0)
                        {
                            if (bp.Where(s => s.Status == "P" || s.Status == "R" || s.Status == "A").Count() == 0)
                            {
                                if (!BlockedYears.Contains(pd.PerjawatanYear) && OpenYears.Contains(pd.PerjawatanYear))
                                {
                                    decimal total = 0;
                                    decimal amt = bp.Select(x => x.Amount).FirstOrDefault();

                                    total = Convert.ToDecimal(dt[dipohon]) + amt;

                                    UpdateAmountFromUploadFile(dipohon, Convert.ToString(dt["ServiceCode"]), total);
                                    res = true;
                                }
                            }
                        }
                        else
                        {
                            if (!BlockedYears.Contains(pd.PerjawatanYear) && OpenYears.Contains(pd.PerjawatanYear))
                            {
                                UpdateAmountFromUploadFile(dipohon, Convert.ToString(dt["ServiceCode"]), Convert.ToDecimal(dt[dipohon]));
                                res = true;
                            }
                        }
                    }
                }
            }

            return res;
        }

        public void UpdateAmountFromUploadFile(string ColumnName, string ServiceCode, decimal amount)
        {
            for (int r = 0; r < gvGroupPerjawatans.Rows.Count; r++)
            {
                for (int c = 0; c < gvGroupPerjawatans.Columns.Count; c++)
                {
                    if (gvGroupPerjawatans.Columns[c].HeaderText.Contains(ColumnName))
                    {
                        string val = (string)gvGroupPerjawatans.DataKeys[r]["GroupPerjawatanCode"];
                        if (val == ServiceCode)
                        {
                            int PeriodPerjawatanID = Convert.ToInt32(((Label)gvGroupPerjawatans.Rows[r].Cells[c].Controls[0]).Text);

                            TextBox tb = ((TextBox)gvGroupPerjawatans.Rows[r].Cells[c].FindControl("tb_" + PeriodPerjawatanID));
                            tb.Text = (amount > 0) ? amount.ToString("#,##0.00") : Convert.ToDecimal(0).ToString("#,##0.00");

                            try
                            {
                                //SaveUploadedFile(ServiceCode, PeriodMenguruID, amount);
                                string strstatus = string.Empty;

                                BudgetPerjawatan newBudgetPerjawatan = new BudgetPerjawatan();
                                newBudgetPerjawatan.GroupPerjawatanCode = ServiceCode;
                                newBudgetPerjawatan.PeriodPerjawatanID = PeriodPerjawatanID;
                                newBudgetPerjawatan.Amount = amount;

                                List<BudgetPerjawatan> BudgetData = ((List<BudgetPerjawatan>)Session["BudgetData"]).Where(x => x.GroupPerjawatanCode == ServiceCode && x.PeriodPerjawatanID == PeriodPerjawatanID).ToList();

                                if (BudgetData.FirstOrDefault() != null)
                                {
                                    strstatus = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                                    : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);

                                    newBudgetPerjawatan.Status = strstatus;
                                    newBudgetPerjawatan.ModifiedBy = AuthUser.UserID;
                                    newBudgetPerjawatan.ModifiedTimeStamp = DateTime.Now;

                                    List<int> LstSegmentDetailIDs = ((List<JuncBgtPerjawatanSegDtl>)Session["ListSegmentDetails"]).Select(x => x.SegmentDetailID).ToList();
                                    if (new BudgetPerjawatanBAL().UpdateBudgetPerjawatans(newBudgetPerjawatan, LstSegmentDetailIDs))
                                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget updated successfully");
                                    else
                                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Budget");
                                }
                                else
                                {
                                    strstatus = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                                    : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);

                                    newBudgetPerjawatan.Status = strstatus;
                                    newBudgetPerjawatan.CreatedBy = AuthUser.UserID;
                                    newBudgetPerjawatan.CreatedTimeStamp = DateTime.Now;
                                    newBudgetPerjawatan.ModifiedBy = AuthUser.UserID;
                                    newBudgetPerjawatan.ModifiedTimeStamp = DateTime.Now;

                                    BudgetData.Add(newBudgetPerjawatan);

                                    List<JuncBgtPerjawatanSegDtl> lstBgtSegDtl = (List<JuncBgtPerjawatanSegDtl>)Session["ListSegmentDetails"];

                                    foreach (JuncBgtPerjawatanSegDtl obj in lstBgtSegDtl)
                                        obj.BudgetPerjawatan = newBudgetPerjawatan;

                                    if (new BudgetPerjawatanBAL().InsertBudgetPerjawatans(newBudgetPerjawatan, lstBgtSegDtl))
                                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget saved successfully");
                                    else
                                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while saving Budget");
                                }
                            }
                            catch (Exception ex)
                            {
                                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
                            }
                        }
                    }
                }
            }
        }
    }
}