using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
public partial class AddCar : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["admin"] == null)
        {
            Response.Redirect("Home.aspx");
            return;
        }
        if (!IsPostBack)
        {
            LoadBranches();
        }
    }
    private void LoadBranches()
    {
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT Id, Name + N' - ' + City AS DisplayName FROM Branches ORDER BY City");
        foreach (DataRow row in dt.Rows)
        {
            ddlBranch.Items.Add(new System.Web.UI.WebControls.ListItem(row["DisplayName"].ToString(), row["Id"].ToString()));
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            string manufacturer = txtManufacturer.Text.Trim();
            string model = txtModel.Text.Trim();
            string category = ddlCategory.SelectedValue;
            string imageUrl = txtImageUrl.Text.Trim();
            string description = txtDescription.Text.Trim();
            string yearText = txtYear.Text.Trim();
            string priceText = txtPrice.Text.Trim();
            string stockText = txtStock.Text.Trim();
            string seatsText = txtSeats.Text.Trim();
            string rangeText = txtRangeKm.Text.Trim();
            string luggageText = txtLuggageCapacity.Text.Trim();

            // וולידציה - שדות חובה בסיסיים
            if (string.IsNullOrEmpty(manufacturer) || string.IsNullOrEmpty(model) ||
                string.IsNullOrEmpty(yearText) || string.IsNullOrEmpty(priceText) || string.IsNullOrEmpty(stockText))
            {
                lblMessage.Text = "שגיאה: חובה למלא את שדות החובה.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            int year, stock;
            decimal price;
            if (!int.TryParse(yearText, out year) || !decimal.TryParse(priceText, out price) || !int.TryParse(stockText, out stock))
            {
                lblMessage.Text = "שגיאה: שנת ייצור, מחיר ומלאי חייבים להיות מספרים חוקיים.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // שדות המפרט הטכני החדשים - אופציונליים, אך אם הוזנו חייבים להיות מספרים חוקיים
            object seatsValue = DBNull.Value;
            if (!string.IsNullOrEmpty(seatsText))
            {
                int seats;
                if (!int.TryParse(seatsText, out seats))
                {
                    lblMessage.Text = "שגיאה: מספר המושבים חייב להיות מספר חוקי.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                seatsValue = seats;
            }

            object rangeValue = DBNull.Value;
            if (!string.IsNullOrEmpty(rangeText))
            {
                int range;
                if (!int.TryParse(rangeText, out range))
                {
                    lblMessage.Text = "שגיאה: טווח הנסיעה חייב להיות מספר חוקי.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                rangeValue = range;
            }

            object luggageValue = DBNull.Value;
            if (!string.IsNullOrEmpty(luggageText))
            {
                int luggage;
                if (!int.TryParse(luggageText, out luggage))
                {
                    lblMessage.Text = "שגיאה: קיבולת המטען חייבת להיות מספר חוקי.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                luggageValue = luggage;
            }

            object branchIdValue = string.IsNullOrEmpty(ddlBranch.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlBranch.SelectedValue);

            string sql = "INSERT INTO Cars (Manufacturer, Model, Year, Price, Category, ImageUrl, Description, Stock, BranchId, " +
                         "Seats, FuelType, RangeKm, Transmission, LuggageCapacity) " +
                         "VALUES (@Manufacturer, @Model, @Year, @Price, @Category, @ImageUrl, @Description, @Stock, @BranchId, " +
                         "@Seats, @FuelType, @RangeKm, @Transmission, @LuggageCapacity)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Manufacturer", manufacturer),
                new SqlParameter("@Model", model),
                new SqlParameter("@Year", year),
                new SqlParameter("@Price", price),
                new SqlParameter("@Category", category),
                new SqlParameter("@ImageUrl", imageUrl),
                new SqlParameter("@Description", description),
                new SqlParameter("@Stock", stock),
                new SqlParameter("@BranchId", branchIdValue),
                new SqlParameter("@Seats", seatsValue),
                new SqlParameter("@FuelType", ddlFuelType.SelectedValue),
                new SqlParameter("@RangeKm", rangeValue),
                new SqlParameter("@Transmission", ddlTransmission.SelectedValue),
                new SqlParameter("@LuggageCapacity", luggageValue)
            };

            MyAdoHelper.DoQuery(sql, parameters);
            lblMessage.Text = "הרכב נוסף בהצלחה למלאי!";
            lblMessage.ForeColor = System.Drawing.Color.Green;

            // איפוס שדות
            txtManufacturer.Text = ""; txtModel.Text = ""; txtYear.Text = ""; txtPrice.Text = ""; txtStock.Text = ""; txtDescription.Text = "";
            txtSeats.Text = ""; txtRangeKm.Text = ""; txtLuggageCapacity.Text = "";
        }
        catch (Exception ex)
        {
            lblMessage.Text = "שגיאה: " + ex.Message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
    }
}