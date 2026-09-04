<%@ Page Title="פרטי רכב" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="CarDetails.aspx.cs" Inherits="CarDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- ספריות העיצוב והקוד של Flatpickr ליומן מודרני -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    <link rel="stylesheet" type="text/css" href="https://npmcdn.com/flatpickr/dist/themes/airbnb.css">
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <script src="https://npmcdn.com/flatpickr/dist/l10n/he.js"></script>

    <div class="car-details-container" style="max-width: 1000px; margin: 30px auto; background: #fff; padding: 30px; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); border: 1px solid #e2e8f0; direction: rtl;">
        
        <!-- בחירת תאריכים בחלק העליון -->
        <div id="rentalForm" runat="server" visible="false" style="background: #f8fafc; border: 1px solid #cbd5e1; border-radius: 12px; padding: 20px; margin-bottom: 25px;">
            <div style="font-weight: 700; color: #0f172a; margin-bottom: 12px; font-size: 1.05rem;">📅 בחירת תאריכי השכרה:</div>
            
            <asp:HiddenField ID="hfBlockedDates" runat="server" />

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px;">
                <div>
                    <label for="txtStartDate" style="display:block; margin-bottom:4px; font-size:0.85rem; color:#475569; font-weight:600;">תאריך איסוף</label>
                    <input type="text" id="txtStartDate" runat="server" placeholder="בחר תאריך איסוף..." style="width:100%; padding:10px; border:1px solid #cbd5e1; border-radius:8px; background:#fff; color:#0f172a; font-weight:500; cursor:pointer;" readonly />
                </div>
                <div>
                    <label for="txtEndDate" style="display:block; margin-bottom:4px; font-size:0.85rem; color:#475569; font-weight:600;">תאריך החזרה</label>
                    <input type="text" id="txtEndDate" runat="server" placeholder="בחר תאריך החזרה..." style="width:100%; padding:10px; border:1px solid #cbd5e1; border-radius:8px; background:#fff; color:#0f172a; font-weight:500; cursor:pointer;" readonly />
                </div>
            </div>
            
            <asp:Label ID="lblRentMessage" runat="server" CssClass="error-message" style="display:block; margin-top:8px; color:#ef4444; font-weight:600;"></asp:Label>
        </div>

        <!-- הצגת פרטי הרכב, התמונה ושורת הכפתורים -->
        <asp:Literal ID="ltrCarDetails" runat="server"></asp:Literal>

        <!-- כפתור ההשכרה הישיר של ASP.NET ושאר הכפתורים -->
        <div style="display:flex; gap:10px; align-items:center; flex-wrap:wrap; margin-top:20px;">
            <asp:Button ID="btnRent" runat="server" Text="השכר רכב זה" OnClick="btnRent_Click" CssClass="btn" Style="background-color:#1e293b; color:#fff; font-weight:600;" />
            <asp:Literal ID="ltrFavButton" runat="server"></asp:Literal>
            <a href="Cars.aspx" class="btn" style="background:#64748b; color:#fff;">חזרה לקטלוג</a>
        </div>
    </div>

    <!-- Modal לסיכום הזמנה -->
    <div id="orderModal" runat="server" visible="false" class="modal-overlay">
        <div class="modal-content">
            <h2>סיכום הזמנה</h2>
            <p><strong>רכב:</strong> <asp:Label ID="lblModalCar" runat="server"></asp:Label></p>
            <p><strong>תאריכים:</strong> <asp:Label ID="lblModalDates" runat="server"></asp:Label></p>
            <p><strong>מחיר ליום:</strong> <asp:Label ID="lblModalDailyPrice" runat="server"></asp:Label></p>
            <p style="margin-top:15px;"><strong>סה"כ לתשלום:</strong> <asp:Label ID="lblModalTotalPrice" runat="server" Font-Bold="true" Font-Size="Large" ForeColor="#1e293b"></asp:Label></p>

            <div style="margin-top: 20px; background:#f8fafc; padding:15px; border-radius:6px;">
                <label style="display:block; margin-bottom:5px; font-weight:600;">שלח קבלה לאימייל (אופציונלי):</label>
                <asp:TextBox ID="txtEmail" runat="server" placeholder="הכנס כתובת Email..." style="width:100%; padding:8px; border:1px solid #cbd5e1; border-radius:4px; box-sizing:border-box;"></asp:TextBox>
            </div>

            <asp:Label ID="lblModalError" runat="server" ForeColor="Red" style="display:block; margin-top:10px;"></asp:Label>

            <div class="card-actions" style="margin-top:25px; display:flex; gap:10px; justify-content:space-between;">
                <asp:Button ID="btnConfirmOrder" runat="server" Text="אשר הזמנה" CssClass="btn btn-main" OnClick="btnConfirmOrder_Click" style="background:#1e293b; color:#fff;" />
                <asp:Button ID="btnConfirmAndEmail" runat="server" Text="אשר ושלח למייל" CssClass="btn btn-edit" OnClick="btnConfirmAndEmail_Click" style="background:#475569; color:#fff;" />
                <asp:Button ID="btnCancelModal" runat="server" Text="חזור" CssClass="btn btn-delete" OnClick="btnCancelModal_Click" style="background:#64748b; color:#fff;" />
            </div>
        </div>
    </div>

    <!-- Modal הצלחה -->
    <div id="successModal" runat="server" visible="false" class="modal-overlay">
        <div class="modal-content" style="text-align: center; padding: 40px;">
            <div style="font-size: 48px; color: #16a34a; margin-bottom: 15px;">✓</div>
            <h2 style="border: none; color: #1e293b;">ההזמנה בוצעה בהצלחה!</h2>
            <p style="color: #64748b; margin-bottom: 25px;">בקשת ההשכרה שלך נקלטה במערכת בהצלחה.</p>
            <a href="Cars.aspx" class="btn btn-main" style="display: inline-block; padding: 10px 25px; text-decoration: none; background:#1e293b;">חזרה לקטלוג הרכבים</a>
        </div>
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var blockedDatesElem = document.getElementById('<%= hfBlockedDates.ClientID %>');
            var blockedDatesStr = blockedDatesElem ? blockedDatesElem.value : "";
            var blockedDates = blockedDatesStr ? JSON.parse(blockedDatesStr) : [];

            var startPicker = document.getElementById('<%= txtStartDate.ClientID %>');
            if (startPicker) {
                flatpickr("#<%= txtStartDate.ClientID %>", {
                    locale: "he",
                    minDate: "today",
                    disable: blockedDates,
                    dateFormat: "Y-m-d"
                });
            }

            var endPicker = document.getElementById('<%= txtEndDate.ClientID %>');
            if (endPicker) {
                flatpickr("#<%= txtEndDate.ClientID %>", {
                    locale: "he",
                    minDate: new Date().fp_incr(1),
                    disable: blockedDates,
                    dateFormat: "Y-m-d"
                });
            }
        });
    </script>
</asp:Content>