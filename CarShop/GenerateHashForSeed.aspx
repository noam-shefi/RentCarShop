<%@ Page Language="C#" %>
<%@ Import Namespace="CarShop" %>
<!DOCTYPE html>
<html>
<head>
    <title>Generate Password Hash for Seed Data</title>
</head>
<body>
    <h2>Password Hashing Tool</h2>
    <p>Use this to generate PBKDF2 hashes for seed data passwords.</p>

    <%
        // Passwords to hash for seed data
        string adminHash = PasswordHelper.HashPassword("123456");
        string noamHash = PasswordHelper.HashPassword("123456");
    %>

    <p>Hash for 'admin' / '123456':<br/>
    <code><%=adminHash%></code></p>

    <p>Hash for 'noam' / '123456':<br/>
    <code><%=noamHash%></code></p>

    <h3>Copy this SQL for InsertSampleCars.sql:</h3>
    <pre>INSERT INTO Users
(Username, Password, FirstName, LastName, Email, IsAdmin)
VALUES
(N'admin', N'<%=adminHash%>', N'מנהל', N'מערכת', N'admin@carshop.com', 1),
(N'noam', N'<%=noamHash%>', N'noam', N'noam', N'noam@gmail.com', 0);</pre>
</body>
</html>
