<%@ Page Title="עדכון פרופיל" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="UpdateProfile.aspx.cs" Inherits="UpdateProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="form-container">
        <h2>עדכון פרופיל</h2>

        <asp:Label runat="server" Text="שם פרטי" AssociatedControlID="txtFirstName"></asp:Label>
        <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>

        <asp:Label runat="server" Text="שם משפחה" AssociatedControlID="txtLastName"></asp:Label>
        <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>

        <asp:Label runat="server" Text="אימייל" AssociatedControlID="txtEmail"></asp:Label>
        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"></asp:TextBox>

        <asp:Label runat="server" Text="טלפון" AssociatedControlID="txtPhone"></asp:Label>
        <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>

        <br /><br />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>
        <br />

        <asp:Button ID="btnUpdate" runat="server" Text="עדכן" CssClass="btn" OnClick="btnUpdate_Click" />
    </div>

</asp:Content>
