<%@ Page Title="ניהול סניפים" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ManageBranches.aspx.cs" Inherits="ManageBranches" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="section" style="margin-top:30px;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
            <h2 style="margin: 0;">סניפים קיימים</h2>
            
            <!-- כפתור פתיחת חלונית ההוספה -->
            <asp:Button ID="btnOpenModal" runat="server" Text="+ סניף חדש" CssClass="btn btn-main" OnClick="btnOpenModal_Click" style="background:#52667a; color:white; padding: 10px 20px;" />
        </div>

        <!-- הודעת הצלחה ראשית שתוצג מעל הטבלה -->
        <asp:Label ID="lblMainMessage" runat="server" style="display:block; margin-bottom:15px; font-weight:bold;"></asp:Label>

        <!-- טבלת הסניפים -->
        <asp:Literal ID="ltrBranches" runat="server"></asp:Literal>
    </div>

    <!-- חלונית קופצת (Modal) להוספת סניף חדש -->
    <div id="addModal" runat="server" visible="false" class="modal-overlay">
        <div class="modal-content" style="max-width: 400px; text-align: right;">
            <h2 style="margin-top:0; color:#1e293b; border-bottom:2px solid #e2e8f0; padding-bottom:10px;">הוספת סניף חדש</h2>

            <asp:Label runat="server" Text="שם הסניף" AssociatedControlID="txtName" style="display:block; margin-top:15px;"></asp:Label>
            <asp:TextBox ID="txtName" runat="server" style="width:100%; padding:8px; border:1px solid #cbd5e1; border-radius:4px; box-sizing:border-box;"></asp:TextBox>

            <asp:Label runat="server" Text="עיר" AssociatedControlID="txtCity" style="display:block; margin-top:10px;"></asp:Label>
            <asp:TextBox ID="txtCity" runat="server" style="width:100%; padding:8px; border:1px solid #cbd5e1; border-radius:4px; box-sizing:border-box;"></asp:TextBox>

            <asp:Label runat="server" Text="כתובת" AssociatedControlID="txtAddress" style="display:block; margin-top:10px;"></asp:Label>
            <asp:TextBox ID="txtAddress" runat="server" style="width:100%; padding:8px; border:1px solid #cbd5e1; border-radius:4px; box-sizing:border-box;"></asp:TextBox>

            <asp:Label runat="server" Text="טלפון" AssociatedControlID="txtPhone" style="display:block; margin-top:10px;"></asp:Label>
            <asp:TextBox ID="txtPhone" runat="server" style="width:100%; padding:8px; border:1px solid #cbd5e1; border-radius:4px; box-sizing:border-box; margin-bottom:15px;"></asp:TextBox>

            <asp:Label ID="lblModalMessage" runat="server" ForeColor="Red" style="display:block; margin-bottom:15px; font-size:14px;"></asp:Label>

            <div style="display:flex; gap:10px; justify-content:space-between;">
                <asp:Button ID="btnAdd" runat="server" Text="שמור סניף" CssClass="btn" style="flex:1; background:#52667a; color:white;" OnClick="btnAdd_Click" />
                <asp:Button ID="btnCloseModal" runat="server" Text="ביטול" CssClass="btn" style="flex:1; background:#718397; color:white;" OnClick="btnCloseModal_Click" />
            </div>
        </div>
    </div>

</asp:Content>