using System;
using System.Data;
using System.Text;

public partial class Cars : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.QueryString["action"];
        if (action == "delete" && Session["admin"] != null)
        {
            HandleDelete();
            return;
        }
        if (!IsPostBack) LoadCars();
    }

    private void HandleDelete()
    {
        int carId;
        if (int.TryParse(Request.QueryString["id"], out carId)) MyAdoHelper.DoQuery("DELETE FROM Cars WHERE Id = " + carId);
        Response.Redirect("Cars.aspx");
    }

    private void LoadCars()
    {
        try
        {
            string searchTerm = txtSearch != null ? txtSearch.Text.Trim().Replace("'", "''") : "";
            string category = ddlCategory != null ? ddlCategory.SelectedValue.Replace("'", "''") : "";
            decimal maxPrice = 0;
            bool hasMaxPrice = false;

            System.Web.UI.WebControls.ContentPlaceHolder cp = (System.Web.UI.WebControls.ContentPlaceHolder)Master.FindControl("MainContent");

            // קריאת מחיר
            System.Web.UI.WebControls.TextBox txtCustomPriceCtrl = (System.Web.UI.WebControls.TextBox)cp.FindControl("txtCustomPrice");
            System.Web.UI.WebControls.DropDownList ddlMaxPriceCtrl = (System.Web.UI.WebControls.DropDownList)cp.FindControl("ddlMaxPrice");

            if (txtCustomPriceCtrl != null && decimal.TryParse(txtCustomPriceCtrl.Text.Trim(), out maxPrice)) hasMaxPrice = true;
            else if (ddlMaxPriceCtrl != null && decimal.TryParse(ddlMaxPriceCtrl.SelectedValue, out maxPrice)) hasMaxPrice = true;

            // קריאת תאריכים מ-Request.Form
            System.Web.UI.WebControls.TextBox txtSearchStartCtrl = (System.Web.UI.WebControls.TextBox)cp.FindControl("txtSearchStart");
            System.Web.UI.WebControls.TextBox txtSearchEndCtrl = (System.Web.UI.WebControls.TextBox)cp.FindControl("txtSearchEnd");

            DateTime searchStart = DateTime.MinValue, searchEnd = DateTime.MinValue;
            bool hasDates = false;

            if (txtSearchStartCtrl != null && txtSearchEndCtrl != null)
            {
                string startDateStr = Request.Form[txtSearchStartCtrl.UniqueID];
                string endDateStr = Request.Form[txtSearchEndCtrl.UniqueID];

                if (!string.IsNullOrEmpty(startDateStr) && !string.IsNullOrEmpty(endDateStr))
                {
                    txtSearchStartCtrl.Text = startDateStr;
                    txtSearchEndCtrl.Text = endDateStr;
                    if (DateTime.TryParseExact(startDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out searchStart) &&
                        DateTime.TryParseExact(endDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out searchEnd))
                    {
                        if (searchEnd >= searchStart) hasDates = true;
                    }
                }
            }

            string sql =
                "SELECT Cars.*, Branches.City AS BranchCity, " +
                "(SELECT COUNT(*) FROM Orders o WHERE o.CarId = Cars.Id AND o.Status NOT IN (N'בוטל', N'נדחה') AND o.StartDate <= CAST(GETDATE() AS DATE) AND o.EndDate >= CAST(GETDATE() AS DATE)) AS ActiveRentals, " +
                "(SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews r WHERE r.CarId = Cars.Id) AS AvgRating, " +
                "(SELECT COUNT(*) FROM Reviews r WHERE r.CarId = Cars.Id) AS ReviewCount " +
                "FROM Cars LEFT JOIN Branches ON Cars.BranchId = Branches.Id WHERE 1=1";

            if (!string.IsNullOrEmpty(searchTerm)) sql += " AND (Cars.Manufacturer LIKE '%" + searchTerm + "%' OR Cars.Model LIKE '%" + searchTerm + "%')";
            if (!string.IsNullOrEmpty(category)) sql += " AND Cars.Category = '" + category + "'";
            if (hasMaxPrice) sql += " AND Cars.Price <= " + maxPrice.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // סינון תאריכים
            if (hasDates)
            {
                string startStr = searchStart.ToString("yyyy-MM-dd");
                string endStr = searchEnd.ToString("yyyy-MM-dd");
                sql += " AND Cars.Stock > (SELECT COUNT(*) FROM Orders o WHERE o.CarId = Cars.Id AND o.Status NOT IN (N'בוטל', N'נדחה') AND o.StartDate <= '" + endStr + "' AND o.EndDate >= '" + startStr + "')";
            }

            DataTable dt = MyAdoHelper.ExecuteDataTable(sql);
            bool isAdmin = (Session["admin"] != null);

            if (dt != null && dt.Rows.Count > 0)
            {
                StringBuilder html = new StringBuilder();
                foreach (DataRow row in dt.Rows)
                {
                    int carId = Convert.ToInt32(row["Id"]);
                    string manufacturer = (row["Manufacturer"] ?? "").ToString();
                    string model = (row["Model"] ?? "").ToString();
                    decimal pricePerDay = Convert.ToDecimal(row["Price"]);
                    string imageUrl = (row["ImageUrl"] ?? "").ToString();
                    string description = (row["Description"] ?? "").ToString();
                    int stock = Convert.ToInt32(row["Stock"] ?? 0);
                    int activeRentals = Convert.ToInt32(row["ActiveRentals"] ?? 0);
                    int availableNow = Math.Max(0, stock - activeRentals);
                    int reviewCount = Convert.ToInt32(row["ReviewCount"] ?? 0);
                    string ratingText = reviewCount == 0 ? "אין ביקורות עדיין" : "⭐ " + Convert.ToDouble(row["AvgRating"]).ToString("0.0") + " (" + reviewCount + ")";
                    string branchCity = row["BranchCity"] == DBNull.Value ? "" : System.Web.HttpUtility.HtmlEncode(row["BranchCity"].ToString());

                    string searchAttr = System.Web.HttpUtility.HtmlEncode((manufacturer + " " + model).ToLower());
                    html.Append("<div class='card' data-search=\"" + searchAttr + "\">");
                    html.Append("<img src='" + (string.IsNullOrEmpty(imageUrl) ? "Images/car1.png" : imageUrl) + "' alt='" + System.Web.HttpUtility.HtmlEncode(manufacturer + " " + model) + "' />");
                    html.Append("<div class='card-body'><h3 style='color:#1e293b; margin-bottom:6px;'>" + System.Web.HttpUtility.HtmlEncode(manufacturer + " " + model) + "</h3>");

                    if (!string.IsNullOrEmpty(branchCity)) html.Append("<p style='font-size:13px; color:#64748b; margin-bottom:4px;'>📍 " + branchCity + "</p>");

                    html.Append("<p style='font-size:13px; color:#64748b; margin-bottom:8px;'>" + ratingText + "</p>");

                    // ===== התוספת של Claude: תגיות מפרט טכני בקטלוג =====
                    string seats = row["Seats"] != DBNull.Value ? row["Seats"].ToString() : null;
                    string fuelType = row["FuelType"] != DBNull.Value ? System.Web.HttpUtility.HtmlEncode(row["FuelType"].ToString()) : null;
                    string transmission = row["Transmission"] != DBNull.Value ? System.Web.HttpUtility.HtmlEncode(row["Transmission"].ToString()) : null;

                    if (seats != null || fuelType != null || transmission != null)
                    {
                        System.Collections.Generic.List<string> specParts = new System.Collections.Generic.List<string>();
                        if (seats != null) specParts.Add("🪑 " + seats);
                        if (fuelType != null) specParts.Add("⛽ " + fuelType);
                        if (transmission != null) specParts.Add("⚙️ " + transmission);

                        html.Append("<p style='font-size:12px; color:#64748b; margin-bottom:8px;'>" + string.Join(" &nbsp;|&nbsp; ", specParts) + "</p>");
                    }
                    // ===== סוף התוספת =====

                    html.Append("<p style='color:#475569; font-size:14px; margin-bottom:12px;'>" + System.Web.HttpUtility.HtmlEncode(description) + "</p>");
                    html.Append("<div class='card-price' style='color:#0f172a; font-weight:bold;'>" + pricePerDay.ToString("C2") + " <span style='font-size:13px; color:#64748b; font-weight:normal;'>/ יום</span></div>");

                    if (stock == 0) html.Append("<p class='error-message' style='margin-bottom:12px;'>לא זמין להשכרה</p>");
                    else html.Append("<p style='font-size:13px; color:#64748b; margin-bottom:12px;'>זמינים כרגע: " + availableNow + " מתוך " + stock + "</p>");

                    // כפתור ראשי - פרטים והשכרה
                    html.Append("<a href='CarDetails.aspx?id=" + carId + "' class='btn' style='display:block; text-align:center;'>פרטים והשכרה</a>");

                    // כפתורי מנהל - שורה נפרדת מתחת, שני כפתורים קטנים, אלגנטיים ומאוזנים
                    if (isAdmin)
                    {
                        html.Append("<div class='card-actions' style='margin-top:8px; display:flex; gap:6px;'>");

                        // כפתור ערוך
                        html.Append("<a href='EditCar.aspx?id=" + carId + "' style='flex:1; text-align:center; padding:6px; background:#f1f5f9; color:#475569; border:1px solid #cbd5e1; border-radius:6px; text-decoration:none; font-size:13px; font-weight:600;'>ערוך</a>");

                        // כפתור מחק
                        html.Append("<a href='Cars.aspx?action=delete&id=" + carId + "' onclick=\"return confirm('למחוק את הרכב? הפעולה אינה הפיכה.');\" style='flex:1; text-align:center; padding:6px; background:var(--color-destructive-soft); color:var(--color-destructive); border:1px solid var(--color-destructive-border); border-radius:6px; text-decoration:none; font-size:13px; font-weight:600;'>מחק</a>");

                        html.Append("</div>");
                    }

                    // --- סגירת ה-divs החסרה שהרסה לך את העיצוב הוחזרה הנה! ---
                    html.Append("</div></div>");
                }
                if (ltrCards != null) ltrCards.Text = html.ToString();
            }
            else
            {
                if (ltrCards != null) ltrCards.Text = "<p style='text-align: center; padding: 40px; font-size: 16px; grid-column: 1 / -1;'>לא נמצאו רכבים התואמים לחיפוש שלך.</p>";
            }
        }
        catch (Exception ex)
        {
            if (ltrCards != null) ltrCards.Text = "<p style='text-align: center; color: red;'>שגיאה: " + System.Web.HttpUtility.HtmlEncode(ex.Message) + "</p>";
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e) { LoadCars(); }
}