<%@ Page Title="ניהול מלאי" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ManageStock.aspx.cs" Inherits="ManageStock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="section">
        <h2>ניהול מלאי רכבים</h2>
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </div>

    <div style="margin-top: 20px;">
        <asp:Literal ID="ltrStockTable" runat="server"></asp:Literal>
    </div>

    <script>
        function updateStock(carId) {
            var input = document.getElementById('stock_' + carId);
            var value = parseInt(input.value, 10);

            if (isNaN(value) || value < 0) {
                alert('נא להזין מספר תקין (0 ומעלה)');
                return;
            }

            window.location.href = 'ManageStock.aspx?action=update&id=' + carId + '&stock=' + value;
        }
    </script>

</asp:Content>
