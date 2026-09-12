using System;
using System.Data;
using System.Data.SqlClient;
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
            "WHERE o.Id = @OrderId AND o.UserId = @UserId";

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@OrderId", _orderId),
            new SqlParameter("@UserId", _userId)
        };
        DataTable dt = MyAdoHelper.ExecuteDataTable(sql, parameters);

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

        SqlParameter[] checkReviewParams = new SqlParameter[] { new SqlParameter("@OrderId", _orderId) };
        bool alreadyReviewed = MyAdoHelper.IsExist("SELECT Id FROM Reviews WHERE OrderId = @OrderId", checkReviewParams);
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
        SqlParameter[] checkParams = new SqlParameter[] { new SqlParameter("@OrderId", _orderId) };
        bool alreadyReviewed = MyAdoHelper.IsExist("SELECT Id FROM Reviews WHERE OrderId = @OrderId", checkParams);
        if (alreadyReviewed)
        {
            lblMessage.Text = "כבר השארת ביקורת עבור הזמנה זו.";
            btnSubmit.Visible = false;
            return;
        }

        int rating = int.Parse(ddlRating.SelectedValue);
        string comment = txtComment.Text.Trim();

        SqlParameter[] reviewParams = new SqlParameter[]
        {
            new SqlParameter("@UserId", _userId),
            new SqlParameter("@CarId", carId),
            new SqlParameter("@OrderId", _orderId),
            new SqlParameter("@Rating", rating),
            new SqlParameter("@Comment", comment)
        };

        string sql = "INSERT INTO Reviews (UserId, CarId, OrderId, Rating, Comment, ReviewDate) VALUES (@UserId, @CarId, @OrderId, @Rating, @Comment, GETDATE())";
        MyAdoHelper.DoQuery(sql, reviewParams);

        // PRG - הפניה אחרי POST למניעת שליחה כפולה בריענון
        Response.Redirect("MyOrders.aspx?reviewMsg=ok");
    }

    private int GetUserIdByUsername(string username)
    {
        SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Username", username) };
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT Id FROM Users WHERE Username = @Username", parameters);
        if (dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0]["Id"]);
        }
        return 0;
    }
}