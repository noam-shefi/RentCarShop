using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;

public partial class ManageBranches : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["admin"] == null)
        {
            Response.Redirect("Home.aspx");
            return;
        }

        string action = Request.QueryString["action"];
        if (action == "delete")
        {
            HandleDelete();
            return;
        }

        if (!IsPostBack)
        {
            LoadBranches();
        }
    }

    private void HandleDelete()
    {
        try
        {
            int branchId;
            if (int.TryParse(Request.QueryString["id"], out branchId))
            {
                MyAdoHelper.DoQuery("UPDATE Cars SET BranchId = NULL WHERE BranchId = " + branchId);
                MyAdoHelper.DoQuery("DELETE FROM Branches WHERE Id = " + branchId);

                // שמירת הודעת הצלחה כדי להציג אותה אחרי הרענון
                Session["BranchMsg"] = "הסניף נמחק בהצלחה!";
            }
        }
        catch (Exception) { }

        Response.Redirect("ManageBranches.aspx");
    }

    // פותח את החלונית הקופצת
    protected void btnOpenModal_Click(object sender, EventArgs e)
    {
        lblModalMessage.Text = "";
        addModal.Visible = true;
    }

    // סוגר את החלונית הקופצת מבלי לשמור
    protected void btnCloseModal_Click(object sender, EventArgs e)
    {
        addModal.Visible = false;
    }

    // שומר את הנתונים מהחלונית
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            string name = txtName.Text.Trim();
            string city = txtCity.Text.Trim();
            string address = txtAddress.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city))
            {
                lblModalMessage.Text = "שגיאה: חובה למלא לפחות שם סניף ועיר.";
                return; // יוצא מהפונקציה ומשאיר את החלונית פתוחה לתיקון
            }

            string sql = "INSERT INTO Branches (Name, City, Address, Phone) VALUES (@Name, @City, @Address, @Phone)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Name", name),
                new SqlParameter("@City", city),
                new SqlParameter("@Address", address),
                new SqlParameter("@Phone", phone)
            };

            MyAdoHelper.DoQuery(sql, parameters);

            // איפוס השדות לקראת ההוספה הבאה
            txtName.Text = "";
            txtCity.Text = "";
            txtAddress.Text = "";
            txtPhone.Text = "";

            // סגירת החלונית הקופצת והצגת הודעת הצלחה במסך הראשי
            addModal.Visible = false;

            lblMainMessage.CssClass = "success-message";
            lblMainMessage.ForeColor = System.Drawing.Color.Green;
            lblMainMessage.Text = "הסניף החדש נוסף בהצלחה!";

            LoadBranches(); // רענון הטבלה
        }
        catch (Exception ex)
        {
            lblModalMessage.Text = "שגיאה בהוספת הסניף: " + ex.Message;
        }
    }

    private void LoadBranches()
    {
        // אם יש הודעה על מחיקה מה-Session, נציג אותה וננקה
        if (Session["BranchMsg"] != null)
        {
            lblMainMessage.CssClass = "success-message";
            lblMainMessage.ForeColor = System.Drawing.Color.Green;
            lblMainMessage.Text = Session["BranchMsg"].ToString();
            Session.Remove("BranchMsg");
        }

        DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT * FROM Branches ORDER BY City");
        ltrBranches.Text = BuildBranchesHtml(dt);
    }

    private string BuildBranchesHtml(DataTable dt)
    {
        if (dt.Rows.Count == 0)
        {
            return "<p class='text-center' style='padding:20px;'>אין עדיין סניפים.</p>";
        }

        StringBuilder html = new StringBuilder();
        html.Append("<table class='data-table'>");
        html.Append("<tr><th>שם</th><th>עיר</th><th>כתובת</th><th>טלפון</th><th></th></tr>");

        foreach (DataRow row in dt.Rows)
        {
            int id = Convert.ToInt32(row["Id"]);
            html.Append("<tr>");
            html.Append("<td>" + HttpUtility.HtmlEncode(row["Name"].ToString()) + "</td>");
            html.Append("<td>" + HttpUtility.HtmlEncode(row["City"].ToString()) + "</td>");
            html.Append("<td>" + HttpUtility.HtmlEncode(row["Address"].ToString()) + "</td>");
            html.Append("<td>" + HttpUtility.HtmlEncode(row["Phone"].ToString()) + "</td>");
            html.Append("<td><a href='ManageBranches.aspx?action=delete&id=" + id + "' " +
                        "onclick=\"return confirm('למחוק את הסניף? רכבים המשויכים אליו יעברו למצב ללא סניף.');\" " +
                        "class='btn' style='background:#900; padding:6px 12px;'>מחק</a></td>");
            html.Append("</tr>");
        }

        html.Append("</table>");
        return html.ToString();
    }
}