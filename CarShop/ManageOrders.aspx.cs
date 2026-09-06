using System;
using System.Data;
using System.Text;
using System.Web;

public partial class ManageOrders : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // דף זה מיועד למנהלים בלבד
        if (Session["admin"] == null)
        {
            Response.Redirect("Home.aspx");
            return;
        }

        // אם התקבלה בקשת שינוי סטטוס (אישור/דחייה/ביטול) - נבצע אותה קודם
        string action = Request.QueryString["action"];
        if (!string.IsNullOrEmpty(action))
        {
            HandleStatusChange(action);
            return; // HandleStatusChange מבצע Redirect בסוף
        }

        // הצגת הודעת הצלחה/שגיאה אחרי הפניה חוזרת
        string msg = Request.QueryString["msg"];
        if (msg == "updated")
        {
            lblMessage.CssClass = "success-message";
            lblMessage.Text = "סטטוס ההזמנה עודכן בהצלחה.";
        }
        else if (msg == "error")
        {
            lblMessage.CssClass = "error-message";
            lblMessage.Text = "אירעה שגיאה בעדכון ההזמנה.";
        }

        if (!IsPostBack)
        {
            LoadOrders();
        }
    }

    /// <summary>
    /// מעדכן את סטטוס ההזמנה (אישור / דחייה / ביטול / אישור ביטול / דחיית ביטול)
    /// </summary>
    private void HandleStatusChange(string action)
    {
        int orderId;
        if (!int.TryParse(Request.QueryString["id"], out orderId))
        {
            Response.Redirect("ManageOrders.aspx?msg=error");
            return;
        }

        string newStatus;
        switch (action)
        {
            case "approve":
                newStatus = "מאושר";
                break;
            case "reject":
                newStatus = "נדחה";
                break;
            case "cancel":
            case "approve_cancel": // אישור בקשת הביטול של הלקוח
                newStatus = "בוטל";
                break;
            case "reject_cancel": // דחיית בקשת הביטול והחזרה למצב מאושר
                newStatus = "מאושר";
                break;
            default:
                Response.Redirect("ManageOrders.aspx?msg=error");
                return;
        }

        MyAdoHelper.DoQuery("UPDATE Orders SET Status = N'" + newStatus + "' WHERE Id = " + orderId);
        Response.Redirect("ManageOrders.aspx?msg=updated");
    }

    private void LoadOrders()
    {
        // מציג לכל הזמנה גם את שם הלקוח וגם את פרטי הרכב
        string sql = "SELECT o.*, u.Username, u.FirstName, u.LastName, c.Manufacturer, c.Model " +
                     "FROM Orders o " +
                     "JOIN Users u ON o.UserId = u.Id " +
                     "JOIN Cars c ON o.CarId = c.Id " +
                     "ORDER BY o.StartDate DESC";

        DataTable dt = MyAdoHelper.ExecuteDataTable(sql);
        ltrOrdersTable.Text = BuildOrdersTableHtml(dt);
    }

    /// <summary>
    /// בונה טבלת HTML של כל ההזמנות באתר כולל טיפול בבקשות ביטול
    /// </summary>
    private string BuildOrdersTableHtml(DataTable dt)
    {
        if (dt.Rows.Count == 0)
        {
            return "<p class='text-center' style='padding:30px;'>אין עדיין הזמנות באתר.</p>";
        }

        StringBuilder html = new StringBuilder();
        html.Append("<table class='data-table'>");
        html.Append("<tr><th>לקוח</th><th>רכב</th><th>מתאריך</th><th>עד תאריך</th><th>מחיר כולל</th><th>סטטוס</th><th>פעולות</th></tr>");

        foreach (DataRow row in dt.Rows)
        {
            int orderId = Convert.ToInt32(row["Id"]);

            string customerName = HttpUtility.HtmlEncode(row["FirstName"].ToString() + " " + row["LastName"].ToString() +
                                                       " (" + row["Username"].ToString() + ")");
            string carName = HttpUtility.HtmlEncode(row["Manufacturer"].ToString() + " " + row["Model"].ToString());
            DateTime startDate = Convert.ToDateTime(row["StartDate"]);
            DateTime endDate = Convert.ToDateTime(row["EndDate"]);
            string status = row["Status"].ToString();

            decimal totalPrice = 0;
            decimal.TryParse(row["TotalPrice"].ToString(), out totalPrice);

            html.Append("<tr>");
            html.Append("<td>" + customerName + "</td>");
            html.Append("<td>" + carName + "</td>");
            html.Append("<td>" + startDate.ToString("dd/MM/yyyy") + "</td>");
            html.Append("<td>" + endDate.ToString("dd/MM/yyyy") + "</td>");
            html.Append("<td>" + totalPrice.ToString("C2") + "</td>");
            html.Append("<td>" + HttpUtility.HtmlEncode(status) + "</td>");

            html.Append("<td>");

            // מקרה 1: הזמנה בסטטוס "בקשת ביטול"
            if (status == "בקשת ביטול")
            {
                html.Append("<a href='ManageOrders.aspx?action=approve_cancel&id=" + orderId + "' " +
                            "onclick=\"return confirm('לאשר את בקשת הביטול של הלקוח?');\" " +
                            "style='display:inline-block; padding:6px 12px; background:var(--color-destructive-soft); color:var(--color-destructive); border:1px solid var(--color-destructive-border); border-radius:6px; text-decoration:none; font-size:13px; font-weight:600; margin-left:6px;'>אשר ביטול</a>");

                html.Append("<a href='ManageOrders.aspx?action=reject_cancel&id=" + orderId + "' " +
                            "onclick=\"return confirm('לדחות את בקשת הביטול ולהשאיר את ההזמנה פעילה?');\" " +
                            "style='display:inline-block; padding:6px 12px; background:#f3f5f6; color:#52667a; border:1px solid #ced7df; border-radius:7px; text-decoration:none; font-size:13px; font-weight:600;'>דחה בקשה</a>");
            }
            // מקרה 2: הזמנה בסטטוס "ממתין"
            else if (status == "ממתין")
            {
                html.Append("<a href='ManageOrders.aspx?action=approve&id=" + orderId + "' " +
                            "onclick=\"return confirm('לאשר את ההזמנה?');\" " +
                            "style='display:inline-block; padding:6px 12px; background:#e8f3ec; color:#35634a; border:1px solid #b9d8c2; border-radius:6px; text-decoration:none; font-size:13px; font-weight:600; margin-left:6px;'>אשר</a>");

                html.Append("<a href='ManageOrders.aspx?action=reject&id=" + orderId + "' " +
                            "onclick=\"return confirm('לדחות את ההזמנה?');\" " +
                            "style='display:inline-block; padding:6px 12px; background:var(--color-destructive-soft); color:var(--color-destructive); border:1px solid var(--color-destructive-border); border-radius:6px; text-decoration:none; font-size:13px; font-weight:600; margin-left:6px;'>דחה</a>");

                html.Append("<a href='ManageOrders.aspx?action=cancel&id=" + orderId + "' " +
                            "onclick=\"return confirm('לבטל את ההזמנה?');\" " +
                            "style='display:inline-block; padding:6px 12px; background:var(--color-destructive-soft); color:var(--color-destructive); border:1px solid var(--color-destructive-border); border-radius:7px; text-decoration:none; font-size:13px; font-weight:600;'>בטל</a>");
            }
            // מקרה 3: הזמנה בסטטוס "מאושר"
            else if (status == "מאושר")
            {
                html.Append("<a href='ManageOrders.aspx?action=cancel&id=" + orderId + "' " +
                            "onclick=\"return confirm('לבטל את ההזמנה?');\" " +
                            "style='display:inline-block; padding:6px 12px; background:var(--color-destructive-soft); color:var(--color-destructive); border:1px solid var(--color-destructive-border); border-radius:7px; text-decoration:none; font-size:13px; font-weight:600;'>בטל</a>");
            }

            html.Append("</td>");
            html.Append("</tr>");
        }

        html.Append("</table>");
        return html.ToString();
    }
}