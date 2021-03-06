﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBBAL;
using OBDAL;
using BudgetPrep.Classes;
using System.Data;

namespace BudgetPrep
{
    public partial class StatusPerjawatan : System.Web.UI.Page
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
                    ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Perjawatan";
                    LoadDropDown();
                    GetData();
                    CreateTreeData();
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
                List<GroupPerjawatan> GroupPerjawatansData = new List<GroupPerjawatan>();

                if (AuthUser.JuncUserRoles.FirstOrDefault().RoleID == 1 || AuthUser.JuncUserRoles.FirstOrDefault().RoleID == 5)
                    GroupPerjawatansData = lstGroupPerjawatan;
                else
                {
                    GroupPerjawatansData = AuthUser.UserPerjawatanWorkflows.Where(x => x.Status == "A").Select(x => x.GroupPerjawatan).ToList();
                    if (GroupPerjawatansData.Count == 0)
                    {
                        lstGroupPerjawatan = new List<GroupPerjawatan>();
                    }
                    else
                    {
                        List<string> lstprntcodes = GroupPerjawatansData.Select(x => x.ParentGroupPerjawatanID).Distinct().ToList();
                        while (lstprntcodes.Count > 0)
                        {
                            List<GroupPerjawatan> lstprnts = lstGroupPerjawatan.Where(x => lstprntcodes.Contains(x.GroupPerjawatanCode)).ToList();
                            foreach (GroupPerjawatan o in lstprnts)
                                if (GroupPerjawatansData.Where(x => x.GroupPerjawatanCode == o.GroupPerjawatanCode).Count() == 0)
                                    GroupPerjawatansData.Add(o);
                            lstprntcodes = lstprnts.Select(x => x.ParentGroupPerjawatanID).Distinct().ToList();
                        }
                    }
                }
                Session["GroupPerjawatansData"] = lstGroupPerjawatan;

                List<int> lstperiod = GetSelectedPeriods();
                List<PeriodPerjawatan> PeriodData = new PeriodPerjawatanBAL().GetPeriodPerjawatans().Where(x => x.Status == "A" && lstperiod.Contains(x.PeriodPerjawatanID))
                    .OrderBy(x => x.PerjawatanYear).ThenBy(x => x.FieldPerjawatan.FieldPerjawatanDesc).ToList();

