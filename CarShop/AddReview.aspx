<%@ Page Title="השאר ביקורת" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AddReview.aspx.cs" Inherits="AddReview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="form-container">
        <h2>השאר ביקורת</h2>
        <asp:Literal ID="ltrCarInfo" runat="server"></asp:Literal>

        <asp:Label runat="server" Text="דירוג" AssociatedControlID="ddlRating"></asp:Label>
        <asp:DropDownList ID="ddlRating" runat="server">
            <asp:ListItem Text="5 - מצוין" Value="5"></asp:ListItem>
            <asp:ListItem Text="4 - טוב מאוד" Value="4"></asp:ListItem>
            <asp:ListItem Text="3 - סביר" Value="3"></asp:ListItem>
            <asp:ListItem Text="2 - לא טוב" Value="2"></asp:ListItem>
            <asp:ListItem Text="1 - גרוע" Value="1"></asp:ListItem>
        </asp:DropDownList>

        <asp:Label runat="server" Text="תגובה (אופציונלי)" AssociatedControlID="txtComment"></asp:Label>
        <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="4"></asp:TextBox>

        <br /><br />
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
        <br />
        <asp:Button ID="btnSubmit" runat="server" Text="שלח ביקורת" CssClass="btn" OnClick="btnSubmit_Click" />
    </div>

</asp:Content>
