<%@ Page Title="דף הבית" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <section class="hero">
        <div class="hero-image-container">
            <img src="Images/car1.png" alt="חנות הרכב שלי - השכרת רכבים איכותית" class="hero-image" />
        </div>
        <div class="hero-text">
            <h1>ברוכים הבאים לחנות הרכב שלי</h1>
            <p>
                אצלנו תמצאו את מבחר הרכבים הרחב והאיכותי ביותר להשכרה - מרכבי סדאן משפחתיים
                ועד רכבי ספורט מהיומנים. בוחרים את הרכב, בוחרים את התאריכים, ואנחנו כבר
                נדאג לשאר. מחירים הוגנים ליום, ליווי צמוד לאורך כל תקופת ההשכרה, וגמישות
                מלאה בבחירת תאריכי ההשכרה. עיינו בקטלוג הרכבים שלנו ומצאו את הרכב הבא
                שלכם לנסיעה כבר היום.
            </p>
            <asp:HyperLink ID="hlActionBtn" runat="server" NavigateUrl="~/Cars.aspx" CssClass="btn" Text="לקטלוג הרכבים להשכרה"></asp:HyperLink>
        </div>
    </section>

    <div style="text-align: center; margin: 50px 0;">
        <h2>למה לבחור בנו?</h2>
    </div>

    <div class="card-grid" style="margin-bottom: 40px;">
        <div class="card">
            <div class="card-body">
                <h3>🎯 בחירה ענקית</h3>
                <p>מבחר רחב של מכוניות להשכרה מכל יצרנים מובילים - סדאן, ג'יפ, ספורט ועוד.</p>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <h3>📅 גמישות בתאריכים</h3>
                <p>בוחרים תאריך התחלה וסיום מלוח השנה - השכרה ליום, לשבוע או לכל תקופה שתרצו.</p>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <h3>💰 מחירים הוגנים ליום</h3>
                <p>מחירון שקוף וללא הפתעות - משלמים בדיוק לפי מספר הימים שהזמנתם.</p>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <h3>🔒 ביטחון וביטוח</h3>
                <p>כל רכב בדוק ומגובה בביטוח מלא - שקט נפשי לאורך כל תקופת ההשכרה.</p>
            </div>
        </div>
    </div>

</asp:Content>
