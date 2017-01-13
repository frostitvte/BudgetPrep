<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserGroupSetup.aspx.cs" Inherits="BudgetPrep.UserGroupSetup" %>

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
                    </div>
                    <div class="no-move"></div>
                </div>
                <div id="Div1" class="box-content">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblGroupName" runat="server" class="control-label">Group Name</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbGroupName" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="tbGroupName" ForeColor="Red"
                                            CssClass="failureNotification" ErrorMessage="Field Mengurus is required." ToolTip="Field Mengurus is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
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
                            <asp:GridView ID="gvUserGroup" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                CssClass="table table-bordered table-striped table-hover" DataKeyNames="GroupID"
                                OnRowCommand="gvUserGroup_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="GroupName" HeaderText="Group Name" />
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
