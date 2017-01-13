<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FieldPerjawatanSetup.aspx.cs" Inherits="BudgetPrep.FieldPerjawatanetup" %>

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
                <div id="Div1" class="box-content">
                    <%--<h4 class="page-header">Registration form</h4>--%>
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <%--<div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <label class="control-label">FieldPerjawatan Code</label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbFieldPerjawatanID" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>--%>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblFieldPerjawatan" runat="server" class="control-label">Field Perjawatan</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbFieldPerjawatanDesc" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="tbFieldPerjawatanDesc" ForeColor="Red"
                                            CssClass="failureNotification" ErrorMessage="Field Perjawatan is required." ToolTip="Field Perjawatan is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <%--<div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <label class="control-label">FieldPerjawatan SDesc</label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbFieldPerjawatanSDesc" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>--%>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblStatus" runat="server" class="control-label">Status</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="pull-right">
                                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-label-left" ValidationGroup="SaveValidation" OnClick="btnSave_Click">
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
                        <%--<a class="close-link">
                    <i class="fa fa-times"></i>
                </a>--%>
                    </div>
                    <div class="no-move"></div>
                </div>
                <div id="GridBox" class="box-content">
                    <%--<h4 class="page-header">Registration form</h4>--%>
                    <form class="form-horizontal" role="form">
                        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-label-left" OnClick="btnAdd_Click">
                            <span><i class="fa fa-file-o txt-primary"></i></span>Tambah
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnPrint" runat="server" CssClass="pull-right btn btn-label-left" OnClick="btnPrint_Click">
                            <span><i class="fa fa-print txt-info"></i></span>Cetak
                        </asp:LinkButton>
                        <%--<input id="cc" class="easyui-combotree" data-options="url:'tree_data1.json',method:'get',required:true" style="width:200px;">--%>
                        <div style="overflow: auto">
                            <asp:GridView ID="gvFieldPerjawatanetup" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                CssClass="table table-bordered table-striped table-hover" DataKeyNames="FieldPerjawatanID"
                                OnRowCommand="gvFieldPerjawatanetup_RowCommand">
                                <Columns>
                                    <%--<asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="gvChk" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:BoundField DataField="FieldPerjawatanDesc" HeaderText="Field Perjawatan Description" />
                                    <%--<asp:BoundField DataField="FieldPerjawatanSDesc" HeaderText="FieldPerjawatanSDesc" />--%>
                                    <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEditRow" runat="server" CommandName="EditRow" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <span><i class="fa fa-edit"></i></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEditDetails" runat="server" CommandName="EditDetails" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <span><i class="fa fa-info-circle"></i></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <%--<asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <div class="btn-group">
                                                <a class="btn btn-primary dropdown-toggle" data-toggle="dropdown" href="#">
                                                    <span class="fa fa-caret-down"></span></a>
                                                <ul class="dropdown-menu">
                                                    <li>
                                                        <asp:LinkButton ID="LinkButton1" runat="server" CommandName="EditDetails" CssClass="btn-label-left" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-plus"></i></span> Add Item
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton ID="LinkButton2" runat="server" CommandName="EditDetails" CssClass="btn-label-left" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-level-down"></i></span> Add Hierarchy
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li class="divider"></li>
                                                    <li>
                                                        <asp:LinkButton ID="LinkButton3" runat="server" CommandName="EditDetails" CssClass="btn-label-left" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-cut"></i></span> Cut
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton ID="LinkButton4" runat="server" CommandName="EditDetails" CssClass="btn-label-left" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-paste"></i></span> Paste
                                                        </asp:LinkButton>
                                                    </li>
                                                </ul>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                </Columns>
                            </asp:GridView>
                            <div style="height: 100px;">
                            </div>
                        </div>
                        <div id="capturedShot"></div>
                    </form>
                </div>
                <%--<button id="downloadinner">Download Pdf</button>--%>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            WinMove();
        });
    </script>
</asp:Content>
