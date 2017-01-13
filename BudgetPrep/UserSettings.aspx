<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserSettings.aspx.cs" Inherits="BudgetPrep.UserSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .userTab
        {
            padding-bottom: 15px;
            min-height: 100px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row-fluid">
        <div id="dashboard_links" class="col-xs-12 col-sm-2 pull-right">
            <ul class="nav nav-pills nav-stacked">
                <li class="active"><a href="#" class="tab-link" id="OverView">Profile</a></li>
                <li><a href="#" class="tab-link" id="ChangePwd">Change Password</a></li>
                <li><a href="#" class="tab-link" id="SecQuestion">Security Info</a></li>
            </ul>
        </div>
        <div id="dashboard_tabs" class="col-xs-12 col-sm-10">
            <!--Tab 1-->
            <div id="dashboard-OverView" class="row userTab" style="visibility: visible; position: relative;">
                <div id="Div1" class="box-content">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <h3>
                                        <asp:Label ID="lblSecMsg" runat="server" CssClass="control-label label-danger" Visible="false">Security Information is mandatory</asp:Label>
                                    </h3>
                                </div>
                            </div>
                            <hr />
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="Label6" runat="server" CssClass="control-label" Font-Bold="true" Font-Underline="true">Profile</asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="Label1" runat="server" CssClass="control-label">User Name:</asp:Label>
                                    <asp:TextBox ID="tbUserName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="Label2" runat="server" CssClass="control-label">Group Name:</asp:Label>
                                    <asp:TextBox ID="tbGroupName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="Label3" runat="server" CssClass="control-label">User Email:</asp:Label>
                                    <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!--Tab 2-->
            <div id="dashboard-ChangePwd" class="row userTab" style="visibility: hidden; position: absolute;">
                <div id="Div3" class="box-content">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="lblOldPassword" runat="server" CssClass="control-label">Old Password:</asp:Label>
                                    <asp:TextBox ID="tbOldPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbOldPassword" ForeColor="Red"
                                        ErrorMessage="Old Password is required." ToolTip="Old Password is required." Display="Dynamic"
                                        ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="lblNewPassword" runat="server" CssClass="control-label">New Password:</asp:Label>
                                    <asp:TextBox ID="tbNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbNewPassword" ForeColor="Red"
                                        ErrorMessage="New Password is required." ToolTip="New Password is required." Display="Dynamic"
                                        ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="lblReNewPassword" runat="server" CssClass="control-label">Re-Enter New Password:</asp:Label>
                                    <asp:TextBox ID="tbReNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbReNewPassword" ForeColor="Red"
                                        ErrorMessage="Re New Password is required." ToolTip="Re New Password is required." Display="Dynamic"
                                        ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="tbReNewPassword" ForeColor="Red"
                                        ControlToCompare="tbNewPassword" ErrorMessage="Password must be same" ToolTip="Password must be same" 
                                        ValidationGroup="SaveValidation"/>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="pull-right">
                                        <asp:LinkButton ID="btnResetPwd" runat="server" CssClass="btn btn-label-left" ValidationGroup="SaveValidation"
                                            OnClick="btnResetPwd_Click">
                                            <span><i class="fa fa-save txt-success"></i></span>Simpan
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!--Tab 3-->
            <div id="dashboard-SecQuestion" class="row userTab" style="visibility: hidden; position: absolute;">
                <div id="Div2" class="box-content">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="Label4" runat="server" CssClass="control-label">Security Question:</asp:Label>
                                    <asp:TextBox ID="tbSecQuestion" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbSecQuestion" ForeColor="Red"
                                        ErrorMessage="Security Question is required." ToolTip="Security Question is required." Display="Dynamic"
                                        ValidationGroup="SecQuestionValidation"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <asp:Label ID="Label5" runat="server" CssClass="control-label">Security Answer:</asp:Label>
                                    <asp:TextBox ID="tbSecAnswer" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="tbSecAnswer" ForeColor="Red"
                                        ErrorMessage="Security Answer is required." ToolTip="Security Answer is required." Display="Dynamic"
                                        ValidationGroup="SecQuestionValidation"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="pull-right">
                                        <asp:LinkButton ID="btnSaveSecurityInfo" runat="server" CssClass="btn btn-label-left" ValidationGroup="SecQuestionValidation"
                                            OnClick="btnSaveSecurityInfo_Click">
                                            <span><i class="fa fa-save txt-success"></i></span>Simpan
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            //debugger;
            DashboardTabChecker();
        });
    </script>
</asp:Content>
