<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GroupPerjawatanSetup.aspx.cs" Inherits="BudgetPrep.GroupPerjawatanSetup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnPrint" />
            <asp:PostBackTrigger ControlID="btnSample" />
            <asp:PostBackTrigger ControlID="btnUpload" />
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
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblServiceCode" runat="server" class="control-label">Service Code</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbCode" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="tbCode" ForeColor="Red"
                                        CssClass="failureNotification" ErrorMessage="Account Code is required." ToolTip="Account Code is required." Display="Dynamic"
                                        ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblDescription" runat="server" class="control-label">Description</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbDesc" runat="server" CssClass="form-control" ></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbDesc" ForeColor="Red"
                                        CssClass="failureNotification" ErrorMessage="Description is required." ToolTip="Description is required." Display="Dynamic"
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
                                <%--<asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-label-left" OnClick="btnAdd_Click">
                                    <span><i class="fa fa-file-o txt-primary"></i></span>Tambah
                                </asp:LinkButton>                                
                                <asp:LinkButton ID="btnPrint" runat="server" CssClass="btn btn-label-left" OnClick="btnPrint_Click">
                                    <span><i class="fa fa-print txt-info"></i></span>Cetak
                                </asp:LinkButton>--%>
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
                <div id="Div2" class="box-content">
                    <%--<h4 class="page-header">Registration form</h4>--%>
                    <div class="form-horizontal" role="form">
                        <%--<label style="width:10px;"><i class="fa fa-minus-square"></i></label>
                        <label style="width:10px;"></label>
                        <span style="width:10px;"><i class="fa fa-minus-square"></i></span>--%>

                        <%--<asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-label-left" OnClick="btnAdd_Click">
                            <span><i class="fa fa-file-o txt-primary"></i></span>Tambah
                        </asp:LinkButton>--%>

                        <asp:Label ID="lblMsg" runat="server" Visible="false" style="font-size:10px;color:red;font-style:italic;">*Add/Enable current year to enable upload button</asp:Label>

                        <div>
                            <button ID="btnFileUpload" runat="server" visible="false" class=" btn btn-label-left" type="button" data-toggle="collapse" data-target="#LegendExample" 
                                    aria-expanded="false" aria-controls="LegendExample">
                                    <span><i class="fa fa-upload txt-primary"></i></span>Upload
                                </button>

                            <asp:LinkButton ID="btnPrint" runat="server" CssClass="btn btn-label-left" OnClick="btnPrint_Click">
                                <span><i class="fa fa-print txt-info"></i></span>Cetak
                            </asp:LinkButton>
                        </div>

                        <%--Upload button - start --%>
                        <div class="collapse" id="LegendExample">
                            <div class="well">
                                <div class="row text-center">
                                    <asp:LinkButton ID="btnSample" runat="server" Text="Template" OnClick="btnSample_Click">
                                    </asp:LinkButton>
                                </div>
                                <div class="row">
                                    <%--<div class="col-lg-6 col-lg-push-3 col-sm-12">--%>
                                    <div class="col-md-11 col-sm-10 col-xs-10">
                                        <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control btn btn-info" />
                                    </div>
                                    <div class="col-md-1 col-sm-2 col-xs-2">
                                        <asp:LinkButton ID="btnUpload" runat="server" CssClass="btn btn-info btn-app-xs" OnClick="btnUpload_Click">
                                                <span><i class="fa fa-upload txt-primary"></i></span>
                                        </asp:LinkButton>
                                    </div>
                                    <%--</div>--%>
                                </div>
                            </div>
                        </div>
                        <%--Upload button - end --%>

                        <div style="overflow: auto">
                            <asp:GridView ID="gvGroupPerjawatans" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                CssClass="table table-bordered table-striped table-hover" DataKeyNames="GroupPerjawatanCode"
                                OnRowCommand="gvGroupPerjawatans_RowCommand" OnRowDataBound="gvGroupPerjawatans_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="Code" HeaderStyle-CssClass="treecontainer" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblIndent" runat="server" Text=""></asp:Label>--%>
                                            <asp:LinkButton ID="btnExpand" runat="server" CommandName="Expand" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <%--<span><i class="fa fa-edit"></i></span>--%>
                                            </asp:LinkButton>
                                            <%--<asp:Label ID="lblDetailCode" runat="server" Text=""></asp:Label>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:BoundField DataField="DetailCode" HeaderText="Detail Code" />--%>
                                    <asp:BoundField DataField="GroupPerjawatanDesc" HeaderText="Account Description" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText="Actions" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="50px">
                                        <ItemTemplate>
                                            <div class="row btn-group">
                                                <%--<a class="btn btn-primary" href="#"><i class="fa fa-user fa-fw"></i>User</a>
                                            <a class="btn btn-primary dropdown-toggle" data-toggle="dropdown" href="#">
                                                <span class="fa fa-caret-down"></span></a>--%>
                                                <a class="btn btn-primary dropdown-toggle" data-toggle="dropdown" href="#">
                                                    <span class="fa fa-caret-down"></span></a>
                                                <ul class="dropdown-menu"> 
                                                    <li>
                                                        <asp:LinkButton ID="lbEit" runat="server" CommandName="CmdEdit" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-edit"></i></span> Edit
                                                        </asp:LinkButton>
                                                    </li> 
                                                    <li>
                                                        <asp:LinkButton ID="lbDelete" runat="server" CommandName="CmdDelete" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-trash-o"></i></span> Delete
                                                        </asp:LinkButton>
                                                    </li>       
                                                    <li class="divider"></li>                                           
                                                    <li>
                                                        <asp:LinkButton ID="lbCut" runat="server" CommandName="CmdCut" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-cut"></i></span> Cut
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton ID="lbPaste" runat="server" CommandName="CmdPaste" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-paste"></i></span> Paste
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li class="divider"></li>
                                                    <li>
                                                        <asp:LinkButton ID="lbAddItem" runat="server" Visible="false" CommandName="AddItem" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-plus"></i></span> Add Root
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton ID="lbMakeRoot" runat="server" CommandName="MakeRoot" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-plus"></i></span> Make Root
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton ID="lbAddChild" runat="server" CommandName="AddChild" CommandArgument='<%# Container.DataItemIndex %>'>
                                                        <span><i class="fa fa-level-down"></i></span> Add Child
                                                        </asp:LinkButton>
                                                    </li>
                                                </ul>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div style="height:100px;">

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
        });
    </script>
</asp:Content>
