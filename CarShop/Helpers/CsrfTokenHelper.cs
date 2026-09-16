using System;
using System.Web;
using System.Web.UI;

/// <summary>
/// מחלקת עזר לניהול ואימות CSRF (Cross-Site Request Forgery) tokens
/// </summary>
public static class CsrfTokenHelper
{
    private const string TokenSessionKey = "__CsrfToken__";
    private const string TokenFieldName = "__RequestVerificationToken";

    /// <summary>
    /// יוצר ומאחסן CSRF token ב-Session
    /// </summary>
    public static string GenerateToken()
    {
        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null)
            throw new InvalidOperationException("Session is not available");

        string token = Guid.NewGuid().ToString("N");
        context.Session[TokenSessionKey] = token;
        return token;
    }

    /// <summary>
    /// מחזיר את ה-CSRF token מ-Session
    /// </summary>
    public static string GetToken()
    {
        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null)
            return null;

        object token = context.Session[TokenSessionKey];
        if (token == null)
        {
            return GenerateToken();
        }
        return token.ToString();
    }

    /// <summary>
    /// מוודא שה-CSRF token תקף
    /// </summary>
    public static bool ValidateToken(string token)
    {
        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null)
            return false;

        object storedToken = context.Session[TokenSessionKey];
        if (storedToken == null)
            return false;

        return storedToken.ToString() == token;
    }

    /// <summary>
    /// החזרת שדה קלט חבוי עם CSRF token
    /// </summary>
    public static string GetHiddenField()
    {
        string token = GetToken();
        return string.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", TokenFieldName, HttpUtility.HtmlAttributeEncode(token));
    }

    /// <summary>
    /// מקבל את ה-token מ-Request Form
    /// </summary>
    public static string GetTokenFromRequest()
    {
        return HttpContext.Current.Request.Form[TokenFieldName];
    }
}
