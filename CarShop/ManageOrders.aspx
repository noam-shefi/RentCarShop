<%@ Page Title="ניהול הזמנות" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ManageOrders.aspx.cs" Inherits="ManageOrders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="section">
        <h2>ניהול הזמנות</h2>
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </div>

    <div style="margin-top: 20px;">
        <asp:Literal ID="ltrOrdersTable" runat="server"></asp:Literal>
    </div>

</asp:Content>
