using System;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // זהו דף ברירת המחדל של האתר (מוגדר גם ב-Web.config תחת defaultDocument).
        // תפקידו היחיד הוא להעביר מיד לדף הבית האמיתי, Home.aspx.
        Response.Redirect("Home.aspx");
    }
}
