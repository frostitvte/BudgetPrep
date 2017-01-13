<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SegmentSetup.aspx.cs" Inherits="BudgetPrep.SegmentSetup" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

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
                        <i class="fa fa-arrows"></i><span>Edit</span>
                    </div>
                    <div class="box-icons">
                        <a class="collapse-link"><i class="fa fa-chevron-up"></i></a><a class="expand-link"><i class="fa fa-expand"></i></a><%--<a class="close-link">
                    <i class="fa fa-times"></i>
                </a>--%>
                    </div>
                    <div class="no-move">
                    </div>
                </div>
                <div id="Div1" class="box-content">
                    <%--<h4 class="page-header">Registration form</h4>--%>
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblSegmentName" runat="server" class="control-label">Segment Name</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbSegName" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbSegName" ForeColor="Red"
                                            CssClass="failureNotification" ErrorMessage="Segment Name is required." ToolTip="Segment Name is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblShapeFormat" runat="server" class="control-label">Shape Format</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbSegFormat" runat="server" CssClass="form-control" onkeypress="return IsQuestionKey(event);"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbSegFormat" ForeColor="Red"
                                            CssClass="failureNotification" ErrorMessage="Shape Format is required." ToolTip="Shape Format is required." Display="Dynamic"
                                            ValidationGroup="SaveValidation"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-lg-push-3 col-sm-12">
                                    <div class="col-md-12 col-lg-4">
                                        <asp:Label ID="lblSegmentOrder" runat="server" class="control-label">Segment Order</asp:Label>
                                    </div>
                                    <div class="col-md-12 col-lg-8">
                                        <asp:TextBox ID="tbSegOrder" runat="server" CssClass="form-control" onkeypress="return IsNumberKey(event);"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbSegOrder" ForeColor="Red"
                                            CssClass="failureNotification" ErrorMessage="Segment Order is required." ToolTip="Segment Order is required." Display="Dynamic"
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
                                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
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
                                <%--<asp:LinkButton ID="btnMailTest" runat="server" CssClass="btn btn-label-left" OnClick="btnMailTest_Click">
                                    <span><i class="fa fa-print txt-info"></i></span>test
                                </asp:LinkButton>--%>&nbsp;
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="box">
                <div class="box-header">
                    <div class="box-name">
                        <i class="fa fa-arrows"></i><span>View</span>
                    </div>
                    <div class="box-icons">
                        <a class="collapse-link"><i class="fa fa-chevron-up"></i></a><a class="expand-link"><i class="fa fa-expand"></i></a><%--<a class="close-link">
                    <i class="fa fa-times"></i>
                </a>--%>
                    </div>
                    <div class="no-move">
                    </div>
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
                            <asp:GridView ID="gvSegmentSetup" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                CssClass="table table-bordered table-striped table-hover" DataKeyNames="SegmentID"
                                OnRowCommand="gvSegmentSetup_RowCommand">
                                <Columns>
                                    <%--<asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="gvChk" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                                    <asp:BoundField DataField="SegmentName" HeaderText="Segment Name" />
                                    <asp:BoundField DataField="ShapeFormat" HeaderText="Shape Format" />
                                    <asp:BoundField DataField="SegmentOrder" HeaderText="Segment Order" />
                                    <asp:BoundField DataField="Status" HeaderText="Segment Status" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEditRow" runat="server" CommandName="EditRow" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <span><i class="fa fa-edit"></i></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEditDetails" runat="server" CommandName="EditDetails" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <span><i class="fa fa-info-circle"></i></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
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
                        <div id="capturedShot">
                        </div>
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

            $("input[type=file]").css({
                "position": "relative",
                "text-align": "right",
                "width": "100%",
                "-moz-opacity": "0",
                "filter": "alpha(opacity: 0)",
                "z-index": "2"
            });
        });

        //$('#downloadinner').click(function () {
        //    debugger;
        //    html2canvas($("#mainBox"), {
        //        onrendered: function (canvas) {
        //            debugger;
        //            //Set hidden field's value to image data (base-64 string)
        //            //$('#capturedShot').val(canvas.toDataURL("image/png"));
        //            //document.getElementById("form").submit();
        //            //var div = $('#capturedShot');
        //            document.body.appendChild(canvas);
        //        }
        //    });
        //});

        //$('#download').click(function () {
        //    debugger;
        //    //    html2canvas($("#mainpic"), {
        //    //        onrendered: function (canvas) {
        //    //            debugger;
        //    //            //Set hidden field's value to image data (base-64 string)
        //    //            //$('#capturedShot').val(canvas.toDataURL("image/png"));
        //    //            //document.getElementById("form").submit();
        //    //            var div = $('#capturedShot');
        //    //            document.body.appendChild(canvas);
        //    //        }
        //    //    });

        //    html2canvas($("#canvas"), {
        //        "logging": true, //Enable log (use Web Console for get Errors and Warings)
        //        "proxy": "html2canvasproxy.ashx",
        //        "allowTaint": false,
        //        "onrendered": function (canvas) {
        //            var imgData = canvas.toDataURL('image/png');
        //            var doc = new jsPDF('p', 'mm');
        //            doc.addImage(imgData, 'PNG', 10, 10);
        //            doc.save('sample-file.pdf');
        //        }
        //    });
        //});
    </script>
</asp:Content>
