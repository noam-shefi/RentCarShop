using System;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // איפוס מלא של ה-Session - מנתק גם משתמש רגיל וגם מנהל
        Session.Clear();
        Session.Abandon();

        Response.Redirect("Home.aspx");
    }
}
