using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;

/// <summary>
/// מחלקת עזר סטטית לתקשורת מאובטחת עם מסד הנתונים באמצעות ADO.NET.
/// </summary>
public static class MyAdoHelper
{
    // התיקון הקריטי: מחרוזת התקשרות ישירה שדורסת כל הגדרה ישנה ומצביעה ב-100% לשרת המקומי החדש!
    private static readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=CarShopDB;Integrated Security=True";

    /// <summary>
    /// מבצעת פקודת SQL שאינה מחזירה נתונים (INSERT, UPDATE, DELETE) עם תמיכה בפרמטרים למניעת SQL Injection.
    /// </summary>
    public static void DoQuery(string sql, SqlParameter[] parameters = null)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("שגיאה בביצוע הפעולה במסד הנתונים: " + ex.Message);
            }
        }
    }

    /// <summary>
    /// בודקת האם קיימת רשומה התואמת לשאילתה בצורה יעילה (באמצעות ExecuteScalar).
    /// </summary>
    public static bool IsExist(string sql, SqlParameter[] parameters = null)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            con.Open();
            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                // אם השאילתה מחזירה COUNT
                if (int.TryParse(result.ToString(), out int count))
                {
                    return count > 0;
                }
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// מבצעת שאילתת SELECT ומחזירה את התוצאה כאובייקט DataTable.
    /// </summary>
    public static DataTable ExecuteDataTable(string sql, SqlParameter[] parameters = null)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }
        return dt;
    }

    /// <summary>
    /// מקבלת שאילתת SELECT ומחזירה טבלת HTML מעוצבת ומאובטחת מפני מתקפות XSS.
    /// </summary>
    public static string PrintDataTable(string sql, SqlParameter[] parameters = null)
    {
        DataTable dt = ExecuteDataTable(sql, parameters);
        StringBuilder html = new StringBuilder();

        html.Append("<table class='data-table'>");

        // כותרות הטבלה
        html.Append("<tr>");
        foreach (DataColumn col in dt.Columns)
        {
            string colName = HttpUtility.HtmlEncode(col.ColumnName);
            html.Append("<th>").Append(colName).Append("</th>");
        }
        html.Append("</tr>");

        // שורות הנתונים
        foreach (DataRow row in dt.Rows)
        {
            html.Append("<tr>");
            foreach (DataColumn col in dt.Columns)
            {
                string cellValue = HttpUtility.HtmlEncode(row[col].ToString());
                html.Append("<td>").Append(cellValue).Append("</td>");
            }
            html.Append("</tr>");
        }

        html.Append("</table>");
        return html.ToString();
    }
}