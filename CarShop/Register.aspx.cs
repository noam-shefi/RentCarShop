using System;
using System.Data;
using System.Data.SqlClient;

public partial class Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        if (lblError != null)
        {
            lblError.Text = string.Empty;
        }

        try
        {
            // Get form values with null checking
            string username = (txtUsername != null) ? txtUsername.Text.Trim() : string.Empty;
            string firstName = (txtFirstName != null) ? txtFirstName.Text.Trim() : string.Empty;
            string lastName = (txtLastName != null) ? txtLastName.Text.Trim() : string.Empty;
            string email = (txtEmail != null) ? txtEmail.Text.Trim() : string.Empty;
            string phone = (txtPhone != null) ? txtPhone.Text.Trim() : string.Empty;
            string password = (txtPassword != null) ? txtPassword.Text.Trim() : string.Empty;
            string confirmPassword = (txtConfirmPassword != null) ? txtConfirmPassword.Text.Trim() : string.Empty;

            // Validation
            if (string.IsNullOrEmpty(username) || username.Length <= 2)
            {
                SetError("שם המשתמש חייב להכיל לפחות 3 תווים");
                return;
            }

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                SetError("כתובת האימייל אינה תקינה");
                return;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                SetError("הסיסמה חייבת להכיל לפחות 6 תווים");
                return;
            }

            if (password != confirmPassword)
            {
                SetError("הסיסמה ואימות הסיסמה אינם תואמים");
                return;
            }

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                SetError("שם פרטי ושם משפחה הם שדות חובה");
                return;
            }

            // Check if user already exists using parameterized query
            string checkSql = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email";
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Email", email)
            };
            DataTable checkDt = MyAdoHelper.ExecuteDataTable(checkSql, checkParams);

            if (checkDt != null && checkDt.Rows.Count > 0 && checkDt.Rows[0][0] != null)
            {
                int count = 0;
                if (int.TryParse(checkDt.Rows[0][0].ToString(), out count) && count > 0)
                {
                    SetError("שם משתמש או אימייל זה כבר רשומים בממערכת");
                    return;
                }
            }

            // Insert new user with parameterized query and password hashing
            string hashedPassword = PasswordHelper.HashPassword(password);
            string sql = "INSERT INTO Users (Username, Password, FirstName, LastName, Email, Phone, IsAdmin) VALUES (@Username, @Password, @FirstName, @LastName, @Email, @Phone, 0)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", hashedPassword),
                new SqlParameter("@FirstName", firstName),
                new SqlParameter("@LastName", lastName),
                new SqlParameter("@Email", email),
                new SqlParameter("@Phone", phone)
            };

            MyAdoHelper.DoQuery(sql, parameters);

            // Success - redirect to login
            Response.Redirect("Login.aspx");
        }
        catch (Exception ex)
        {
            SetError("שגיאה בהרשמה: " + ex.Message);
        }
    }

    private void SetError(string message)
    {
        if (lblError != null)
        {
            lblError.Text = message;
        }
    }
}

