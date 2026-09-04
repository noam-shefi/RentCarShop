USE CarShopDB;
GO

-- ============================================
-- INSERT USERS
-- ============================================

INSERT INTO Users
(Username, Password, FirstName, LastName, Email, IsAdmin)
VALUES
(N'admin', N'123456', N'מנהל', N'מערכת', N'admin@carshop.com', 1),
(N'noam', N'123456', N'noam', N'noam', N'noam@gmail.com', 0);

-- ============================================
-- INSERT BRANCHES
-- ============================================

INSERT INTO Branches
(Name, City, Address, Phone)
VALUES
(N'סניף תל אביב', N'תל אביב', N'רחוב אלנבי 12', N'03-1234567'),
(N'סניף חיפה', N'חיפה', N'שדרות בן גוריון 5', N'04-1234567'),
(N'סניף ירושלים', N'ירושלים', N'רחוב יפו 40', N'02-1234567');

-- ============================================
-- INSERT CARS
-- ============================================

INSERT INTO Cars
(Manufacturer, Model, Year, Price, Category, ImageUrl, Description, Stock, BranchId, Seats, FuelType, RangeKm, Transmission, LuggageCapacity)
VALUES

-- Ford Mustang GT
(N'פורד', N'מוסטנג GT', 2023, 450, N'ספורט',
N'https://autoimage.capitalone.com/stock-media/evox/2023-Ford-Mustang-GT-AE-52239_cc2400_001_AE.png?height=480&width=640',
N'רכב ספורט חזק ומרשים', 6, 1, 4, N'בנזין', 550, N'אוטומטית', 2),

-- Jeep Wrangler
(N'ג''יפ', N'רנגלר', 2022, 500, N'ג''יפ',
N'https://www.motorbiscuit.com/wp-content/uploads/2022/08/2022-Jeep-Wrangler.jpg?w=1200',
N'רכב שטח עוצמתי', 4, 1, 5, N'בנזין', 500, N'אוטומטית', 3),

-- Toyota RAV4
(N'טויוטה', N'RAV4', 2023, 280, N'ג''יפ',
N'https://cars-asset.tvbs.com.tw/images/trims/6WGrsBjROOlccCcsjjZv4YvueHmIaAAsNK6KmpPV.jpg',
N'ג''יפ משפחתי היברידי', 2, 2, 5, N'היברידי', 850, N'אוטומטית', 3),

-- Chevrolet Corvette
(N'שברולט', N'קורבט', 2024, 950, N'ספורט',
N'https://hagerty-media-prod.imgix.net/2023/10/2024-chevrolet-corvette-e-ray-3lz-102.jpg?auto=format%2Ccompress',
N'מכונית על עם ביצועים יוצאי דופן', 1, 2, 2, N'בנזין', 500, N'אוטומטית', 1),

-- Dodge Challenger
(N'דודג''', N'צ''לנג''ר', 2022, 750, N'ספורט',
N'https://commons.wikimedia.org/wiki/Special:FilePath/White%20Dodge%20Challenger%20SRT%20side%20view.jpg',
N'רכב שרירים אמריקאי', 2, 1, 5, N'בנזין', 550, N'אוטומטית', 2),

-- Toyota Sienna
(N'טויוטה', N'סיינה', 2023, 320, N'משפחתי',
N'https://dealerimages.dealereprocess.com/image/upload/2852389',
N'מיניוואן מרווח למשפחות', 3, 3, 8, N'היברידי', 750, N'אוטומטית', 5),

-- Chrysler Pacifica
(N'קרייזלר', N'פאסיפיקה', 2023, 350, N'משפחתי',
N'https://crdms.images.consumerreports.org/c_lfill%2Cw_768%2Cq_auto%2Cf_auto/prod/cars/chrome/white/2023CRV100019_1280_03',
N'רכב משפחתי מפנק', 2, 3, 7, N'בנזין', 700, N'אוטומטית', 4),

-- Mazda 3
(N'מאזדה', N'3', 2022, 180, N'סדאן',
N'https://static.tcimg.net/vehicles/exterior_spin/aa56f8588538aada/2022-Mazda-Mazda3.jpg',
N'רכב משפחתי איכותי', 7, 1, 5, N'בנזין', 600, N'אוטומטית', 2),

-- Hyundai Ioniq
(N'יונדאי', N'איוניק', 2023, 200, N'סדאן',
N'https://a.storyblok.com/f/143588/1600x1067/db9d6a25f0/hyundai_ioniq_exterior1.jpg/m/filters%3Aquality%2880%29',
N'רכב ירוק וחסכוני', 3, 2, 5, N'היברידי', 900, N'אוטומטית', 3),

-- Kia Sportage
(N'קיה', N'ספורטאז''', 2023, 250, N'ג''יפ',
N'https://cfwww.hgregoire.com/photos/by-size/722454/3648x2048/7434882.JPG',
N'רכב פנאי-שטח מאובזר', 5, 2, 5, N'בנזין', 600, N'אוטומטית', 3);

GO