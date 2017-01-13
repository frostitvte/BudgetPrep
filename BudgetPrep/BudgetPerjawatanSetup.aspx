﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BudgetPerjawatanSetup.aspx.cs" Inherits="BudgetPrep.BudgetPerjawatanSetup" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .DDlPanel
        {
            position: fixed;
            z-index: 9999999;
            visibility: hidden;
        }

        .btnContent
        {
            content: "&#9635;";
        }
        
        .PeriodGrid
        {
            margin-top: -21px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnPrint" />
            <asp:PostBackTrigger ControlID="btnSample" />
            <asp:PostBackTrigger ControlID="btnUpload" />
        </Triggers>
        <ContentTemplate>
            <div id="EditBox" runat="server" class="box">
                <div class="box-header">
                    <div class="box-name">
                        <i class="fa fa-search"></i>
                        <span>Search</span>
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
                                <%--col-lg-push-3 --%>
                                <script type="text/javascript">
                                    function cancelClick(e) {
                                        debugger;
                                        if (e.stopPropagation) e.stopPropagation();
                                        e.cancelBubble = true;
                                    }
                                    //$get("pnlSegmentDDL").onclick = function (e) {
                                    //    debugger;
                                    //    if (!e) e = window.event;
                                    //    e.cancelBubble = true;
                                    //};
                                </script>
                                <div class="col-lg-6 col-sm-12" style="overflow: auto; min-height: 300px; max-height: 300px; margin-top: -25px;">
                                    <div class="text-center">
                                        <h4>Segments</h4>
                                    </div>
                                    <asp:GridView ID="gvSegmentDLLs" runat="server" AutoGenerateColumns="false" AllowSorting="true" ShowHeader="false"
                                        CssClass="table table-bordered table-striped" DataKeyNames="SegmentID"
                                        OnRowDataBound="gvSegmentDLLs_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="SegmentName" HeaderText="Segment" />
                                            <asp:TemplateField HeaderText="Value" HeaderStyle-CssClass="treecontainer" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="tbSegmentDDL" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="tbSegmentDDL" ForeColor="Red"
                                                        CssClass="failureNotification" ErrorMessage="Please select Segment." ToolTip="Please select Segment." Display="Dynamic"
                                                        ValidationGroup="SearchValidation"></asp:RequiredFieldValidator>
                                                    <ajaxToolkit:DropDownExtender ID="DropDownExtender1" runat="server" TargetControlID="tbSegmentDDL"
                                                        DropDownControlID="pnlSegmentDDL">
                                                    </ajaxToolkit:DropDownExtender>
                                                    <asp:Panel ID="pnlSegmentDDL" runat="server" OnClientClick="cancelClick(event)" BackColor="#e8e8e8" CssClass="DDlPanel"
                                                        Width="50%" Height="200px" ScrollBars="Auto">
                                                        <asp:TreeView ID="tvSegmentDDL" runat="server" BackColor="#e8e8e8" OnClientClick="cancelClick(event)"
                                                            OnSelectedNodeChanged="tvSegmentDDL_SelectedNodeChanged">
                                                            <%--<Nodes>
                                                                <asp:TreeNode Text="1" SelectAction="Select">
                                                                    <asp:TreeNode Text="2" SelectAction="Select"></asp:TreeNode>
                                                                </asp:TreeNode>
                                                                <asp:TreeNode Text="3" SelectAction="Select"></asp:TreeNode>
                                                            </Nodes>--%>
                                                        </asp:TreeView>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <%--<div style="height:300px;">

                                    </div>--%>
                                </div>
                                <div class="col-lg-6 col-sm-12" style="overflow: auto; max-height: 300px; margin-top: -25px; margin-left: -14px;">
                                    <div class="text-center">
                                        <h4>Period</h4>
                                    </div>
                                    <%--<div class="col-sm-12 col-md-12 col-lg-12">--%>
                                    <%--<div class="well well-sm col-lg-12 col-sm-12">
                                            <div class="col-sm-12 col-md-4 col-lg-4">
                                                <asp:CheckBox ID="chkMedan" runat="server" Text=" Medan" CssClass="form-control" />
                                            </div>
                                            <div class="col-sm-12 col-md-4 col-lg-4">
                                                <asp:CheckBox ID="chkKeterangan" runat="server" Text=" Keterangan" CssClass="form-control" />
                                            </div>
                                            <div class="col-sm-12 col-md-4 col-lg-4">
                                                <asp:CheckBox ID="chkPengiraan" runat="server" Text=" Pengiraan" CssClass="form-control"/>
                                            </div>
                                        </div>--%>
                                    <%--</div>--%>
                                    <table class="table table-bordered table-striped">
                                        <tr>
                                            <td style="width: 70px; text-align: center;">
                                                <asp:CheckBox ID="chkMedan" runat="server" AutoPostBack="true" OnCheckedChanged="chkMedan_CheckedChanged" /></td>
                                            <td>Medan</td>
                                        </tr>
                                        <%--<tr>
                                            <td style="text-align: center;">
                                                <asp:CheckBox ID="chkKeterangan" runat="server" /></td>
                                            <td>Keterangan</td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: center;">
                                                <asp:CheckBox ID="chkPengiraan" runat="server" /></td>
                                            <td>Pengiraan</td>
                                        </tr>--%>
                                    </table>
                                    <asp:GridView ID="gvPeriod" runat="server" AutoGenerateColumns="false" AllowSorting="true" ShowHeader="false"
                                        CssClass="table table-bordered table-striped PeriodGrid" DataKeyNames="PeriodPerjawatanID"
                                        OnRowDataBound="gvPeriod_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Value" HeaderStyle-Width="70px" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="cbPeriodSelect" runat="server"></asp:CheckBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="PerjawatanYear" HeaderText="Perjawatan Year" />
                                            <asp:BoundField DataField="FieldPerjawatanDesc" HeaderText="Description" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <%--<div class="col-sm-12 col-md-6 col-lg-6">
                                    <div class="well well-sm col-lg-12 col-sm-12">
                                        <div class="col-sm-12 col-md-6 col-lg-6">
                                            <asp:CheckBox ID="chkKeterangan" runat="server" Text="Keterangan" CssClass="form-control" />
                                        </div>
                                        <div class="col-sm-12 col-md-6 col-lg-6">
                                            <asp:CheckBox ID="chkPengiraan" runat="server" Text="Pengiraan" CssClass="form-control"/>
                                        </div>
                                    </div>
                                </div>--%>
                                <div class="col-sm-12 col-md-12 col-lg-12">
                                    <div class="pull-right">
                                        <%--<asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-label-left" OnClick="btnAdd_Click">
                                            <span><i class="fa fa-file-o txt-primary"></i></span>Tambah
                                        </asp:LinkButton>                                
                                        <asp:LinkButton ID="btnPrint" runat="server" CssClass="btn btn-label-left" OnClick="btnPrint_Click">
                                            <span><i class="fa fa-print txt-info"></i></span>Cetak
                                        </asp:LinkButton>--%>
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-label-left" ValidationGroup="SearchValidation" OnClick="btnSearch_Click">
                                            <span><i class="fa fa-search txt-success"></i></span>Search
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
                </div>
            </div>
            <div id="BudgetBox" runat="server" class="box" visible="false">
                <div class="box-header">
                    <div class="box-name">
                        <i class="fa fa-indent"></i>
                        <span>Budget</span>
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
                        <%--<div class="well">
                            <h4>Legend</h4>
                            <div class="row">
                                <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #7575ff;">Saved</div>
                                <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #5cffad;">Prepared</div>
                                <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #baff75;">Reviewed</div>
                                <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #33dd11;">Approved</div>
                                <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #dd4dd4;">Reviewer Rejected</div>
                                <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #ff4242;">Approver Rejected</div>
                            </div>
                        </div>--%>
                        <button id="btnSubmit" runat="server" class="btn btn-label-left" type="button" data-toggle="collapse" 
                            data-target="#BulkSubmitExample" aria-expanded="false" aria-controls="BulkSubmitExample">
                            <span><i class="fa fa-upload txt-success"></i></span>Submit
                        </button>
                        <asp:LinkButton ID="btnSearchbox" runat="server" CssClass="btn btn-label-left" Visible="true" OnClick="btnSearchbox_Click">
                            <span><i class="fa fa-filter txt-warning"></i></span>Filter
                        </asp:LinkButton>
                        <button class="btn btn-label-left" type="button" data-toggle="collapse" data-target="#LegendExample" aria-expanded="false" aria-controls="LegendExample">
                            <span><i class="fa fa-list txt-primary"></i></span>Legend
                        </button>

                        <div class="pull-right">
                            <button ID="btnFileUpload" runat="server" class=" btn btn-label-left" type="button" data-toggle="collapse" data-target="#UploadExample" 
                                aria-expanded="false" aria-controls="UploadExample">
                                <span><i class="fa fa-upload txt-primary"></i></span>Upload
                            </button>

                            <asp:LinkButton ID="btnPrint" runat="server" CssClass="btn btn-label-left" Visible="true" OnClick="btnPrint_Click">
                                <span><i class="fa fa-print txt-info"></i></span>Cetak
                            </asp:LinkButton>
                        </div>

                        <div class="collapse" id="LegendExample">
                            <div class="well">
                                <div class="row">
                                    <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #999999;">Saved</div>
                                    <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #ffff00;">Prepared</div>
                                    <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #00ffff;">Reviewed</div>
                                    <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #00ff00;">Approved</div>
                                    <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #ff00ff;">Reviewer Rejected</div>
                                    <div class="col-lg-2 col-md-4 col-sm-4" style="background-color: #ff0000;">Approver Rejected</div>
                                </div>
                            </div>
                        </div>
                        <div class="collapse" id="BulkSubmitExample">
                            <div class="well">
                                <div class="row">
                                    <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                        <div class="col-md-12 col-lg-4">
                                            <label class="control-label">Decision</label>
                                        </div>
                                        <div class="col-md-12 col-lg-8">
                                            <asp:DropDownList ID="ddlBulkDecision" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="Accept" />
                                                <asp:ListItem Text="Reject" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                        <div class="col-md-12 col-lg-4">
                                            <label class="control-label">Remarks</label>
                                        </div>
                                        <div class="col-md-12 col-lg-8">
                                            <asp:TextBox ID="tbBulkRemarks" runat="server" TextMode="MultiLine" Height="60px" Text="" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                        <div class="col-md-12 col-lg-4">
                                        </div>
                                        <div class="col-md-12 col-lg-8">
                                            <asp:LinkButton ID="btnBulkUpdate" runat="server" CssClass="btn btn-label-left" OnClick="btnBulkUpdate_Click">
                                                <span><i class="fa fa-save txt-success"></i></span>Simpan
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                         <%--Upload button - start --%>
                        <div class="collapse" id="UploadExample">
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
                        <div style="overflow: auto; width: 99%;" class="center-block">
                            <%--AllowPaging="true" PageSize="2" OnPageIndexChanged="gvGroupPerjawatanCodes_PageIndexChanged"--%>
                            <%--<button class="btn btn-primary btn-sm" style="background-color:#890890">1000.00<span class="badge">5</span></button>      
                            <button class="btn btn-primary btn-xs" style="background-color:#ABCDE0">1000.00<span class="badge">5</span></button>  --%>
                            <asp:GridView ID="gvGroupPerjawatans" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                CssClass="table table-bordered table-striped" DataKeyNames="GroupPerjawatanCode" Visible="true" ShowFooter="true"
                                OnRowCommand="gvGroupPerjawatans_RowCommand" OnRowDataBound="gvGroupPerjawatans_RowDataBound" OnRowCreated="gvGroupPerjawatans_RowCreated">
                                <FooterStyle Font-Bold="true" BackColor="LightGray" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Code" HeaderStyle-CssClass="treecontainer" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnExpand" runat="server" CommandName="Expand" CommandArgument='<%# Container.DataItemIndex %>'>
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
            <script type="text/javascript">
                function CloseModal() {
                    debugger;
                    $('#myDecisionModal').modal('hide');
                    return true;
                }
            </script>
            <div class="modal fade" id="myDecisionModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblDecisionModalTitle" runat="server" Text="Budget Perjawatan"></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblDecisionModalPeriodID" runat="server" Text="" class="control-label" Visible="false"></asp:Label>
                            <asp:Label ID="lblDecisionModalGroupPerjawatanCode" runat="server" Text="" class="control-label" Visible="false"></asp:Label><br />
                            <asp:Label ID="lblDecisionModalAccount" runat="server" Text="" class="control-label"></asp:Label><br />
                            <asp:Label ID="lblDecisionModalPeriod" runat="server" Text="" class="control-label"></asp:Label><br />
                            <asp:Label ID="lblDecisionModalAmount" runat="server" Text="" class="control-label"></asp:Label><br />
                            <hr />
                            <asp:RadioButtonList ID="rbldecision" runat="server">
                                <asp:ListItem Text="Accept" Value="A" Selected="True" />
                                <asp:ListItem Text="Reject" Value="R" />
                            </asp:RadioButtonList>
                            <asp:Label ID="Label1" runat="server" Text="Remarks" class="control-label"></asp:Label>
                            <asp:TextBox ID="tbRemarks" runat="server" TextMode="MultiLine" Height="60px" Text="" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="btnDecisionSave" runat="server" CssClass="btn btn-label-left"
                                OnClientClick="return CloseModal();" OnClick="btnDecisionSave_Click">
                                    <span><i class="fa fa-save txt-success"></i></span>Simpan
                            </asp:LinkButton>
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

            $('input[type=a], label').click(function (e) {
                if (!e) var e = window.event;
                e.cancelBubble = true;
                if (e.stopPropagation) e.stopPropagation();
            });
        });
    </script>
</asp:Content>
