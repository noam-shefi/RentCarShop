-- 1. Clear out all active orders and cars with corrupted characters
DELETE FROM Orders;
DELETE FROM Cars;

-- 2. Insert clean, fully formatted car entries with proper Unicode (N'') strings
INSERT INTO Cars (Manufacturer, Model, Year, Price, Category, ImageUrl, Description, Stock) VALUES
(N'פורד', N'מוסטנג GT', 2023, 160000.00, N'ספורט', N'Images/car1.png', N'רכב ספורט חזק ומרשים למחפשי חווית נהיגה', 6),
(N'ג''יפ', N'רנגלר', 2022, 150000.00, N'ג''יפ', N'Images/car1.png', N'רכב שטח עוצמתי המתאים לכל תנאי דרך', 4),
(N'טויוטה', N'RAV4', 2023, 200000.00, N'ג''יפ', N'Images/car1.png', N'ג''יפ משפחתי היברידי חסכוני ואמין', 2),
(N'שברולט', N'קורבט', 2024, 350000.00, N'ספורט', N'Images/car1.png', N'מכונית על עם ביצועים יוצאי דופן', 1),
(N'דודג''', N'צ''לנג''ר', 2022, 300000.00, N'ספורט', N'Images/car1.png', N'רכב שרירים אמריקאי קלאסי בעל מראה ייחודי', 2),
(N'טויוטה', N'סיינה', 2023, 200000.00, N'משפחתי', N'Images/car1.png', N'מיניוואן מרווח ונוח במיוחד למשפחות גדולות', 3),
(N'קרייזלר', N'פאסיפיקה', 2023, 210000.00, N'משפחתי', N'Images/car1.png', N'רכב יוקרתי ומפנק בעל 7 מקומות ישיבה', 2),
(N'מאזדה', N'3', 2022, 115000.00, N'סדאן', N'Images/car1.png', N'רכב משפחתי איכותי ונוח להשכרה', 7),
(N'יונדאי', N'איוניק', 2023, 130000.00, N'סדאן', N'Images/car1.png', N'רכב ירוק וחסכוני במיוחד בדלק', 3),
(N'קיה', N'ספורטאז''', 2023, 125000.00, N'ג''יפ', N'Images/car1.png', N'רכב פנאי-שטח פופולרי ומאובזר', 5);