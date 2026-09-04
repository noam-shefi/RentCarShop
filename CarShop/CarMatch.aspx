<%@ Page Title="התאמת רכב חכמה" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="CarMatch.aspx.cs" Inherits="CarShop.CarMatch" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="section">
        <h2>מצא את הרכב המושלם לטיול שלך</h2>
        <p style="color:var(--color-text-light); margin-bottom: 20px;">
            ספר לנו קצת על הטיול המתוכנן שלך (יעד, מספר נוסעים, כמות מטען, סוג הנסיעה וכו') ונציע לך את הרכב המתאים ביותר מתוך המלאי שלנו.
        </p>
    </div>

    <div class="form-container" style="max-width: 650px;">
        <label>פרטי הטיול שלך</label>
        <asp:TextBox
            ID="txtTripDetails"
            runat="server"
            TextMode="MultiLine"
            Rows="6"
            placeholder="לדוגמה: אנחנו משפחה עם שני ילדים קטנים, נוסעים לטיול קמפינג בצפון לשבוע, צריכים המון מקום למטען..."
            style="width:100%; resize:vertical;">
        </asp:TextBox>

        <asp:Button
            ID="btnFindCar"
            runat="server"
            Text="מצא לי רכב מתאים"
            CssClass="btn btn-main"
            OnClick="btnFindCar_Click"
            style="margin-top:16px; width:100%;" />

        <asp:Label
            ID="lblStatus"
            runat="server"
            style="display:block; margin-top:12px; font-size:14px;">
        </asp:Label>

        <div id="resultBox" runat="server" visible="false" style="margin-top:20px; padding:18px; background-color:#f8fafc; border:1px solid var(--color-gray-dark); border-radius:10px;">
            <h3 style="margin-bottom:10px; color:var(--color-black);">ההמלצה שלנו</h3>
            <asp:Label ID="lblResult" runat="server" style="line-height:1.8; display:block;"></asp:Label>
        </div>
    </div>
</asp:Content>