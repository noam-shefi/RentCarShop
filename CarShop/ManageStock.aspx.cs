using System;
using System.Data;
using System.Text;
using System.Web;

public partial class ManageStock : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // דף זה מיועד למנהלים בלבד
        if (Session["admin"] == null)
        {
            Response.Redirect("Home.aspx");
            return;
        }

        // אם התקבלה בקשת עדכון כמות - נבצע אותה קודם
        string action = Request.QueryString["action"];
        if (action == "update")
        {
            HandleUpdate();
            return; // HandleUpdate מבצע Redirect בסוף
        }

        // הצגת הודעת הצלחה/שגיאה אחרי הפניה חוזרת
        string msg = Request.QueryString["msg"];
        if (msg == "updated")
        {
            lblMessage.CssClass = "success-message";
            lblMessage.Text = "כמות הרכבים עודכנה בהצלחה.";
        }
        else if (msg == "error")
        {
            lblMessage.CssClass = "error-message";
            lblMessage.Text = "אירעה שגיאה בעדכון הכמות. נא להזין מספר תקין.";
        }

        if (!IsPostBack)
        {
            LoadCars();
        }
    }

    /// <summary>
    /// מעדכן את כמות הרכבים הזמינים (Stock) עבור רכב מסוים, לפי הפרמטרים
    /// שהתקבלו ב-Query String (?action=update&id=5&stock=10), ואז מפנה
    /// בחזרה לדף כדי למנוע עדכון כפול ברענון הדפדפן.
    /// </summary>
    private void HandleUpdate()
    {
        int carId, stock;
        bool idOk = int.TryParse(Request.QueryString["id"], out carId);
        bool stockOk = int.TryParse(Request.QueryString["stock"], out stock);

        if (!idOk || !stockOk || stock < 0)
        {
            Response.Redirect("ManageStock.aspx?msg=error");
            return;
        }

        MyAdoHelper.DoQuery("UPDATE Cars SET Stock = " + stock + " WHERE Id = " + carId);
        Response.Redirect("ManageStock.aspx?msg=updated");
    }

    private void LoadCars()
    {
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT * FROM Cars ORDER BY Manufacturer, Model");
        ltrStockTable.Text = BuildStockTableHtml(dt);
    }

    /// <summary>
    /// בונה טבלת HTML של כל הרכבים, כולל שדה כמות הניתן לעריכה וכפתור
    /// שמירה לכל שורה (מתבצע דרך JavaScript שמנווט עם הערך שהוזן).
    /// </summary>
    private string BuildStockTableHtml(DataTable dt)
    {
        StringBuilder html = new StringBuilder();

        html.Append("<table class='data-table'>");
        html.Append("<tr><th>Id</th><th>יצרן</th><th>דגם</th><th>קטגוריה</th><th>מחיר ליום</th><th>כמות זמינה</th><th></th></tr>");

        foreach (DataRow row in dt.Rows)
        {
            int id = Convert.ToInt32(row["Id"]);
            string manufacturer = HttpUtility.HtmlEncode(row["Manufacturer"].ToString());
            string model = HttpUtility.HtmlEncode(row["Model"].ToString());
            string category = HttpUtility.HtmlEncode(row["Category"].ToString());

            decimal price = 0;
            decimal.TryParse(row["Price"].ToString(), out price);

            int stock = 0;
            int.TryParse(row["Stock"].ToString(), out stock);

            html.Append("<tr>");
            html.Append("<td>" + id + "</td>");
            html.Append("<td>" + manufacturer + "</td>");
            html.Append("<td>" + model + "</td>");
            html.Append("<td>" + category + "</td>");
            html.Append("<td>" + price.ToString("C2") + "</td>");
            html.Append("<td><input type='number' id='stock_" + id + "' value='" + stock +
                        "' min='0' style='width:70px;padding:6px;border:2px solid var(--color-gray-dark);border-radius:6px;' /></td>");
            html.Append("<td><button type='button' class='btn' onclick='updateStock(" + id + ")'>שמור</button></td>");
            html.Append("</tr>");
        }

        html.Append("</table>");
        return html.ToString();
    }
}
