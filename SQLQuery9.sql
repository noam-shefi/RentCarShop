-- עדכון קטגוריות בצורה תקינה עם N לכל הרכבים
UPDATE Cars SET Category = N'סדאן' WHERE Model LIKE N'%3%' OR Model LIKE N'%איוניק%';
UPDATE Cars SET Category = N'ג''יפ' WHERE Model LIKE N'%רנגלר%' OR Model LIKE N'%RAV4%' OR Model LIKE N'%ספורטאז''%';
UPDATE Cars SET Category = N'ספורט' WHERE Model LIKE N'%מוסטנג%' OR Model LIKE N'%קורבט%' OR Model LIKE N'%צ''לנג''ר%';
UPDATE Cars SET Category = N'משפחתי' WHERE Model LIKE N'%סיינה%' OR Model LIKE N'%פאסיפיקה%';