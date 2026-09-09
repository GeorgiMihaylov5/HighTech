SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF OBJECT_ID('tempdb..#SeedProducts') IS NOT NULL
    DROP TABLE #SeedProducts;

CREATE TABLE #SeedProducts
(
    CategoryName nvarchar(100) NOT NULL,
    ProductKey nvarchar(220) NOT NULL,
    Manufacturer nvarchar(80) NOT NULL,
    Model nvarchar(120) NOT NULL,
    Warranty int NOT NULL,
    Price decimal(18, 2) NOT NULL,
    Discount decimal(18, 2) NOT NULL,
    Quantity int NOT NULL,
    Image nvarchar(500) NOT NULL,
    FieldName nvarchar(100) NOT NULL,
    FieldValue nvarchar(250) NOT NULL
);

MERGE dbo.Categories AS target
USING (VALUES ('RAM')) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name);

MERGE dbo.Fields AS target
USING
(
    VALUES
        ('RAM Type', 18),
        ('RAM Speed (MHz)', 9),
        ('Capacity', 18),
        ('Timings', 18)
) AS source (Name, TypeCode)
ON target.Name = source.Name
WHEN MATCHED THEN
    UPDATE SET target.TypeCode = source.TypeCode
WHEN NOT MATCHED THEN
    INSERT (Id, Name, TypeCode)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name, source.TypeCode);

INSERT INTO dbo.CategoryFields (Id, CategoryId, FieldId)
SELECT
    CONVERT(nvarchar(36), NEWID()) AS Id,
    c.Id AS CategoryId,
    f.Id AS FieldId
FROM dbo.Categories c
INNER JOIN dbo.Fields f
    ON f.Name IN ('RAM Type', 'RAM Speed (MHz)', 'Capacity', 'Timings')
WHERE c.Name = 'RAM'
    AND NOT EXISTS
    (
        SELECT 1
        FROM dbo.CategoryFields cf
        WHERE cf.CategoryId = c.Id
            AND cf.FieldId = f.Id
    );

INSERT INTO #SeedProducts
    (CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image, FieldName, FieldValue)
