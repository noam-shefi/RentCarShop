<%@ Page Title="קטלוג רכבים" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Cars.aspx.cs" Inherits="Cars" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- ספריות Flatpickr ליומן מעוצב וחכם -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    <link rel="stylesheet" type="text/css" href="https://npmcdn.com/flatpickr/dist/themes/airbnb.css">
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <script src="https://npmcdn.com/flatpickr/dist/l10n/he.js"></script>

    <div class="section">
        <h2>קטלוג הרכבים שלנו</h2>
    </div>

    <div class="search-bar">
        <asp:TextBox ID="txtSearch" runat="server" placeholder="חיפוש לפי יצרן או דגם..." onkeyup="filterCarsLive(this.value)" autocomplete="off"></asp:TextBox>

        <asp:DropDownList ID="ddlCategory" runat="server">
            <asp:ListItem Text="כל הקטגוריות" Value=""></asp:ListItem>
            <asp:ListItem Text="סדאן" Value="סדאן"></asp:ListItem>
            <asp:ListItem Text="ג'יפ" Value="ג'יפ"></asp:ListItem>
            <asp:ListItem Text="ספורט" Value="ספורט"></asp:ListItem>
            <asp:ListItem Text="משפחתי" Value="משפחתי"></asp:ListItem>
        </asp:DropDownList>

        <!-- שינוי סוג התיבות ל-text כדי למנוע את היומן המובנה והמכוער של הכרום -->
        <asp:TextBox ID="txtSearchStart" runat="server" placeholder="תאריך איסוף" type="text" style="width: 140px; border-radius:6px; border:1px solid #cbd5e1; padding:10px; background:#fff; cursor:pointer;" readonly></asp:TextBox>
        <asp:TextBox ID="txtSearchEnd" runat="server" placeholder="תאריך החזרה" type="text" style="width: 140px; border-radius:6px; border:1px solid #cbd5e1; padding:10px; background:#fff; cursor:pointer;" readonly></asp:TextBox>

        <asp:TextBox ID="txtCustomPrice" runat="server" placeholder="מחיר מקסימלי (₪)" type="number" min="0" step="50" style="width: 140px;"></asp:TextBox>

        <asp:Button ID="btnSearch" runat="server" Text="חפש" CssClass="btn" OnClick="btnSearch_Click" />
    </div>

    <div class="card-grid" id="cardGrid">
        <asp:Literal ID="ltrCards" runat="server"></asp:Literal>
    </div>

    <p id="noLiveResults" style="display:none; text-align:center; padding:40px; font-size:16px; color:#64748b;">לא נמצאו רכבים התואמים לחיפוש שלך.</p>

    <!-- הפעלת היומן המעוצב בשורת החיפוש -->0
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            flatpickr("#<%= txtSearchStart.ClientID %>", {
                locale: "he",
                minDate: "today",
                dateFormat: "Y-m-d"
            });

            flatpickr("#<%= txtSearchEnd.ClientID %>", {
                locale: "he",
                minDate: new Date().fp_incr(1),
                dateFormat: "Y-m-d"
            });
        });

        // סינון חי של הכרטיסים תוך כדי הקלדה - ללא רענון עמוד
        function filterCarsLive(query) {
            query = query.trim().toLowerCase();
            var cards = document.querySelectorAll('#cardGrid .card');
            var visibleCount = 0;

            cards.forEach(function (card) {
                var text = card.getAttribute('data-search') || '';
                var match = text.indexOf(query) !== -1;
                card.style.display = match ? '' : 'none';
                if (match) visibleCount++;
            });

            var noResultsElem = document.getElementById('noLiveResults');
            if (noResultsElem) {
                noResultsElem.style.display = (visibleCount === 0 && cards.length > 0) ? 'block' : 'none';
            }
        }
    </script>

</asp:Content>