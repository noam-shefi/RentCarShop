using System;
using System.Data;
using System.Text;
using System.Web;

public partial class MyOrders : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null)
        {
            Response.Redirect("Login.aspx");
            return;
        }

        // טיפול בלחיצה על כפתורי ביטול
        string action = Request.QueryString["action"];
        string orderId = Request.QueryString["id"];

        if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(orderId))
        {
            HandleCancelRequest(action, orderId);
        }

        if (Request.QueryString["reviewMsg"] == "ok")
        {
            lblMessage.Text = "תודה! הביקורת שלך נשלחה בהצלחה.";
            lblMessage.Style["color"] = "#16a34a";
            lblMessage.Style["font-weight"] = "600";
        }

        if (!IsPostBack)
        {
            LoadMyOrders();
        }
    }

    private void HandleCancelRequest(string action, string orderId)
    {
        int userId = GetUserIdByUsername(Session["user"].ToString());
        if (userId == 0) return;

        // בדיקה שאכן ההזמנה שייכת למשתמש הזה ושהסטטוס שלה הוא ממתין
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT OrderDate, Status FROM Orders WHERE Id = " + orderId + " AND UserId = " + userId);

        if (dt.Rows.Count > 0)
        {
            string currentStatus = dt.Rows[0]["Status"].ToString();
            DateTime orderDate = Convert.ToDateTime(dt.Rows[0]["OrderDate"]);
            TimeSpan timePassed = DateTime.Now - orderDate;

            if (currentStatus == "ממתין")
            {
                if (action == "cancel" && timePassed.TotalMinutes <= 10)
                {
                    // ביטול אוטומטי ומיידי (פחות מ-10 דקות)
                    MyAdoHelper.DoQuery("UPDATE Orders SET Status = N'בוטל' WHERE Id = " + orderId);
                }
                else if (action == "requestCancel" && timePassed.TotalMinutes > 10)
                {
                    // בקשת ביטול מהמנהל (עברו יותר מ-10 דקות)
                    MyAdoHelper.DoQuery("UPDATE Orders SET Status = N'בקשת ביטול' WHERE Id = " + orderId);
                }
            }
        }

        // ריענון העמוד ללא הפרמטרים בשורת הכתובת למניעת לחיצה כפולה
        Response.Redirect("MyOrders.aspx");
    }

    private void LoadMyOrders()
    {
        int userId = GetUserIdByUsername(Session["user"].ToString());
        if (userId == 0) return;

        // שליפת ההזמנות כולל תאריך יצירת ההזמנה (OrderDate) וסימון האם כבר קיימת ביקורת
        string sql = "SELECT o.Id, c.Manufacturer, c.Model, o.StartDate, o.EndDate, o.TotalPrice, o.Status, o.OrderDate, " +
                     "(SELECT COUNT(*) FROM Reviews r WHERE r.OrderId = o.Id) AS HasReview " +
                     "FROM Orders o JOIN Cars c ON o.CarId = c.Id " +
                     "WHERE o.UserId = " + userId + " " +
                     "ORDER BY o.Id DESC";

        DataTable dt = MyAdoHelper.ExecuteDataTable(sql);

        if (dt.Rows.Count == 0)
        {
            ltrOrders.Text = "<p class='text-center' style='padding:40px; color:#64748b; font-size:16px;'>אין לך הזמנות כרגע.</p>";
            return;
        }

        StringBuilder html = new StringBuilder();
        html.Append("<table class='data-table'>");
        html.Append("<thead><tr><th>רכב</th><th>מתאריך</th><th>עד תאריך</th><th>מחיר כולל</th><th>סטטוס</th><th>פעולות</th></tr></thead><tbody>");

        foreach (DataRow row in dt.Rows)
        {
            string orderId = row["Id"].ToString();
            string carName = HttpUtility.HtmlEncode(row["Manufacturer"].ToString() + " " + row["Model"].ToString());
            DateTime startDateVal = Convert.ToDateTime(row["StartDate"]);
            DateTime endDateVal = Convert.ToDateTime(row["EndDate"]);
            string start = startDateVal.ToString("dd/MM/yyyy");
            string end = endDateVal.ToString("dd/MM/yyyy");
            string price = Convert.ToDecimal(row["TotalPrice"]).ToString("C2");
            string status = row["Status"].ToString();
            DateTime orderDate = Convert.ToDateTime(row["OrderDate"]);
            bool hasReview = Convert.ToInt32(row["HasReview"]) > 0;

            // חישוב הזמן שעבר מאז יצירת ההזמנה
            TimeSpan timePassed = DateTime.Now - orderDate;

            // עיצוב צבע הסטטוס
            string statusColor = "#475569";
            if (status == "מאושר") statusColor = "#16a34a"; // ירוק
            if (status == "ממתין") statusColor = "#475569"; // אפור
            if (status == "בוטל" || status == "נדחה") statusColor = "#8b6b6b"; // מאווב-ורוד
            if (status == "בקשת ביטול") statusColor = "#475569"; // אפור

            html.Append("<tr>");
            html.Append("<td><strong>" + carName + "</strong></td>");
            html.Append("<td>" + start + "</td>");
            html.Append("<td>" + end + "</td>");
            html.Append("<td><strong>" + price + "</strong></td>");
            html.Append("<td><span style='color:" + statusColor + "; font-weight:600;'>" + status + "</span></td>");

            // עמודת פעולות
            html.Append("<td>");

            bool isCompleted = status == "מאושר" && endDateVal.Date < DateTime.Now.Date;

            if (status == "ממתין")
            {
                if (timePassed.TotalMinutes <= 10)
                {
                    // כפתור ביטול מיידי
                    html.Append("<a href='MyOrders.aspx?action=cancel&id=" + orderId + "' class='btn btn-delete' onclick=\"return confirm('האם אתה בטוח שברצונך לבטל את ההזמנה מידית?');\">ביטול הזמנה</a>");
                }
                else
                {
                    // כפתור בקשת ביטול
                    html.Append("<a href='MyOrders.aspx?action=requestCancel&id=" + orderId + "' class='btn btn-delete' onclick=\"return confirm('עברו למעלה מ-10 דקות, האם לשלוח בקשת ביטול למנהל?');\">בקש ביטול ממנהל</a>");
                }
            }
            else if (status == "בקשת ביטול")
            {
                html.Append("<span style='background-color:var(--color-destructive-soft); color:var(--color-destructive); border:1px solid var(--color-destructive-border); padding:6px 10px; border-radius:6px; font-size:13px; font-weight:600;'>הבקשה בהמתנה למנהל</span>");
            }
            else if (isCompleted && !hasReview)
            {
                html.Append("<a href='AddReview.aspx?orderId=" + orderId + "' class='btn btn-main'>השאר ביקורת</a>");
            }
            else if (isCompleted && hasReview)
            {
                html.Append("<span style='color:#64748b; font-size:13px;'>✓ נשלחה ביקורת</span>");
            }
            else
            {
                html.Append("-");
            }

            html.Append("</td>");
            html.Append("</tr>");
        }

        html.Append("</tbody></table>");
        ltrOrders.Text = html.ToString();
    }

    private int GetUserIdByUsername(string username)
    {
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT Id FROM Users WHERE Username = '" + username.Replace("'", "''") + "'");
        if (dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0]["Id"]);
        }
        return 0;
    }
}