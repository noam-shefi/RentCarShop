using System;
using System.Data;
using System.Data.SqlClient;

public partial class UpdateProfile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // דף זה מיועד רק למשתמשים מחוברים
        if (Session["user"] == null)
        {
            Response.Redirect("Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadUserData();
        }
    }

    /// <summary>
    /// שולף מהמסד את נתוני המשתמש המחובר (לפי ה-Session) וממלא בהם את תיבות הטקסט
    /// </summary>
    private void LoadUserData()
    {
        string username = Session["user"].ToString();
        SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Username", username) };
        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT * FROM Users WHERE Username = @Username", parameters);

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            txtFirstName.Text = row["FirstName"].ToString();
            txtLastName.Text = row["LastName"].ToString();
            txtEmail.Text = row["Email"].ToString();
            txtPhone.Text = row["Phone"].ToString();
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        string username = Session["user"].ToString();

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@FirstName", txtFirstName.Text.Trim()),
            new SqlParameter("@LastName", txtLastName.Text.Trim()),
            new SqlParameter("@Email", txtEmail.Text.Trim()),
            new SqlParameter("@Phone", txtPhone.Text.Trim()),
            new SqlParameter("@Username", username)
        };

        string sql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone WHERE Username = @Username";
        MyAdoHelper.DoQuery(sql, parameters);

        lblMessage.Text = "הפרטים עודכנו בהצלחה";
    }
}
