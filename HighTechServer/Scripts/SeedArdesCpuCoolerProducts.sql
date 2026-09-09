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
USING (VALUES ('CPU Cooler')) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name);

MERGE dbo.Fields AS target
USING
(
    VALUES
        ('Socket Type', 18),
        ('Height (mm)', 9),
        ('Cooling Type', 18),
        ('Connectors', 18),
        ('Voltage', 18),
        ('Fan Speed', 18),
        ('Fans', 18)
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
    ON f.Name IN ('Socket Type', 'Height (mm)', 'Cooling Type',
                  'Connectors', 'Voltage', 'Fan Speed', 'Fans')
WHERE c.Name = 'CPU Cooler'
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
('CPU Cooler', 'NH-D15', 'Noctua', 'NH-D15', 60, 100.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-nh-d15-143169.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155, LGA1156, LGA2066, LGA2011'),
('CPU Cooler', 'NH-D15', 'Noctua', 'NH-D15', 60, 100.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-nh-d15-143169.jpg', 'Height (mm)', '165'),
('CPU Cooler', 'NH-D15.CH.BK', 'Noctua', 'NH-D15 chromax.black', 72, 118.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-nh-d15-chromax-black-272686.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155, LGA1156, LGA2066, LGA2011'),
('CPU Cooler', 'NH-D15.CH.BK', 'Noctua', 'NH-D15 chromax.black', 72, 118.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-nh-d15-chromax-black-272686.jpg', 'Height (mm)', '165'),
('CPU Cooler', 'R-AK400-BKADMN-G', 'Deepcool', 'AK400 Digital', 36, 46.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-ak400-digital-cpu-air-cooler-1x120mm-fk12-485749.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155'),
('CPU Cooler', 'R-AK400-BKADMN-G', 'Deepcool', 'AK400 Digital', 36, 46.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-ak400-digital-cpu-air-cooler-1x120mm-fk12-485749.jpg', 'Height (mm)', '156'),
('CPU Cooler', 'NH-D15S', 'Noctua', 'NH-D15S', 72, 92.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-nh-d15s-146985.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155, LGA1156, LGA2066, LGA2011'),
('CPU Cooler', 'NH-D15S', 'Noctua', 'NH-D15S', 72, 92.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-nh-d15s-146985.jpg', 'Height (mm)', '160'),
('CPU Cooler', 'NH-U12S', 'Noctua', 'NH-U12S', 72, 101.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-nh-u12s-165913.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155, LGA1156, LGA2066, LGA2011'),
('CPU Cooler', 'NH-U12S', 'Noctua', 'NH-U12S', 72, 101.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-nh-u12s-165913.jpg', 'Height (mm)', '158'),
('CPU Cooler', 'NH-U12A', 'Noctua', 'NH-U12A', 60, 109.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-ohladitel-cpu-cooler-nh-u12a-dual-fans-2066-249867.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155, LGA1156, LGA2066, LGA2011'),
('CPU Cooler', 'NH-U12A', 'Noctua', 'NH-U12A', 60, 109.00, 0.00, 10, 'https://ardes.bg/uploads/original/noctua-ohladitel-cpu-cooler-nh-u12a-dual-fans-2066-249867.jpg', 'Height (mm)', '158'),
('CPU Cooler', 'R-AG300-BKNNMN-G', 'Deepcool', 'AG300', 24, 16.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-ohladitel-cpu-cooler-ag300-lga1700-am5-414954.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155'),
('CPU Cooler', 'R-AG300-BKNNMN-G', 'Deepcool', 'AG300', 24, 16.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-ohladitel-cpu-cooler-ag300-lga1700-am5-414954.jpg', 'Height (mm)', '150'),
('CPU Cooler', 'R-AK620-BKNNMT-G', 'DeepCool', 'AK620', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/ohladitel-za-intel-amd-protsesori-deepcool-ak620-382981.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155'),
('CPU Cooler', 'R-AK620-BKNNMT-G', 'DeepCool', 'AK620', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/ohladitel-za-intel-amd-protsesori-deepcool-ak620-382981.jpg', 'Height (mm)', '160'),
('CPU Cooler', 'R-AK620-BKNNMT-G-1', 'Deepcool', 'AK620 Zero Dark', 36, 53.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-ohladitel-cpu-cooler-ak620-zero-dark-dual-414986.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155'),
('CPU Cooler', 'R-AK620-BKNNMT-G-1', 'Deepcool', 'AK620 Zero Dark', 36, 53.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-ohladitel-cpu-cooler-ak620-zero-dark-dual-414986.jpg', 'Height (mm)', '160'),
('CPU Cooler', 'ACFRE00123A', 'Arctic', 'Freezer 36 Black', 72, 34.00, 0.00, 10, 'https://ardes.bg/uploads/original/arctic-ohladitel-freezer-36-black-lga1851-lga1700-541631.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155'),
('CPU Cooler', 'ACFRE00123A', 'Arctic', 'Freezer 36 Black', 72, 34.00, 0.00, 10, 'https://ardes.bg/uploads/original/arctic-ohladitel-freezer-36-black-lga1851-lga1700-541631.jpg', 'Height (mm)', '159'),
('CPU Cooler', 'R-AK400-BKNNMN-G-1', 'DeepCool', 'AK400', 24, 29.00, 0.00, 10, 'https://ardes.bg/uploads/original/ohladitel-za-intel-amd-protsesori-deepcool-ak400-382967.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155'),
('CPU Cooler', 'R-AK400-BKNNMN-G-1', 'DeepCool', 'AK400', 24, 29.00, 0.00, 10, 'https://ardes.bg/uploads/original/ohladitel-za-intel-amd-protsesori-deepcool-ak400-382967.jpg', 'Height (mm)', '155'),
('CPU Cooler', 'ACFRE00124A', 'Arctic', 'Freezer 36 A-RGB Black', 72, 41.00, 0.00, 10, 'https://ardes.bg/uploads/original/arctic-ohladitel-freezer-36-a-rgb-black-lga1851-lg-541638.jpg', 'Socket Type', 'AM5, AM4, LGA1851, LGA1700, LGA1200, LGA1151, LGA1150, LGA1155'),
('CPU Cooler', 'ACFRE00124A', 'Arctic', 'Freezer 36 A-RGB Black', 72, 41.00, 0.00, 10, 'https://ardes.bg/uploads/original/arctic-ohladitel-freezer-36-a-rgb-black-lga1851-lg-541638.jpg', 'Height (mm)', '159');

INSERT INTO #SeedProducts
    (CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image, FieldName, FieldValue)
SELECT
    p.CategoryName, p.ProductKey, p.Manufacturer, p.Model, p.Warranty, p.Price, p.Discount, p.Quantity, p.Image,
    s.FieldName, s.FieldValue
FROM
(
    SELECT DISTINCT CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image
    FROM #SeedProducts
    WHERE CategoryName = 'CPU Cooler'
) p
INNER JOIN
(
    VALUES
        -- Noctua NH-D15
        ('NH-D15',              'Cooling Type', 'Air'),
        ('NH-D15',              'Connectors',   '4 Pin PWM'),
        ('NH-D15',              'Voltage',      '12 V'),
        ('NH-D15',              'Fan Speed',    '1500 RPM'),
        ('NH-D15',              'Fans',         '2 x 140 mm'),
        -- Noctua NH-D15 chromax.black
        ('NH-D15.CH.BK',        'Cooling Type', 'Air'),
        ('NH-D15.CH.BK',        'Connectors',   '4 Pin PWM'),
        ('NH-D15.CH.BK',        'Voltage',      '12 V'),
        ('NH-D15.CH.BK',        'Fan Speed',    '1500 RPM'),
        ('NH-D15.CH.BK',        'Fans',         '2 x 140 mm'),
        -- Deepcool AK400 Digital
        ('R-AK400-BKADMN-G',    'Cooling Type', 'Air'),
        ('R-AK400-BKADMN-G',    'Connectors',   '4 Pin PWM'),
        ('R-AK400-BKADMN-G',    'Voltage',      '12 V'),
        ('R-AK400-BKADMN-G',    'Fan Speed',    '1850 RPM'),
        ('R-AK400-BKADMN-G',    'Fans',         '1 x 120 mm'),
        -- Noctua NH-D15S
        ('NH-D15S',             'Cooling Type', 'Air'),
        ('NH-D15S',             'Connectors',   '4 Pin PWM'),
        ('NH-D15S',             'Voltage',      '12 V'),
        ('NH-D15S',             'Fan Speed',    '1500 RPM'),
        ('NH-D15S',             'Fans',         '1 x 140 mm'),
        -- Noctua NH-U12S
        ('NH-U12S',             'Cooling Type', 'Air'),
        ('NH-U12S',             'Connectors',   '4 Pin PWM'),
        ('NH-U12S',             'Voltage',      '12 V'),
        ('NH-U12S',             'Fan Speed',    '1500 RPM'),
        ('NH-U12S',             'Fans',         '1 x 120 mm'),
        -- Noctua NH-U12A
        ('NH-U12A',             'Cooling Type', 'Air'),
        ('NH-U12A',             'Connectors',   '4 Pin PWM'),
        ('NH-U12A',             'Voltage',      '12 V'),
        ('NH-U12A',             'Fan Speed',    '2000 RPM'),
        ('NH-U12A',             'Fans',         '2 x 120 mm'),
        -- Deepcool AG300
        ('R-AG300-BKNNMN-G',    'Cooling Type', 'Air'),
        ('R-AG300-BKNNMN-G',    'Connectors',   '4 Pin PWM'),
        ('R-AG300-BKNNMN-G',    'Voltage',      '12 V'),
        ('R-AG300-BKNNMN-G',    'Fan Speed',    '2050 RPM'),
        ('R-AG300-BKNNMN-G',    'Fans',         '1 x 92 mm'),
        -- DeepCool AK620
        ('R-AK620-BKNNMT-G',    'Cooling Type', 'Air'),
        ('R-AK620-BKNNMT-G',    'Connectors',   '4 Pin PWM'),
        ('R-AK620-BKNNMT-G',    'Voltage',      '12 V'),
        ('R-AK620-BKNNMT-G',    'Fan Speed',    '1850 RPM'),
        ('R-AK620-BKNNMT-G',    'Fans',         '2 x 120 mm'),
        -- Deepcool AK620 Zero Dark
        ('R-AK620-BKNNMT-G-1',  'Cooling Type', 'Air'),
        ('R-AK620-BKNNMT-G-1',  'Connectors',   '4 Pin PWM'),
        ('R-AK620-BKNNMT-G-1',  'Voltage',      '12 V'),
        ('R-AK620-BKNNMT-G-1',  'Fan Speed',    '1850 RPM'),
        ('R-AK620-BKNNMT-G-1',  'Fans',         '2 x 120 mm'),
        -- Arctic Freezer 36 Black
        ('ACFRE00123A',         'Cooling Type', 'Air'),
        ('ACFRE00123A',         'Connectors',   '4 Pin PWM'),
        ('ACFRE00123A',         'Voltage',      '12 V'),
        ('ACFRE00123A',         'Fan Speed',    '1800 RPM'),
        ('ACFRE00123A',         'Fans',         '2 x 120 mm'),
        -- DeepCool AK400
        ('R-AK400-BKNNMN-G-1',  'Cooling Type', 'Air'),
        ('R-AK400-BKNNMN-G-1',  'Connectors',   '4 Pin PWM'),
        ('R-AK400-BKNNMN-G-1',  'Voltage',      '12 V'),
        ('R-AK400-BKNNMN-G-1',  'Fan Speed',    '1850 RPM'),
        ('R-AK400-BKNNMN-G-1',  'Fans',         '1 x 120 mm'),
        -- Arctic Freezer 36 A-RGB Black
        ('ACFRE00124A',         'Cooling Type', 'Air'),
        ('ACFRE00124A',         'Connectors',   '4 Pin PWM'),
        ('ACFRE00124A',         'Voltage',      '12 V'),
        ('ACFRE00124A',         'Fan Speed',    '1800 RPM'),
        ('ACFRE00124A',         'Fans',         '2 x 120 mm A-RGB')
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
WHERE c.Name = 'CPU Cooler'
    AND p.IsRemoved = 0
GROUP BY c.Name;
