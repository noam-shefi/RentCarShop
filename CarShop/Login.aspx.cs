using System;
using System.Data;
using System.Data.SqlClient;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();

        // Fetch user by username only
        string sql = "SELECT * FROM Users WHERE Username = @Username";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Username", username)
        };
        DataTable dt = MyAdoHelper.ExecuteDataTable(sql, parameters);

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            string storedPassword = row["Password"].ToString();
            int userId = Convert.ToInt32(row["Id"]);

            bool passwordMatches = false;

            // Try hashed password verification first (new format)
            try
            {
                passwordMatches = PasswordHelper.VerifyPassword(password, storedPassword);
            }
            catch
            {
                // If hashing fails, fall back to plaintext comparison for backward compatibility
                passwordMatches = false;
            }

            // Fallback to plaintext comparison for existing accounts during migration
            if (!passwordMatches && storedPassword == password)
            {
                passwordMatches = true;

                // Auto-upgrade to hashed password
                try
                {
                    string hashedPassword = PasswordHelper.HashPassword(password);
                    SqlParameter[] updateParams = new SqlParameter[] 
                    { 
                        new SqlParameter("@Password", hashedPassword),
                        new SqlParameter("@UserId", userId)
                    };
                    MyAdoHelper.DoQuery("UPDATE Users SET Password = @Password WHERE Id = @UserId", updateParams);
                }
                catch { } // Silently continue even if auto-upgrade fails
            }

            if (passwordMatches)
            {
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
                return;
            }
        }

        lblError.Text = "שם המשתמש או הסיסמה שגויים";
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