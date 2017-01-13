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
    public partial class BudgetMengurusSetup : PageHelper
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
                        :(IsReviewer ? "<span><i class=\"fa fa-upload txt-success\"></i></span>Review" : "<span><i class=\"fa fa-upload txt-success\"></i></span>Approve");
                btnSubmit.InnerHtml = innerstr;

                ddlBulkDecision.SelectedIndex = -1;
                ddlBulkDecision.SelectedIndex = 0;
                ddlBulkDecision.Enabled = !IsPreparer;

                if (!Page.IsPostBack)
                {
                    Session["SelectedNodes"] = null;
                    ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Budget";
                    ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Mengurus";

                    Page.Form.Attributes.Add("enctype", "multipart/form-data");
                }
                else
                {
                    //AccountCodesData = (Session["AccountCodesData"] != null) ? (List<AccountCode>)Session["AccountCodesData"] : new List<AccountCode>();
                    //PeriodData = (Session["PeriodData"] != null) ? (List<PeriodMenguru>)Session["PeriodData"] : new List<PeriodMenguru>();
                    //BudgetData = (Session["BudgetData"] != null) ? (List<BudgetMenguru>)Session["BudgetData"] : new List<BudgetMenguru>();
                    //SelectedNodes = (Session["SelectedNodes"] != null) ? (List<string>)Session["SelectedNodes"] : new List<string>();
                    //TreeData = (Session["AccountCodesTree"] != null) ? (List<AccountCodeTreeHelper>)Session["AccountCodesTree"] : new List<AccountCodeTreeHelper>();
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
                List<AccountCode> lstAccountCode = new AccountCodeBAL().GetAccountCodes().Where(x => x.Status == "A").ToList();
                List<AccountCode> AccountCodesData = AuthUser.UserMengurusWorkflows.Where(x => x.Status == "A").Select(x => x.AccountCode1).ToList();
                List<string> lstprntcodes = AccountCodesData.Select(x => x.ParentAccountCode).Distinct().ToList();
                while (lstprntcodes.Count > 0)
                {
                    List<AccountCode> lstprnts = lstAccountCode.Where(x => lstprntcodes.Contains(x.AccountCode1)).ToList();
                    foreach (AccountCode o in lstprnts)
                        if (AccountCodesData.Where(x => x.AccountCode1 == o.AccountCode1).Count() == 0)
                            AccountCodesData.Add(o);
                    lstprntcodes = lstprnts.Select(x => x.ParentAccountCode).Distinct().ToList();
                }
                Session["AccountCodesData"] = AccountCodesData.OrderBy(x=>x.AccountCode1).ToList();

                List<int> lstperiod = GetSelectedPeriods();
                List<PeriodMenguru> PeriodData = new PeriodMengurusBAL().GetPeriodMengurus().Where(x => x.Status == "A" && lstperiod.Contains(x.PeriodMengurusID))
                    .OrderBy(x => x.MengurusYear).ThenBy(x => x.FieldMenguru.FieldMengurusDesc).ToList();

                List<PeriodMenguru> FixedData = ((List<FieldMenguru>)Session["FixedFieldMengurus"]).Where(x => lstperiod.Contains(x.FieldMengurusID))
                   .Select(x => new PeriodMenguru
                   {
                       //assign FieldMengurusID to PeriodMengurusID for use after. FieldMengurusID in PeriodMenguru will be 0 to indicated its fixed
                       PeriodMengurusID = lstperiod.Contains(x.FieldMengurusID) ? x.FieldMengurusID : 0,
                       MengurusYear = DateTime.Now.Year,
                       FieldMenguru = new FieldMenguru
                       {
                           FieldMengurusID = x.FieldMengurusID,
                           FieldMengurusDesc = x.FieldMengurusDesc,
                           Status = x.Status
                       }
                   }).OrderBy(x => x.FieldMengurusID).ToList();

                FixedData.AddRange(PeriodData);
                Session["PeriodData"] = FixedData;

                bool CanEdit = false;
                List<int> LstSegmentDetailIDs = GetSegDetals().Select(x => x.SegmentDetailID).ToList();
                List<BudgetMenguru> BudgetData = new BudgetMengurusBAL().GetBudgetMengurusWithTreeCalc(LstSegmentDetailIDs, ref CanEdit)
                //List<BudgetMenguru> BudgetData = new BudgetMengurusBAL().GetBudgetMengurus(LstSegmentDetailIDs)
                    .Select(x => new BudgetMenguru
                    {
                        BudgetMengurusID = x.BudgetMengurusID,
                        AccountCode = x.AccountCode,
                        PeriodMengurusID = x.PeriodMengurusID,
                        Status = x.Status,
                        Remarks = x.Remarks,
                        Amount = (AccountCodesData.Where(y => y.ParentAccountCode == x.AccountCode).Count() == 0) ? x.Amount : 0
                    }).ToList();
                Session["BudgetData"] = BudgetData;
                Session["CanEdit"] = CanEdit;

                List<int> BlockedYears = new BudgetMengurusBAL().GetBlockedMagurusYears();
                Session["BlockedYears"] = BlockedYears;

                List<int> OpenYears = new BudgetMengurusBAL().GetOpenMengurusYears();
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
                //for (int i = gvAccountCodes.Columns.Count - 1; i > 0; i--)
                //{
                //    gvAccountCodes.Columns.RemoveAt(i);
                //}
                gvAccountCodes.Columns.Clear();
                List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];
                List<BudgetMenguru> BudgetData = (List<BudgetMenguru>)Session["BudgetData"];
                List<AccountCode> AccountCodesData = (List<AccountCode>)Session["AccountCodesData"];
                List<string> parentcodes = AccountCodesData.Select(x => x.ParentAccountCode).Distinct().ToList();
                List<string> ChildCodes = AccountCodesData.Where(x => !parentcodes.Contains(x.AccountCode1)).Select(x => x.AccountCode1).ToList();

                TemplateField templateField1 = new TemplateField();
                templateField1.HeaderText = "Object";
                //templateField1.HeaderStyle.CssClass = "treecontainer";
                templateField1.ItemTemplate = new GridViewCustomTemplate(0, "", 0);
                templateField1.FooterText = "Total";
                gvAccountCodes.Columns.Add(templateField1);

                BoundField bf1 = new BoundField();
                bf1.HeaderText = "Description";
                bf1.DataField = "AccountDesc";
                gvAccountCodes.Columns.Add(bf1);

                if (chkKeterangan.Checked)
                {
                    //BoundField bf = new BoundField();
                    //bf.HeaderText = "Keterangan";
                    //bf.DataField = "Keterangan";
                    //gvAccountCodes.Columns.Add(bf);

                    TemplateField tf = new TemplateField();
                    tf.HeaderText = "Keterangan";
                    GridViewCustomTemplate objTemp = new GridViewCustomTemplate(2, "Keterangan", 0);
                    objTemp.OnCustomKetPenTextChanged += new CustomKetPenTextChangedEventHandler(objTemp_OnCustomKetPenTextChanged);
                    tf.ItemTemplate = objTemp;
                    gvAccountCodes.Columns.Add(tf);
                }
                if (chkPengiraan.Checked)
                {
                    //BoundField bf = new BoundField();
                    //bf.HeaderText = "Pengiraan";
                    //bf.DataField = "Pengiraan";
                    //gvAccountCodes.Columns.Add(bf);

                    TemplateField tf = new TemplateField();
                    tf.HeaderText = "Pengiraan";
                    GridViewCustomTemplate objTemp = new GridViewCustomTemplate(2, "Pengiraan", 0);
                    objTemp.OnCustomKetPenTextChanged += new CustomKetPenTextChangedEventHandler(objTemp_OnCustomKetPenTextChanged);
                    tf.ItemTemplate = objTemp;
                    gvAccountCodes.Columns.Add(tf);
                }

                foreach (PeriodMenguru pm in PeriodData)
                {
                    TemplateField templateField = new TemplateField();
                    templateField.HeaderText = pm.MengurusYear.ToString() + " " + pm.FieldMenguru.FieldMengurusDesc;
                    GridViewCustomTemplate objTemp = new GridViewCustomTemplate(1, pm.MengurusYear.ToString() + " " + pm.FieldMenguru.FieldMengurusDesc, pm.PeriodMengurusID);
                    objTemp.OnCustomTextChanged += new CustomTextChangedEventHandler(objTemp_OnCustomTextChanged);
                    objTemp.OnCustomClicked += new CustomClickedEventHandler(objTemp_OnCustomClicked);
                    templateField.ItemTemplate = objTemp;

                    PeriodMenguru objpm = new PeriodMengurusBAL().GetAllPeriodMengurus().Where(x => x.MengurusYear == DateTime.Now.Year).FirstOrDefault();
                    if (pm.PeriodMengurusID == Convert.ToInt32("1"))
                    {
                        if (objpm != null)
                        {
                            templateField.FooterText = BudgetData.Where(x => x.PeriodMengurusID == objpm.PeriodMengurusID && ChildCodes.Contains(x.AccountCode) 
                                && x.Status == "A").Select(x => x.Amount).Sum().ToString("#,##0.00");
                        }
                    }
                    else
                        templateField.FooterText = BudgetData.Where(x => x.PeriodMengurusID == pm.PeriodMengurusID && ChildCodes.Contains(x.AccountCode)).Select(x => x.Amount).Sum().ToString("#,##0.00");;
                                        
                    templateField.FooterStyle.HorizontalAlign = HorizontalAlign.Right;
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
                //((SiteMaster)this.Master).ChangeLanguage();

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
            //gvAccountCodes.Visible = true;
            //btnPrint.Visible = true;
            //btnSubmit.Visible = true;
            //btnSearchbox.Visible = true;
            BudgetBox.Visible = true;

            //SetBudgetCanEditable();
            GetPrefixAcountCode();
            GetData();

            bool IsBudgetEditable = Convert.ToBoolean(Session["CanEdit"]);
            if (!IsBudgetEditable)
            {
                chkKeterangan.Checked = false;
                chkPengiraan.Checked = false;
            }

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

        protected void gvAccountCodes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];
                    List<BudgetMenguru> BudgetData = (List<BudgetMenguru>)Session["BudgetData"];
                    List<AccountCodeTreeHelper> TreeData = (List<AccountCodeTreeHelper>)Session["AccountCodesTree"];
                    List<AccountCode> AccountCodesData = (List<AccountCode>)Session["AccountCodesData"];
                    List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];
                    List<int> BlockedYears = (List<int>)Session["BlockedYears"];
                    List<int> OpenYears = (List<int>)Session["OpenYears"];

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
                        if (rowItem.ChildCount > 0)
                            ((TextBox)e.Row.Cells[index].Controls[0]).Visible = false;
                        else
                            ((TextBox)e.Row.Cells[index].Controls[0]).Text = (rowItem.Keterangan != null) ? rowItem.Keterangan.Trim() : string.Empty;
                        index++;
                    }
                    if (chkPengiraan.Checked)
                    {
                        if (rowItem.ChildCount > 0)
                            ((TextBox)e.Row.Cells[index].Controls[0]).Visible = false;
                        else
                            ((TextBox)e.Row.Cells[index].Controls[0]).Text = (rowItem.Pengiraan != null) ? rowItem.Pengiraan.Trim() : string.Empty;
                        index++;
                    }

                    bool IsBudgetEditable = Convert.ToBoolean(Session["CanEdit"]);
                    for (int c = index; c < gvAccountCodes.Columns.Count; c++)
                    {
                        //int PeriodMenguruID = Convert.ToInt32(((Label)e.Row.Cells[c].FindControl("lbl_PeriodMenguruID")).Text);
                        int PeriodMenguruID = Convert.ToInt32(((Label)e.Row.Cells[c].Controls[0]).Text);
                        PeriodMenguru pm = PeriodData.Where(x => x.PeriodMengurusID == PeriodMenguruID).FirstOrDefault();
                        PeriodMenguru objpm = new PeriodMengurusBAL().GetAllPeriodMengurus().Where(x => x.MengurusYear == Convert.ToInt32(pm.MengurusYear)).FirstOrDefault();
                        BudgetMenguru ObjBudgetMenguru = BudgetData.Where(x => x.AccountCode == rowItem.AccountCode && x.PeriodMengurusID == PeriodMenguruID).FirstOrDefault();
                        
                        e.Row.Cells[c].BackColor = (ObjBudgetMenguru != null) ? new Helper().GetColorByStatusValue(Convert.ToChar(ObjBudgetMenguru.Status)) : System.Drawing.Color.White;
                        e.Row.Cells[c].Style.Add(HtmlTextWriterStyle.TextAlign, "right");

                        TextBox tb = ((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodMenguruID));
                        Label lbl = ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodMenguruID));
                        Button btn = ((Button)e.Row.Cells[c].FindControl("btn_" + PeriodMenguruID));
                        tb.CssClass = "ltor";

                        if (rowItem.ChildCount == 0 && IsBudgetEditable)
                        {

                            tb.Text = (ObjBudgetMenguru != null) ? ObjBudgetMenguru.Amount.ToString("#,##0.00") : Convert.ToDecimal(0).ToString("#,##0.00"); ;
                            //lbl.Text = (ObjBudgetMenguru != null) ? ObjBudgetMenguru.Amount.ToString("#,##0.00") : Convert.ToDecimal(0).ToString("#,##0.00"); ;
                            btn.Text = Server.HtmlDecode("&#9635;");

                            if (PeriodMenguruID == Convert.ToInt32("1"))
                            {
                                if (objpm != null)
                                {
                                    BudgetMenguru getamount = BudgetData.Where(x => x.AccountCode == rowItem.AccountCode && x.PeriodMengurusID == objpm.PeriodMengurusID
                                        && x.Status == "A").FirstOrDefault();

                                    lbl.Text = (getamount != null) ? getamount.Amount.ToString("#,##0.00") : Convert.ToDecimal(0).ToString("#,##0.00"); ;
                                }
                            }
                            else
                                lbl.Text = (ObjBudgetMenguru != null) ? ObjBudgetMenguru.Amount.ToString("#,##0.00") : Convert.ToDecimal(0).ToString("#,##0.00"); ;

                            if (IsPreparer)
                            {
                                if (ObjBudgetMenguru == null || ObjBudgetMenguru.Status == "O" || ObjBudgetMenguru.Status == "S" || ObjBudgetMenguru.Status == "X" || ObjBudgetMenguru.Status == "Y")
                                {
                                    tb.Visible = ((pm.MengurusYear > DateTime.Now.Year) && !BlockedYears.Contains(pm.MengurusYear) && OpenYears.Contains(pm.MengurusYear));
                                    lbl.Visible = !((pm.MengurusYear > DateTime.Now.Year) && !BlockedYears.Contains(pm.MengurusYear) && OpenYears.Contains(pm.MengurusYear));
                                    btn.Visible = false;
                                }
                                else if (ObjBudgetMenguru.Status == "P" || ObjBudgetMenguru.Status == "R" || ObjBudgetMenguru.Status == "A")
                                {
                                    tb.Visible = false;
                                    lbl.Visible = true;
                                    btn.Visible = false;
                                }
                            }
                            if (IsReviewer)
                            {
                                if (ObjBudgetMenguru == null || ObjBudgetMenguru.Status == "O" || ObjBudgetMenguru.Status == "S" || ObjBudgetMenguru.Status == "P" || ObjBudgetMenguru.Status == "X" || ObjBudgetMenguru.Status == "Y")
                                {
                                    tb.Visible = ((pm.MengurusYear > DateTime.Now.Year) && !BlockedYears.Contains(pm.MengurusYear) && OpenYears.Contains(pm.MengurusYear));
                                    lbl.Visible = !((pm.MengurusYear > DateTime.Now.Year) && !BlockedYears.Contains(pm.MengurusYear) && OpenYears.Contains(pm.MengurusYear));
                                    if (ObjBudgetMenguru != null && ObjBudgetMenguru.Status == "P")
                                        btn.Visible = true;
                                    else
                                        btn.Visible = false;
                                        //btn.Visible = true;
                                }
                                else if (ObjBudgetMenguru.Status == "R" || ObjBudgetMenguru.Status == "A")
                                {
                                    tb.Visible = false;
                                    lbl.Visible = true;
                                    btn.Visible = false;
                                }
                            }
                            if (IsApprover)
                            {
                                tb.Visible = false;
                                lbl.Visible = true;
                                if (ObjBudgetMenguru != null && ObjBudgetMenguru.Status == "R")
                                    btn.Visible = true;
                                else
                                    btn.Visible = false;
                            }
                            //tb.Text = (ObjBudgetMenguru != null) ? ObjBudgetMenguru.Amount.ToString() : (0.00).ToString();
                            //tb.Visible = true;
                            //lbl.Visible = false;

                            //btn.Text = Server.HtmlDecode("&#9635;");
                            //btn.Visible = true;
                        }
                        else
                        {
                            int cnt = 0;
                            decimal amount = 0;
                            
                            List<string> ChildIDs = new List<string>() { rowItem.AccountCode };
                            List<string> RefChildIDs = new List<string>();
                            while (ChildIDs.Count > 0)
                            {
                                RefChildIDs.Clear();
                                foreach (AccountCode t in AccountCodesData.Where(x => ChildIDs.Contains(x.AccountCode1)))  //&& x.ChildCount == 0
                                {
                                    if (PeriodMenguruID == Convert.ToInt32("1"))
                                    {
                                        if (objpm != null)
                                        {
                                            amount = amount + BudgetData.Where(x => x.AccountCode == t.AccountCode1 && x.PeriodMengurusID == objpm.PeriodMengurusID
                                                && x.Status == "A").Select(x => x.Amount).Sum();
                                        }
                                    }
                                    else
                                        amount = amount + BudgetData.Where(x => x.AccountCode == t.AccountCode1 && x.PeriodMengurusID == PeriodMenguruID).Select(x => x.Amount).Sum();
                                    
                                    foreach (string s in AccountCodesData.Where(x => x.ParentAccountCode == t.AccountCode1).Select(x => x.AccountCode1).ToList())
                                        RefChildIDs.Add(s);
                                    if (IsPreparer)
                                    {
                                        cnt = cnt + BudgetData.Where(x => x.AccountCode == t.AccountCode1 && x.PeriodMengurusID == PeriodMenguruID
                                            && (x.Status == "S" || x.Status == "X" || x.Status == "Y"))
                                            .Select(x => x.BudgetMengurusID).Count();
                                    }
                                    if (IsReviewer)
                                    {
                                        cnt = cnt + BudgetData.Where(x => x.AccountCode == t.AccountCode1 && x.PeriodMengurusID == PeriodMenguruID
                                            && (x.Status == "P" || x.Status == "X" || x.Status == "Y"))
                                            .Select(x => x.BudgetMengurusID).Count();
                                    }
                                    if (IsApprover)
                                    {
                                        cnt = cnt + BudgetData.Where(x => x.AccountCode == t.AccountCode1 && x.PeriodMengurusID == PeriodMenguruID 
                                            && x.Status == "R")
                                            .Select(x => x.BudgetMengurusID).Count();
                                    }
                                }
                                ChildIDs.Clear();
                                foreach (string s in RefChildIDs)
                                    ChildIDs.Add(s);
                                //ChildIDs = TreeData.Where(x => ChildIDs.Contains(x.ParentAccountCode) && x.ChildCount > 0).Select(x => x.AccountCode).ToList();
                            }

                            string strBadge = (IsPreparer) ? string.Empty : ((cnt == 0) ? string.Empty : " <span class=\"badge\">" + cnt.ToString() + "</span>");
                            lbl.Text = amount.ToString("#,##0.00") + strBadge;
                            lbl.Visible = true;
                            tb.Visible = false;
                            btn.Visible = false;
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

        protected void gvAccountCodes_RowCreated(object sender, GridViewRowEventArgs e) { }

        protected void objTemp_OnCustomTextChanged(object sender, CustomEvenArgs e)
        {
            try
            {
                string strAccountCode = e.Code;
                int PeriodMenguruID = e.PeriodID;
                decimal amount = e.Amount;
                string strstatus = string.Empty;

                List<BudgetMenguru> BudgetData = (List<BudgetMenguru>)Session["BudgetData"];

                //string strAccountCode = ((GridView)((TextBox)sender).Parent.Parent.Parent.Parent).DataKeys[((GridViewRow)((TextBox)sender).Parent.Parent).RowIndex][0].ToString();
                ////int PeriodMenguruID = Convert.ToInt32(((Label)(((TextBox)sender).Parent).FindControl("lbl_PeriodMenguruID")).Text);
                //int PeriodMenguruID = Convert.ToInt32(((Label)(((TextBox)sender).Parent).Controls[0]).Text);
                BudgetMenguru item = BudgetData.Where(x => x.AccountCode == strAccountCode && x.PeriodMengurusID == PeriodMenguruID).FirstOrDefault();

                if (item != null)
                {
                    strstatus = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                        : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);

                    //item.Amount = (((TextBox)sender).Text.Trim() == string.Empty) ? 0 : Convert.ToDecimal(((TextBox)sender).Text.Trim());
                    item.Amount = amount;
                    item.Status = strstatus;

                    BudgetMenguru newBudgetMenguru = new BudgetMenguru();
                    newBudgetMenguru.AccountCode = strAccountCode;
                    newBudgetMenguru.PeriodMengurusID = PeriodMenguruID;
                    newBudgetMenguru.Amount = amount;
                    newBudgetMenguru.Status = strstatus;
                    newBudgetMenguru.ModifiedBy = AuthUser.UserID;
                    newBudgetMenguru.ModifiedTimeStamp = DateTime.Now;

                    List<int> LstSegmentDetailIDs = ((List<JuncBgtMengurusSegDtl>)Session["ListSegmentDetails"]).Select(x => x.SegmentDetailID).ToList();
                    if(new BudgetMengurusBAL().UpdateBudgetMengurus(newBudgetMenguru, LstSegmentDetailIDs))
                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Budget");
                }
                else
                {
                    BudgetMenguru newBudgetMenguru = new BudgetMenguru();
                    newBudgetMenguru.AccountCode = strAccountCode;
                    newBudgetMenguru.PeriodMengurusID = PeriodMenguruID;
                    //newBudgetMenguru.Amount = (((TextBox)sender).Text.Trim() == string.Empty) ? 0 : Convert.ToDecimal(((TextBox)sender).Text.Trim());
                    newBudgetMenguru.Amount = amount;
                    newBudgetMenguru.Status = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                        : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);
                    newBudgetMenguru.CreatedBy = AuthUser.UserID;
                    newBudgetMenguru.CreatedTimeStamp = DateTime.Now;
                    newBudgetMenguru.ModifiedBy = AuthUser.UserID;
                    newBudgetMenguru.ModifiedTimeStamp = DateTime.Now;

                    BudgetData.Add(newBudgetMenguru);

                    List<JuncBgtMengurusSegDtl> lstBgtSegDtl = (List<JuncBgtMengurusSegDtl>)Session["ListSegmentDetails"];
                    foreach (JuncBgtMengurusSegDtl obj in lstBgtSegDtl)
                        obj.BudgetMenguru = newBudgetMenguru;

                    if(new BudgetMengurusBAL().InsertBudgetMengurus(newBudgetMenguru, lstBgtSegDtl))
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
                lblDecisionModalAccountCode.Text = e.Code;
                lblDecisionModalPeriodID.Text = e.PeriodID.ToString();
                lblDecisionModalPeriod.Text = "Period : " + e.Period;
                lblDecisionModalAmount.Text = "Amount : " + e.Amount.ToString("#,##0.00");

                rbldecision.SelectedIndex = 0;
                tbRemarks.Text = string.Empty;

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myDecisionModal", "$('#myDecisionModal').modal();", true);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void objTemp_OnCustomKetPenTextChanged(object sender, CustomKetPenEvenArgs e)
        {
            AccountCode objAccountCode = new AccountCodeBAL().GetAccountCodes().Where(x => x.AccountCode1.ToUpper().Trim() == e.AccountCode.ToUpper().Trim()).FirstOrDefault();
            bool isChanged = false;
            if (e.KetorPen == "Keterangan")
            {
                isChanged = (objAccountCode.Keterangan != ((TextBox)sender).Text);
                objAccountCode.Keterangan = ((TextBox)sender).Text;
            }
            else if (e.KetorPen == "Pengiraan")
            {
                isChanged = (objAccountCode.Pengiraan != ((TextBox)sender).Text);
                objAccountCode.Pengiraan = ((TextBox)sender).Text;
            }

            if (isChanged)
            {
                if (new AccountCodeBAL().UpdateAccountCode(objAccountCode))
                    ((SiteMaster)this.Master).ShowMessage("Success", e.KetorPen + " updated successfully");
                else
                    ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating " + e.KetorPen);
            }
        }

        private List<JuncBgtMengurusSegDtl> GetSegDetals()
        {
            List<JuncBgtMengurusSegDtl> lst = new List<JuncBgtMengurusSegDtl>();
            try
            {
                foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
                {
                    TreeView tv = (TreeView)gvr.Cells[0].FindControl("tvSegmentDDL");
                    if (tv != null)
                    {
                        JuncBgtMengurusSegDtl obj = new JuncBgtMengurusSegDtl();
                        try
                        {
                            obj.SegmentDetailID = Convert.ToInt32(tv.SelectedValue);
                        }
                        catch(Exception)
                        {
                            return (List<JuncBgtMengurusSegDtl>)Session["ListSegmentDetails"];
                        }
                        //obj.BudgetMenguru = BudgetMenguru;
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
                List<PeriodMenguru> lstPeriodMengurus = new PeriodMengurusBAL().GetPeriodMengurus().Where(x => x.Status == "A" && x.MengurusYear > DateTime.Now.Year)
                    .OrderBy(x => x.MengurusYear).ThenBy(x => x.FieldMenguru.FieldMengurusDesc).ToList();

                List<FieldMenguru> obj = new FieldMenguruBAL().GetFieldMengurus().Where(x => x.Status == "F").OrderBy(x => x.FieldMengurusID).ToList();
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
 
                    lstSD = AuthUser.UserSegDtlWorkflows.Where(x => x.Status == "A" && x.SegmentDetail.SegmentID == rowItem.SegmentID).Select(x => x.SegmentDetail).ToList();
                    List<int> parntids = lstSD.Select(x=>Convert.ToInt32(x.ParentDetailID)).Distinct().ToList();
                    
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

                chkKeterangan.Checked = (CanEdit) ? chkKeterangan.Checked : false;
                chkPengiraan.Checked = (CanEdit) ? chkPengiraan.Checked : false;
                chkKeterangan.Enabled = CanEdit;
                chkPengiraan.Enabled = CanEdit;
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
                string strAccountCode = lblDecisionModalAccountCode.Text;
                int PeriodMenguruID = Convert.ToInt32(lblDecisionModalPeriodID.Text);
                decimal amount = Convert.ToDecimal(lblDecisionModalAmount.Text.Replace("Amount : ", "").Trim());

                List<BudgetMenguru> BudgetData = (List<BudgetMenguru>)Session["BudgetData"];
                BudgetMenguru item = BudgetData.Where(x => x.AccountCode == strAccountCode && x.PeriodMengurusID == PeriodMenguruID).FirstOrDefault();
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

                    BudgetMenguru newBudgetMenguru = new BudgetMenguru();
                    newBudgetMenguru.AccountCode = strAccountCode;
                    newBudgetMenguru.PeriodMengurusID = PeriodMenguruID;
                    newBudgetMenguru.Amount = amount;
                    newBudgetMenguru.Status = strstatus;
                    newBudgetMenguru.Remarks = tbRemarks.Text;
                    newBudgetMenguru.ModifiedBy = AuthUser.UserID;
                    newBudgetMenguru.ModifiedTimeStamp = DateTime.Now;

                    List<int> LstSegmentDetailIDs = ((List<JuncBgtMengurusSegDtl>)Session["ListSegmentDetails"]).Select(x => x.SegmentDetailID).ToList();
                    //if (new BudgetMengurusBAL().UpdateBudgetMengurus(newBudgetMenguru, LstSegmentDetailIDs)) 
                    //    ((SiteMaster)this.Master).ShowMessage("Success", "Budget updated successfully");
                    //else
                    //    ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Budget");
                    new BudgetMengurusBAL().UpdateBudgetMengurus(newBudgetMenguru, LstSegmentDetailIDs);
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
                List<int> LstSegmentDetailIDs = ((List<JuncBgtMengurusSegDtl>)Session["ListSegmentDetails"]).Select(x => x.SegmentDetailID).ToList();
                List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];
                List<int> PeriodIDs = PeriodData.Where(x => x.MengurusYear > DateTime.Now.Year).Select(x => x.PeriodMengurusID).ToList();
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

                if (new BudgetMengurusBAL().UpdateMultipleBudgetMengurus(LstSegmentDetailIDs, PeriodIDs, FromChar, ToChar, tbBulkRemarks.Text.Trim(), LoggedInUser))
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

        protected void gvAccountCodes_PageIndexChanged(object sender, EventArgs e)
        {
            BuildGrid();
            BindGrid();
        }

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
                ds.Tables.Add(GetMengurusDataTable());
                string filename = Session["PrefixAcountCode"].ToString().Substring(0, Session["PrefixAcountCode"].ToString().Length - 1);
                new ReportHelper().ToExcel(ds, "BudgetMengurus_" + filename + ".xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        private DataTable GetMengurusDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                List<AccountCodeTreeHelper> accountcodes = CreateExportData();
                List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];
                List<BudgetMenguru> BudgetData = ((List<BudgetMenguru>)Session["BudgetData"]).ToList();  //.Where(x => x.Status == "A").ToList();
                List<string> PrefixDesc = (List<string>)Session["PrefixDesc"];

                //Start Build DataTable
                DataColumn dc = new DataColumn();
                foreach (GridViewRow gvr in gvSegmentDLLs.Rows)
                {
                    dc = new DataColumn(gvr.Cells[0].Text);
                    dt.Columns.Add(dc);
                }

                dc = new DataColumn("AccountCode");
                dt.Columns.Add(dc);
                //dc = new DataColumn("Objeck");
                //dt.Columns.Add(dc);
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

                    foreach (string s in PrefixDesc)
                    {
                        dr[c] = s;
                        c++;
                    }

                    dr[c] = new Helper().GetLevelString(acchelp.AccountCode, acchelp.Level);
                    c++;
                    //dr[c] = Session["PrefixAcountCode"].ToString() + acchelp.AccountCode;
                    //c++;
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
                        BudgetMenguru ObjBudgetMenguru = BudgetData.Where(x => x.AccountCode == acchelp.AccountCode && x.PeriodMengurusID == pm.PeriodMengurusID).FirstOrDefault();
                        decimal amount = Convert.ToDecimal(0);
                        if (acchelp.ChildCount == 0)
                        {
                            amount = (ObjBudgetMenguru != null) ? ObjBudgetMenguru.Amount : Convert.ToDecimal(0);
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
                        dr[c] = amount.ToString("#,##0.00");;
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
                ds.Tables.Add(BudgetMengurusImport());

                new ReportHelper().ToExcel(ds, "BudgetMengurusSample.xls", ref Response);
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected DataTable BudgetMengurusImport()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn();

            List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];

            dc = new DataColumn("AccountCode");
            dt.Columns.Add(dc);
            dc = new DataColumn("Description");
            dt.Columns.Add(dc);

            foreach (PeriodMenguru pm in PeriodData)
            {
                int count = dt.Columns.Cast<DataColumn>().Where(x => x.ColumnName == pm.MengurusYear.ToString() + "_" + pm.FieldMenguru.FieldMengurusDesc).Count();
                string colname = pm.MengurusYear.ToString() + "_" + pm.FieldMenguru.FieldMengurusDesc;
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
                        dt = new ReportHelper().ValidateBudgetImport<BudgetMengurusSetup>(ds, gvAccountCodes, ref lstErrors);

                        if (lstErrors.Count == 0)
                        {
                            List<AccountCode> AccountCodesData = ((List<AccountCode>)Session["AccountCodesData"]).Where(x =>
                                !String.IsNullOrEmpty(x.ParentAccountCode)).ToList();

                            foreach (DataRow row in dt.Rows)
                            {
                                if (AccountCodesData.Where(x => x.AccountCode1 == Convert.ToString(row["AccountCode"])).Select(y => y.AccountCode1).Count() > 0)
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

            List<PeriodMenguru> PeriodData = (List<PeriodMenguru>)Session["PeriodData"];
            List<BudgetMenguru> BudgetData = (List<BudgetMenguru>)Session["BudgetData"];
            List<int> BlockedYears = (List<int>)Session["BlockedYears"];
            List<int> OpenYears = (List<int>)Session["OpenYears"];
            List<string> ag = (List<string>)Session["BudgetCol"];

            foreach (string dipohon in ag)
            {
                if (dt.Table.Columns.Contains(dipohon))
                {
                    PeriodMenguru pd = PeriodData.Where(x => (x.MengurusYear.ToString() + " " + x.FieldMenguru.FieldMengurusDesc).Contains(dipohon)).FirstOrDefault();

                    if (pd != null)
                    {
                        List<BudgetMenguru> bp = BudgetData.Where(x => x.PeriodMengurusID == pd.PeriodMengurusID && x.AccountCode == Convert.ToString(dt["AccountCode"])).ToList();

                        if (bp.Count > 0)
                        {
                            if (bp.Where(s => s.Status == "P" || s.Status == "R" || s.Status == "A").Count() == 0)
                            {
                                if (!BlockedYears.Contains(pd.MengurusYear) && OpenYears.Contains(pd.MengurusYear))
                                {
                                    decimal total = 0;
                                    decimal amt = bp.Select(x => x.Amount).FirstOrDefault();

                                    total = Convert.ToDecimal(dt[dipohon]) + amt;

                                    UpdateAmountFromUploadFile(dipohon, Convert.ToString(dt["AccountCode"]), total);
                                    res = true;
                                }
                            }
                        }
                        else
                        {
                            if (!BlockedYears.Contains(pd.MengurusYear) && OpenYears.Contains(pd.MengurusYear))
                            {
                                UpdateAmountFromUploadFile(dipohon, Convert.ToString(dt["AccountCode"]), Convert.ToDecimal(dt[dipohon]));
                                res = true;
                            }
                        }
                    }
                }
            }

            return res;
        }

        public void UpdateAmountFromUploadFile(string ColumnName, string AccountCode, decimal amount)
        {
            for (int r = 0; r < gvAccountCodes.Rows.Count; r++)
            {
                for (int c = 0; c < gvAccountCodes.Columns.Count; c++)
                {
                    if (gvAccountCodes.Columns[c].HeaderText.Contains(ColumnName))
                    {
                        string val = (string)gvAccountCodes.DataKeys[r]["AccountCode"];
                        if (val == AccountCode)
                        {
                            int PeriodMenguruID = Convert.ToInt32(((Label)gvAccountCodes.Rows[r].Cells[c].Controls[0]).Text);

                            TextBox tb = ((TextBox)gvAccountCodes.Rows[r].Cells[c].FindControl("tb_" + PeriodMenguruID));
                            tb.Text = (amount > 0) ? amount.ToString("#,##0.00") : Convert.ToDecimal(0).ToString("#,##0.00");

                            try
                            {
                                //SaveUploadedFile(AccountCode, PeriodMenguruID, amount);
                                string strstatus = string.Empty;

                                BudgetMenguru newBudgetMenguru = new BudgetMenguru();
                                newBudgetMenguru.AccountCode = AccountCode;
                                newBudgetMenguru.PeriodMengurusID = PeriodMenguruID;
                                newBudgetMenguru.Amount = amount;

                                List<BudgetMenguru> BudgetData = ((List<BudgetMenguru>)Session["BudgetData"]).Where(x => x.AccountCode == AccountCode && x.PeriodMengurusID == PeriodMenguruID).ToList();

                                if (BudgetData.FirstOrDefault() != null)
                                {
                                    strstatus = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                                    : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);

                                    newBudgetMenguru.Status = strstatus;
                                    newBudgetMenguru.ModifiedBy = AuthUser.UserID;
                                    newBudgetMenguru.ModifiedTimeStamp = DateTime.Now;

                                    List<int> LstSegmentDetailIDs = ((List<JuncBgtMengurusSegDtl>)Session["ListSegmentDetails"]).Select(x => x.SegmentDetailID).ToList();
                                    if (new BudgetMengurusBAL().UpdateBudgetMengurus(newBudgetMenguru, LstSegmentDetailIDs))
                                        ((SiteMaster)this.Master).ShowMessage("Success", "Budget updated successfully");
                                    else
                                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating Budget");
                                }
                                else
                                {
                                    strstatus = (IsPreparer) ? new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Saved)
                                    : new Helper().GetBudgetItemStatusEnumValue(Helper.BudgetItemStatus.Reviewed);

                                    newBudgetMenguru.Status = strstatus;
                                    newBudgetMenguru.CreatedBy = AuthUser.UserID;
                                    newBudgetMenguru.CreatedTimeStamp = DateTime.Now;
                                    newBudgetMenguru.ModifiedBy = AuthUser.UserID;
                                    newBudgetMenguru.ModifiedTimeStamp = DateTime.Now;

                                    BudgetData.Add(newBudgetMenguru);

                                    List<JuncBgtMengurusSegDtl> lstBgtSegDtl = (List<JuncBgtMengurusSegDtl>)Session["ListSegmentDetails"];

                                    foreach (JuncBgtMengurusSegDtl obj in lstBgtSegDtl)
                                        obj.BudgetMenguru = newBudgetMenguru;

                                    if (new BudgetMengurusBAL().InsertBudgetMengurus(newBudgetMenguru, lstBgtSegDtl))
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