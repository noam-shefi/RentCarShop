<%@ Page Title="הרשמה" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function validateRegisterForm() {
            var username = document.getElementById('<%= txtUsername.ClientID %>').value;
            var email = document.getElementById('<%= txtEmail.ClientID %>').value;
            var password = document.getElementById('<%= txtPassword.ClientID %>').value;
            var confirmPassword = document.getElementById('<%= txtConfirmPassword.ClientID %>').value;

            if (username.length <= 2) {
                alert("שם המשתמש חייב להכיל יותר מ-2 תווים");
                return false;
            }

            if (email.indexOf("@") === -1) {
                alert("כתובת האימייל אינה תקינה");
                return false;
            }

            if (password.length < 6) {
                alert("הסיסמה חייבת להכיל לפחות 6 תווים");
                return false;
            }

            if (password !== confirmPassword) {
                alert("הסיסמה ואימות הסיסמה אינם תואמים");
                return false;
            }

            return true;
        }
    </script>

    <div class="form-container">
        <h2>הרשמה</h2>

        <label>שם משתמש</label>
        <asp:TextBox ID="txtUsername" runat="server" placeholder="בחר שם משתמש..."></asp:TextBox>

        <label>שם פרטי</label>
        <asp:TextBox ID="txtFirstName" runat="server" placeholder="הקלד שם פרטי..."></asp:TextBox>

        <label>שם משפחה</label>
        <asp:TextBox ID="txtLastName" runat="server" placeholder="הקלד שם משפחה..."></asp:TextBox>

        <label>אימייל</label>
        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="הקלד אימייל..."></asp:TextBox>

        <label>טלפון</label>
        <asp:TextBox ID="txtPhone" runat="server" placeholder="הקלד טלפון..."></asp:TextBox>

        <label>סיסמה</label>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="בחר סיסמה..."></asp:TextBox>

        <label>אימות סיסמה</label>
        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder="אימות את הסיסמה..."></asp:TextBox>

        <asp:Label ID="lblError" runat="server" CssClass="error-message"></asp:Label>

        <asp:Button ID="btnRegister" runat="server" Text="הירשם" CssClass="btn"
            OnClientClick="return validateRegisterForm();" OnClick="btnRegister_Click" />

        <div style="text-align: center; margin-top: 20px;">
            <p>כבר יש לך חשבון? <asp:HyperLink ID="hlLogin" NavigateUrl="~/Login.aspx" runat="server">התחבר</asp:HyperLink></p>
        </div>
    </div>

</asp:Content>
