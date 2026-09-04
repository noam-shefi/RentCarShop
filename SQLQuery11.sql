-- =====================================================================
-- הרחבת מסד הנתונים: הוספת סניפים, ביקורות ומועדפים
-- מריצים פעם אחת מול מסד הנתונים הקיים (CarShopDB), אחרי
-- UpdateDatabaseForRental.sql שכבר הרצת קודם.
-- =====================================================================

-- ---------- סניפים ----------
CREATE TABLE Branches
(
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Name    NVARCHAR(100) NOT NULL,
    City    NVARCHAR(50)  NOT NULL,
    Address NVARCHAR(200) NULL,
    Phone   NVARCHAR(20)  NULL
);

-- קישור כל רכב לסניף שבו הוא נמצא. NULL מותר כדי לא לשבור רכבים קיימים
-- שעדיין לא שויכו לסניף - המנהל ישייך אותם דרך EditCar.aspx.
ALTER TABLE Cars ADD BranchId INT NULL;
ALTER TABLE Cars ADD CONSTRAINT FK_Cars_Branches FOREIGN KEY (BranchId) REFERENCES Branches(Id);

-- כמה סניפים לדוגמה, כדי שיהיה מיד מה לבחור ברשימה הנפתחת
INSERT INTO Branches (Name, City, Address, Phone) VALUES
(N'סניף תל אביב', N'תל אביב', N'רחוב אלנבי 12', N'03-1234567'),
(N'סניף חיפה', N'חיפה', N'שדרות בן גוריון 5', N'04-1234567'),
(N'סניף ירושלים', N'ירושלים', N'רחוב יפו 40', N'02-1234567');

-- ---------- ביקורות ----------
-- כל ביקורת קשורה להזמנה ספציפית (OrderId ייחודי - הזמנה אחת = ביקורת אחת לכל היותר),
-- כדי לוודא שרק מי שבאמת השכיר את הרכב יכול לדרג אותו.
CREATE TABLE Reviews
(
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    UserId     INT           NOT NULL,
    CarId      INT           NOT NULL,
    OrderId    INT           NOT NULL,
    Rating     INT           NOT NULL,
    Comment    NVARCHAR(500) NULL,
    ReviewDate DATETIME      NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Reviews_Users   FOREIGN KEY (UserId)  REFERENCES Users(Id),
    CONSTRAINT FK_Reviews_Cars    FOREIGN KEY (CarId)   REFERENCES Cars(Id),
    CONSTRAINT FK_Reviews_Orders  FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT UQ_Reviews_Order   UNIQUE (OrderId),
    CONSTRAINT CK_Reviews_Rating  CHECK (Rating BETWEEN 1 AND 5)
);

-- ---------- מועדפים ----------
-- טבלת קשר קלאסית Many-to-Many בין Users ל-Cars.
-- ה-UNIQUE מונע הוספה כפולה של אותו רכב למועדפים של אותו משתמש.
CREATE TABLE Favorites
(
    Id        INT      IDENTITY(1,1) PRIMARY KEY,
    UserId    INT      NOT NULL,
    CarId     INT      NOT NULL,
    AddedDate DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Favorites_Users   FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_Favorites_Cars    FOREIGN KEY (CarId)  REFERENCES Cars(Id),
    CONSTRAINT UQ_Favorites_UserCar UNIQUE (UserId, CarId)
);