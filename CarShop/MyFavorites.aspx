<%@ Page Title="המועדפים שלי" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="MyFavorites.aspx.cs" Inherits="MyFavorites" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="section">
        <h2>המועדפים שלי</h2>
    </div>

    <asp:Literal ID="ltrFavorites" runat="server"></asp:Literal>

</asp:Content>
