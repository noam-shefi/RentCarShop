-- עדכון תמונות אמיתיות לרכבים שעדיין מציגים את הרכב הכחול של ברירת המחדל
UPDATE Cars SET ImageUrl = N'https://images.unsplash.com/photo-1549399542-7e3f8b79c341?w=600' WHERE Model LIKE N'%פאסיפיקה%';
UPDATE Cars SET ImageUrl = N'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=600' WHERE Model LIKE N'%סיינה%';
UPDATE Cars SET ImageUrl = N'https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=600' WHERE Model LIKE N'%איוניק%';
UPDATE Cars SET ImageUrl = N'https://images.unsplash.com/photo-1568605117036-5fe5e7bab0b7?w=600' WHERE Model LIKE N'%ספורטאז''%';