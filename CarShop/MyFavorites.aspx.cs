using System;
using System.Data;
using System.Text;
using System.Web;

public partial class MyFavorites : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null)
        {
            Response.Redirect("Login.aspx?returnUrl=" + HttpUtility.UrlEncode("MyFavorites.aspx"));
            return;
        }

        string action = Request.QueryString["action"];
        if (action == "remove")
        {
            HandleRemove();
            return; // HandleRemove מבצע Redirect בסוף
        }

        if (!IsPostBack)
        {
            LoadFavorites();
        }
    }

    private void HandleRemove()
    {
        int carId;
        if (int.TryParse(Request.QueryString["carId"], out carId))
        {
            int userId = GetUserIdByUsername(Session["user"].ToString());
            MyAdoHelper.DoQuery("DELETE FROM Favorites WHERE UserId = " + userId + " AND CarId = " + carId);
        }
        Response.Redirect("MyFavorites.aspx");
    }

    private void LoadFavorites()
    {
        int userId = GetUserIdByUsername(Session["user"].ToString());

        string sql = "SELECT c.* FROM Favorites f " +
                     "JOIN Cars c ON f.CarId = c.Id " +
                     "WHERE f.UserId = " + userId + " " +
                     "ORDER BY f.AddedDate DESC";

        DataTable dt = MyAdoHelper.ExecuteDataTable(sql);
        ltrFavorites.Text = BuildFavoritesHtml(dt);
    }

    private string BuildFavoritesHtml(DataTable dt)
    {
        if (dt.Rows.Count == 0)
        {
            return "<p class='text-center' style='padding:30px;'>עדיין אין לך רכבים במועדפים. <a href='Cars.aspx'>עיינו בקטלוג</a>.</p>";
        }

        StringBuilder html = new StringBuilder("<div class='card-grid'>");

        foreach (DataRow row in dt.Rows)
        {
            int carId = Convert.ToInt32(row["Id"]);
            string manufacturer = HttpUtility.HtmlEncode(row["Manufacturer"].ToString());
            string model = HttpUtility.HtmlEncode(row["Model"].ToString());
            string imageUrl = (row["ImageUrl"] ?? "").ToString();

            decimal price = 0;
            decimal.TryParse((row["Price"] ?? "0").ToString(), out price);

            html.Append("<div class='card'>");
            html.Append("<img src='" + (string.IsNullOrEmpty(imageUrl) ? "Images/car1.png" : imageUrl) + "' alt='" + manufacturer + " " + model + "' />");
            html.Append("<div class='card-body'>");
            html.Append("<h3>" + manufacturer + " " + model + "</h3>");
            html.Append("<div class='card-price'>" + price.ToString("C2") +
                        " <span style='font-size:13px;color:var(--color-text-light);'>/ יום</span></div>");

            // שורת כפתורים אחידה עם שאר האתר - כחול לפעולה ראשית, אדום להסרה
            html.Append("<div class='card-actions'>");
            html.Append("<a href='CarDetails.aspx?id=" + carId + "' class='btn btn-main'>פרטים והשכרה</a>");
            html.Append("<a href='MyFavorites.aspx?action=remove&carId=" + carId + "' " +
                        "onclick=\"return confirm('להסיר מהמועדפים?');\" " +
                        "class='btn btn-delete'>הסר</a>");
            html.Append("</div>");

            html.Append("</div></div>");
        }

        html.Append("</div>");
        return html.ToString();
    }

    private int GetUserIdByUsername(string username)
    {
        string safeUsername = username.Replace("'", "''");
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT Id FROM Users WHERE Username = '" + safeUsername + "'");
        if (dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0]["Id"]);
        }
        return 0;
    }
}