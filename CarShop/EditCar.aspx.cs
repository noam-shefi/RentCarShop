using System;
using System.Data;
using System.Data.SqlClient;

public partial class EditCar : System.Web.UI.Page
{
    private int _carId;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["admin"] == null)
        {
            Response.Redirect("Home.aspx");
            return;
        }
        if (!int.TryParse(Request.QueryString["id"], out _carId))
        {
            Response.Redirect("Cars.aspx");
            return;
        }
        if (!IsPostBack)
        {
            LoadBranches();
            LoadCar();
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

    private void LoadCar()
    {
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT * FROM Cars WHERE Id = " + _carId);
        if (dt.Rows.Count == 0) return;
        DataRow row = dt.Rows[0];
        txtManufacturer.Text = row["Manufacturer"].ToString();
        txtModel.Text = row["Model"].ToString();
        txtYear.Text = row["Year"].ToString();
        txtPrice.Text = row["Price"].ToString();
        ddlCategory.SelectedValue = row["Category"].ToString();
        txtImageUrl.Text = row["ImageUrl"].ToString();
        txtDescription.Text = row["Description"].ToString();
        txtStock.Text = row["Stock"].ToString();
        if (row["BranchId"] != DBNull.Value) ddlBranch.SelectedValue = row["BranchId"].ToString();

        if (row.Table.Columns.Contains("Seats") && row["Seats"] != DBNull.Value)
            txtSeats.Text = row["Seats"].ToString();

        if (row.Table.Columns.Contains("FuelType") && row["FuelType"] != DBNull.Value)
            ddlFuelType.SelectedValue = row["FuelType"].ToString();

        if (row.Table.Columns.Contains("RangeKm") && row["RangeKm"] != DBNull.Value)
            txtRangeKm.Text = row["RangeKm"].ToString();

        if (row.Table.Columns.Contains("Transmission") && row["Transmission"] != DBNull.Value)
            ddlTransmission.SelectedValue = row["Transmission"].ToString();

        if (row.Table.Columns.Contains("LuggageCapacity") && row["LuggageCapacity"] != DBNull.Value)
            txtLuggageCapacity.Text = row["LuggageCapacity"].ToString();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int year = Convert.ToInt32(txtYear.Text.Trim());
            decimal price = Convert.ToDecimal(txtPrice.Text.Trim());
            int stock = Convert.ToInt32(txtStock.Text.Trim());
            object branchIdValue = string.IsNullOrEmpty(ddlBranch.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlBranch.SelectedValue);

            object seatsValue = DBNull.Value;
            string seatsText = txtSeats.Text.Trim();
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
            string rangeText = txtRangeKm.Text.Trim();
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
            string luggageText = txtLuggageCapacity.Text.Trim();
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

            string sql = "UPDATE Cars SET Manufacturer=@Manufacturer, Model=@Model, Year=@Year, Price=@Price, " +
                         "Category=@Category, BranchId=@BranchId, ImageUrl=@ImageUrl, Description=@Description, Stock=@Stock, " +
                         "Seats=@Seats, FuelType=@FuelType, RangeKm=@RangeKm, Transmission=@Transmission, LuggageCapacity=@LuggageCapacity " +
                         "WHERE Id=@Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Manufacturer", txtManufacturer.Text.Trim()),
                new SqlParameter("@Model", txtModel.Text.Trim()),
                new SqlParameter("@Year", year),
                new SqlParameter("@Price", price),
                new SqlParameter("@Category", ddlCategory.SelectedValue),
                new SqlParameter("@BranchId", branchIdValue),
                new SqlParameter("@ImageUrl", txtImageUrl.Text.Trim()),
                new SqlParameter("@Description", txtDescription.Text.Trim()),
                new SqlParameter("@Stock", stock),
                new SqlParameter("@Seats", seatsValue),
                new SqlParameter("@FuelType", ddlFuelType.SelectedValue),
                new SqlParameter("@RangeKm", rangeValue),
                new SqlParameter("@Transmission", ddlTransmission.SelectedValue),
                new SqlParameter("@LuggageCapacity", luggageValue),
                new SqlParameter("@Id", _carId)
            };

            MyAdoHelper.DoQuery(sql, parameters);
            lblMessage.Text = "פרטי הרכב עודכנו בהצלחה!";
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lblMessage.Text = "שגיאה: " + ex.Message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
    }
}