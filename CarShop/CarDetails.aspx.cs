using System;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Web;

public partial class CarDetails : System.Web.UI.Page
{
    private int _carId;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!int.TryParse(Request.QueryString["id"], out _carId))
        {
            Response.Redirect("Cars.aspx");
            return;
        }

        string action = Request.QueryString["action"];
        if (action == "togglefav" && Session["user"] != null)
        {
            HandleToggleFavorite();
            return;
        }

        if (!IsPostBack) LoadCarDetails();
    }

    private void HandleToggleFavorite()
    {
        int userId = GetUserIdByUsername(Session["user"].ToString());
        if (userId == 0) return;

        DataTable existing = MyAdoHelper.ExecuteDataTable("SELECT Id FROM Favorites WHERE UserId = " + userId + " AND CarId = " + _carId);
        if (existing.Rows.Count > 0) MyAdoHelper.DoQuery("DELETE FROM Favorites WHERE UserId = " + userId + " AND CarId = " + _carId);
        else MyAdoHelper.DoQuery("INSERT INTO Favorites (UserId, CarId, AddedDate) VALUES (" + userId + ", " + _carId + ", GETDATE())");

        Response.Redirect("CarDetails.aspx?id=" + _carId);
    }

    private void LoadCarDetails()
    {
        System.Web.UI.WebControls.ContentPlaceHolder cp = (System.Web.UI.WebControls.ContentPlaceHolder)Master.FindControl("MainContent");
        System.Web.UI.WebControls.Literal ltrFavBtn = (System.Web.UI.WebControls.Literal)cp.FindControl("ltrFavButton");

        string sql =
            "SELECT Cars.*, Branches.Name AS BranchName, Branches.City AS BranchCity, " +
            "(SELECT COUNT(*) FROM Orders o WHERE o.CarId = Cars.Id AND o.Status NOT IN (N'בוטל', N'נדחה') AND o.StartDate <= CAST(GETDATE() AS DATE) AND o.EndDate >= CAST(GETDATE() AS DATE)) AS ActiveRentals, " +
            "(SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews r WHERE r.CarId = Cars.Id) AS AvgRating, " +
            "(SELECT COUNT(*) FROM Reviews r WHERE r.CarId = Cars.Id) AS ReviewCount " +
            "FROM Cars LEFT JOIN Branches ON Cars.BranchId = Branches.Id WHERE Cars.Id = " + _carId;

        DataTable dt = MyAdoHelper.ExecuteDataTable(sql);
        if (dt.Rows.Count == 0)
        {
            ltrCarDetails.Text = "<p class='text-error text-center'>הרכב המבוקש לא נמצא.</p><div class='text-center'><a href='Cars.aspx' class='btn'>חזרה לקטלוג</a></div>";
            rentalForm.Visible = false;
            btnRent.Visible = false;
            if (ltrFavBtn != null) ltrFavBtn.Visible = false;
            return;
        }

        DataRow row = dt.Rows[0];
        string manufacturer = HttpUtility.HtmlEncode((row["Manufacturer"] ?? "").ToString());
        string model = HttpUtility.HtmlEncode((row["Model"] ?? "").ToString());
        string year = HttpUtility.HtmlEncode((row["Year"] ?? "").ToString());
        string category = HttpUtility.HtmlEncode((row["Category"] ?? "").ToString());
        string description = HttpUtility.HtmlEncode((row["Description"] ?? "").ToString());
        string imageUrl = (row["ImageUrl"] ?? "").ToString();
        string branchText = row["BranchName"] == DBNull.Value ? "לא צוין" : HttpUtility.HtmlEncode(row["BranchName"] + " (" + row["BranchCity"] + ")");

        // מפרט טכני - עשוי להיות NULL עבור רכבים שטרם עודכנו
        string seats = row["Seats"] != DBNull.Value ? row["Seats"].ToString() : "לא צוין";
        string fuelType = row["FuelType"] != DBNull.Value ? HttpUtility.HtmlEncode(row["FuelType"].ToString()) : "לא צוין";
        string rangeKm = row["RangeKm"] != DBNull.Value ? row["RangeKm"].ToString() + " ק\"מ" : "לא צוין";
        string transmission = row["Transmission"] != DBNull.Value ? HttpUtility.HtmlEncode(row["Transmission"].ToString()) : "לא צוין";
        string luggage = row["LuggageCapacity"] != DBNull.Value ? row["LuggageCapacity"].ToString() : "לא צוין";

        int stock = Convert.ToInt32(row["Stock"] ?? 0);
        int activeRentals = Convert.ToInt32(row["ActiveRentals"] ?? 0);
        int availableNow = Math.Max(0, stock - activeRentals);
        decimal pricePerDay = Convert.ToDecimal(row["Price"] ?? 0);
        int reviewCount = Convert.ToInt32(row["ReviewCount"] ?? 0);
        string ratingText = reviewCount == 0 ? "אין עדיין ביקורות" : "⭐ " + Convert.ToDouble(row["AvgRating"]).ToString("0.0") + " מתוך 5 (" + reviewCount + " ביקורות)";

        // חישוב מערך תאריכים חסומים ליומן (Flatpickr)
        string orderSql = "SELECT StartDate, EndDate FROM Orders WHERE CarId = " + _carId + " AND Status NOT IN (N'בוטל', N'נדחה') AND EndDate >= GETDATE()";
        DataTable dtOrders = MyAdoHelper.ExecuteDataTable(orderSql);

        System.Collections.Generic.Dictionary<DateTime, int> dateCounts = new System.Collections.Generic.Dictionary<DateTime, int>();
        foreach (DataRow r in dtOrders.Rows)
        {
            DateTime s = Convert.ToDateTime(r["StartDate"]);
            DateTime end = Convert.ToDateTime(r["EndDate"]);
            for (DateTime d = s.Date; d <= end.Date; d = d.AddDays(1))
            {
                if (!dateCounts.ContainsKey(d)) dateCounts[d] = 0;
                dateCounts[d]++;
            }
        }

        System.Collections.Generic.List<string> blockedDates = new System.Collections.Generic.List<string>();
        foreach (var kvp in dateCounts)
        {
            if (kvp.Value >= stock)
            {
                blockedDates.Add("\"" + kvp.Key.ToString("yyyy-MM-dd") + "\"");
            }
        }

        System.Web.UI.WebControls.HiddenField hfBlockedDatesCtrl = (System.Web.UI.WebControls.HiddenField)cp.FindControl("hfBlockedDates");
        if (hfBlockedDatesCtrl != null)
        {
            hfBlockedDatesCtrl.Value = "[" + string.Join(",", blockedDates) + "]";
        }

        string msg = Request.QueryString["msg"];
        if (msg == "rented")
        {
            System.Web.UI.HtmlControls.HtmlGenericControl successModalCtrl = (System.Web.UI.HtmlControls.HtmlGenericControl)cp.FindControl("successModal");
            if (successModalCtrl != null) successModalCtrl.Visible = true;
        }

        // גריד תמונה ופרטים
        StringBuilder html = new StringBuilder();
        html.Append("<div style='display: grid; grid-template-columns: 1fr 1fr; gap: 40px; align-items: start;'>");
        html.Append("<div><img src='" + (string.IsNullOrEmpty(imageUrl) ? "Images/car1.png" : imageUrl) + "' style='width: 100%; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); object-fit: cover;' /></div>");
        html.Append("<div style='text-align: right;'>");
        html.Append("<h1 style='font-size: 2.2rem; color: #0f172a; margin-top: 0; margin-bottom: 15px; font-weight: 800;'>" + manufacturer + " " + model + "</h1>");
        html.Append("<p style='margin-bottom: 8px; color: #475569; font-size: 1rem;'><strong>שנת ייצור:</strong> " + year + "</p>");
        html.Append("<p style='margin-bottom: 8px; color: #475569; font-size: 1rem;'><strong>קטגוריה:</strong> " + category + "</p>");
        html.Append("<p style='margin-bottom: 8px; color: #475569; font-size: 1rem;'><strong>סניף:</strong> " + branchText + "</p>");
        html.Append("<p style='margin-bottom: 8px; color: #475569; font-size: 1rem;'><strong>דירוג:</strong> " + ratingText + "</p>");
        html.Append("<p style='margin-bottom: 8px; color: #475569; font-size: 1rem;'><strong>תיאור:</strong> " + description + "</p>");
        html.Append("<p style='margin-bottom: 8px; color: #475569; font-size: 1rem;'><strong>זמינים כרגע:</strong> " + availableNow + " מתוך " + stock + "</p>");

        // ===== מפרט טכני - החלק החדש =====
        html.Append("<div style='display:grid; grid-template-columns:1fr 1fr; gap:10px; margin:18px 0; padding:16px; background:#f8fafc; border-radius:10px; border:1px solid #e2e8f0;'>");
        html.Append("<div style='font-size:0.9rem; color:#334155;'>🪑 <strong>מושבים:</strong> " + seats + "</div>");
        html.Append("<div style='font-size:0.9rem; color:#334155;'>⛽ <strong>סוג דלק:</strong> " + fuelType + "</div>");
        html.Append("<div style='font-size:0.9rem; color:#334155;'>🛣️ <strong>טווח נסיעה:</strong> " + rangeKm + "</div>");
        html.Append("<div style='font-size:0.9rem; color:#334155;'>⚙️ <strong>תיבת הילוכים:</strong> " + transmission + "</div>");
        html.Append("<div style='font-size:0.9rem; color:#334155; grid-column: span 2;'>🧳 <strong>קיבולת מזוודות:</strong> " + luggage + "</div>");
        html.Append("</div>");
        // ===== סוף מפרט טכני =====

        html.Append("<div style='font-size: 2rem; font-weight: bold; color: #1e293b; margin: 20px 0;'>" + pricePerDay.ToString("C2") + " <span style='font-size: 1.1rem; color: #64748b; font-weight: normal;'>ליום</span></div>");
        html.Append("</div></div>");

        html.Append(BuildReviewsHtml());
        ltrCarDetails.Text = html.ToString();

        // ניהול מצבי כפתורים
        if (stock == 0 || Session["user"] == null)
        {
            btnRent.Visible = false;
            rentalForm.Visible = false;
        }
        else
        {
            btnRent.Visible = true;
            rentalForm.Visible = true;
        }

        if (Session["user"] != null && ltrFavBtn != null)
        {
            int userId = GetUserIdByUsername(Session["user"].ToString());
            bool isFavorite = userId != 0 && MyAdoHelper.ExecuteDataTable("SELECT Id FROM Favorites WHERE UserId = " + userId + " AND CarId = " + _carId).Rows.Count > 0;
            string favLabel = isFavorite ? "💔 הסר מהמועדפים" : "🤍 הוסף למועדפים";
            ltrFavBtn.Text = "<a href='CarDetails.aspx?id=" + _carId + "&action=togglefav' class='btn' style='background:#475569; color:#fff;'>" + favLabel + "</a>";
            ltrFavBtn.Visible = true;
        }
        else if (ltrFavBtn != null)
        {
            ltrFavBtn.Visible = false;
        }
    }

    private string BuildReviewsHtml()
    {
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT r.Rating, r.Comment, r.ReviewDate, u.Username FROM Reviews r JOIN Users u ON r.UserId = u.Id WHERE r.CarId = " + _carId + " ORDER BY r.ReviewDate DESC");
        if (dt.Rows.Count == 0) return "";
        StringBuilder html = new StringBuilder();
        html.Append("<div class='section' style='margin-top:30px;'><h3>ביקורות לקוחות</h3>");
        foreach (DataRow row in dt.Rows)
        {
            int rating = Convert.ToInt32(row["Rating"]);
            string username = HttpUtility.HtmlEncode(row["Username"].ToString());
            string comment = HttpUtility.HtmlEncode((row["Comment"] ?? "").ToString());
            html.Append("<div style='border-bottom:1px solid var(--color-gray-dark);padding:12px 0;'>");
            html.Append("<strong>" + username + "</strong> - " + new string('⭐', rating) + " <span style='color:var(--color-text-light);font-size:13px;'>(" + Convert.ToDateTime(row["ReviewDate"]).ToString("dd/MM/yyyy") + ")</span>");
            if (!string.IsNullOrEmpty(comment)) html.Append("<p style='margin-top:6px;margin-bottom:0;'>" + comment + "</p>");
            html.Append("</div>");
        }
        html.Append("</div>");
        return html.ToString();
    }

    protected void btnRent_Click(object sender, EventArgs e)
    {
        if (Session["user"] == null)
        {
            Response.Redirect("Login.aspx?returnUrl=" + HttpUtility.UrlEncode("CarDetails.aspx?id=" + _carId));
            return;
        }

        DateTime startDate, endDate;
        if (!DateTime.TryParse(txtStartDate.Value, out startDate) || !DateTime.TryParse(txtEndDate.Value, out endDate))
        {
            ShowRentError("נא לבחור תאריכים תקינים.");
            return;
        }

        if (startDate.Date < DateTime.Today || endDate.Date <= startDate.Date)
        {
            ShowRentError("טווח התאריכים אינו תקין.");
            return;
        }

        DataTable carDt = MyAdoHelper.ExecuteDataTable("SELECT Manufacturer, Model, Price, Stock FROM Cars WHERE Id = " + _carId);
        if (carDt.Rows.Count == 0)
        {
            Response.Redirect("Cars.aspx");
            return;
        }

        int stock = Convert.ToInt32(carDt.Rows[0]["Stock"]);
        decimal pricePerDay = Convert.ToDecimal(carDt.Rows[0]["Price"]);
        string carName = carDt.Rows[0]["Manufacturer"].ToString() + " " + carDt.Rows[0]["Model"].ToString();

        string overlapSql =
            "SELECT StartDate, EndDate FROM Orders WHERE CarId = " + _carId +
            " AND Status NOT IN (N'בוטל', N'נדחה')" +
            " AND StartDate <= '" + endDate.ToString("yyyy-MM-dd") + "'" +
            " AND EndDate >= '" + startDate.ToString("yyyy-MM-dd") + "'";

        DataTable dtOrders = MyAdoHelper.ExecuteDataTable(overlapSql);

        bool isAvailable = true;
        for (DateTime d = startDate.Date; d < endDate.Date; d = d.AddDays(1))
        {
            int bookedCount = 0;
            foreach (DataRow r in dtOrders.Rows)
            {
                DateTime s = Convert.ToDateTime(r["StartDate"]);
                DateTime end = Convert.ToDateTime(r["EndDate"]);
                if (d >= s && d <= end)
                {
                    bookedCount++;
                }
            }

            if (bookedCount >= stock)
            {
                isAvailable = false;
                break;
            }
        }

        if (!isAvailable)
        {
            ShowRentError("לצערנו אין מספיק רכבים זמינים מדגם זה בחלק מהתאריכים שבחרת בטווח.");
            return;
        }

        int days = (endDate.Date - startDate.Date).Days;
        decimal totalPrice = days * pricePerDay;

        ViewState["RentStart"] = startDate;
        ViewState["RentEnd"] = endDate;
        ViewState["RentTotal"] = totalPrice;

        System.Web.UI.WebControls.ContentPlaceHolder cp = (System.Web.UI.WebControls.ContentPlaceHolder)Master.FindControl("MainContent");

        ((System.Web.UI.WebControls.Label)cp.FindControl("lblModalCar")).Text = HttpUtility.HtmlEncode(carName);
        ((System.Web.UI.WebControls.Label)cp.FindControl("lblModalDates")).Text = startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy");
        ((System.Web.UI.WebControls.Label)cp.FindControl("lblModalDailyPrice")).Text = pricePerDay.ToString("C2");
        ((System.Web.UI.WebControls.Label)cp.FindControl("lblModalTotalPrice")).Text = totalPrice.ToString("C2");

        System.Web.UI.HtmlControls.HtmlGenericControl modal = (System.Web.UI.HtmlControls.HtmlGenericControl)cp.FindControl("orderModal");
        if (modal != null)
        {
            modal.Visible = true;
        }
    }

    protected void btnCancelModal_Click(object sender, EventArgs e)
    {
        System.Web.UI.HtmlControls.HtmlGenericControl modal = (System.Web.UI.HtmlControls.HtmlGenericControl)((System.Web.UI.WebControls.ContentPlaceHolder)Master.FindControl("MainContent")).FindControl("orderModal");
        if (modal != null) modal.Visible = false;
    }

    protected void btnConfirmOrder_Click(object sender, EventArgs e) { FinalizeOrder(false); }
    protected void btnConfirmAndEmail_Click(object sender, EventArgs e) { FinalizeOrder(true); }

    private void FinalizeOrder(bool sendEmail)
    {
        if (ViewState["RentStart"] == null) return;
        int userId = GetUserIdByUsername(Session["user"].ToString());
        if (userId == 0) return;

        DateTime startDate = Convert.ToDateTime(ViewState["RentStart"]);
        DateTime endDate = Convert.ToDateTime(ViewState["RentEnd"]);
        decimal totalPrice = Convert.ToDecimal(ViewState["RentTotal"]);

        string sql = "INSERT INTO Orders (UserId, CarId, OrderDate, StartDate, EndDate, Status, TotalPrice) VALUES (" +
                     userId + ", " + _carId + ", GETDATE(), '" + startDate.ToString("yyyy-MM-dd") + "', '" + endDate.ToString("yyyy-MM-dd") + "', N'ממתין', " + totalPrice.ToString(CultureInfo.InvariantCulture) + ")";
        MyAdoHelper.DoQuery(sql);

        if (sendEmail)
        {
            System.Web.UI.WebControls.ContentPlaceHolder cp = (System.Web.UI.WebControls.ContentPlaceHolder)Master.FindControl("MainContent");
            System.Web.UI.WebControls.TextBox txtEmail = (System.Web.UI.WebControls.TextBox)cp.FindControl("txtEmail");
            if (txtEmail != null && !string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                try { SendEmailReceipt(txtEmail.Text.Trim(), startDate, endDate, totalPrice); }
                catch (Exception ex)
                {
                    ((System.Web.UI.WebControls.Label)cp.FindControl("lblModalError")).Text = "ההזמנה נקלטה אך המייל נכשל: " + ex.Message; return;
                }
            }
        }
        Response.Redirect("CarDetails.aspx?id=" + _carId + "&msg=rented");
    }

    private void SendEmailReceipt(string toEmail, DateTime start, DateTime end, decimal total)
    {
        string fromEmail = "noam.shefi10@gmail.com";
        string appPassword = "dsbx atka jzov ugem";
        string carName = ((System.Web.UI.WebControls.Label)((System.Web.UI.WebControls.ContentPlaceHolder)Master.FindControl("MainContent")).FindControl("lblModalCar")).Text;

        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(fromEmail, "חברת השכרת רכב");
        mail.To.Add(toEmail);
        mail.Subject = "קבלה על הזמנת רכב - " + carName;
        mail.Body = "<h2 style='color:#2563eb;'>תודה רבה על הזמנתך!</h2><p><strong>פרטי הרכב:</strong> " + carName + "</p><p><strong>תאריכי השכרה:</strong> " + start.ToString("dd/MM/yyyy") + " עד " + end.ToString("dd/MM/yyyy") + "</p><p><strong>מחיר כולל:</strong> " + total.ToString("C2") + "</p>";
        mail.IsBodyHtml = true;

        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new System.Net.NetworkCredential(fromEmail, appPassword);
        smtp.EnableSsl = true;
        smtp.Send(mail);
    }

    private void ShowRentError(string msg) { lblRentMessage.CssClass = "error-message"; lblRentMessage.Text = msg; }
    private int GetUserIdByUsername(string u) { DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT Id FROM Users WHERE Username = '" + u.Replace("'", "''") + "'"); return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["Id"]) : 0; }
}