VALUES
('RAM', 'KF432C16BBK2/16', 'Kingston', 'Fury Beast 2x8GB DDR4 3200', 60, 169.00, 0.00, 10, 'https://ardes.bg/uploads/original/2x8gb-ddr4-3200-kingston-fury-beast-black-337569.jpg', 'RAM Type', 'DDR4'),
('RAM', 'KF432C16BBK2/16', 'Kingston', 'Fury Beast 2x8GB DDR4 3200', 60, 169.00, 0.00, 10, 'https://ardes.bg/uploads/original/2x8gb-ddr4-3200-kingston-fury-beast-black-337569.jpg', 'RAM Speed (MHz)', '3200'),
('RAM', 'KF432C16BB/16', 'Kingston', 'Fury Beast 16GB DDR4 3200', 60, 155.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-dram-16gb-3200mhz-ddr4-cl16-dimm-fury-bea-341138.jpg', 'RAM Type', 'DDR4'),
('RAM', 'KF432C16BB/16', 'Kingston', 'Fury Beast 16GB DDR4 3200', 60, 155.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-dram-16gb-3200mhz-ddr4-cl16-dimm-fury-bea-341138.jpg', 'RAM Speed (MHz)', '3200'),
('RAM', 'CMW16GX4M2C3200C16', 'Corsair', 'Vengeance RGB Pro 2x8GB DDR4 3200', 60, 189.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-corsair-ddr4-3200mhz-16gb-2-x-8gb-288-dimm-u-215785.jpg', 'RAM Type', 'DDR4'),
('RAM', 'CMW16GX4M2C3200C16', 'Corsair', 'Vengeance RGB Pro 2x8GB DDR4 3200', 60, 189.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-corsair-ddr4-3200mhz-16gb-2-x-8gb-288-dimm-u-215785.jpg', 'RAM Speed (MHz)', '3200'),
('RAM', 'TED38G1600C1101', 'Team Group', 'Elite 8GB DDR3 1600', 60, 35.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-team-group-elite-ddr3-8gb-1600-mhz-cl11-11-1-177957.jpg', 'RAM Type', 'DDR3'),
('RAM', 'TED38G1600C1101', 'Team Group', 'Elite 8GB DDR3 1600', 60, 35.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-team-group-elite-ddr3-8gb-1600-mhz-cl11-11-1-177957.jpg', 'RAM Speed (MHz)', '1600'),
('RAM', 'CMK16GX4M2B3200C16', 'Corsair', 'Vengeance LPX 2x8GB DDR4 3200', 60, 169.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-corsair-ddr4-3200mhz-16gb-2-x-8gb-288-dimm-u-230216.jpg', 'RAM Type', 'DDR4'),
('RAM', 'CMK16GX4M2B3200C16', 'Corsair', 'Vengeance LPX 2x8GB DDR4 3200', 60, 169.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-corsair-ddr4-3200mhz-16gb-2-x-8gb-288-dimm-u-230216.jpg', 'RAM Speed (MHz)', '3200'),
('RAM', 'KF432C16BBK2/32', 'Kingston', 'Fury Beast 2x16GB DDR4 3200', 60, 319.00, 0.00, 10, 'https://ardes.bg/uploads/original/2x16g-ddr4-3200-kingst-beast-341693.jpg', 'RAM Type', 'DDR4'),
('RAM', 'KF432C16BBK2/32', 'Kingston', 'Fury Beast 2x16GB DDR4 3200', 60, 319.00, 0.00, 10, 'https://ardes.bg/uploads/original/2x16g-ddr4-3200-kingst-beast-341693.jpg', 'RAM Speed (MHz)', '3200'),
('RAM', 'KF432C16BB1K2/32', 'Kingston', 'Fury Beast RGB 2x16GB DDR4 3200', 60, 324.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-32gb-ddr4-3200-kingston-fury-beast-rgb-337592.jpg', 'RAM Type', 'DDR4'),
('RAM', 'KF432C16BB1K2/32', 'Kingston', 'Fury Beast RGB 2x16GB DDR4 3200', 60, 324.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-32gb-ddr4-3200-kingston-fury-beast-rgb-337592.jpg', 'RAM Speed (MHz)', '3200'),
('RAM', 'KF432C16BB/8', 'Kingston', 'Fury Beast 8GB DDR4 3200', 60, 85.00, 0.00, 10, 'https://ardes.bg/uploads/original/8g-ddr4-3200-kingst-fury-beast-336294.jpg', 'RAM Type', 'DDR4'),
('RAM', 'KF432C16BB/8', 'Kingston', 'Fury Beast 8GB DDR4 3200', 60, 85.00, 0.00, 10, 'https://ardes.bg/uploads/original/8g-ddr4-3200-kingst-fury-beast-336294.jpg', 'RAM Speed (MHz)', '3200'),
('RAM', 'SP008GBLTU160N02', 'Silicon Power', '8GB DDR3 1600', 60, 39.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-silicon-power-8gb-ddr3-pc3-12800-1600mhz-cl1-304937.jpg', 'RAM Type', 'DDR3'),
('RAM', 'SP008GBLTU160N02', 'Silicon Power', '8GB DDR3 1600', 60, 39.00, 0.00, 10, 'https://ardes.bg/uploads/original/pamet-silicon-power-8gb-ddr3-pc3-12800-1600mhz-cl1-304937.jpg', 'RAM Speed (MHz)', '1600'),
('RAM', 'KF560C30BBEAK2-32', 'Kingston', 'Fury Beast RGB 2x16GB DDR5 6000', 60, 528.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-64gb-6000mt-s-ddr5-cl30-dimm-kit-of-2-fur-546591.jpg', 'RAM Type', 'DDR5'),
('RAM', 'KF560C30BBEAK2-32', 'Kingston', 'Fury Beast RGB 2x16GB DDR5 6000', 60, 528.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-64gb-6000mt-s-ddr5-cl30-dimm-kit-of-2-fur-546591.jpg', 'RAM Speed (MHz)', '6000'),
('RAM', 'TED48G3200C22016', 'Team Group', 'Elite 8GB DDR4 3200', 60, 65.00, 0.00, 10, 'https://ardes.bg/uploads/original/8gb-ddr4-3200-team-group-elite-406475.jpg', 'RAM Type', 'DDR4'),
('RAM', 'TED48G3200C22016', 'Team Group', 'Elite 8GB DDR4 3200', 60, 65.00, 0.00, 10, 'https://ardes.bg/uploads/original/8gb-ddr4-3200-team-group-elite-406475.jpg', 'RAM Speed (MHz)', '3200'),
('RAM', 'KF560C30BBEK2-32', 'Kingston', 'Fury Beast 2x16GB DDR5 6000', 60, 490.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-32gb-6000mt-s-ddr5-cl30-dimm-kit-of-2-fur-546580.jpg', 'RAM Type', 'DDR5'),
('RAM', 'KF560C30BBEK2-32', 'Kingston', 'Fury Beast 2x16GB DDR5 6000', 60, 490.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-32gb-6000mt-s-ddr5-cl30-dimm-kit-of-2-fur-546580.jpg', 'RAM Speed (MHz)', '6000');

INSERT INTO #SeedProducts
    (CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image, FieldName, FieldValue)
SELECT
    p.CategoryName, p.ProductKey, p.Manufacturer, p.Model, p.Warranty, p.Price, p.Discount, p.Quantity, p.Image,
    s.FieldName, s.FieldValue
FROM
(
    SELECT DISTINCT CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image
    FROM #SeedProducts
    WHERE CategoryName = 'RAM'
) p
INNER JOIN
(
    VALUES
        ('KF432C16BBK2/16',     'Capacity', '16 GB (2 x 8 GB)'),
        ('KF432C16BBK2/16',     'Timings',  'CL16-18-18-36'),
        ('KF432C16BB/16',       'Capacity', '16 GB (1 x 16 GB)'),
        ('KF432C16BB/16',       'Timings',  'CL16-18-18-36'),
        ('CMW16GX4M2C3200C16',  'Capacity', '16 GB (2 x 8 GB)'),
        ('CMW16GX4M2C3200C16',  'Timings',  'CL16-18-18-36'),
        ('TED38G1600C1101',     'Capacity', '8 GB (1 x 8 GB)'),
        ('TED38G1600C1101',     'Timings',  'CL11-11-11-28'),
        ('CMK16GX4M2B3200C16',  'Capacity', '16 GB (2 x 8 GB)'),
        ('CMK16GX4M2B3200C16',  'Timings',  'CL16-18-18-36'),
        ('KF432C16BBK2/32',     'Capacity', '32 GB (2 x 16 GB)'),
        ('KF432C16BBK2/32',     'Timings',  'CL16-20-20-38'),
        ('KF432C16BB1K2/32',    'Capacity', '32 GB (2 x 16 GB)'),
        ('KF432C16BB1K2/32',    'Timings',  'CL16-20-20-38'),
        ('KF432C16BB/8',        'Capacity', '8 GB (1 x 8 GB)'),
        ('KF432C16BB/8',        'Timings',  'CL16-18-18-36'),
        ('SP008GBLTU160N02',    'Capacity', '8 GB (1 x 8 GB)'),
        ('SP008GBLTU160N02',    'Timings',  'CL11'),
        ('KF560C30BBEAK2-32',   'Capacity', '32 GB (2 x 16 GB)'),
        ('KF560C30BBEAK2-32',   'Timings',  'CL30-38-38-80'),
        ('TED48G3200C22016',    'Capacity', '8 GB (1 x 8 GB)'),
        ('TED48G3200C22016',    'Timings',  'CL22-22-22-52'),
        ('KF560C30BBEK2-32',    'Capacity', '32 GB (2 x 16 GB)'),
        ('KF560C30BBEK2-32',    'Timings',  'CL30-38-38-80')
) AS s(ProductKey, FieldName, FieldValue)
    ON s.ProductKey = p.ProductKey;

;WITH DistinctProducts AS
(
    SELECT
        ProductKey,
        Manufacturer,
        Model,
        Warranty,
        Price,
        Discount,
        Quantity,
        Image,
        ROW_NUMBER() OVER (PARTITION BY ProductKey ORDER BY ProductKey) AS RowNumber
    FROM #SeedProducts
)
MERGE dbo.Products AS target
USING
(
    SELECT ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image
    FROM DistinctProducts
    WHERE RowNumber = 1
) AS source
ON target.Manufacturer = source.Manufacturer
    AND target.Model = source.Model
    AND target.Image = source.Image
WHEN MATCHED THEN
    UPDATE SET
        target.Warranty = source.Warranty,
        target.Price = source.Price,
        target.Discount = source.Discount,
        target.Quantity = source.Quantity,
        target.Image = source.Image,
        target.IsRemoved = 0
WHEN NOT MATCHED THEN
    INSERT (Id, Manufacturer, Model, Warranty, Price, Discount, Quantity, IsRemoved, Image)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Manufacturer, source.Model, source.Warranty, source.Price, source.Discount, source.Quantity, 0, source.Image);

DELETE pc
FROM dbo.ProductFieldValues pc
INNER JOIN dbo.Products p ON p.Id = pc.ProductId
INNER JOIN #SeedProducts sp ON sp.Manufacturer = p.Manufacturer AND sp.Model = p.Model
    AND sp.Image = p.Image
INNER JOIN dbo.CategoryFields cf ON cf.Id = pc.CategoryFieldId
INNER JOIN dbo.Categories c ON c.Id = cf.CategoryId AND c.Name = sp.CategoryName;

INSERT INTO dbo.ProductFieldValues (Id, ProductId, CategoryFieldId, Value)
SELECT
    CONVERT(nvarchar(36), NEWID()) AS Id,
    p.Id AS ProductId,
    cf.Id AS CategoryFieldId,
    sp.FieldValue AS Value
FROM #SeedProducts sp
INNER JOIN dbo.Products p
    ON p.Manufacturer = sp.Manufacturer
    AND p.Model = sp.Model
    AND p.Image = sp.Image
INNER JOIN dbo.Categories c
    ON c.Name = sp.CategoryName
INNER JOIN dbo.Fields f
    ON f.Name = sp.FieldName
INNER JOIN dbo.CategoryFields cf
    ON cf.CategoryId = c.Id
    AND cf.FieldId = f.Id;

COMMIT TRANSACTION;

SELECT
    c.Name AS Category,
    COUNT(DISTINCT p.Id) AS ProductCount
FROM dbo.Products p
INNER JOIN dbo.ProductFieldValues pc ON pc.ProductId = p.Id
INNER JOIN dbo.CategoryFields cf ON cf.Id = pc.CategoryFieldId
INNER JOIN dbo.Categories c ON c.Id = cf.CategoryId
WHERE c.Name = 'RAM'
    AND p.IsRemoved = 0
GROUP BY c.Name;
