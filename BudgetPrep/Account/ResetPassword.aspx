<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="BudgetPrep.Account.ResetPassword" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <title>Login</title>
    <meta name="description" content="description">
    <meta name="author" content="Evgeniya">
    <meta name="keyword" content="keywords">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../css/bootstrap.css" rel="stylesheet" />
    <link href="../Styles/Other/jquery-ui.min.css" rel="stylesheet" />
    <link href="http://netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.css" rel="stylesheet">
    <%--<link href="Styles/Other/font-awesome.css" rel="stylesheet" type="text/css" />--%>
    <link href="../Styles/Other/style.css" rel="stylesheet" type="text/css" />
    <%--<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />--%>
    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
				<script src="http://getbootstrap.com/docs-assets/js/html5shiv.js"></script>
				<script src="http://getbootstrap.com/docs-assets/js/respond.min.js"></script>
		<![endif]-->
</head>
<body>
    <form id="main" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div class="container-fluid">
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                    <img alt="" src="../img/budget2.png" class="img-rounded img-responsive" />
                </div>
                <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                    <div id="page-login" class="row">
                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                            <div class="box">
                                <div class="box-content">
                                    <div class="form-group">
                                        <%--<p>Enter your details</p>--%>

                                        <asp:Label ID="lblUserName" runat="server" CssClass="control-label">Username:</asp:Label>
                                        <asp:TextBox ID="tbUserName" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="tbUserName" ForeColor="Red"
                                            CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required." Display="Dynamic"
                                            ValidationGroup="ResetQuesValidation"></asp:RequiredFieldValidator>

                                        <%--<asp:Label ID="lblEmail" runat="server" CssClass="control-label">Registered Email:</asp:Label>
                                        <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbEmail" ForeColor="Red"
                                            CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="Email is required." Display="Dynamic"
                                            ValidationGroup="ResetValidation"></asp:RequiredFieldValidator>
                                        <br />
                                        <p>Security Question</p>--%>
                                        <asp:LinkButton ID="btnVerify" runat="server" CssClass="btn btn-label-left pull-right" ValidationGroup="ResetQuesValidation"
                                            OnClick="btnVerify_Click">
                                            <span><i class="fa fa-refresh txt-warning"></i></span> Verify
                                        </asp:LinkButton>
                                        <br />
                                        <asp:Panel ID="pnlQuestion" runat="server" Visible="false">
                                            <br />
                                            <asp:Label ID="lblQuestionlbl" runat="server" CssClass="control-label ">Security Question:</asp:Label>
                                            <br />
                                            <asp:Label ID="lblQuestion" runat="server" CssClass="control-label txt-danger"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblAnswer" runat="server" CssClass="control-label">Answer:</asp:Label>
                                            <asp:TextBox ID="tbAnswer" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbAnswer" ForeColor="Red"
                                                CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="Answer is required." Display="Dynamic"
                                                ValidationGroup="ResetValidation"></asp:RequiredFieldValidator>
                                            <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-label-left pull-right" ValidationGroup="ResetValidation"
                                                OnClick="btnReset_Click">
                                            <span><i class="fa fa-refresh txt-warning"></i></span> Reset
                                            </asp:LinkButton>
                                            <br />
                                        </asp:Panel>
                                        <br />
                                        <asp:Label ID="lblMessage" runat="server" CssClass="control-label txt-info"></asp:Label>
                                        <div id="divMsg" runat="server">

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