                List<PeriodPerjawatan> FixedData = ((List<FieldPerjawatan>)Session["FixedFieldPerjawatan"]).Where(x => lstperiod.Contains(x.FieldPerjawatanID))
                  .Select(x => new PeriodPerjawatan
                  {
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
                List<BudgetPerjawatan> BudgetData = new BudgetPerjawatanBAL().GetBudgetPerjawatansStatus(GetSelectedSegmentDetails(), ref CanEdit)
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

                //if (!CanEdit)
                //{
                //    chkKeterangan.Checked = false;
                //    chkPengiraan.Checked = false;
                //}
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

            Session["SegDtlsDictionary"] = DicSegDtls;
            return DicSegDtls;
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
                gvGroupPerjawatans.Columns.Clear();
                List<PeriodPerjawatan> PeriodData = (List<PeriodPerjawatan>)Session["PeriodData"];
                List<BudgetPerjawatan> BudgetData = (List<BudgetPerjawatan>)Session["BudgetData"];
                List<GroupPerjawatan> GroupPerjawatansData = (List<GroupPerjawatan>)Session["GroupPerjawatansData"];
                List<string> parentcodes = GroupPerjawatansData.Select(x => x.ParentGroupPerjawatanID).Distinct().ToList();
                List<string> ChildCodes = GroupPerjawatansData.Where(x => !parentcodes.Contains(x.GroupPerjawatanCode)).Select(x => x.GroupPerjawatanCode).ToList();

                TemplateField templateField1 = new TemplateField();
                templateField1.HeaderText = "Objek";
                templateField1.ItemTemplate = new GridViewCustomTemplate(0, "", 0);
                templateField1.FooterText = "Jumlah";
                gvGroupPerjawatans.Columns.Add(templateField1);

                BoundField bf1 = new BoundField();
                bf1.HeaderText = "Description";
                bf1.DataField = "GroupPerjawatanDesc";
                gvGroupPerjawatans.Columns.Add(bf1);

                //if (chkKeterangan.Checked)
                //{
                //    BoundField bf2 = new BoundField();
                //    bf2.HeaderText = "Keterangan";
                //    bf2.DataField = "Keterangan";
                //    gvGroupPerjawatans.Columns.Add(bf2);
                //}

                //if (chkPengiraan.Checked)
                //{
                //    BoundField bf2 = new BoundField();
                //    bf2.HeaderText = "Pengiraan";
                //    bf2.DataField = "Pengiraan";
                //    gvGroupPerjawatans.Columns.Add(bf2);
                //}

                foreach (PeriodPerjawatan pm in PeriodData)
                {
                    TemplateField templateField = new TemplateField();
                    templateField.HeaderText = pm.PerjawatanYear.ToString() + " " + pm.FieldPerjawatan.FieldPerjawatanDesc;
                    GridViewStatusCustomTemplate objTemp = new GridViewStatusCustomTemplate(0, pm.PerjawatanYear.ToString() + " " + pm.FieldPerjawatan.FieldPerjawatanDesc, pm.PeriodPerjawatanID);
                    objTemp.OnCustomStatusClicked += new CustomStatusClickedEventHandler(objTemp_OnCustomStatusClicked);
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
                    //    //if (rowItem.ChildCount > 0)
                    //    //    ((TextBox)e.Row.Cells[index].Controls[0]).Visible = false;
                    //    index++;
                    //}
                    //if (chkPengiraan.Checked)
                    //{
                    //    //if (rowItem.ChildCount > 0)
                    //    //    ((TextBox)e.Row.Cells[index].Controls[0]).Visible = false;
                    //    index++;
                    //}

                    bool IsBudgetEditable = Convert.ToBoolean(Session["CanEdit"]);
                    for (int c = index; c < gvGroupPerjawatans.Columns.Count; c++)
                    {
                        //int PeriodPerjawatanID = Convert.ToInt32(((Label)e.Row.Cells[c].FindControl("lbl_PeriodPerjawatanID")).Text);
                        int PeriodPerjawatanID = Convert.ToInt32(((Label)e.Row.Cells[c].Controls[0]).Text);
                        PeriodPerjawatan pm = PeriodData.Where(x => x.PeriodPerjawatanID == PeriodPerjawatanID).FirstOrDefault();
                        BudgetPerjawatan ObjBudgetPerjawatan = BudgetData.Where(x => x.GroupPerjawatanCode == rowItem.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID).FirstOrDefault();

                        Label lbl = ((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID));
                        LinkButton btnSaved = ((LinkButton)e.Row.Cells[c].FindControl("btnSaved_" + PeriodPerjawatanID));
                        LinkButton btnPrepared = ((LinkButton)e.Row.Cells[c].FindControl("btnPrepared_" + PeriodPerjawatanID));
                        LinkButton btnReviewed = ((LinkButton)e.Row.Cells[c].FindControl("btnReviewed_" + PeriodPerjawatanID));
                        LinkButton btnApproved = ((LinkButton)e.Row.Cells[c].FindControl("btnApproved_" + PeriodPerjawatanID));
                        LinkButton btnRevRej = ((LinkButton)e.Row.Cells[c].FindControl("btnRevRej_" + PeriodPerjawatanID));
                        LinkButton btnAprRej = ((LinkButton)e.Row.Cells[c].FindControl("btnAprRej_" + PeriodPerjawatanID));


                        if (rowItem.ChildCount == 0 && IsBudgetEditable)
                        {
                            lbl.Text = (ObjBudgetPerjawatan != null) ? ObjBudgetPerjawatan.Amount.ToString() : Convert.ToDecimal(0).ToString("F");

                            lbl.Visible = true;
                            btnSaved.Visible = false;
                            btnPrepared.Visible = false;
                            btnReviewed.Visible = false;
                            btnApproved.Visible = false;
                            btnRevRej.Visible = false;
                            btnAprRej.Visible = false;

                            e.Row.Cells[c].BackColor = (ObjBudgetPerjawatan != null) ? new Helper().GetColorByStatusValue(Convert.ToChar(ObjBudgetPerjawatan.Status)) : System.Drawing.Color.White;
                        }
                        else
                        {
                            int Scnt = 0;
                            int Pcnt = 0;
                            int Rcnt = 0;
                            int Acnt = 0;
                            int RRcnt = 0;
                            int ARcnt = 0;

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
                                    if (IsBudgetEditable)
                                    {
                                        Scnt = Scnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "S").Select(x => x.BudgetPerjawatanID).Count();

                                        Pcnt = Pcnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "P").Select(x => x.BudgetPerjawatanID).Count();

                                        Rcnt = Rcnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "R").Select(x => x.BudgetPerjawatanID).Count();

                                        Acnt = Acnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "A").Select(x => x.BudgetPerjawatanID).Count();

                                        RRcnt = RRcnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "X").Select(x => x.BudgetPerjawatanID).Count();

                                        ARcnt = ARcnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "Y").Select(x => x.BudgetPerjawatanID).Count();
                                    }
                                    else
                                    {
                                        Scnt = Scnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "S").Select(x => x.BudgetPerjawatanID).Sum();

                                        Pcnt = Pcnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "P").Select(x => x.BudgetPerjawatanID).Sum();

                                        Rcnt = Rcnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "R").Select(x => x.BudgetPerjawatanID).Sum();

                                        Acnt = Acnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "A").Select(x => x.BudgetPerjawatanID).Sum();

                                        RRcnt = RRcnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "X").Select(x => x.BudgetPerjawatanID).Sum();

                                        ARcnt = ARcnt + BudgetData.Where(x => x.GroupPerjawatanCode == t.GroupPerjawatanCode && x.PeriodPerjawatanID == PeriodPerjawatanID
                                            && x.Status == "Y").Select(x => x.BudgetPerjawatanID).Sum();
                                    }
                                }
                                ChildIDs.Clear();
                                foreach (string s in RefChildIDs)
                                    ChildIDs.Add(s);
                                //ChildIDs = TreeData.Where(x => ChildIDs.Contains(x.ParentGroupPerjawatanID) && x.ChildCount > 0).Select(x => x.GroupPerjawatan).ToList();
                            }

                            string Sstr = (Scnt > 0) ? "<span class=\"badge\">" + Scnt.ToString() + "</span>" : string.Empty;
                            string Pstr = (Pcnt > 0) ? "<span class=\"badge\">" + Pcnt.ToString() + "</span>" : string.Empty;
                            string Rstr = (Rcnt > 0) ? "<span class=\"badge\">" + Rcnt.ToString() + "</span>" : string.Empty;
                            string Astr = (Acnt > 0) ? "<span class=\"badge\">" + Acnt.ToString() + "</span>" : string.Empty;
                            string RRstr = (RRcnt > 0) ? "<span class=\"badge\">" + RRcnt.ToString() + "</span>" : string.Empty;
                            string ARstr = (ARcnt > 0) ? "<span class=\"badge\">" + ARcnt.ToString() + "</span>" : string.Empty;

                            btnSaved.Text = Sstr;
                            btnPrepared.Text = Pstr;
                            btnReviewed.Text = Rstr;
                            btnApproved.Text = Astr;
                            btnRevRej.Text = RRstr;
                            btnAprRej.Text = ARstr;

                            lbl.Visible = false;
                            btnSaved.Visible = (Sstr != string.Empty);
                            btnPrepared.Visible = (Pstr != string.Empty);
                            btnReviewed.Visible = (Rstr != string.Empty);
                            btnApproved.Visible = (Astr != string.Empty);
                            btnRevRej.Visible = (RRstr != string.Empty);
                            btnAprRej.Visible = (ARstr != string.Empty);

                            //string Sstr = (Scnt > 0) ? "<button type=\"button\" class=\"btn btn-primary btn-sm\" style=\"background-color:" + new Helper().GetColorByStatusValue('S').Name + ";\"><span class=\"badge\">" + Scnt.ToString() + "</span></button>" : string.Empty;
                            //string Pstr = (Pcnt > 0) ? "<button type=\"button\" class=\"btn btn-primary btn-sm\" style=\"background-color:" + new Helper().GetColorByStatusValue('P').Name + ";\"><span class=\"badge\">" + Pcnt.ToString() + "</span></button>" : string.Empty;
                            //string Rstr = (Rcnt > 0) ? "<button type=\"button\" class=\"btn btn-primary btn-sm\" style=\"background-color:" + new Helper().GetColorByStatusValue('R').Name + ";\"><span class=\"badge\">" + Rcnt.ToString() + "</span></button>" : string.Empty;
                            //string Astr = (Acnt > 0) ? "<button type=\"button\" class=\"btn btn-primary btn-sm\" style=\"background-color:" + new Helper().GetColorByStatusValue('A').Name + ";\"><span class=\"badge\">" + Acnt.ToString() + "</span></button>" : string.Empty;
                            //string RRstr = (RRcnt > 0) ? "<button type=\"button\" class=\"btn btn-primary btn-sm\" style=\"background-color:" + new Helper().GetColorByStatusValue('X').Name + ";\"><span class=\"badge\">" + RRcnt.ToString() + "</span></button>" : string.Empty;
                            //string ARstr = (ARcnt > 0) ? "<button type=\"button\" class=\"btn btn-primary btn-sm\" style=\"background-color:" + new Helper().GetColorByStatusValue('Y').Name + ";\"><span class=\"badge\">" + ARcnt.ToString() + "</span></button>" : string.Empty;

                            //string strBadge = Sstr + Pstr + Rstr + Astr + RRstr + ARstr;
                            //((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Text = strBadge;
                            //((Label)e.Row.Cells[c].FindControl("lbl_" + PeriodPerjawatanID)).Visible = true;
                            //((TextBox)e.Row.Cells[c].FindControl("tb_" + PeriodPerjawatanID)).Visible = false;
                            //((Button)e.Row.Cells[c].FindControl("btn_" + PeriodPerjawatanID)).Visible = false;
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
                    string strGroupPerjawatan = gvGroupPerjawatans.DataKeys[selectedRow.RowIndex]["GroupPerjawatanCode"].ToString();
                    List<string> SelectedNodes = (List<string>)Session["SelectedNodes"];
                    if (!SelectedNodes.Contains(strGroupPerjawatan))
                    {
                        if (TreeData.Where(x => x.GroupPerjawatanCode == strGroupPerjawatan).FirstOrDefault().ChildCount > 0)
                            SelectedNodes.Add(strGroupPerjawatan);
                    }
                    else
                    {
                        SelectedNodes.Remove(strGroupPerjawatan);
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

        protected void objTemp_OnCustomStatusClicked(object sender, CustomEvenArgs e)
        {
            try
            {
                lblDecisionModalGroupPerjawatanCode.Text = "Account Code : " + e.Code;
                lblDecisionModalPeriod.Text = "Period : " + e.Period;

                LinkButton btn = (LinkButton)sender;
                string status = (btn.ID == "btnSaved_" + e.PeriodID.ToString()) ? "S" :
                                    ((btn.ID == "btnPrepared_" + e.PeriodID.ToString()) ? "P" :
                                        ((btn.ID == "btnReviewed_" + e.PeriodID.ToString()) ? "R" :
                                            ((btn.ID == "btnApproved_" + e.PeriodID.ToString()) ? "A" :
                                                ((btn.ID == "btnRevRej_" + e.PeriodID.ToString()) ? "X" :
                                                    ((btn.ID == "btnAprRej_" + e.PeriodID.ToString()) ? "Y" : string.Empty)))));

                ColorHeader.Style.Add("background-color", new Helper().GetColorByStatusValue(Convert.ToChar(status)).Name);
                //ColorHeader.Style.Add("width", "100%");
                ColorHeader.InnerText = "Budget Perjawatan - " + new Helper().GetBudgetStatusEnumName(Convert.ToChar(status));

                //Grid
                List<SegmentDetail> segdtls = new SegmentDetailsBAL().GetSegmentDetails().ToList();

                int seglength = new SegmentBAL().GetSegments().Where(x => x.Status == "A").Count();

                List<int> segdtlids = new List<int>();
                foreach (KeyValuePair<int, int> pair in (Dictionary<int, int>)Session["SegDtlsDictionary"])
                {
                    List<int> parntsegdtlids = new List<int>();
                    parntsegdtlids = segdtls.Select(x => Convert.ToInt32(x.ParentDetailID)).Distinct().ToList();
                    if (pair.Value == 0)
                    {
                        foreach (int i in segdtls.Where(x => !parntsegdtlids.Contains(x.SegmentDetailID) && x.Status == "A" && x.SegmentID == pair.Key)
                                        .Select(x => x.SegmentDetailID).ToList())
                            segdtlids.Add(i);
                    }
                    else
                    {
                        if (!parntsegdtlids.Contains(pair.Value))
                            segdtlids.Add(pair.Value);
                        else
                        {
                            List<SegmentDetail> lstsubdtl = segdtls.Where(x => x.ParentDetailID == pair.Value).ToList();
                            for (int i = 0; i < lstsubdtl.Count; i++)
                            {
                                if (!parntsegdtlids.Contains(lstsubdtl[i].SegmentDetailID))
                                    segdtlids.Add(lstsubdtl[i].SegmentDetailID);
                                else
                                {
                                    foreach (SegmentDetail o in segdtls.Where(x => x.ParentDetailID == lstsubdtl[i].SegmentDetailID))
                                        lstsubdtl.Add(o);
                                }
                            }
                        }
                    }
                }

                IQueryable<GroupPerjawatan> lstacccode = new GroupPerjawatanDAL().GetGroupPerjawatans();
                List<string> accntcodes = new List<string>();
                List<string> parntacntcodes = lstacccode.Select(x => x.ParentGroupPerjawatanID).Distinct().ToList();
                if (!parntacntcodes.Contains(e.Code))
                    accntcodes.Add(e.Code);
                else
                {
                    List<GroupPerjawatan> lstsubacccode = lstacccode.Where(x => x.ParentGroupPerjawatanID == e.Code).ToList();
                    for (int i = 0; i < lstsubacccode.Count; i++)
                    {
                        if (!parntacntcodes.Contains(lstsubacccode[i].GroupPerjawatanCode))
                            accntcodes.Add(lstsubacccode[i].GroupPerjawatanCode);
                        else
                        {
                            string ac = lstsubacccode[i].GroupPerjawatanCode;
                            foreach (GroupPerjawatan o in lstacccode.Where(x => x.ParentGroupPerjawatanID == ac))
                                lstsubacccode.Add(o);
                        }
                    }
                }

                List<int> budids = new BudgetPerjawatanBAL().GetPerjawatanSegDtls()
                    .Where(x => segdtlids.Contains(x.SegmentDetailID))
                    .GroupBy(x => new
                    {
                        x.BudgetPerjawatanID
                    })
                    .Select(x => new
                    {
                        BudgetPerjawatanID = x.Key.BudgetPerjawatanID,
                        JuncBgtPerjawatanSegDtlID = x.Count()
                    })
                    .Where(x => x.JuncBgtPerjawatanSegDtlID == seglength)
                    .Select(x => x.BudgetPerjawatanID).ToList();

                List<BudgetPerjawatan> budData = new BudgetPerjawatanBAL().GetBudgetPerjawatans()
                    .Where(x => x.Status == status && accntcodes.Contains(x.GroupPerjawatanCode) && x.PeriodPerjawatanID == e.PeriodID && budids.Contains(x.BudgetPerjawatanID)).ToList();

                budids = budData.Select(y => y.BudgetPerjawatanID).ToList();
                var jundata = new BudgetPerjawatanBAL().GetPerjawatanSegDtls()
                    .AsEnumerable()
                    .Where(x => budids.Contains(x.BudgetPerjawatanID))
                    .OrderBy(x => x.BudgetPerjawatanID)
                    .ThenBy(x => x.SegmentDetail.Segment.SegmentOrder)
                    .GroupBy(x => new
                    {
                        x.BudgetPerjawatanID
                    })
                    .Select(x => new
                    {
                        BudgetPerjawatanID = x.Key.BudgetPerjawatanID,
                        //GroupPerjawatan = String.Join("-", x.Select(y => y.SegmentDetail.DetailCode)) 
                        GroupPerjawatan = x.Select(y => y.SegmentDetail.DetailCode).Aggregate((a, b) => a + "-" + b)
                        //GroupPerjawatan = x.Aggregate(string.Empty, (a, b) => a + "," + i.text)
                    })
                    .ToList();



                if (status == "S" || status == "P" || status == "R" || status == "A")
                {
                    gvModelDetails.DataSource =
                        (from x in budData
                         join y in jundata on x.BudgetPerjawatanID equals y.BudgetPerjawatanID
                         join z in new UserBAL().GetUsers() on x.ModifiedBy equals z.UserID
                         select new
                         {
                             GroupPerjawatan = y.GroupPerjawatan + "-" + x.GroupPerjawatanCode,
                             Amount = x.Amount,
                             Username = z.UserName,
                             Email = z.UserEmail,
                             Contact = z.PhoneNO,
                             DateTime = x.ModifiedTimeStamp
                         }).ToList();
                }
                else
                {
                    gvModelDetails.DataSource =
                        (from x in budData
                         join y in jundata on x.BudgetPerjawatanID equals y.BudgetPerjawatanID
                         join z in new UserBAL().GetUsers() on x.ModifiedBy equals z.UserID
                         select new
                         {
                             GroupPerjawatan = y.GroupPerjawatan + "-" + x.GroupPerjawatan,
                             Amount = x.Amount,
                             Remarks = x.Remarks,
                             Username = z.UserName,
                             Email = z.UserEmail,
                             Contact = z.PhoneNO,
                             DateTime = x.ModifiedTimeStamp
                         }).ToList();
                }
                gvModelDetails.DataBind();

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
                        lst.Add(Convert.ToInt32(gvPeriod.DataKeys[gvr.RowIndex]["PeriodPerjawatanID"].ToString()));
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

                //chkKeterangan.Checked = (CanEdit) ? chkKeterangan.Checked : false;
                //chkPengiraan.Checked = (CanEdit) ? chkPengiraan.Checked : false;
                //chkKeterangan.Enabled = CanEdit;
                //chkPengiraan.Enabled = CanEdit;
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
                ds.Tables.Add(GetPerjawatanDataTable());
                string filename = Session["PrefixAcountCode"].ToString().Substring(0, Session["PrefixAcountCode"].ToString().Length - 1);
                new ReportHelper().ToExcel(ds, "BudgetPerjawatan_" + filename + ".xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        private DataTable GetPerjawatanDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                List<GroupPerjawatanTreeHelper> GroupPerjawatans = CreateExportData();
                List<PeriodPerjawatan> PeriodData = (List<PeriodPerjawatan>)Session["PeriodData"];
                List<BudgetPerjawatan> BudgetData = ((List<BudgetPerjawatan>)Session["BudgetData"]).Where(x => x.Status == "A").ToList();

                //Start Build DataTable
                DataColumn dc = new DataColumn("GroupPerjawatan");
                dt.Columns.Add(dc);
                dc = new DataColumn("Objeck");
                dt.Columns.Add(dc);
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

                    dr[c] = new Helper().GetLevelString(acchelp.GroupPerjawatanCode, acchelp.Level);
                    c++;
                    dr[c] = Session["PrefixAcountCode"].ToString() + acchelp.GroupPerjawatanCode;
                    c++;
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

        protected void btnMengurus_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/StatusMengurus.aspx");
        }

        protected void btnProjek_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/StatusProjek.aspx");
        }
    }
}