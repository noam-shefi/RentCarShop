using System;
using System.Data;
using System.Text;
using System.Web;

public partial class Admin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // דף זה מיועד למנהלים בלבד
        if (Session["admin"] == null)
        {
            Response.Redirect("Home.aspx");
            return;
        }

        // אם הגענו לכאן עם פעולה (מחיקה / הפיכה למנהל / הסרת ניהול)
        string action = Request.QueryString["action"];
        if (!string.IsNullOrEmpty(action))
        {
            HandleAction(action);
            return;
        }

        // הצגת הודעות לאחר פעולה
        string msg = Request.QueryString["msg"];
        if (msg == "deleted")
        {
            lblMessage.CssClass = "success-message";
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "המשתמש נמחק בהצלחה.";
        }
        else if (msg == "promoted")
        {
            lblMessage.CssClass = "success-message";
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "המשתמש הפך למנהל בהצלחה.";
        }
        else if (msg == "demoted")
        {
            lblMessage.CssClass = "success-message";
            lblMessage.ForeColor = System.Drawing.Color.Orange;
            lblMessage.Text = "הרשאות הניהול הוסרו בהצלחה.";
        }
        else if (msg == "self")
        {
            lblMessage.CssClass = "error-message";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "לא ניתן לבצע פעולה זו על המשתמש המחובר כרגע.";
        }
        else if (msg == "error")
        {
            lblMessage.CssClass = "error-message";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "אירעה שגיאה בביצוע הפעולה.";
        }

        if (!IsPostBack)
        {
            LoadUsers();
        }
    }

    private void HandleAction(string action)
    {
        int userId;
        if (!int.TryParse(Request.QueryString["id"], out userId))
        {
            Response.Redirect("Admin.aspx?msg=error");
            return;
        }

        // הגנה: מנהל לא יכול למחוק או להסיר הרשאות לעצמו
        string currentUsername = Session["user"] != null ? Session["user"].ToString() : "";
        string targetUsername = GetUsernameById(userId);

        if (!string.IsNullOrEmpty(currentUsername) && currentUsername == targetUsername)
        {
            Response.Redirect("Admin.aspx?msg=self");
            return;
        }

        if (action == "delete")
        {
            MyAdoHelper.DoQuery("DELETE FROM Users WHERE Id = " + userId);
            Response.Redirect("Admin.aspx?msg=deleted");
        }
        else if (action == "promote")
        {
            MyAdoHelper.DoQuery("UPDATE Users SET IsAdmin = 1 WHERE Id = " + userId);
            Response.Redirect("Admin.aspx?msg=promoted");
        }
        else if (action == "demote")
        {
            MyAdoHelper.DoQuery("UPDATE Users SET IsAdmin = 0 WHERE Id = " + userId);
            Response.Redirect("Admin.aspx?msg=demoted");
        }
        else
        {
            Response.Redirect("Admin.aspx");
        }
    }

    private string GetUsernameById(int id)
    {
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT Username FROM Users WHERE Id = " + id);
        if (dt.Rows.Count > 0)
        {
            return dt.Rows[0]["Username"].ToString();
        }
        return "";
    }

    private void LoadUsers()
    {
        string sql = "SELECT * FROM Users";
        DataTable dt = MyAdoHelper.ExecuteDataTable(sql);
        ltrUsersTable.Text = BuildUsersTableHtml(dt);
    }

    private string BuildUsersTableHtml(DataTable dt)
    {
        StringBuilder html = new StringBuilder();

        // שימוש בקלאס admin-table החדש
        html.Append("<table class='admin-table'>");
        html.Append("<tr>");
        html.Append("<th>Id</th><th>שם משתמש</th><th>שם פרטי</th><th>שם משפחה</th><th>אימייל</th><th>מנהל?</th><th>פעולות</th>");
        html.Append("</tr>");

        foreach (DataRow row in dt.Rows)
        {
            int id = Convert.ToInt32(row["Id"]);
            bool isAdmin = Convert.ToBoolean(row["IsAdmin"]);

            html.Append("<tr>");
            html.Append("<td><strong>" + id + "</strong></td>");
            html.Append("<td>" + HttpUtility.HtmlEncode(row["Username"].ToString()) + "</td>");
            html.Append("<td>" + HttpUtility.HtmlEncode(row["FirstName"].ToString()) + "</td>");
            html.Append("<td>" + HttpUtility.HtmlEncode(row["LastName"].ToString()) + "</td>");
            html.Append("<td>" + HttpUtility.HtmlEncode(row["Email"].ToString()) + "</td>");

            // Uniform gray text for Admin status
            string adminText = isAdmin ? "כן" : "לא";
            html.Append("<td><span style='color: #475569;'>" + adminText + "</span></td>");

            html.Append("<td><div style='display:flex; gap:8px;'>");

            // Action Buttons
            if (!isAdmin)
            {
                // Make Admin button (neutral gray background)
                html.Append("<a href='Admin.aspx?action=promote&id=" + id + "' " +
                            "onclick=\"return confirm('להפוך את המשתמש למנהל?');\" " +
                            "class='btn admin-btn' style='background-color:#52667a; color:#fff; border:none; padding:8px 14px; border-radius:7px; font-weight:600; text-decoration:none;'>הפוך למנהל</a>");
            }
            else
            {
                // Remove Admin button (neutral gray background)
                html.Append("<a href='Admin.aspx?action=demote&id=" + id + "' " +
                            "onclick=\"return confirm('להסיר הרשאות ניהול ממשתמש זה?');\" " +
                            "class='btn admin-btn' style='background-color:#52667a; color:#fff; border:none; padding:8px 14px; border-radius:7px; font-weight:600; text-decoration:none;'>הסר ניהול</a>");
            }

            // Delete button (soft mauve background)
            html.Append("<a href='Admin.aspx?action=delete&id=" + id + "' " +
                        "onclick=\"return confirm('למחוק את המשתמש? הפעולה אינה הפיכה.');\" " +
                        "class='btn admin-btn' style='background-color:var(--color-destructive-soft); color:var(--color-destructive); border:1px solid var(--color-destructive-border); padding:8px 14px; border-radius:7px; font-weight:600; text-decoration:none;'>מחק</a>");

            html.Append("</div></td>");
            html.Append("</tr>");
        }

        html.Append("</table>");
        return html.ToString();
    }
}