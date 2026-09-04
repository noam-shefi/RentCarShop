using System;

public partial class Home : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // דף הבית - בדף זה יוצגו הרכבים הפופולריים או מידע כללי על החנות
        if (!IsPostBack)
        {
            // אתחול הכפתור
            if (hlActionBtn != null)
            {
                hlActionBtn.NavigateUrl = "~/Cars.aspx";
                hlActionBtn.Text = "לקטלוג הרכבים";
            }
        }
    }
}
