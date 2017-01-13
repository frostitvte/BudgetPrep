<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MailInBox.aspx.cs" Inherits="BudgetPrep.MailInBox" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="box">
        <div class="box-header">
            <div class="box-name">
                <i class="fa fa-arrows"></i>
                <span>Inbox</span>
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

                    <div style="overflow: auto; height: 250px;">
                        <asp:GridView ID="gvMails" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                            CssClass="table table-bordered table-striped table-hover" DataKeyNames="Title,Object" OnRowCommand="gvMails_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="Title" HeaderText="Title" />
                                <asp:BoundField DataField="Object" HeaderText="Object" />
                                <asp:BoundField DataField="NoCount" HeaderText="Count" HeaderStyle-Width="50px" />
                                <asp:BoundField DataField="LastModDateTime" HeaderText="Last Modified" HtmlEncode="false" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}"
                                    HeaderStyle-Width="200px" />
                                <asp:TemplateField HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEditDetails" runat="server" CommandName="Details" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <span><i class="fa fa-info-circle"></i></span>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="box">
        <div class="box-header">
            <div class="box-name">
                <i class="fa fa-arrows"></i>
                <span>Details</span>
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
                <div class="form-group">
                    <div style="overflow: auto; height: 300px;">
                        <asp:GridView ID="gvDetail" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                            CssClass="table table-bordered table-striped table-hover">
                            <Columns>
                                <asp:BoundField DataField="Title" HeaderText="Title" />
                                <asp:BoundField DataField="Object" HeaderText="Object" />
                                <asp:TemplateField HeaderText="Details">
                                    <ItemTemplate>
                                        <div>
                                            <%#Eval("Detail")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ModifiedBy" HeaderText="Modified By" />
                                <asp:BoundField DataField="LastModDateTime" HeaderText="Last Modified" HtmlEncode="false" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="server">
    $(document).ready(function () {
            WinMove();
        });
</asp:Content>
