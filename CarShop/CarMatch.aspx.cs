using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CarShop
{
    public partial class CarMatch : System.Web.UI.Page
    {
        private const string GROQ_API_KEY = "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";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected async void btnFindCar_Click(object sender, EventArgs e)
        {
            resultBox.Visible = false;
            lblResult.Text = "";

            string tripDetails = txtTripDetails.Text.Trim();

            if (string.IsNullOrEmpty(tripDetails))
            {
                lblStatus.Text = "אנא כתבו כמה פרטים על הטיול שלכם לפני החיפוש.";
                lblStatus.CssClass = "error-message";
                return;
            }

            lblStatus.Text = "מחפשים את הרכב המתאים ביותר עבורכם...";
            lblStatus.CssClass = "";

            try
            {
                List<string> availableCars = GetAvailableCars();

                if (availableCars.Count == 0)
                {
                    lblStatus.Text = "מצטערים, אין כרגע רכבים זמינים במלאי.";
                    lblStatus.CssClass = "error-message";
                    return;
                }

                string prompt = BuildPrompt(tripDetails, availableCars);
                string aiResponseText = await SendRequestToGroqAsync(prompt);

                lblResult.Text = aiResponseText.Replace("\n", "<br/>");
                resultBox.Visible = true;
                lblStatus.Text = "";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "אירעה שגיאה: " + HttpUtility.HtmlEncode(ex.Message);
                lblStatus.CssClass = "error-message";
            }
        }

        private List<string> GetAvailableCars()
        {
            List<string> carList = new List<string>();
            // שימוש בעמודה הנכונה Id מתוך טבלת Cars
            DataTable dt = MyAdoHelper.ExecuteDataTable("SELECT Id, Manufacturer, Model, Year, Price, Category FROM Cars WHERE Stock > 0");

            foreach (DataRow row in dt.Rows)
            {
                string carId = row["Id"].ToString();
                string carDetails = string.Format("{0} {1} ({2}) - קטגוריה: {3}, מחיר: {4:C2} ליום",
                    row["Manufacturer"], row["Model"], row["Year"], row["Category"], Convert.ToDecimal(row["Price"]));

                carList.Add(carDetails + " [קישור לצפייה והזמנה: CarDetails.aspx?id=" + carId + "]");
            }

            return carList;
        }

        private string BuildPrompt(string tripDetails, List<string> availableCars)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("פרטי הטיול של הלקוח: " + tripDetails);
            sb.AppendLine("רשימת הרכבים הזמינים במלאי (כולל תגית קישור מוכנה לשימוש לכל רכב):");

            foreach (string car in availableCars)
            {
                sb.AppendLine("- " + car);
            }

            return sb.ToString();
        }

        private async Task<string> SendRequestToGroqAsync(string userPrompt)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<string> errorLog = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CarRentShopApp/1.0");
                string cleanApiKey = GROQ_API_KEY.Trim();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cleanApiKey);
                client.Timeout = TimeSpan.FromSeconds(30);

                List<string> modelsToTry = await GetAvailableModelsAsync(client, serializer);
                string endpoint = "https://api.groq.com/openai/v1/chat/completions";

                foreach (string modelId in modelsToTry)
                {
                    var payload = new
                    {
                        model = modelId,
                        messages = new object[]
                        {
                            new
                            {
                                role = "system",
                                content = "אתה סוכן השכרת רכב מקצועי. תפקידך לבחור את הרכב המתאים ביותר מתוך הרשימה בהתאם לפרטי הטיול של הלקוח, ולכתוב הסבר מפורט וידידותי בעברית בלבד מדוע הוא הנבחר. חובה עליך להוציא בסוף התשובה תגית HTML תקינה של קישור (למשל: <a href='CarDetails.aspx?id=X'>לחץ כאן לצפייה והזמנת הרכב</a>) בהתבסס על הקישור שסופק ברשימת הרכבים עבור אותו רכב נבחר. חל איסור מוחלט להחזיר מספרים בודדים או קוד גולמי, אלא טקסט מילולי עשיר עם הקישור."
                            },
                            new
                            {
                                role = "user",
                                content = userPrompt
                            }
                        },
                        temperature = 0.7
                    };

                    string jsonPayload = serializer.Serialize(payload);
                    using (StringContent httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
                    {
                        try
                        {
                            HttpResponseMessage response = await client.PostAsync(endpoint, httpContent);
                            string responseBody = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                return ParseGroqResponse(responseBody, serializer);
                            }
                            else
                            {
                                errorLog.Add("מודל " + modelId + " נדחה: " + responseBody);
                            }
                        }
                        catch (Exception ex)
                        {
                            errorLog.Add("מודל " + modelId + " שגיאת רשת: " + ex.Message);
                        }
                    }
                }
            }

            throw new Exception("כל המודלים נכשלו. ייתכן שיש בעיה בחשבון ה-Groq שלך. פירוט:\n" + string.Join("\n", errorLog));
        }

        private async Task<List<string>> GetAvailableModelsAsync(HttpClient client, JavaScriptSerializer serializer)
        {
            List<string> validModels = new List<string>();

            try
            {
                HttpResponseMessage response = await client.GetAsync("https://api.groq.com/openai/v1/models");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Dictionary<string, object> dict = serializer.Deserialize<Dictionary<string, object>>(json);

                    if (dict != null && dict.ContainsKey("data"))
                    {
                        ArrayList data = dict["data"] as ArrayList;
                        if (data != null && data.Count > 0)
                        {
                            foreach (object item in data)
                            {
                                Dictionary<string, object> model = item as Dictionary<string, object>;
                                if (model != null && model.ContainsKey("id"))
                                {
                                    string id = model["id"].ToString();
                                    string lowerId = id.ToLower();

                                    if (!lowerId.Contains("guard") && !lowerId.Contains("vision") &&
                                        !lowerId.Contains("whisper") && !lowerId.Contains("embed"))
                                    {
                                        validModels.Add(id);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            if (validModels.Count == 0)
            {
                validModels.AddRange(new string[] { "llama3-8b-8192", "mixtral-8x7b-32768", "gemma2-9b-it", "llama3-70b-8192" });
            }

            return validModels;
        }

        private string ParseGroqResponse(string jsonResponse, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> parsedJson = serializer.Deserialize<Dictionary<string, object>>(jsonResponse);

            if (parsedJson == null || !parsedJson.ContainsKey("choices"))
            {
                throw new Exception("מבנה התשובה מהשרת אינו תקין (חסר choices).");
            }

            ArrayList choicesArray = parsedJson["choices"] as ArrayList;

            if (choicesArray == null || choicesArray.Count == 0)
            {
                throw new Exception("השרת החזיר מערך תשובות ריק.");
            }

            Dictionary<string, object> firstChoice = choicesArray[0] as Dictionary<string, object>;

            if (firstChoice != null && firstChoice.ContainsKey("message"))
            {
                Dictionary<string, object> messageObj = firstChoice["message"] as Dictionary<string, object>;

                if (messageObj != null && messageObj.ContainsKey("content"))
                {
                    return messageObj["content"].ToString();
                }
            }

            throw new Exception("לא הצלחנו לחלץ את הטקסט מתוך תשובת ה-AI.");
        }
    }
}