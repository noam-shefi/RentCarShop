USE CarShopDB;
GO

-- הוספת עמודות מפרט טכני לטבלת הרכבים, לשימוש בהתאמת AI לפי צרכי הטיול

ALTER TABLE Cars ADD Seats INT NULL;
ALTER TABLE Cars ADD FuelType NVARCHAR(20) NULL;       -- בנזין / דיזל / חשמלי / היברידי
ALTER TABLE Cars ADD RangeKm INT NULL;                  -- טווח נסיעה במיכל/בטעינה מלאה (ק"מ)
ALTER TABLE Cars ADD Transmission NVARCHAR(20) NULL;    -- אוטומטית / ידנית
ALTER TABLE Cars ADD LuggageCapacity INT NULL;           -- מספר מזוודות גדולות
GO

-- ערכי ברירת מחדל סבירים לרכבים קיימים, כדי שלא יישארו NULL עד לעדכון ידני
UPDATE Cars SET
    Seats = 5,
    FuelType = N'בנזין',
    Transmission = N'אוטומטית',
    LuggageCapacity = 2
WHERE Seats IS NULL;
GO