<%@ Page Title="Log In" Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="BudgetPrep.Account.Login" %>

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
    <style type="text/css">
        .vcenter
        {
            display: inline-block;
            vertical-align: middle;
            float: none;
        }
    </style>
</head>
<body>
    <form id="main" runat="server">
        <div class="container-fluid">
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                    <img alt="" src="../img/budget2.png" class="img-rounded img-responsive" />
                </div><!--
                --><div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                    <div id="page-login" class="row">
                        <%--<div class="col-xs-0 col-sm-0 col-md-2 col-lg-2">
                        </div>--%>
                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                            <%--<div class="text-right">
                        <a href="page_register.html" class="txt-default">Need an account?</a>
                    </div>--%>

                            <div class="box">
                                <div class="box-content">
                                    <%--<div class="text-center">
                                <h3 class="page-header">Login Page</h3>
                            </div>--%>
                                    <div class="form-group">
                                        <p>Sign in with your organizational account</p>
                                        <%--<p>
                                    Please enter your username and password.
                                    <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="false">Register</asp:HyperLink>
                                    if you don't have an account.
                                </p>--%>
                                        <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false"
                                            TextBoxStyle-Width="200%" TextBoxStyle-CssClass="form-control" LoginButtonStyle-CssClass="btn btn-primary" LabelStyle-CssClass="control-label"
                                            ValidatorTextStyle-ForeColor="Red" OnAuthenticate="LoginUser_Authenticate">
                                            <LayoutTemplate>
                                                <%--<asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification" ForeColor="Red"
                                            ValidationGroup="LoginUserValidationGroup" />--%>
                                                <div class="accountInfo">
                                                    <fieldset class="login">
                                                        <%--<legend>Please enter your username and password</legend>--%>
                                                        <p>
                                                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" CssClass="control-label">Username:</asp:Label>
                                                            <asp:TextBox ID="UserName" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ForeColor="Red"
                                                                CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required." Display="Dynamic"
                                                                ValidationGroup="LoginUserValidationGroup"></asp:RequiredFieldValidator>
                                                        </p>
                                                        <p>
                                                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="control-label">Password:</asp:Label>
                                                            <asp:TextBox ID="Password" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ForeColor="Red"
                                                                CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required." Display="Dynamic"
                                                                ValidationGroup="LoginUserValidationGroup"></asp:RequiredFieldValidator>
                                                        </p>
                                                        <%--<p>
                                                    <asp:CheckBox ID="RememberMe" runat="server" />
                                                    <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">Keep me logged in</asp:Label>
                                                </p>--%>
                                                    </fieldset>
                                                    <p class="submitButton">
                                                        <%--<asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="LoginUserValidationGroup" />--%>
                                                        <asp:LinkButton ID="LoginButton" runat="server" CommandName="Login" CssClass="btn btn-label-left" ValidationGroup="LoginUserValidationGroup">
                                                            <span><i class="fa fa-sign-in txt-success"></i></span> Log In
                                                        </asp:LinkButton>
                                                    </p>
                                                    <span class="failureNotification txt-danger">
                                                        <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                                                    </span>
                                                </div>
                                            </LayoutTemplate>
                                        </asp:Login>
                                        <br />
                                        <a href="./ResetPassword.aspx">
                                            <span><i class="fa fa-refresh txt-warning"></i></span>Reset Password
                                        </a>
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
