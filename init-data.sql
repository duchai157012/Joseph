-- Init Data for CatalogDb
USE CatalogDb;
GO

IF NOT EXISTS (SELECT * FROM Products)
BEGIN
    INSERT INTO Products (Id, Name, Description, Price, PictureUrl, AvailableStock, CreatedAt)
    VALUES 
    (NEWID(), 'iPhone 15 Pro', 'Titanium design', 999.00, 'https://example.com/iphone15.png', 100, GETDATE()),
    (NEWID(), 'Samsung Galaxy S24', 'AI Phone', 899.00, 'https://example.com/s24.png', 50, GETDATE()),
    (NEWID(), 'MacBook Air M3', 'Super fast laptop', 1299.00, 'https://example.com/macbook.png', 20, GETDATE());
    
    PRINT 'Seeded CatalogDb.Products';
END
ELSE
BEGIN
    PRINT 'CatalogDb.Products already has data';
END
GO

-- Init Data for OrderDb
USE OrderDb;
GO

IF NOT EXISTS (SELECT * FROM Orders)
BEGIN
    INSERT INTO Orders (Id, TotalPrice, Status, CreatedAt)
    VALUES 
    (NEWID(), 999.00, 'Completed', GETDATE()),
    (NEWID(), 450.50, 'Pending', GETDATE());

    PRINT 'Seeded OrderDb.Orders';
END
ELSE
BEGIN
    PRINT 'OrderDb.Orders already has data';
END
GO
