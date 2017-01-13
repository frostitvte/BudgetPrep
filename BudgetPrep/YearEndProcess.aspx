<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="YearEndProcess.aspx.cs" Inherits="BudgetPrep.YearEndProcess" %>

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
                        <i class="fa fa-search"></i>
                        <span>Year End</span>
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
                <div id="Div1" class="box-content">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblBudgetType" runat="server" class="control-label">Budget Type</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:DropDownList ID="ddlBudgetType" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="Mengurus">Mengurus</asp:ListItem>
                                            <asp:ListItem Value="Perjawatan">Perjawatan</asp:ListItem>
                                            <asp:ListItem Value="Projek">Projek</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblYear" runat="server" class="control-label">Year</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:DropDownList ID="ddlBudgetYear" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblStatus" runat="server" class="control-label">Status</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="A">Open</asp:ListItem>
                                            <asp:ListItem Value="D">End</asp:ListItem>
                                        </asp:DropDownList>
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
                    </div>
                    <div class="no-move"></div>
                </div>
                <div id="GridBox" class="box-content">
                    <form class="form-horizontal" role="form">
                        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-label-left" OnClick="btnAdd_Click">
                            <span><i class="fa fa-file-o txt-primary"></i></span>Tambah
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnPrint" runat="server" CssClass="pull-right btn btn-label-left" OnClick="btnPrint_Click">
                            <span><i class="fa fa-print txt-info"></i></span>Cetak
                        </asp:LinkButton>
                        <div style="overflow: auto">
                            <asp:GridView ID="gvYearEnd" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                CssClass="table table-bordered table-striped table-hover" DataKeyNames="YearEndID"
                                OnRowCommand="gvYearEnd_RowCommand" OnRowDataBound="gvYearEnd_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="BudgetYear" HeaderText="Year" />
                                    <asp:BoundField DataField="BudgetType" HeaderText="Budget Type" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Width="150px" Visible="false"/>
                                    <asp:TemplateField HeaderText="Status" HeaderStyle-Width="150px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnStatus" runat="server" CommandName="Download" CommandArgument='<%# Container.DataItemIndex %>'/>  
                                            <asp:Label ID="lblStatus" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
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
