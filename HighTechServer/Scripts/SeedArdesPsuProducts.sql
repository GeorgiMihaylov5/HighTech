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
USING (VALUES ('PSU')) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name);

MERGE dbo.Fields AS target
USING
(
    VALUES
        ('Wattage (W)', 9),
        ('Form Factor', 18),
        ('Connectors', 18),
        ('Cooling', 18),
        ('Efficiency', 18),
        ('Energy Efficiency Rating', 18),
        ('PFC', 18),
        ('Cabling Type', 18),
        ('Protections', 18),
        ('Dimensions', 18)
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
    ON f.Name IN ('Wattage (W)', 'Form Factor', 'Connectors', 'Cooling',
                  'Efficiency', 'Energy Efficiency Rating', 'PFC',
                  'Cabling Type', 'Protections', 'Dimensions')
WHERE c.Name = 'PSU'
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
('PSU', 'ACPB-LD65AEC.11', 'AeroCool', 'LUX Bronze 650W', 24, 44.00, 0.00, 10, 'https://ardes.bg/uploads/original/aerocool-zahranvane-psu-lux-650w-bronze-acpb-ld65a-252015.jpg', 'Wattage (W)', '650'),
('PSU', 'ACPB-LX65AEC.11', 'AeroCool', 'LUX RGB 650W', 24, 45.00, 0.00, 10, 'https://ardes.bg/uploads/original/aerocool-zahranvane-psu-lux-rgb-650w-bronze-rgb-ad-280081.jpg', 'Wattage (W)', '650'),
('PSU', 'ACPB-LD75AEC.11', 'AeroCool', 'LUX Bronze 750W', 24, 49.00, 0.00, 10, 'https://ardes.bg/uploads/original/aerocool-zahranvane-psu-lux-750w-bronze-acpb-ld75a-280043.jpg', 'Wattage (W)', '750'),
('PSU', 'ZM700-TXII', 'Zalman', 'MegaMax 700W', 24, 54.00, 0.00, 10, 'https://ardes.bg/uploads/original/zalman-zahranvane-psu-megamax-700w-80-zm700-txii-265763.jpg', 'Wattage (W)', '700'),
('PSU', '306-7ZP2B11-CE0', 'MSI', 'MAG A650BN 650W', 24, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-a650bn-650w-80-plus-bronze-120mm-low-noise-363688.jpg', 'Wattage (W)', '650'),
('PSU', 'PS-600FK', '1stPlayer', 'FK 6.0 600W', 24, 33.00, 0.00, 10, 'https://ardes.bg/uploads/original/1stplayer-zahranvasht-blok-psu-600w-apfc-ps-600fk-418734.jpg', 'Wattage (W)', '600'),
('PSU', 'ACPB-LD55AEC.11', 'AeroCool', 'LUX Bronze 550W', 24, 37.00, 0.00, 10, 'https://ardes.bg/uploads/original/aerocool-zahranvane-psu-lux-550w-bronze-acpb-ld55a-252037.jpg', 'Wattage (W)', '550'),
('PSU', '306-7ZP8B11-CE0', 'MSI', 'MAG A750GL 750W', 24, 85.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-a750gl-pcie5-750w-80-plus-gold-atx-form-fa-479908.jpg', 'Wattage (W)', '750'),
('PSU', '90YE00S2-B0NA00', 'ASUS', 'TUF Gaming 80+ Gold 850W', 24, 129.00, 0.00, 10, 'https://ardes.bg/uploads/original/asus-tuf-gaming-850w-gold-fully-modular-power-supp-424228.jpg', 'Wattage (W)', '850'),
('PSU', '306-7ZP8A11-CE0', 'MSI', 'MAG A850GL 80+ Gold 850W', 24, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-a850gl-pcie5-600w-80-plus-gold-120mm-fluid-486155.jpg', 'Wattage (W)', '850'),
('PSU', 'ACPB-LX55AEC.11', 'AeroCool', 'LUX RGB 550W', 24, 38.00, 0.00, 10, 'https://ardes.bg/uploads/original/aerocool-zahranvane-psu-lux-rgb-550w-bronze-rgb-ad-280098.jpg', 'Wattage (W)', '550'),
('PSU', '9PA4507901', 'Fortron', 'SP500-A 450W', 24, 39.00, 0.00, 10, 'https://ardes.bg/uploads/original/psu-fortron-sp500-a-450w-157200.jpg', 'Wattage (W)', '450');

INSERT INTO #SeedProducts
    (CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image, FieldName, FieldValue)
SELECT
    p.CategoryName, p.ProductKey, p.Manufacturer, p.Model, p.Warranty, p.Price, p.Discount, p.Quantity, p.Image,
    s.FieldName, s.FieldValue
FROM
(
    SELECT DISTINCT CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image
    FROM #SeedProducts
    WHERE CategoryName = 'PSU'
) p
INNER JOIN
(
    VALUES
        -- AeroCool LUX Bronze 650W
        ('ACPB-LD65AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LD65AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 4 x SATA, 3 x Molex'),
        ('ACPB-LD65AEC.11', 'Cooling',                   '120 mm fan'),
        ('ACPB-LD65AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LD65AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LD65AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LD65AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LD65AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LD65AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        -- AeroCool LUX RGB 650W
        ('ACPB-LX65AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LX65AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 4 x SATA, 3 x Molex'),
        ('ACPB-LX65AEC.11', 'Cooling',                   '120 mm RGB fan'),
        ('ACPB-LX65AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LX65AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LX65AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LX65AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LX65AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LX65AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        -- AeroCool LUX Bronze 750W
        ('ACPB-LD75AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LD75AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 6 x SATA, 3 x Molex'),
        ('ACPB-LD75AEC.11', 'Cooling',                   '120 mm fan'),
        ('ACPB-LD75AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LD75AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LD75AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LD75AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LD75AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LD75AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        -- Zalman MegaMax 700W
        ('ZM700-TXII',      'Form Factor',               'ATX'),
        ('ZM700-TXII',      'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 6 x SATA, 3 x Molex'),
        ('ZM700-TXII',      'Cooling',                   '120 mm fan'),
        ('ZM700-TXII',      'Efficiency',                '85%'),
        ('ZM700-TXII',      'Energy Efficiency Rating',  '80 Plus'),
        ('ZM700-TXII',      'PFC',                       'Active PFC'),
        ('ZM700-TXII',      'Cabling Type',              'Non-Modular'),
        ('ZM700-TXII',      'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ZM700-TXII',      'Dimensions',                '140 x 150 x 86 mm'),
        -- MSI MAG A650BN 650W
        ('306-7ZP2B11-CE0', 'Form Factor',               'ATX'),
        ('306-7ZP2B11-CE0', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 6 x SATA, 3 x Molex'),
        ('306-7ZP2B11-CE0', 'Cooling',                   '120 mm low-noise fan'),
        ('306-7ZP2B11-CE0', 'Efficiency',                '88%'),
        ('306-7ZP2B11-CE0', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('306-7ZP2B11-CE0', 'PFC',                       'Active PFC'),
        ('306-7ZP2B11-CE0', 'Cabling Type',              'Non-Modular'),
        ('306-7ZP2B11-CE0', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('306-7ZP2B11-CE0', 'Dimensions',                '140 x 150 x 86 mm'),
        -- 1stPlayer FK 6.0 600W
        ('PS-600FK',        'Form Factor',               'ATX'),
        ('PS-600FK',        'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 1 x 6+2 PCIe, 4 x SATA, 2 x Molex'),
        ('PS-600FK',        'Cooling',                   '120 mm fan'),
        ('PS-600FK',        'Efficiency',                '82%'),
        ('PS-600FK',        'Energy Efficiency Rating',  '80 Plus'),
        ('PS-600FK',        'PFC',                       'Active PFC'),
        ('PS-600FK',        'Cabling Type',              'Non-Modular'),
        ('PS-600FK',        'Protections',               'OVP, UVP, SCP, OCP, OPP'),
        ('PS-600FK',        'Dimensions',                '140 x 150 x 86 mm'),
        -- AeroCool LUX Bronze 550W
        ('ACPB-LD55AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LD55AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 4 x SATA, 3 x Molex'),
        ('ACPB-LD55AEC.11', 'Cooling',                   '120 mm fan'),
        ('ACPB-LD55AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LD55AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LD55AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LD55AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LD55AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LD55AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        -- MSI MAG A750GL 750W Gold
        ('306-7ZP8B11-CE0', 'Form Factor',               'ATX'),
        ('306-7ZP8B11-CE0', 'Connectors',                '1 x 24-pin ATX, 2 x 4+4 CPU, 1 x 12+4 PCIe (12VHPWR), 3 x 6+2 PCIe, 8 x SATA, 4 x Molex'),
        ('306-7ZP8B11-CE0', 'Cooling',                   '120 mm fluid dynamic bearing fan'),
        ('306-7ZP8B11-CE0', 'Efficiency',                '90%'),
        ('306-7ZP8B11-CE0', 'Energy Efficiency Rating',  '80 Plus Gold'),
        ('306-7ZP8B11-CE0', 'PFC',                       'Active PFC'),
        ('306-7ZP8B11-CE0', 'Cabling Type',              'Fully Modular'),
        ('306-7ZP8B11-CE0', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('306-7ZP8B11-CE0', 'Dimensions',                '140 x 150 x 86 mm'),
        -- ASUS TUF Gaming 850W Gold
        ('90YE00S2-B0NA00', 'Form Factor',               'ATX'),
        ('90YE00S2-B0NA00', 'Connectors',                '1 x 24-pin ATX, 2 x 4+4 CPU, 4 x 6+2 PCIe, 8 x SATA, 4 x Molex'),
        ('90YE00S2-B0NA00', 'Cooling',                   '135 mm dual ball bearing fan'),
        ('90YE00S2-B0NA00', 'Efficiency',                '90%'),
        ('90YE00S2-B0NA00', 'Energy Efficiency Rating',  '80 Plus Gold'),
        ('90YE00S2-B0NA00', 'PFC',                       'Active PFC'),
        ('90YE00S2-B0NA00', 'Cabling Type',              'Fully Modular'),
        ('90YE00S2-B0NA00', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('90YE00S2-B0NA00', 'Dimensions',                '150 x 150 x 86 mm'),
        -- MSI MAG A850GL 850W Gold
        ('306-7ZP8A11-CE0', 'Form Factor',               'ATX'),
        ('306-7ZP8A11-CE0', 'Connectors',                '1 x 24-pin ATX, 2 x 4+4 CPU, 1 x 12+4 PCIe (12VHPWR), 3 x 6+2 PCIe, 8 x SATA, 4 x Molex'),
        ('306-7ZP8A11-CE0', 'Cooling',                   '120 mm fluid dynamic bearing fan'),
        ('306-7ZP8A11-CE0', 'Efficiency',                '90%'),
        ('306-7ZP8A11-CE0', 'Energy Efficiency Rating',  '80 Plus Gold'),
        ('306-7ZP8A11-CE0', 'PFC',                       'Active PFC'),
        ('306-7ZP8A11-CE0', 'Cabling Type',              'Fully Modular'),
        ('306-7ZP8A11-CE0', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('306-7ZP8A11-CE0', 'Dimensions',                '140 x 150 x 86 mm'),
        -- AeroCool LUX RGB 550W
        ('ACPB-LX55AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LX55AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 4 x SATA, 3 x Molex'),
        ('ACPB-LX55AEC.11', 'Cooling',                   '120 mm RGB fan'),
        ('ACPB-LX55AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LX55AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LX55AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LX55AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LX55AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LX55AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        -- Fortron SP500-A 450W
        ('9PA4507901',      'Form Factor',               'ATX'),
        ('9PA4507901',      'Connectors',                '1 x 24-pin ATX, 1 x 4-pin CPU, 1 x 6-pin PCIe, 3 x SATA, 2 x Molex'),
        ('9PA4507901',      'Cooling',                   '120 mm fan'),
        ('9PA4507901',      'Efficiency',                '80%'),
        ('9PA4507901',      'Energy Efficiency Rating',  '80 Plus'),
        ('9PA4507901',      'PFC',                       'Active PFC'),
        ('9PA4507901',      'Cabling Type',              'Non-Modular'),
        ('9PA4507901',      'Protections',               'OVP, UVP, SCP, OCP, OPP'),
        ('9PA4507901',      'Dimensions',                '140 x 150 x 86 mm')
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
WHERE c.Name = 'PSU'
    AND p.IsRemoved = 0
GROUP BY c.Name;
