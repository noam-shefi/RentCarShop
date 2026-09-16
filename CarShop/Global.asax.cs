using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.SessionState;

namespace CarShop
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            EnsureAdminAccount();
        }

        private void EnsureAdminAccount()
        {
            try
            {
                // Make sure Password column can store hashed values
                string ensurePasswordColumnSql = @"
                IF EXISTS (
                    SELECT 1
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'Users'
                      AND COLUMN_NAME = 'Password'
                      AND CHARACTER_MAXIMUM_LENGTH IS NOT NULL
                      AND CHARACTER_MAXIMUM_LENGTH < 128
                )
                BEGIN
                    ALTER TABLE Users ALTER COLUMN [Password] NVARCHAR(256) NOT NULL;
                END";
                MyAdoHelper.DoQuery(ensurePasswordColumnSql);

                string adminUsername = ConfigurationManager.AppSettings["AdminUsername"] ?? "admin";
                string adminPlainPassword = ConfigurationManager.AppSettings["AdminDefaultPassword"] ?? "123456";
                string adminEmail = ConfigurationManager.AppSettings["AdminEmail"] ?? "admin@carshop.com";
                string adminHashedPassword = PasswordHelper.HashPassword(adminPlainPassword);

                SqlParameter[] existsParams = new SqlParameter[]
                {
                    new SqlParameter("@Username", adminUsername)
                };

                bool adminExists = MyAdoHelper.IsExist("SELECT COUNT(1) FROM Users WHERE Username = @Username", existsParams);

                if (!adminExists)
                {
                    SqlParameter[] insertParams = new SqlParameter[]
                    {
                        new SqlParameter("@Username", adminUsername),
                        new SqlParameter("@Password", adminHashedPassword),
                        new SqlParameter("@FirstName", "מנהל"),
                        new SqlParameter("@LastName", "מערכת"),
                        new SqlParameter("@Email", adminEmail)
                    };

                    MyAdoHelper.DoQuery(
                        "INSERT INTO Users (Username, Password, FirstName, LastName, Email, IsAdmin) VALUES (@Username, @Password, @FirstName, @LastName, @Email, 1)",
                        insertParams);
                }
                else
                {
                    SqlParameter[] updateParams = new SqlParameter[]
                    {
                        new SqlParameter("@Username", adminUsername),
                        new SqlParameter("@Password", adminHashedPassword)
                    };

                    MyAdoHelper.DoQuery(
                        "UPDATE Users SET Password = @Password, IsAdmin = 1 WHERE Username = @Username",
                        updateParams);
                }
            }
            catch
            {
                // Avoid blocking app startup if DB is not ready yet
            }
        }
    }
}
