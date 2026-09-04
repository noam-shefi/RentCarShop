using System;
using System.Data;

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

            // SQL escape to prevent injection
            username = username.Replace("'", "''");
            firstName = firstName.Replace("'", "''");
            lastName = lastName.Replace("'", "''");
            email = email.Replace("'", "''");
            phone = phone.Replace("'", "''");
            password = password.Replace("'", "''");

            // Check if user already exists
            string checkSql = "SELECT COUNT(*) FROM Users WHERE Username = '" + username + "' OR Email = '" + email + "'";
            DataTable checkDt = MyAdoHelper.ExecuteDataTable(checkSql);

            if (checkDt != null && checkDt.Rows.Count > 0 && checkDt.Rows[0][0] != null)
            {
                int count = 0;
                if (int.TryParse(checkDt.Rows[0][0].ToString(), out count) && count > 0)
                {
                    SetError("שם משתמש או אימייל זה כבר רשומים בממערכת");
                    return;
                }
            }

            // Insert new user
            string sql = "INSERT INTO Users (Username, Password, FirstName, LastName, Email, Phone, IsAdmin) VALUES ('" +
                         username + "', '" + password + "', '" + firstName + "', '" + lastName + "', '" + email + "', '" + phone + "', 0)";

            MyAdoHelper.DoQuery(sql);

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
