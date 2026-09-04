using System;
using System.Data;

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
        string sql = "SELECT * FROM Users WHERE Username = '" + username + "'";
        DataTable dt = MyAdoHelper.ExecuteDataTable(sql);

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

        string sql = "UPDATE Users SET FirstName = '" + txtFirstName.Text.Trim() +
                     "', LastName = '" + txtLastName.Text.Trim() +
                     "', Email = '" + txtEmail.Text.Trim() +
                     "', Phone = '" + txtPhone.Text.Trim() +
                     "' WHERE Username = '" + username + "'";

        MyAdoHelper.DoQuery(sql);

        lblMessage.Text = "הפרטים עודכנו בהצלחה";
    }
}
