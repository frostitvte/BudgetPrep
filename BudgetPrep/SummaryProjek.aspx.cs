using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBBAL;
using OBDAL;
using BudgetPrep.Classes;
using System.Threading;
using System.Data;

namespace BudgetPrep
{
    public partial class SummaryProjek : System.Web.UI.Page
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

                if (!Page.IsPostBack)
                {
                    Session["SelectedNodes"] = null;
                    ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Budget";
                    ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Projek";
                    LoadDropDown();
                    GetData();
                    CreateTreeData();
                }
                else
                {
                    //AccountCodesData = (Session["AccountCodesData"] != null) ? (List<AccountCode>)Session["AccountCodesData"] : new List<AccountCode>();
                    //PeriodData = (Session["PeriodData"] != null) ? (List<PeriodMenguru>)Session["PeriodData"] : new List<PeriodMenguru>();
                    //BudgetData = (Session["BudgetData"] != null) ? (List<BudgetProjek>)Session["BudgetData"] : new List<BudgetProjek>();
                    //SelectedNodes = (Session["SelectedNodes"] != null) ? (List<string>)Session["SelectedNodes"] : new List<string>();
                    //TreeData = (Session["AccountCodesTree"] != null) ? (List<AccountCodeTreeHelper>)Session["AccountCodesTree"] : new List<AccountCodeTreeHelper>();
                }
                if (BudgetBox.Visible)
                {
                    BuildGrid();
                    BindGrid();
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
                List<AccountCode> lstAccountCode = new AccountCodeBAL().GetAccountCodes().Where(x => x.Status == "A").ToList();
                List<AccountCode> AccountCodesData = new List<AccountCode>();

                if (AuthUser.JuncUserRoles.FirstOrDefault().RoleID == 1 || AuthUser.JuncUserRoles.FirstOrDefault().RoleID == 5)
                    AccountCodesData = lstAccountCode;
                else
                {
                    AccountCodesData = AuthUser.UserMengurusWorkflows.Where(x => x.Status == "A").Select(x => x.AccountCode1).ToList();
                    if (AccountCodesData.Count == 0)
                    {
                        lstAccountCode = new List<AccountCode>();
                    }
                    else
                    {
                        List<string> lstprntcodes = AccountCodesData.Select(x => x.ParentAccountCode).Distinct().ToList();
                        while (lstprntcodes.Count > 0)
                        {
                            List<AccountCode> lstprnts = lstAccountCode.Where(x => lstprntcodes.Contains(x.AccountCode1)).ToList();
                            foreach (AccountCode o in lstprnts)
                                if (AccountCodesData.Where(x => x.AccountCode1 == o.AccountCode1).Count() == 0)
                                    AccountCodesData.Add(o);
                            lstprntcodes = lstprnts.Select(x => x.ParentAccountCode).Distinct().ToList();
                        }
                    }
                }
                Session["AccountCodesData"] = lstAccountCode;

                List<int> lstperiod = GetSelectedPeriods();
                List<PeriodMenguru> PeriodData = new PeriodMengurusBAL().GetPeriodMengurus().Where(x => x.Status == "A" && lstperiod.Contains(x.PeriodMengurusID))
                    .OrderBy(x => x.MengurusYear).ThenBy(x => x.FieldMenguru.FieldMengurusDesc).ToList();

                List<PeriodMenguru> FixedData = ((List<FieldMenguru>)Session["FixedFieldMengurus"]).Where(x => lstperiod.Contains(x.FieldMengurusID))
                   .Select(x => new PeriodMenguru
                   {
                       PeriodMengurusID = lstperiod.Contains(x.FieldMengurusID) ? x.FieldMengurusID : 0,
                       MengurusYear = DateTime.Now.Year,
                       FieldMenguru = new FieldMenguru
                       {
                           FieldMengurusID = x.FieldMengurusID,
                           FieldMengurusDesc = x.FieldMengurusDesc,
                           Status = x.Status
                       }
                   }).OrderBy(x => x.MengurusYear).ThenBy(x => x.FieldMenguru.FieldMengurusDesc).ToList();

                FixedData.AddRange(PeriodData);
                Session["PeriodData"] = FixedData;

                bool CanEdit = false;
                List<BudgetProjek> BudgetData = new BudgetProjekBAL().GetBudgetProjekStatus(GetSelectedSegmentDetails(), ref CanEdit)
                    .Where(x => x.Status == "A")
                    .Select(x => new BudgetProjek
                    {
                        BudgetProjekID = 0,
                        AccountCode = x.AccountCode,
                        PeriodMengurusID = x.PeriodMengurusID,
                        Status = x.Status,
                        Remarks = string.Empty,
                        Amount = (AccountCodesData.Where(y => y.ParentAccountCode == x.AccountCode).Count() == 0) ? x.Amount : 0
                    })
                    .GroupBy(x => new
                    {
                        x.AccountCode,
                        x.PeriodMengurusID,
                        x.Status
                    })
                    .Select(x => new BudgetProjek
                    {
                        AccountCode = x.Key.AccountCode,
                        PeriodMengurusID = x.Key.PeriodMengurusID,
                        Status = x.Key.Status,
                        Amount = x.Sum(y => y.Amount)
                    })
                    .ToList();
                Session["BudgetData"] = BudgetData;
                Session["CanEdit"] = CanEdit;

                if (!CanEdit)
                {
                    chkKeterangan.Checked = false;
                    chkPengiraan.Checked = false;
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        private Dictionary<int, int> GetSelectedSegmentDetails()
        {
            Dictionary<int, int> DicSegDtls = new Dictionary<int, int>();
            try
            {
                foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
                {
                    TreeView tv = (TreeView)gvr.Cells[0].FindControl("tvSegmentDDL");
                    TextBox tb = (TextBox)gvr.Cells[0].FindControl("tbSegmentDDL");

                    DicSegDtls.Add(Convert.ToInt32(gvSegmentDLLs.DataKeys[gvr.RowIndex]["SegmentID"].ToString()),
                        (tv == null || tb.Text.Trim() == string.Empty) ? 0 : Convert.ToInt32(tv.SelectedValue));
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
            return DicSegDtls;
        }

        private void CreateTreeData()
        {
            try
            {
                List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];
                List<AccountCode> AccountCodesData = (List<AccountCode>)Session["AccountCodesData"];
                List<AccountCodeTreeHelper> TreeData = new List<AccountCodeTreeHelper>();
                if (AccountCodesData.Count > 0)
                {
                    TreeData = AccountCodesData.Where(x => x.ParentAccountCode == string.Empty && x.Status == "A").OrderBy(x => x.AccountCode1).Select(x =>
                            new AccountCodeTreeHelper()
                            {
                                AccountCode = x.AccountCode1,
                                AccountDesc = x.AccountDesc,
                                Keterangan = x.Keterangan,
                                Pengiraan = x.Pengiraan,
                                ParentAccountCode = x.ParentAccountCode,
                                Status = x.Status,
                                Level = 0,
                                ChildCount = AccountCodesData.Where(y => y.ParentAccountCode == x.AccountCode1).Count()
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
                            if (SelectedNodes.Contains(TreeData[i].AccountCode))
                            {
                                foreach (AccountCode sd in AccountCodesData.Where(x => x.ParentAccountCode == TreeData[i].AccountCode && x.Status == "A").OrderByDescending(x => x.AccountCode1))
                                {
                                    AccountCodeTreeHelper objSH = new AccountCodeTreeHelper()
                                    {
                                        AccountCode = sd.AccountCode1,
                                        AccountDesc = sd.AccountDesc,
                                        Keterangan = sd.Keterangan,
                                        Pengiraan = sd.Pengiraan,
                                        ParentAccountCode = sd.ParentAccountCode,
                                        Status = sd.Status,
                                        Level = TreeData[i].Level + 1,
                                        ChildCount = AccountCodesData.Where(y => y.ParentAccountCode == sd.AccountCode1).Count()
                                    };
                                    TreeData.Insert(i + 1, objSH);
                                }
                            }
                        }
                    }
                }
                Session["AccountCodesTree"] = TreeData;
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
                gvAccountCodes.Columns.Clear();
                List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];
                List<BudgetProjek> BudgetData = (List<BudgetProjek>)Session["BudgetData"];
                List<AccountCode> AccountCodesData = (List<AccountCode>)Session["AccountCodesData"];
                List<string> parentcodes = AccountCodesData.Select(x => x.ParentAccountCode).Distinct().ToList();
                List<string> ChildCodes = AccountCodesData.Where(x => !parentcodes.Contains(x.AccountCode1)).Select(x => x.AccountCode1).ToList();

                TemplateField templateField1 = new TemplateField();
                templateField1.HeaderText = "Objek";
                templateField1.ItemTemplate = new GridViewCustomTemplate(0, "", 0);
                templateField1.FooterText = "Jumlah";
                gvAccountCodes.Columns.Add(templateField1);

                BoundField bf1 = new BoundField();
                bf1.HeaderText = "Description";
                bf1.DataField = "AccountDesc";
                gvAccountCodes.Columns.Add(bf1);

                if (chkKeterangan.Checked)
                {
                    BoundField bf2 = new BoundField();
                    bf2.HeaderText = "Keterangan";
                    bf2.DataField = "Keterangan";
                    gvAccountCodes.Columns.Add(bf2);
                }

                if (chkPengiraan.Checked)
                {
                    BoundField bf2 = new BoundField();
                    bf2.HeaderText = "Pengiraan";
                    bf2.DataField = "Pengiraan";
                    gvAccountCodes.Columns.Add(bf2);
                }

                foreach (PeriodMenguru pm in PeriodData)
                {
                    TemplateField templateField = new TemplateField();
                    templateField.HeaderText = pm.MengurusYear.ToString() + " " + pm.FieldMenguru.FieldMengurusDesc;
                    GridViewStatusCustomTemplate objTemp = new GridViewStatusCustomTemplate(0, pm.MengurusYear.ToString() + " " + pm.FieldMenguru.FieldMengurusDesc, pm.PeriodMengurusID);
                    objTemp.OnCustomStatusClicked += new CustomStatusClickedEventHandler(objTemp_OnCustomStatusClicked);
                    templateField.ItemTemplate = objTemp;
                    templateField.FooterText = BudgetData.Where(x => x.PeriodMengurusID == pm.PeriodMengurusID && ChildCodes.Contains(x.AccountCode)).Select(x => x.Amount).Sum().ToString("F");
                    gvAccountCodes.Columns.Add(templateField);
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
                gvAccountCodes.DataSource = (List<AccountCodeTreeHelper>)Session["AccountCodesTree"];
                gvAccountCodes.DataBind();
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
                //gvAccountCodes.Visible = false;
                //btnPrint.Visible = false;
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

                    int PeriodMengurusID = Convert.ToInt32(gvPeriod.DataKeys[i]["PeriodMengurusID"].ToString());
                    List<FieldMenguru> obj = ((List<FieldMenguru>)Session["FixedFieldMengurus"]).Where(x => x.FieldMengurusID == PeriodMengurusID).ToList();

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
            BudgetBox.Visible = true;

            GetData();
            CreateTreeData();
            BuildGrid();
            BindGrid();

            EditBox.Visible = false;
        }

        protected void btnSearchbox_Click(object sender, EventArgs e)
        {
            EditBox.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPageData();

            EditBox.Visible = true;
        }

        protected void gvAccountCodes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];
                    List<BudgetProjek> BudgetData = (List<BudgetProjek>)Session["BudgetData"];
                    List<AccountCodeTreeHelper> TreeData = (List<AccountCodeTreeHelper>)Session["AccountCodesTree"];
                    List<AccountCode> AccountCodesData = (List<AccountCode>)Session["AccountCodesData"];
                    List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];
                    AccountCodeTreeHelper rowItem = (AccountCodeTreeHelper)e.Row.DataItem;

                    /*Start Account Code logics*/
                    int width = rowItem.Level * (new Helper().IndentPixels);
                    string strHTML = string.Empty;
                    if (rowItem.ChildCount > 0)
                    {
                        if (SelectedNodes.Contains(rowItem.AccountCode))
                            strHTML = "<label style=\"display: inline-block;width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i class=\"fa fa-minus-square pull-right\"></i></label> ";
                        else
                            strHTML = "<label style=\"display: inline-block;width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i class=\"fa fa-plus-square pull-right\"></i></label> ";
                    }
                    else
                        strHTML = "<label style=\"display: inline-block;width:" + (width + 10).ToString() + "px;vertical-align:middle;\"><i></i></label> ";

                    LinkButton btnExpand = ((LinkButton)e.Row.Cells[0].FindControl("btnExpand"));
                    //btnExpand.Text = "<div style=\"max-width:200px;overflow:hidden;white-space:nowrap;\">" + strHTML + Session["PrefixAcountCode"].ToString() + rowItem.AccountCode + "</div>";
                    btnExpand.Text = "<div style=\"max-width:200px;overflow:hidden;white-space:nowrap;\">" + strHTML + rowItem.AccountCode + "</div>";
                    btnExpand.ToolTip = rowItem.AccountDesc;

                    if (Session["SelectedAccountCode"] != null && ((AccountCode)Session["SelectedAccountCode"]).AccountCode1 == rowItem.AccountCode)
                    {
                        e.Row.Style["background-color"] = "gold";
                    }
                    /*End Account Code logics*/

                    /*Start Buget logics*/
                    int index = 2;
                    if (chkKeterangan.Checked)
                    {
                        //if (rowItem.ChildCount > 0)
                        //    ((TextBox)e.Row.Cells[index].Controls[0]).Visible = false;
                        index++;
                    }
                    if (chkPengiraan.Checked)
                    {
                        //if (rowItem.ChildCount > 0)
                        //    ((TextBox)e.Row.Cells[index].Controls[0]).Visible = false;
                        index++;
                    }

                    bool IsBudgetEditable = Convert.ToBoolean(Session["CanEdit"]);
                    for (int c = index; c < gvAccountCodes.Columns.Count; c++)
                    {
                        //int PeriodMenguruID = Convert.ToInt32(((Label)e.Row.Cells[c].FindControl("lbl_PeriodMenguruID")).Text);
                        int PeriodMenguruID = Convert.ToInt32(((Label)e.Row.Cells[c].Controls[0]).Text);
                        PeriodMenguru pm = PeriodData.Where(x => x.PeriodMengurusID == PeriodMenguruID).FirstOrDefault();
                        BudgetProjek ObjBudgetProjek = BudgetData.Where(x => x.AccountCode == rowItem.AccountCode && x.PeriodMengurusID == PeriodMenguruID).FirstOrDefault();

                        Label lbl = ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodMenguruID));
                        LinkButton btnSaved = ((LinkButton)e.Row.Cells[c].FindControl("btnSaved_" + PeriodMenguruID));
                        LinkButton btnPrepared = ((LinkButton)e.Row.Cells[c].FindControl("btnPrepared_" + PeriodMenguruID));
                        LinkButton btnReviewed = ((LinkButton)e.Row.Cells[c].FindControl("btnReviewed_" + PeriodMenguruID));
                        LinkButton btnApproved = ((LinkButton)e.Row.Cells[c].FindControl("btnApproved_" + PeriodMenguruID));
                        LinkButton btnRevRej = ((LinkButton)e.Row.Cells[c].FindControl("btnRevRej_" + PeriodMenguruID));
                        LinkButton btnAprRej = ((LinkButton)e.Row.Cells[c].FindControl("btnAprRej_" + PeriodMenguruID));

                        lbl.Visible = true;
                        btnSaved.Visible = false;
                        btnPrepared.Visible = false;
                        btnReviewed.Visible = false;
                        btnApproved.Visible = false;
                        btnRevRej.Visible = false;
                        btnAprRej.Visible = false;

                        e.Row.Cells[c].BackColor = ((ObjBudgetProjek != null) ? new Helper().GetColorByStatusValue('A') : new System.Drawing.Color());

                        if (rowItem.ChildCount == 0)
                        {
                            lbl.Text = (ObjBudgetProjek != null) ? ObjBudgetProjek.Amount.ToString() : string.Empty;
                        }
                        else
                        {
                            decimal amount = 0;

                            List<string> ChildIDs = new List<string>() { rowItem.AccountCode };
                            List<string> RefChildIDs = new List<string>();
                            while (ChildIDs.Count > 0)
                            {
                                RefChildIDs.Clear();
                                foreach (AccountCode t in AccountCodesData.Where(x => ChildIDs.Contains(x.AccountCode1)))
                                {
                                    amount = amount + BudgetData.Where(x => x.AccountCode == t.AccountCode1 && x.PeriodMengurusID == PeriodMenguruID).Select(x => x.Amount).Sum();
                                    foreach (string s in AccountCodesData.Where(x => x.ParentAccountCode == t.AccountCode1).Select(x => x.AccountCode1).ToList())
                                        RefChildIDs.Add(s);
                                }
                                ChildIDs.Clear();
                                foreach (string s in RefChildIDs)
                                    ChildIDs.Add(s);
                            }

                            lbl.Text = (amount != 0) ? amount.ToString("F") : string.Empty;
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

        protected void gvAccountCodes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                List<AccountCodeTreeHelper> TreeData = (List<AccountCodeTreeHelper>)Session["AccountCodesTree"];
                //GridViewRow selectedRow = gvAccountCodes.Rows[Convert.ToInt32(e.CommandArgument)];
                if (e.CommandName == "Expand")
                {
                    GridViewRow selectedRow = (GridViewRow)((LinkButton)e.CommandSource).Parent.Parent;
                    string AccountCode = gvAccountCodes.DataKeys[selectedRow.RowIndex]["AccountCode"].ToString();
                    List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];
                    if (!SelectedNodes.Contains(AccountCode))
                    {
                        if (TreeData.Where(x => x.AccountCode == AccountCode).FirstOrDefault().ChildCount > 0)
                            SelectedNodes.Add(AccountCode);
                    }
                    else
                    {
                        SelectedNodes.Remove(AccountCode);
                    }
                    CreateTreeData();
                    BuildGrid();
                    BindGrid();
                }
                else if (e.CommandName == "Page")
                {
                    gvAccountCodes.PageIndex = Convert.ToInt32(e.CommandArgument) - 1;
                    BuildGrid();
                    BindGrid();
                }
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
                List<PeriodMenguru> lstPeriodMengurus = new PeriodMengurusBAL().GetPeriodMengurus().Where(x => x.Status == "A" && x.MengurusYear > DateTime.Now.Year)
                    .OrderBy(x => x.MengurusYear).ThenBy(x => x.FieldMenguru.FieldMengurusDesc).ToList();

                List<FieldMenguru> obj = new FieldMenguruBAL().GetFieldMengurus().Where(x => x.Status == "F").ToList();
                Session["FixedFieldMengurus"] = obj;

                foreach (FieldMenguru item in obj)
                {
                    PeriodMenguru pm = new PeriodMenguru();

                    pm.PeriodMengurusID = item.FieldMengurusID;
                    pm.MengurusYear = DateTime.Now.Year;
                    pm.FieldMenguru = new FieldMenguru { FieldMengurusDesc = item.FieldMengurusDesc };

                    lstPeriodMengurus.Add(pm);
                }

                gvPeriod.DataSource = lstPeriodMengurus.Select(x => new
                {
                    x.PeriodMengurusID,
                    x.MengurusYear,
                    x.FieldMenguru.FieldMengurusDesc
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

                    if (AuthUser.JuncUserRoles.FirstOrDefault().RoleID == 1 || AuthUser.JuncUserRoles.FirstOrDefault().RoleID == 5)
                        lstSD = lst.Where(x => x.Status == "A" && x.SegmentID == rowItem.SegmentID).ToList();
                    else
                    {
                        lstSD = AuthUser.UserSegDtlWorkflows.Where(x => x.Status == "A" && x.SegmentDetail.SegmentID == rowItem.SegmentID).Select(x => x.SegmentDetail).ToList();
                        List<int> parntids = lstSD.Select(x => Convert.ToInt32(x.ParentDetailID)).Distinct().ToList();

                        while (parntids.Count > 0)
                        {
                            List<SegmentDetail> lstprnts = lst.Where(x => parntids.Contains(x.SegmentDetailID)).ToList();
                            foreach (SegmentDetail o in lstprnts)
                                lstSD.Add(o);
                            parntids = lstprnts.Select(x => Convert.ToInt32(x.ParentDetailID)).Distinct().ToList();
                        }
                    }

                    lstSD = lstSD.OrderBy(x => x.ParentDetailID).ThenBy(x => x.DetailCode).ToList();

                    List<TreeNode> lstTN = new List<TreeNode>();
                    TreeNode tn = new TreeNode();
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

        protected void gvPeriod_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string stryear = e.Row.Cells[1].Text;
                    ((CheckBox)e.Row.Cells[0].FindControl("cbPeriodSelect")).Checked = (stryear == (DateTime.Now.Year + 1).ToString());

                    int PeriodMengurusID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "PeriodMengurusID"));
                    List<FieldMenguru> obj = ((List<FieldMenguru>)Session["FixedFieldMengurus"]).Where(x => x.FieldMengurusID == PeriodMengurusID).ToList();

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

        protected void objTemp_OnCustomStatusClicked(object sender, CustomEvenArgs e)
        {
            try
            {
                lblDecisionModalAccountCode.Text = "Account Code : " + e.Code;
                lblDecisionModalPeriod.Text = "Period : " + e.Period;

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
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
                        lst.Add(Convert.ToInt32(gvPeriod.DataKeys[gvr.RowIndex]["PeriodMengurusID"].ToString()));
                }
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
            return lst;
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

                chkKeterangan.Checked = (CanEdit) ? chkKeterangan.Checked : false;
                chkPengiraan.Checked = (CanEdit) ? chkPengiraan.Checked : false;
                chkKeterangan.Enabled = CanEdit;
                chkPengiraan.Enabled = CanEdit;
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
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

        //Print
        private List<AccountCodeTreeHelper> CreateExportData()
        {
            List<AccountCodeTreeHelper> TreeData = new List<AccountCodeTreeHelper>();
            try
            {
                List<AccountCode> data = (List<AccountCode>)Session["AccountCodesData"];
                if (data.Count > 0)
                {
                    TreeData = data.Where(x => x.ParentAccountCode == string.Empty).OrderBy(x => x.AccountCode1).Select(x =>
                            new AccountCodeTreeHelper()
                            {
                                AccountCode = x.AccountCode1,
                                AccountDesc = x.AccountDesc,
                                Keterangan = x.Keterangan,
                                Pengiraan = x.Pengiraan,
                                ParentAccountCode = x.ParentAccountCode,
                                Status = x.Status,
                                Level = 0,
                                ChildCount = data.Where(y => y.ParentAccountCode == x.AccountCode1).Count()
                            }).ToList();

                    for (int i = 0; i < TreeData.Count; i++)
                    {
                        foreach (AccountCode sd in data.Where(x => x.ParentAccountCode == TreeData[i].AccountCode).OrderByDescending(x => x.AccountCode1))
                        {
                            AccountCodeTreeHelper objSH = new AccountCodeTreeHelper()
                            {
                                AccountCode = sd.AccountCode1,
                                AccountDesc = sd.AccountDesc,
                                Keterangan = sd.Keterangan,
                                Pengiraan = sd.Pengiraan,
                                ParentAccountCode = sd.ParentAccountCode,
                                Status = sd.Status,
                                Level = TreeData[i].Level + 1,
                                ChildCount = data.Where(y => y.ParentAccountCode == sd.AccountCode1).Count()
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
                ds.Tables.Add(GetProjekDataTable());
                string filename = Session["PrefixAcountCode"].ToString().Substring(0, Session["PrefixAcountCode"].ToString().Length - 1);
                new ReportHelper().ToExcel(ds, "BudgetProjek_" + filename + ".xls", ref Response);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Failure", ex.ToString());
            }
        }

        private DataTable GetProjekDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                List<AccountCodeTreeHelper> accountcodes = CreateExportData();
                List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];
                List<BudgetProjek> BudgetData = ((List<BudgetProjek>)Session["BudgetData"]).Where(x => x.Status == "A").ToList();

                //Start Build DataTable
                DataColumn dc = new DataColumn("AccountCode");
                dt.Columns.Add(dc);
                dc = new DataColumn("Objeck");
                dt.Columns.Add(dc);
                dc = new DataColumn("Description");
                dt.Columns.Add(dc);
                if (chkKeterangan.Checked)
                {
                    dc = new DataColumn("Keterangan");
                    dt.Columns.Add(dc);
                }
                if (chkPengiraan.Checked)
                {
                    dc = new DataColumn("Pengiraan");
                    dt.Columns.Add(dc);
                }
                foreach (PeriodMenguru pm in PeriodData)
                {
                    int count = dt.Columns.Cast<DataColumn>().Where(x => x.ColumnName == pm.MengurusYear.ToString() + "_" + pm.FieldMenguru.FieldMengurusDesc).Count();
                    string colname = pm.MengurusYear.ToString() + "_" + pm.FieldMenguru.FieldMengurusDesc;
                    colname = (count == 0) ? colname : colname + "_" + (count + 1).ToString();
                    dc = new DataColumn(colname);
                    dt.Columns.Add(dc);
                }
                //End Build DataTable

                //Start pushing data into DataTable
                foreach (AccountCodeTreeHelper acchelp in accountcodes)
                {
                    int c = 0;
                    DataRow dr = dt.NewRow();

                    dr[c] = new Helper().GetLevelString(acchelp.AccountCode, acchelp.Level);
                    c++;
                    dr[c] = Session["PrefixAcountCode"].ToString() + acchelp.AccountCode;
                    c++;
                    dr[c] = acchelp.AccountDesc;
                    if (chkKeterangan.Checked)
                    {
                        c++;
                        dr[c] = acchelp.Keterangan;
                    }
                    if (chkPengiraan.Checked)
                    {
                        c++;
                        dr[c] = acchelp.Pengiraan;
                    }

                    foreach (PeriodMenguru pm in PeriodData)
                    {
                        BudgetProjek ObjBudgetProjek = BudgetData.Where(x => x.AccountCode == acchelp.AccountCode && x.PeriodMengurusID == pm.PeriodMengurusID).FirstOrDefault();
                        decimal amount = Convert.ToDecimal(0);
                        if (acchelp.ChildCount == 0)
                        {
                            amount = (ObjBudgetProjek != null) ? ObjBudgetProjek.Amount : Convert.ToDecimal(0);
                        }
                        else
                        {
                            List<string> ChildIDs = new List<string>() { acchelp.AccountCode };
                            List<string> RefChildIDs = new List<string>();
                            while (ChildIDs.Count > 0)
                            {
                                RefChildIDs.Clear();
                                foreach (AccountCodeTreeHelper t in accountcodes.Where(x => ChildIDs.Contains(x.AccountCode)))
                                {
                                    amount = amount + BudgetData.Where(x => x.AccountCode == t.AccountCode && x.PeriodMengurusID == pm.PeriodMengurusID).Select(x => x.Amount).Sum();
                                    foreach (string s in accountcodes.Where(x => x.ParentAccountCode == t.AccountCode).Select(x => x.AccountCode).ToList())
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

        protected void btnMengurus_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SummaryMengurus.aspx");
        }

        protected void btnPerjawatan_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SummaryPerjawatan.aspx");
        }
    }
}