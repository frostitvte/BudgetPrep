<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserSetup.aspx.cs" Inherits="BudgetPrep.UserSetup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnPrint" />
        </Triggers>
        <ContentTemplate>
            <div id="EditBox" runat="server" class="box" visible="false">
                <div class="box-header">
                    <div class="box-name">
                        <i class="fa fa-arrows"></i>
                        <span>Edit</span>
                    </div>
                    <div class="box-icons">
                        <a class="collapse-link">
                            <i class="fa fa-chevron-up"></i>
                        </a>
                        <a class="expand-link">
                            <i class="fa fa-expand"></i>
                        </a>
                        <%--<a class="close-link">
                    <i class="fa fa-times"></i>
                </a>--%>
                    </div>
                    <div class="no-move"></div>
                </div>
                <div id="Div3" class="box-content">
                    <%--<h4 class="page-header">Registration form</h4>--%>
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblUserName" runat="server" class="control-label">User Name</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbUserName" runat="server" CssClass="form-control" onkeypress="return NoSpaceKey(event);"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="tbUserName" ForeColor="Red"
                                            ErrorMessage="User Name is required." ToolTip="User Name is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <%--</div>
                    <div class="row">--%>
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblRole" runat="server" class="control-label">Role</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblIC" runat="server" class="control-label">No. Kad Pengenalan</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbICNO" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="tbICNO" ForeColor="Red"
                                            ErrorMessage="No. Kad is required." ToolTip="No. Kad is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <%--</div>
                    <div class="row">--%>
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblTitle" runat="server" class="control-label">Gelaran</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbTitle" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbTitle" ForeColor="Red"
                                            ErrorMessage="Gelaran is required." ToolTip="Gelaran is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblFullName" runat="server" class="control-label">Nama Penuh</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbFullName" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbFullName" ForeColor="Red"
                                            ErrorMessage="Nama Penuh is required." ToolTip="Nama Penuh is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <%--</div>
                    <div class="row">--%>
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblPositionGrade" runat="server" class="control-label">Gred Jawatan</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbPositionGrade" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbPositionGrade" ForeColor="Red"
                                            ErrorMessage="Gred Jawatan is required." ToolTip="Gred Jawatan is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblPhoneNo" runat="server" class="control-label">No. Telefon</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbPhoneNO" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbPhoneNO" ForeColor="Red"
                                            ErrorMessage="No. Telefon is required." ToolTip="No. Telefon is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="tbPhoneNO" ForeColor="Red"
                                    ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$" ValidationGroup="SaveValidation"
                                    ErrorMessage="Enter valid Phone number." ToolTip="Enter valid Phone number." Display="Dynamic"></asp:RegularExpressionValidator>--%>
                                    </div>
                                </div>
                                <%--</div>
                    <div class="row">--%>
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblFax" runat="server" class="control-label">No. FAX</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbFax" runat="server" CssClass="form-control"></asp:TextBox>
                                        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="tbFax" ForeColor="Red"
                                    ValidationExpression="/^(\+[0-9]{0,3})*( ([0-9]{0,3})){3}$/" ValidationGroup="SaveValidation"
                                    ErrorMessage="Enter valid Phone number." ToolTip="Enter valid Phone number." Display="Dynamic"></asp:RegularExpressionValidator>--%>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblOwnerGroup" runat="server" class="control-label">User Group</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:DropDownList ID="ddlOwnerGroup" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <%--</div>
                    <div class="row">--%>
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblEmail" runat="server" class="control-label">Emel Rasmi</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="tbEmail" ForeColor="Red"
                                            ErrorMessage="Emel Rasmi is required." ToolTip="Emel Rasmi is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="tbEmail" ForeColor="Red"
                                    ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="SaveValidation"
                                    ErrorMessage="Enter valid Email." ToolTip="Enter valid Email." Display="Dynamic" ></asp:RegularExpressionValidator>--%>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblDesignation" runat="server" class="control-label">Jawatan</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbDesignation" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="tbDesignation" ForeColor="Red"
                                            ErrorMessage="Jawatan is required." ToolTip="Jawatan is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <%--</div>
                    <div class="row">--%>
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblDepartment" runat="server" class="control-label">Jabatan</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbDepartment" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="tbDepartment" ForeColor="Red"
                                            ErrorMessage="Jabatan is required." ToolTip="Jabatan is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblPeriodOfService" runat="server" class="control-label">Tempoh Bekerja</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbPeriodOfService" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblStatus" runat="server" class="control-label">Status</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <%--</div>
                    <div class="row">--%>
                                <div class="col-lg-6 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblOfficeAddress" runat="server" class="control-label">Alamat Pejabat</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbOfficeAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Height="60px"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row" id="divWorkFlowRow" runat="server" visible="false" >
                                <hr />
                                <%--<div class="col-lg-6 col-lg-push-3 col-sm-12">
                            <div class="col-md-12 col-lg-4">
                                <asp:Label ID="lblStatus" runat="server" class="control-label">WorkFlow</asp:Label>
                            </div>
                        </div>
                        <br />--%>
                                <div class="col-lg-6 col-lg-push-3 col-sm-12" >
                                    <div class="row-fluid">
                                        <div id="dashboard_links" class="col-xs-12 col-sm-2 pull-right">
                                            <ul class="nav nav-pills nav-stacked">
                                                <li class="active"><a href="#" class="tab-link" id="Mengurus">Mengurus</a></li>
                                                <li><a href="#" class="tab-link" id="Perjawatan">Perjawatan</a></li>
                                                <li><a href="#" class="tab-link" id="SigmentDetails">Segment Details</a></li>
                                            </ul>
                                        </div>
                                        <div id="dashboard_tabs" class="col-xs-12 col-sm-10">
                                            <!--Tab 1-->
                                            <div id="dashboard-Mengurus" class="row userTab" style="visibility: visible; position: relative;">
                                                <div id="Div1" class="box-content" style="border: 1px solid #337ab7;">
                                                    <div class="form-horizontal" role="form">
                                                        <div class="form-group">
                                                            <div style="max-height:300px;overflow-y: auto;">
                                                                <asp:GridView ID="gvMengurusWorkFlow" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                                                    CssClass="table table-bordered table-striped table-hover" DataKeyNames="AccountCode1">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="AccountCode1" HeaderText="Account Code" />
                                                                        <asp:BoundField DataField="AccountDesc" HeaderText="Account Description" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <!--Tab 2-->
                                            <div id="dashboard-Perjawatan" class="row userTab" style="visibility: hidden; position: absolute;">
                                                <div id="Div4" class="box-content" style="border: 1px solid #337ab7;">
                                                    <div class="form-horizontal" role="form">
                                                        <div class="form-group">
                                                            <div style="max-height:300px;overflow-y: auto;">
                                                                <asp:GridView ID="gvPerjawatanWorkFlow" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                                                    CssClass="table table-bordered table-striped table-hover" DataKeyNames="GroupPerjawatanCode">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="GroupPerjawatanCode" HeaderText="Service Code" />
                                                                        <asp:BoundField DataField="GroupPerjawatanDesc" HeaderText="Service Description" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <!--Tab 3-->
                                            <div id="dashboard-SigmentDetails" class="row userTab" style="visibility: hidden; position: absolute;">
                                                <div id="Div6" class="box-content" style="border: 1px solid #337ab7;">
                                                    <div class="form-horizontal" role="form">
                                                        <div class="form-group">
                                                            <div style="max-height:300px;overflow-y: auto;">
                                                                <asp:GridView ID="gvSegmentDetails" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                                                    CssClass="table table-bordered table-striped table-hover" DataKeyNames="SegmentDetailID">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="SegmentName" HeaderText="Segment" />
                                                                        <asp:BoundField DataField="DetailCode" HeaderText="Segment Detail" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="pull-right">
                                <%--<asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-label-left" OnClick="btnAdd_Click">
                                    <span><i class="fa fa-file-o txt-primary"></i></span>Tambah
                                </asp:LinkButton>                                
                                <asp:LinkButton ID="btnPrint" runat="server" CssClass="btn btn-label-left" OnClick="btnPrint_Click">
                                    <span><i class="fa fa-print txt-info"></i></span>Cetak
                                </asp:LinkButton>--%>
                                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-label-left" OnClick="btnSave_Click" ValidationGroup="SaveValidation">
                            <span><i class="fa fa-save txt-success"></i></span>Simpan
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-label-left" OnClick="btnCancel_Click">
                            <span><i class="fa fa-undo txt-warning"></i></span>Batal
                                </asp:LinkButton>
                                &nbsp;
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="box">
                <div class="box-header">
                    <div class="box-name">
                        <i class="fa fa-arrows"></i>
                        <span>View</span>
                    </div>
                    <div class="box-icons">
                        <a class="collapse-link">
                            <i class="fa fa-chevron-up"></i>
                        </a>
                        <a class="expand-link">
                            <i class="fa fa-expand"></i>
                        </a>
                    </div>
                    <div class="no-move"></div>
                </div>
                <div id="Div2" class="box-content">
                    <div class="form-horizontal" role="form">
                        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-label-left" OnClick="btnAdd_Click">
                            <span><i class="fa fa-file-o txt-primary"></i></span>Tambah
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnPrint" runat="server" CssClass="pull-right btn btn-label-left" OnClick="btnPrint_Click">
                            <span><i class="fa fa-print txt-info"></i></span>Cetak
                        </asp:LinkButton>
                        <div style="overflow: auto">
                            <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                CssClass="table table-bordered table-striped table-hover" DataKeyNames="UserID"
                                OnRowCommand="gvFieldMenguruSetup_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                                    <asp:BoundField DataField="UserName" HeaderText="User Name" />
                                    <asp:BoundField DataField="UserEmail" HeaderText="Email" />
                                    <asp:BoundField DataField="PhoneNO" HeaderText="Phone NO" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEditRow" runat="server" CommandName="EditRow" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <span><i class="fa fa-edit"></i></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div style="height: 100px;">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            WinMove();
            DashboardTabChecker();
        });
    </script>
</asp:Content>
