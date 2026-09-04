using System;
using System.Data;
using System.Web;

public partial class AddReview : System.Web.UI.Page
{
    private int _orderId;
    private int _carId;
    private int _userId;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null)
        {
            Response.Redirect("Login.aspx?returnUrl=" + HttpUtility.UrlEncode(Request.RawUrl));
            return;
        }

        if (!int.TryParse(Request.QueryString["orderId"], out _orderId))
        {
            Response.Redirect("MyOrders.aspx");
            return;
        }

        _userId = GetUserIdByUsername(Session["user"].ToString());
        if (_userId == 0)
        {
            Response.Redirect("MyOrders.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadOrderInfo();
        }
    }

    private void LoadOrderInfo()
    {
        // מוודא שההזמנה שייכת למשתמש, שהיא הושלמה, ושעדיין אין לה ביקורת
        string sql =
            "SELECT o.Id, o.CarId, o.Status, o.EndDate, c.Manufacturer, c.Model " +
            "FROM Orders o JOIN Cars c ON o.CarId = c.Id " +
            "WHERE o.Id = " + _orderId + " AND o.UserId = " + _userId;

        DataTable dt = MyAdoHelper.ExecuteDataTable(sql);

        if (dt.Rows.Count == 0)
        {
            lblMessage.Text = "ההזמנה לא נמצאה.";
            btnSubmit.Visible = false;
            return;
        }

        DataRow row = dt.Rows[0];
        string status = row["Status"].ToString();
        DateTime endDate = Convert.ToDateTime(row["EndDate"]);

        if (status != "מאושר" || endDate.Date >= DateTime.Now.Date)
        {
            lblMessage.Text = "ניתן להשאיר ביקורת רק להזמנות שהושלמו.";
            btnSubmit.Visible = false;
            return;
        }

        bool alreadyReviewed = MyAdoHelper.IsExist("SELECT Id FROM Reviews WHERE OrderId = " + _orderId);
        if (alreadyReviewed)
        {
            lblMessage.Text = "כבר השארת ביקורת עבור הזמנה זו.";
            btnSubmit.Visible = false;
            return;
        }

        _carId = Convert.ToInt32(row["CarId"]);
        string carName = HttpUtility.HtmlEncode(row["Manufacturer"].ToString() + " " + row["Model"].ToString());
        ltrCarInfo.Text = "<p style='margin-bottom:15px;'><strong>רכב:</strong> " + carName + "</p>";

        // שמירת CarId להמשך השימוש ב-PostBack
        ViewState["CarId"] = _carId;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (ViewState["CarId"] == null)
        {
            lblMessage.Text = "לא ניתן לשלוח ביקורת עבור הזמנה זו.";
            return;
        }

        int carId = Convert.ToInt32(ViewState["CarId"]);

        // בדיקה חוזרת שאין עדיין ביקורת (מניעת שליחה כפולה בריענון)
        bool alreadyReviewed = MyAdoHelper.IsExist("SELECT Id FROM Reviews WHERE OrderId = " + _orderId);
        if (alreadyReviewed)
        {
            lblMessage.Text = "כבר השארת ביקורת עבור הזמנה זו.";
            btnSubmit.Visible = false;
            return;
        }

        int rating = int.Parse(ddlRating.SelectedValue);
        string comment = txtComment.Text.Trim();

        string sql = "INSERT INTO Reviews (UserId, CarId, OrderId, Rating, Comment, ReviewDate) VALUES (" +
                     _userId + ", " + carId + ", " + _orderId + ", " + rating + ", N'" + comment.Replace("'", "''") + "', GETDATE())";

        MyAdoHelper.DoQuery(sql);

        // PRG - הפניה אחרי POST למניעת שליחה כפולה בריענון
        Response.Redirect("MyOrders.aspx?reviewMsg=ok");
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