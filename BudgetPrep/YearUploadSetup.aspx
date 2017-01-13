<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="YearUploadSetup.aspx.cs" Inherits="BudgetPrep.YearUploadSetup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblYear" runat="server" class="control-label">Year</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblDescription" runat="server" class="control-label">Description</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbDesc" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
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
                                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-label-left" OnClick="btnSave_Click">
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
                        <%--<input id="cc" class="easyui-combotree" data-options="url:'tree_data1.json',method:'get',required:true" style="width:200px;">--%>
                        <div style="overflow: auto">
                            <asp:GridView ID="gvYearUploadSetup" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                            CssClass="table table-bordered table-striped table-hover" DataKeyNames="BudgetYearID"
                            OnRowCommand="gvYearUploadSetup_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField ="BudgetYear" HeaderText="Budget Year" />
                                    <asp:BoundField DataField ="BudgetYearDesc" HeaderText="Description" />
                                    <asp:BoundField DataField ="Status" HeaderText="Status" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField ="BudgetYearID" HeaderText="BudgetYearID" Visible="false"/> 
                                    <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                        <itemtemplate>
                                            <asp:LinkButton ID="btnEditRow" runat="server" CommandName="EditRow" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <span><i class="fa fa-edit"></i></span>
                                            </asp:LinkButton>
                                        </itemtemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div style="height: 100px;">
                            </div>
                        </div>
                        <div id="capturedShot"></div>
                    </form>
                </div>
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
