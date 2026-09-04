using System;
using System.Data;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();

        string sql = "SELECT * FROM Users WHERE Username = '" + username + "' AND Password = '" + password + "'";
        DataTable dt = MyAdoHelper.ExecuteDataTable(sql);

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];

            // אתחול ה-Session עם שם המשתמש שהתחבר
            Session["user"] = row["Username"].ToString();

            // בדיקה האם המשתמש הוא מנהל, ואם כן - אתחול Session נוסף
            bool isAdmin = Convert.ToBoolean(row["IsAdmin"]);
            if (isAdmin)
            {
                Session["admin"] = "yes";
            }

            // אם הגענו לדף ההתחברות מתוך דף אחר (למשל "התחבר כדי להשכיר"
            // בדף פרטי רכב), נחזור בדיוק לאותו דף אחרי התחברות מוצלחת.
            string returnUrl = Request.QueryString["returnUrl"];
            if (IsSafeReturnUrl(returnUrl))
            {
                Response.Redirect(returnUrl);
            }
            else
            {
                Response.Redirect("Home.aspx");
            }
        }
        else
        {
            lblError.Text = "שם המשתמש או הסיסמה שגויים";
        }
    }

    /// <summary>
    /// בודקת של-returnUrl אין ערך שמפנה לאתר חיצוני (Open Redirect) -
    /// מקבלים רק כתובות יחסיות בתוך האתר שלנו, לא כתובות מלאות כמו
    /// http://... או //... שיכולות להפנות למקום זדוני מחוץ לאתר.
    /// </summary>
    private bool IsSafeReturnUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        if (url.Contains("://") || url.StartsWith("//"))
        {
            return false;
        }

        return true;
    }
}