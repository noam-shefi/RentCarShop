<%@ Page Title="התחברות" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="form-container">
        <h2>התחברות</h2>

        <label>שם משתמש</label>
        <asp:TextBox ID="txtUsername" runat="server" placeholder="הקלד שם משתמש..."></asp:TextBox>

        <label>סיסמה</label>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="הקלד סיסמה..."></asp:TextBox>

        <asp:Label ID="lblError" runat="server" CssClass="error-message"></asp:Label>

        <asp:Button ID="btnLogin" runat="server" Text="התחבר" CssClass="btn" OnClick="btnLogin_Click" />

        <div style="text-align: center; margin-top: 20px;">
            <p>עדיין אין לך חשבון? <asp:HyperLink ID="hlRegister" runat="server" NavigateUrl="~/Register.aspx">הירשם כאן</asp:HyperLink></p>
        </div>
    </div>

</asp:Content>
