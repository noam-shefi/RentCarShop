חנות הרכב שלי - פרויקט גמר בגרות (ASP.NET Web Forms + C#)
=============================================================

איך פותחים את הפרויקט ב-Visual Studio
--------------------------------------
זהו פרויקט מסוג "Web Site" (לא "Web Application"), כלומר אין קובץ .sln/.csproj -
פותחים אותו כך:

1. חלץ (Extract) את קובץ ה-ZIP למיקום כלשהו במחשב.
2. פתח את Visual Studio.
3. תפריט File -> Open -> Web Site... (לא "Open Project/Solution").
4. בחר את התיקייה שחילצת (CarShopWebsite) ולחץ Open.
5. Visual Studio ייצור אוטומטית קבצי Designer (Site.master.designer.cs וכו')
   ברגע שתפתח כל דף בפעם הראשונה - זה תקין ולא צריך לגעת בזה.
6. לחיצה על F5 תריץ את האתר. הדף Default.aspx מוגדר כדף ברירת המחדל של השרת
   (גם דרך Web.config וגם כי הוא נקרא Default.aspx) והוא מעביר אוטומטית ל-Home.aspx,
   כך שאין צורך להגדיר ידנית Start Page.

הקמת מסד הנתונים
------------------
1. פתח את SQL Server Object Explorer (או SSMS) והתחבר ל-(localdb)\MSSQLLocalDB.
2. צור מסד נתונים חדש בשם CarShopDB, ושמור אותו כקובץ CarShopDB.mdf
   בתוך תיקיית App_Data של הפרויקט.
3. הרץ את הסקריפט App_Data/CreateDatabase.sql על המסד כדי ליצור את שלוש הטבלאות
   (Users, Cars, Orders).
4. הכנס ידנית לפחות משתמש מנהל אחד לטבלת Users עם IsAdmin = 1,
   כדי שיהיה עם מי להתחבר לפאנל הניהול (Admin.aspx).

מבנה התיקיות
-------------
CarShopWebsite/
├── App_Code/
│   └── MyAdoHelper.cs          - מחלקת עזר ל-ADO.NET (DoQuery / IsExist / ExecuteDataTable / PrintDataTable)
├── App_Data/
│   └── CreateDatabase.sql      - סקריפט יצירת הטבלאות (יש להריץ ידנית, לא רץ אוטומטית)
│   └── (כאן ישב קובץ CarShopDB.mdf לאחר שתיצור אותו)
├── CSS/
│   └── StyleSheet.css
├── Images/
│   └── (תמונות הרכבים ותמונת ה-Hero - ר' README בתיקייה)
├── Web.config
├── Default.aspx / Default.aspx.cs  - דף ברירת מחדל, מפנה מיד ל-Home.aspx
├── Site.master / Site.master.cs
├── Home.aspx / Home.aspx.cs
├── Register.aspx / Register.aspx.cs
├── Login.aspx / Login.aspx.cs
├── UpdateProfile.aspx / UpdateProfile.aspx.cs
├── Admin.aspx / Admin.aspx.cs
├── Cars.aspx / Cars.aspx.cs
├── AddCar.aspx / AddCar.aspx.cs
└── Logout.aspx / Logout.aspx.cs

נקודות שכדאי לזכור לקראת הגנה בעל פה
--------------------------------------
- השאילתות ב-MyAdoHelper בנויות בהדבקת מחרוזות (string concatenation) ולא עם
  SqlParameter, ולכן חשופות תיאורטית ל-SQL Injection. זו נקודה סבירה שבוחן עשוי לשאול עליה.
- txtYear / txtPrice / txtStock (ב-AddCar.aspx) ממירים קלט ישירות עם Convert.ToInt32/ToDecimal
  בלי try/catch - קלט לא מספרי יגרום לשגיאת ריצה. אפשר להוסיף ולידציה אם תרצה.
