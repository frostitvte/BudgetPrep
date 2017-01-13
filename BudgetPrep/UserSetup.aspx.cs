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
    public class UserExport
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string UserEmail { get; set; }
        public string Role { get; set; }
        public string GroupName { get; set; }
        public string ICNO { get; set; }
        public string Title { get; set; }
        public string PositionGrade { get; set; }
        public string PhoneNO { get; set; }
        public string Fax { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string PeriodOfService { get; set; }
        public string OfficeAddress { get; set; }
        public string Status { get; set; }
        public string MengurusWorkflow { get; set; }
        public string PerjawatanWorkflow { get; set; }
    }

    public partial class UserSetup : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "Setup";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "User Setup";

                GetData();
                Session["UsersPageMode"] = Helper.PageMode.New;
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
                if ((Helper.PageMode)Session["UsersPageMode"] == Helper.PageMode.New)
                {
                    if (new UserBAL().GetUsers().Where(x => x.UserName.ToUpper().Trim() == tbUserName.Text.ToUpper().Trim()).Count() > 0)
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "Username already exists");
                        return;
                    }

                    MasterUser objMasterUser = new MasterUser();
                    objMasterUser.UserName = tbUserName.Text.Trim();
                    string newpwd = new Helper().GenerateRandomPassword();
                    objMasterUser.Passcode = newpwd;
                    objMasterUser.GroupID = Convert.ToInt32(ddlOwnerGroup.SelectedValue);
                    objMasterUser.UserEmail = tbEmail.Text.Trim();
                    objMasterUser.ICNO = tbICNO.Text.Trim();
                    objMasterUser.Title = tbTitle.Text.Trim();
                    objMasterUser.FullName = tbFullName.Text.Trim();
                    objMasterUser.PositionGrade = tbPositionGrade.Text.Trim();
                    objMasterUser.PhoneNO = tbPhoneNO.Text.Trim();
                    objMasterUser.Fax = tbFax.Text.Trim();
                    objMasterUser.Designation = tbDesignation.Text.Trim();
                    objMasterUser.Department = tbDepartment.Text.Trim();
                    objMasterUser.PeriodOfService = tbPeriodOfService.Text.Trim();
                    objMasterUser.OfficeAddress = tbOfficeAddress.Text.Trim();
                    objMasterUser.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objMasterUser.Language = "English";
                    objMasterUser.CreatedBy = LoggedInUser.UserID;
                    objMasterUser.CreatedTimeStamp = DateTime.Now;
                    objMasterUser.ModifiedBy = LoggedInUser.UserID;
                    objMasterUser.ModifiedTimeStamp = DateTime.Now;

                    JuncUserRole userrole = new JuncUserRole();
                    userrole.RoleID = Convert.ToInt32(ddlRole.SelectedValue);
                    userrole.MasterUser = objMasterUser;
                    userrole.Status = "A";

                    //Megurus
                    List<UserMengurusWorkflow> lstAccountCode = new List<UserMengurusWorkflow>();
                    for (int i = 0; i < gvMengurusWorkFlow.Rows.Count; i++)
                        if (((CheckBox)gvMengurusWorkFlow.Rows[i].Cells[0].FindControl("chkSelect")).Checked)
                            lstAccountCode.Add(new UserMengurusWorkflow()
                            {
                                AccountCode = gvMengurusWorkFlow.DataKeys[i]["AccountCode1"].ToString(),
                                MasterUser = objMasterUser,
                                Status = "A"
                            });

                    //Perjawatan
                    List<UserPerjawatanWorkflow> lstServiceCode = new List<UserPerjawatanWorkflow>();
                    for (int i = 0; i < gvPerjawatanWorkFlow.Rows.Count; i++)
                        if (((CheckBox)gvPerjawatanWorkFlow.Rows[i].Cells[0].FindControl("chkSelect")).Checked)
                            lstServiceCode.Add(new UserPerjawatanWorkflow()
                            {
                                GroupPerjawatanCode = gvPerjawatanWorkFlow.DataKeys[i]["GroupPerjawatanCode"].ToString(),
                                MasterUser = objMasterUser,
                                Status = "A"
                            });

                    //Segment Details 
                    List<UserSegDtlWorkflow> lstSegmentDetail = new List<UserSegDtlWorkflow>();
                    for (int i = 0; i < gvSegmentDetails.Rows.Count; i++)
                        if (((CheckBox)gvSegmentDetails.Rows[i].Cells[0].FindControl("chkSelect")).Checked)
                            lstSegmentDetail.Add(new UserSegDtlWorkflow()
                            {
                                SegmentDetailID = Convert.ToInt32(gvSegmentDetails.DataKeys[i]["SegmentDetailID"].ToString()),
                                MasterUser = objMasterUser,
                                Status = "A"
                            });

                    if (new UserBAL().InsertUser(objMasterUser, userrole, lstAccountCode, lstServiceCode, lstSegmentDetail))
                    {
                        bool mail = MailHelper.NewPasswordMail(objMasterUser.UserEmail, newpwd);
                        ((SiteMaster)this.Master).ShowMessage("Success", "User created successfully, " + ((mail) ? "Mail Sent" : "Error Sending Mail"));
                    }
                    else
                    {
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while creating User");
                    }
                }
                else if ((Helper.PageMode)Session["UsersPageMode"] == Helper.PageMode.Edit)
                {
                    MasterUser objMasterUser = (MasterUser)Session["SelectedMasterUser"];
                    objMasterUser.UserName = tbUserName.Text.Trim();
                    objMasterUser.GroupID = Convert.ToInt32(ddlOwnerGroup.SelectedValue);
                    objMasterUser.UserEmail = tbEmail.Text.Trim();
                    objMasterUser.ICNO = tbICNO.Text.Trim();
                    objMasterUser.Title = tbTitle.Text.Trim();
                    objMasterUser.FullName = tbFullName.Text.Trim();
                    objMasterUser.PositionGrade = tbPositionGrade.Text.Trim();
                    objMasterUser.PhoneNO = tbPhoneNO.Text.Trim();
                    objMasterUser.Fax = tbFax.Text.Trim();
                    objMasterUser.Designation = tbDesignation.Text.Trim();
                    objMasterUser.Department = tbDepartment.Text.Trim();
                    objMasterUser.PeriodOfService = tbPeriodOfService.Text.Trim();
                    objMasterUser.OfficeAddress = tbOfficeAddress.Text.Trim();
                    objMasterUser.Status = new Helper().GetItemStatusEnumValueByName(ddlStatus.SelectedValue);
                    objMasterUser.ModifiedBy = LoggedInUser.UserID;
                    objMasterUser.ModifiedTimeStamp = DateTime.Now;

                    JuncUserRole userrole = new JuncUserRole();
                    userrole.RoleID = Convert.ToInt32(ddlRole.SelectedValue);
                    userrole.MasterUser = objMasterUser;
                    userrole.Status = "A";

                    //Megurus
                    List<UserMengurusWorkflow> lstAccountCode = new List<UserMengurusWorkflow>();
                    for (int i = 0; i < gvMengurusWorkFlow.Rows.Count; i++)
                        if (((CheckBox)gvMengurusWorkFlow.Rows[i].Cells[0].FindControl("chkSelect")).Checked)
                            lstAccountCode.Add(new UserMengurusWorkflow()
                            {
                                AccountCode = gvMengurusWorkFlow.DataKeys[i]["AccountCode1"].ToString(),
                                MasterUser = objMasterUser,
                                Status = "A"
                            });

                    //Perjawatan
                    List<UserPerjawatanWorkflow> lstServiceCode = new List<UserPerjawatanWorkflow>();
                    for (int i = 0; i < gvPerjawatanWorkFlow.Rows.Count; i++)
                        if (((CheckBox)gvPerjawatanWorkFlow.Rows[i].Cells[0].FindControl("chkSelect")).Checked)
                            lstServiceCode.Add(new UserPerjawatanWorkflow()
                            {
                                GroupPerjawatanCode = gvPerjawatanWorkFlow.DataKeys[i]["GroupPerjawatanCode"].ToString(),
                                MasterUser = objMasterUser,
                                Status = "A"
                            });
                    //Segment Details 
                    List<UserSegDtlWorkflow> lstSegmentDetail = new List<UserSegDtlWorkflow>();
                    for (int i = 0; i < gvSegmentDetails.Rows.Count; i++)
                        if (((CheckBox)gvSegmentDetails.Rows[i].Cells[0].FindControl("chkSelect")).Checked)
                            lstSegmentDetail.Add(new UserSegDtlWorkflow()
                            {
                                SegmentDetailID = Convert.ToInt32(gvSegmentDetails.DataKeys[i]["SegmentDetailID"].ToString()),
                                MasterUser = objMasterUser,
                                Status = "A"
                            });

                    if (new UserBAL().UpdateUser(objMasterUser, userrole, lstAccountCode, lstServiceCode, lstSegmentDetail))
                        ((SiteMaster)this.Master).ShowMessage("Success", "User updated successfully");
                    else
                        ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while updating User");
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
                ds.Tables.Add(new ReportHelper().ToDataTable<UserExport>(
                        ((List<MasterUser>)Session["MasterUserData"])
                            .Select(x => new UserExport()
                            {
                                UserName = x.UserName,
                                FullName = x.FullName,
                                UserEmail = x.UserEmail,
                                Role = (x.JuncUserRoles.Count > 0) ? x.JuncUserRoles.First().MasterRole.RoleName : string.Empty,
                                GroupName = (x.MasterGroup != null) ? x.MasterGroup.GroupName : string.Empty,
                                ICNO = x.ICNO,
                                Title = x.Title,
                                PositionGrade = x.PositionGrade,
                                PhoneNO = x.PhoneNO,
                                Fax = x.Fax,
                                Designation = x.Designation,
                                Department = x.Department,
                                PeriodOfService = x.PeriodOfService,
                                OfficeAddress = x.OfficeAddress,
                                Status = ((x.Status == "A") ? "Active" : "Inactive"),
                                MengurusWorkflow = (x.UserMengurusWorkflows.Count() > 0) ?
                                                    x.UserMengurusWorkflows.ToList().Select(y => y.AccountCode).Aggregate((a, b) => a + "," + b) : string.Empty,
                                PerjawatanWorkflow = (x.UserPerjawatanWorkflows.Count() > 0) ?
                                                    x.UserPerjawatanWorkflows.ToList().Select(y => y.GroupPerjawatanCode).Aggregate((a, b) => a + "," + b) : string.Empty,
                            })
                            .OrderBy(x => x.UserName)
                    ));
                new ReportHelper().ToExcel(ds, "Users.xls", ref Response);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvFieldMenguruSetup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    ClearPageData();

                    GridViewRow selectedRow = gvUsers.Rows[Convert.ToInt32(e.CommandArgument)];
                    selectedRow.Style["background-color"] = "gold";

                    MasterUser objMasterUser = new UserBAL().GetUsers().Where(x => x.UserID == Convert.ToInt32(gvUsers.DataKeys[selectedRow.RowIndex]["UserID"])).ToList().FirstOrDefault();
                    Session["SelectedMasterUser"] = objMasterUser;

                    tbUserName.Text = objMasterUser.UserName;
                    tbEmail.Text = objMasterUser.UserEmail;
                    tbICNO.Text = objMasterUser.ICNO;
                    tbTitle.Text = objMasterUser.Title;
                    tbFullName.Text = objMasterUser.FullName;
                    tbPositionGrade.Text = objMasterUser.PositionGrade;
                    tbPhoneNO.Text = objMasterUser.PhoneNO;
                    tbFax.Text = objMasterUser.Fax;
                    tbDesignation.Text = objMasterUser.Designation;
                    tbDepartment.Text = objMasterUser.Department;
                    tbPeriodOfService.Text = objMasterUser.PeriodOfService;
                    tbOfficeAddress.Text = objMasterUser.OfficeAddress;

                    ddlOwnerGroup.SelectedIndex = -1;
                    if (ddlOwnerGroup.Items.FindByValue(objMasterUser.GroupID.ToString()) != null)
                        ddlOwnerGroup.Items.FindByValue(objMasterUser.GroupID.ToString()).Selected = true;
                    else
                    {
                        if (ddlOwnerGroup.Items.Count > 0)
                            ddlOwnerGroup.SelectedIndex = 0;
                        else ddlOwnerGroup.SelectedIndex = -1;
                    }

                    ddlRole.SelectedIndex = -1;
                    if (objMasterUser.JuncUserRoles.Count > 0)
                        ddlRole.Items.FindByValue(objMasterUser.JuncUserRoles.FirstOrDefault().RoleID.ToString()).Selected = true;
                    ddlRole_SelectedIndexChanged(null, null);

                    ddlStatus.SelectedIndex = -1;
                    ddlStatus.Items.FindByValue(new Helper().GetItemStatusEnumName(Convert.ToChar(objMasterUser.Status))).Selected = true;

                    //Megurus
                    List<string> lstAccountCode = objMasterUser.UserMengurusWorkflows.Where(x => x.Status == "A").Select(x => x.AccountCode).ToList();
                    for (int i = 0; i < gvMengurusWorkFlow.Rows.Count; i++)
                        ((CheckBox)gvMengurusWorkFlow.Rows[i].Cells[0].FindControl("chkSelect")).Checked =
                            lstAccountCode.Contains(gvMengurusWorkFlow.DataKeys[i]["AccountCode1"].ToString());

                    //Perjawatan
                    List<string> lstServiceCode = objMasterUser.UserPerjawatanWorkflows.Where(x => x.Status == "A").Select(x => x.GroupPerjawatanCode).ToList();
                    for (int i = 0; i < gvPerjawatanWorkFlow.Rows.Count; i++)
                        ((CheckBox)gvPerjawatanWorkFlow.Rows[i].Cells[0].FindControl("chkSelect")).Checked =
                            lstServiceCode.Contains(gvPerjawatanWorkFlow.DataKeys[i]["GroupPerjawatanCode"].ToString());

                    //Segment Details
                    List<int> lstSegDtls = objMasterUser.UserSegDtlWorkflows.Where(x => x.Status == "A").Select(x => Convert.ToInt32(x.SegmentDetailID)).ToList();
                    for (int i = 0; i < gvSegmentDetails.Rows.Count; i++)
                        ((CheckBox)gvSegmentDetails.Rows[i].Cells[0].FindControl("chkSelect")).Checked =
                            lstSegDtls.Contains(Convert.ToInt32(gvSegmentDetails.DataKeys[i]["SegmentDetailID"].ToString()));

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
                List<MasterUser> data = new UserBAL().GetUsers().ToList();

                //data = data.ForEach(x => x.Status = new Helper().GetItemStatusEnumName(Convert.ToChar(x.Status)));
                Session["MasterUserData"] = data;
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
                gvUsers.DataSource = (List<MasterUser>)Session["MasterUserData"];
                gvUsers.DataBind();

                List<string> ParAccounts = new AccountCodeBAL().GetAccountCodes().Select(x=>x.ParentAccountCode).Distinct().ToList();
                gvMengurusWorkFlow.DataSource = new AccountCodeBAL().GetAccountCodes().Where(x => x.Status == "A" && !ParAccounts.Contains(x.AccountCode1)).ToList();
                gvMengurusWorkFlow.DataBind();

                List<string> ParServices = new GroupPerjawatanBAL().GetGroupPerjawatans().Select(x => x.ParentGroupPerjawatanID).Distinct().ToList();
                gvPerjawatanWorkFlow.DataSource = new GroupPerjawatanBAL().GetGroupPerjawatans().Where(x => x.Status == "A" && !ParServices.Contains(x.GroupPerjawatanCode)).ToList();
                gvPerjawatanWorkFlow.DataBind();

                List<int> ParSegDtls = new SegmentDetailsBAL().GetSegmentDetails().ToList().Select(x => Convert.ToInt32(x.ParentDetailID)).Distinct().ToList();
                gvSegmentDetails.DataSource = new SegmentDetailsBAL().GetSegmentDetails().Where(x => x.Segment.Status == "A" && x.Status == "A" && !ParSegDtls.Contains(x.SegmentDetailID))
                    .OrderBy(x => x.Segment.SegmentOrder).ThenBy(x => x.DetailCode)
                    .Select(x => new
                    {
                        x.SegmentDetailID,
                        x.Segment.SegmentName,
                        x.DetailCode
                    }).ToList();
                gvSegmentDetails.DataBind();
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

                ddlOwnerGroup.DataSource = new UserBAL().GetGroups().Where(x => x.Status == "A").ToList(); ;
                ddlOwnerGroup.DataTextField = "GroupName";
                ddlOwnerGroup.DataValueField = "GroupID";
                ddlOwnerGroup.DataBind();

                ddlRole.DataSource = new UserBAL().GetRoles();
                ddlRole.DataTextField = "RoleName";
                ddlRole.DataValueField = "RoleID";
                ddlRole.DataBind();
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
                    tbUserName.Enabled = true;
                    break;
                case Helper.PageMode.Edit:
                    tbUserName.Enabled = false;
                    break;
            }
            Session["UsersPageMode"] = pagemode;
        }

        private void ClearPageData()
        {
            try
            {
                tbUserName.Text = string.Empty;
                tbEmail.Text = string.Empty;
                tbICNO.Text = string.Empty;
                tbTitle.Text = string.Empty;
                tbFullName.Text = string.Empty;
                tbPositionGrade.Text = string.Empty;
                tbPhoneNO.Text = string.Empty;
                tbFax.Text = string.Empty;
                tbDesignation.Text = string.Empty;
                tbDepartment.Text = string.Empty;
                tbPeriodOfService.Text = string.Empty;
                tbOfficeAddress.Text = string.Empty;

                if (ddlOwnerGroup.Items.Count > 0)
                    ddlOwnerGroup.SelectedIndex = 0;
                else ddlOwnerGroup.SelectedIndex = -1;

                ddlStatus.SelectedIndex = 0;

                foreach (GridViewRow gvr in gvUsers.Rows)
                    gvr.Style["background-color"] = "";
            }
            catch (Exception ex)
            {
                ((SiteMaster)this.Master).ShowMessage("Error", "An error occurred", ex, true);
            }
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRole.SelectedIndex == 0) //|| ddlRole.SelectedIndex == 4
                divWorkFlowRow.Visible = false;
            else
                divWorkFlowRow.Visible = true;
        }
    }
}