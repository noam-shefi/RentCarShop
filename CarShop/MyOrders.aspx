<%@ Page Title="ההזמנות שלי" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="MyOrders.aspx.cs" Inherits="MyOrders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="section">
        <h2>ההזמנות שלי</h2>
        <!-- הוספת תווית ההודעה החסרה -->
        <asp:Label ID="lblMessage" runat="server" style="display:block; margin-top:10px;"></asp:Label>
    </div>

    <asp:Literal ID="ltrOrders" runat="server"></asp:Literal>

</asp:Content>