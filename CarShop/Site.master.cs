using System;
using System.Data;

public partial class Site : System.Web.UI.MasterPage
{
    public string menu = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        BuildMenu();
    }

    private void BuildMenu()
    {
        if (Session["admin"] != null || Session["user"] != null)
        {
            string username = Session["user"] != null ? Session["user"].ToString() : "";
            string initial = GetFirstNameInitial(username);
            string avatarColor = GetAvatarColor(username);

            string profileDropdown =
                "<div class='nav-dropdown corner-avatar'>" +
                    "<button type='button' class='nav-avatar' style='background:" + avatarColor + ";' " +
                    "onclick='toggleDropdown(this)' title='פרופיל'>" + initial + "</button>" +
                    "<div class='nav-dropdown-menu'>" +
                        "<a href='UpdateProfile.aspx'>עדכון פרופיל</a>" +
                        "<a href='Logout.aspx' class='nav-logout'>התנתקות</a>" +
                    "</div>" +
                "</div>";

            if (Session["admin"] != null)
            {
                menu = "<a href='Home.aspx'>דף בית</a>" +
                       "<a href='Cars.aspx'>קטלוג רכבים</a>" +
                       "<a href='CarMatch.aspx'>התאמת רכב </a>" +
                       "<a href='MyOrders.aspx'>ההזמנות שלי</a>" +
                       "<a href='MyFavorites.aspx'>המועדפים שלי</a>" +
                       "<a href='Admin.aspx'>פאנל ניהול</a>" +
                       "<a href='AddCar.aspx'>הוספת רכב</a>" +
                       "<a href='ManageStock.aspx'>ניהול מלאי</a>" +
                       "<a href='ManageOrders.aspx'>ניהול הזמנות</a>" +
                       "<a href='ManageBranches.aspx'>ניהול סניפים</a>" +
                       profileDropdown;
            }
            else
            {
                menu = "<a href='Home.aspx'>דף בית</a>" +
                       "<a href='Cars.aspx'>קטלוג רכבים</a>" +
                       "<a href='CarMatch.aspx'>התאמת רכב </a>" +
                       "<a href='MyOrders.aspx'>ההזמנות שלי</a>" +
                       "<a href='MyFavorites.aspx'>המועדפים שלי</a>" +
                       profileDropdown;
            }
        }
        else
        {
            menu = "<a href='Home.aspx'>דף בית</a>" +
                   "<a href='Cars.aspx'>קטלוג רכבים</a>" +
                   "<a href='CarMatch.aspx'>התאמת רכב </a>" +
                   "<a href='Login.aspx'>התחברות</a>" +
                   "<a href='Register.aspx'>הרשמה</a>";
        }
    }

    private string GetFirstNameInitial(string username)
    {
        try
        {
            string safeUsername = username.Replace("'", "''");
            DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT FirstName FROM Users WHERE Username = '" + safeUsername + "'");

            if (dt != null && dt.Rows.Count > 0)
            {
                string firstName = dt.Rows[0]["FirstName"].ToString();
                if (!string.IsNullOrEmpty(firstName))
                {
                    return firstName.Substring(0, 1).ToUpper();
                }
            }
        }
        catch (Exception)
        {
            // אם הייתה שגיאת מסד נתונים, נחזור לאות הראשונה של שם המשתמש כברירת מחדל
        }

        return !string.IsNullOrEmpty(username) ? username.Substring(0, 1).ToUpper() : "?";
    }

    private string GetAvatarColor(string username)
    {
        if (string.IsNullOrEmpty(username)) return "#555"; // צבע ברירת מחדל אם אין שם משתמש

        int hash = 0;
        foreach (char c in username)
        {
            hash = (hash * 31 + c) % 360;
        }
        int hue = Math.Abs(hash) % 360;

        return "hsl(" + hue + ", 65%, 45%)";
    }
}