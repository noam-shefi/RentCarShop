USE CarShopDB;
GO
-- 1. ניקוי מפתחות זרים ומחיקת טבלאות קיימות למניעת התנגשויות
WHILE(EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'))
BEGIN
    DECLARE @sql NVARCHAR(MAX);
    SELECT TOP 1 @sql = 'ALTER TABLE [' + TABLE_NAME + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']'
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_TYPE = 'FOREIGN KEY';
    EXEC(@sql);
END
GO
IF OBJECT_ID('Favorites', 'U') IS NOT NULL DROP TABLE Favorites;
IF OBJECT_ID('Reviews', 'U') IS NOT NULL DROP TABLE Reviews;
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('Cars', 'U') IS NOT NULL DROP TABLE Cars;
IF OBJECT_ID('Branches', 'U') IS NOT NULL DROP TABLE Branches;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO
-- 2. יצירת טבלת משתמשים (כולל עמודת Phone שנוספה)
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Email NVARCHAR(100),
    Phone NVARCHAR(20) NULL,
    IsAdmin BIT DEFAULT 0
);
-- 3. יצירת טבלת סניפים
CREATE TABLE Branches (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    City NVARCHAR(50) NOT NULL,
    Address NVARCHAR(200) NULL,
    Phone NVARCHAR(20) NULL
);
-- 4. יצירת טבלת רכבים (כולל עמודות המפרט הטכני לשימוש התאמת ה-AI)
-- FuelType: בנזין / דיזל / חשמלי / היברידי
-- RangeKm: טווח נסיעה במיכל / בטעינה מלאה בק"מ
-- Transmission: אוטומטית / ידנית
-- LuggageCapacity: מספר מזוודות גדולות
CREATE TABLE Cars (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Manufacturer NVARCHAR(50) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    Year INT NULL,
    Category NVARCHAR(50) NULL,
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 1,
    ImageUrl NVARCHAR(MAX) NULL,
    Description NVARCHAR(MAX) NULL,
    BranchId INT NULL FOREIGN KEY REFERENCES Branches(Id),
    Seats INT NULL,
    FuelType NVARCHAR(20) NULL,
    RangeKm INT NULL,
    Transmission NVARCHAR(20) NULL,
    LuggageCapacity INT NULL
);
-- 5. יצירת טבלת הזמנות 
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    CarId INT FOREIGN KEY REFERENCES Cars(Id),
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) DEFAULT N'ממתין'
);
-- 6. יצירת טבלת מועדפים
CREATE TABLE Favorites (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    CarId INT NOT NULL FOREIGN KEY REFERENCES Cars(Id),
    AddedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Favorites_UserCar UNIQUE (UserId, CarId)
);
-- 7. יצירת טבלת ביקורות
CREATE TABLE Reviews (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    CarId INT NOT NULL FOREIGN KEY REFERENCES Cars(Id),
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id),
    Rating INT NOT NULL CONSTRAINT CK_Reviews_Rating CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(500) NULL,
    ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Reviews_Order UNIQUE (OrderId)
);
GO