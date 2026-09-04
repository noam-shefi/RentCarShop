<%@ Page Title="פאנל ניהול" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        /* עיצוב משודרג לטבלת הניהול */
        .admin-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            background-color: #ffffff;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
            border-radius: 12px;
            overflow: hidden;
        }
        .admin-table th {
            background-color: #1e293b; /* כחול כהה כמו ב-Header */
            color: #ffffff;
            text-align: right;
            padding: 16px;
            font-weight: 600;
        }
        .admin-table td {
            padding: 16px;
            border-bottom: 1px solid #e2e8f0;
            color: #334155;
            vertical-align: middle;
        }
        /* אפקט זברה - שורות בצבע מתחלף */
        .admin-table tr:nth-child(even) {
            background-color: #f8fafc;
        }
        .admin-table tr:hover {
            background-color: #f1f5f9;
        }
        
        /* אפקט ריחוף (Hover) על הכפתורים החדשים */
        .admin-btn {
            display: inline-block;
            transition: all 0.2s ease;
        }
        .admin-btn:hover {
            opacity: 0.9;
            transform: translateY(-2px);
            box-shadow: 0 4px 6px rgba(0,0,0,0.15);
        }
    </style>

    <div class="section">
        <h2 style="color: #0f172a; font-weight: 800; margin-bottom: 10px;">פאנל ניהול - רשימת משתמשים</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="success-message" style="display:inline-block; margin-bottom: 15px; font-weight: 600;"></asp:Label>
    </div>

    <div style="margin-top: 15px;">
        <asp:Literal ID="ltrUsersTable" runat="server"></asp:Literal>
    </div>

</asp:Content>