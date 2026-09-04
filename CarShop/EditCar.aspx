<%@ Page Title="עריכת רכב" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="EditCar.aspx.cs" Inherits="EditCar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="form-container">
        <h2>עריכת רכב</h2>
        
        <asp:Label runat="server" Text="יצרן" AssociatedControlID="txtManufacturer"></asp:Label>
        <asp:TextBox ID="txtManufacturer" runat="server"></asp:TextBox>
        
        <asp:Label runat="server" Text="דגם" AssociatedControlID="txtModel"></asp:Label>
        <asp:TextBox ID="txtModel" runat="server"></asp:TextBox>
        
        <asp:Label runat="server" Text="שנת ייצור" AssociatedControlID="txtYear"></asp:Label>
        <asp:TextBox ID="txtYear" runat="server"></asp:TextBox>
        
        <asp:Label runat="server" Text="מחיר ליום השכרה (₪)" AssociatedControlID="txtPrice"></asp:Label>
        <asp:TextBox ID="txtPrice" runat="server"></asp:TextBox>
        
        <asp:Label runat="server" Text="קטגוריה" AssociatedControlID="ddlCategory"></asp:Label>
        <asp:DropDownList ID="ddlCategory" runat="server">
            <asp:ListItem Text="סדאן" Value="סדאן"></asp:ListItem>
            <asp:ListItem Text="ג'יפ" Value="ג'יפ"></asp:ListItem>
            <asp:ListItem Text="ספורט" Value="ספורט"></asp:ListItem>
            <asp:ListItem Text="משפחתי" Value="משפחתי"></asp:ListItem>
        </asp:DropDownList>
        
        <asp:Label runat="server" Text="סניף" AssociatedControlID="ddlBranch"></asp:Label>
        <asp:DropDownList ID="ddlBranch" runat="server">
            <asp:ListItem Text="-- ללא סניף --" Value=""></asp:ListItem>
        </asp:DropDownList>
        
        <asp:Label runat="server" Text="קישור לתמונה" AssociatedControlID="txtImageUrl"></asp:Label>
        <asp:TextBox ID="txtImageUrl" runat="server"></asp:TextBox>
        
        <asp:Label runat="server" Text="תיאור" AssociatedControlID="txtDescription"></asp:Label>
        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4"></asp:TextBox>
        
        <asp:Label runat="server" Text="כמות יחידות זמינות להשכרה" AssociatedControlID="txtStock"></asp:Label>
        <asp:TextBox ID="txtStock" runat="server"></asp:TextBox>

        <!-- ===== מפרט טכני חדש - לשימוש התאמת ה-AI ===== -->
        <h3 style="margin-top:24px; margin-bottom:8px; color:var(--color-black);">מפרט טכני</h3>

        <asp:Label runat="server" Text="מספר מושבים" AssociatedControlID="txtSeats"></asp:Label>
        <asp:TextBox ID="txtSeats" runat="server"></asp:TextBox>

        <asp:Label runat="server" Text="סוג דלק" AssociatedControlID="ddlFuelType"></asp:Label>
        <asp:DropDownList ID="ddlFuelType" runat="server">
            <asp:ListItem Text="בנזין" Value="בנזין"></asp:ListItem>
            <asp:ListItem Text="דיזל" Value="דיזל"></asp:ListItem>
            <asp:ListItem Text="היברידי" Value="היברידי"></asp:ListItem>
            <asp:ListItem Text="חשמלי" Value="חשמלי"></asp:ListItem>
        </asp:DropDownList>

        <asp:Label runat="server" Text="טווח נסיעה במיכל/בטעינה מלאה (ק&quot;מ)" AssociatedControlID="txtRangeKm"></asp:Label>
        <asp:TextBox ID="txtRangeKm" runat="server"></asp:TextBox>

        <asp:Label runat="server" Text="תיבת הילוכים" AssociatedControlID="ddlTransmission"></asp:Label>
        <asp:DropDownList ID="ddlTransmission" runat="server">
            <asp:ListItem Text="אוטומטית" Value="אוטומטית"></asp:ListItem>
            <asp:ListItem Text="ידנית" Value="ידנית"></asp:ListItem>
        </asp:DropDownList>

        <asp:Label runat="server" Text="קיבולת מטען (מספר מזוודות גדולות)" AssociatedControlID="txtLuggageCapacity"></asp:Label>
        <asp:TextBox ID="txtLuggageCapacity" runat="server"></asp:TextBox>

        <br /><br />
        <asp:Label ID="lblMessage" runat="server" CssClass="success-message"></asp:Label>
        <br />
        <asp:Button ID="btnSave" runat="server" Text="שמור שינויים" CssClass="btn" OnClick="btnSave_Click" />
        <a href="Cars.aspx" class="btn" style="background:#555;color:#fff;display:inline-block;margin-top:12px;">חזרה לקטלוג</a>
    </div>
</asp:Content>