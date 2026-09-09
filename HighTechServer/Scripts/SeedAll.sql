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

-- =====================================================================
-- CPU
-- =====================================================================
MERGE dbo.Categories AS target
USING (VALUES ('CPU')) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name);

MERGE dbo.Fields AS target
USING
(
    VALUES
        ('Socket Type', 18),
        ('TDP (W)', 9),
        ('Graphic Core', 18),
        ('Base Clock', 18),
        ('Turbo Boost Clock', 18),
        ('Physical Cores', 9),
        ('Logical Cores', 9),
        ('Cache Memory', 18),
        ('Included Cooler', 18)
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
    ON f.Name IN ('Socket Type', 'TDP (W)', 'Graphic Core',
                  'Base Clock', 'Turbo Boost Clock', 'Physical Cores',
                  'Logical Cores', 'Cache Memory', 'Included Cooler')
WHERE c.Name = 'CPU'
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
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Socket Type', 'LGA1200'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'TDP (W)', '58'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Graphic Core', 'Intel UHD Graphics 610'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Base Clock', '3.5'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Turbo Boost Clock', '3.5'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Physical Cores', '2'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Logical Cores', '2'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Cache Memory', 'L2: 1 MB, L3: 4 MB'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Included Cooler', 'None'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Socket Type', 'AM4'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'TDP (W)', '65'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Graphic Core', 'Radeon Vega 8 Graphics'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Base Clock', '3.6'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Turbo Boost Clock', '4.0'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Physical Cores', '4'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Logical Cores', '4'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Cache Memory', 'L2: 2 MB, L3: 4 MB'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Included Cooler', 'None'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'TDP (W)', '65'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Base Clock', '3.8'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Turbo Boost Clock', '4.0'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Physical Cores', '4'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Logical Cores', '8'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Cache Memory', 'L2: 2 MB, L3: 4 MB'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Included Cooler', 'Wraith Stealth'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Socket Type', 'AM4'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'TDP (W)', '65'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Graphic Core', 'None'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Base Clock', '3.6'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Turbo Boost Clock', '4.2'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Physical Cores', '6'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Logical Cores', '12'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Cache Memory', 'L2: 3 MB, L3: 16 MB'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Included Cooler', 'None'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'TDP (W)', '65'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Base Clock', '3.6'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Turbo Boost Clock', '4.1'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Physical Cores', '6'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Logical Cores', '12'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Cache Memory', 'L2: 3 MB, L3: 8 MB'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Included Cooler', 'Wraith Stealth'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'TDP (W)', '65'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Base Clock', '3.6'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Turbo Boost Clock', '4.2'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Physical Cores', '6'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Logical Cores', '12'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Cache Memory', 'L2: 3 MB, L3: 16 MB'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Included Cooler', 'None'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'TDP (W)', '58'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Graphic Core', 'None'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Base Clock', '3.3'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Turbo Boost Clock', '4.3'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Physical Cores', '4'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Logical Cores', '8'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Included Cooler', 'None'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'TDP (W)', '58'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Graphic Core', 'None'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Base Clock', '3.3'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Turbo Boost Clock', '4.3'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Physical Cores', '4'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Logical Cores', '8'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Included Cooler', 'Intel Laminar RM1'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'TDP (W)', '58'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Graphic Core', 'None'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Base Clock', '3.5'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Turbo Boost Clock', '4.7'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Physical Cores', '4'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Logical Cores', '8'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Included Cooler', 'None'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'TDP (W)', '58'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Graphic Core', 'None'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Base Clock', '3.4'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Turbo Boost Clock', '4.5'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Physical Cores', '4'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Logical Cores', '8'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Included Cooler', 'Intel Laminar RM1'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'TDP (W)', '65'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Base Clock', '3.6'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Turbo Boost Clock', '4.2'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Physical Cores', '6'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Logical Cores', '12'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Cache Memory', 'L2: 3 MB, L3: 16 MB'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Included Cooler', 'Wraith Stealth'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'TDP (W)', '35'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Graphic Core', 'Radeon Graphics'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Base Clock', '3.1'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Turbo Boost Clock', '4.3'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Physical Cores', '8'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Logical Cores', '16'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Cache Memory', 'L2: 4 MB, L3: 8 MB'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Included Cooler', 'None'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Socket Type', 'AM4'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'TDP (W)', '65'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Graphic Core', 'None'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Base Clock', '3.5'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Physical Cores', '6'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Logical Cores', '12'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Cache Memory', 'L2: 3 MB, L3: 32 MB'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Included Cooler', 'None'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'TDP (W)', '58'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Graphic Core', 'None'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Base Clock', '3.5'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Turbo Boost Clock', '4.7'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Physical Cores', '4'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Logical Cores', '8'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Included Cooler', 'Intel Laminar RM1'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Socket Type', 'LGA1200'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'TDP (W)', '65'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Graphic Core', 'Intel UHD Graphics 630'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Base Clock', '3.7'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Physical Cores', '4'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Logical Cores', '8'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Cache Memory', 'L2: 1 MB, L3: 6 MB'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Included Cooler', 'None'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'TDP (W)', '65'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Base Clock', '3.5'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Physical Cores', '6'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Logical Cores', '12'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Cache Memory', 'L2: 3 MB, L3: 32 MB'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Included Cooler', 'None'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'TDP (W)', '60'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Graphic Core', 'Intel UHD Graphics 730'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Base Clock', '3.3'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Turbo Boost Clock', '4.3'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Physical Cores', '4'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Logical Cores', '8'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Included Cooler', 'None'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Socket Type', 'AM5'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'TDP (W)', '65'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Graphic Core', 'None'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Base Clock', '3.7'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Turbo Boost Clock', '5.0'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Physical Cores', '6'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Logical Cores', '12'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Cache Memory', 'L2: 6 MB, L3: 32 MB'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Included Cooler', 'None'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'TDP (W)', '65'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Base Clock', '3.5'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Physical Cores', '6'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Logical Cores', '12'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Cache Memory', 'L2: 3 MB, L3: 32 MB'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Included Cooler', 'Wraith Stealth'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'TDP (W)', '65'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Graphic Core', 'None'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Base Clock', '2.5'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Physical Cores', '6'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Logical Cores', '12'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Cache Memory', 'L2: 7.5 MB, L3: 18 MB'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Included Cooler', 'None'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'TDP (W)', '65'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Graphic Core', 'Radeon Graphics'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Base Clock', '3.6'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Turbo Boost Clock', '4.6'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Physical Cores', '6'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Logical Cores', '12'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Cache Memory', 'L2: 3 MB, L3: 16 MB'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Included Cooler', 'None'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Socket Type', 'AM5'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'TDP (W)', '65'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Graphic Core', 'None'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Base Clock', '4.2'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Turbo Boost Clock', '4.7'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Physical Cores', '6'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Logical Cores', '12'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Cache Memory', 'L2: 6 MB, L3: 16 MB'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Included Cooler', 'None'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Socket Type', 'AM5'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'TDP (W)', '65'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Graphic Core', 'None'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Base Clock', '4.1'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Turbo Boost Clock', '5.0'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Physical Cores', '8'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Logical Cores', '16'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Cache Memory', 'L2: 8 MB, L3: 24 MB'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Included Cooler', 'None'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Socket Type', 'AM4'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'TDP (W)', '65'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Graphic Core', 'None'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Base Clock', '3.7'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Turbo Boost Clock', '4.6'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Physical Cores', '6'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Logical Cores', '12'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Cache Memory', 'L2: 3 MB, L3: 32 MB'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Included Cooler', 'None');

-- =====================================================================
-- Motherboard
-- =====================================================================
MERGE dbo.Categories AS target
USING (VALUES ('Motherboard')) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name);

MERGE dbo.Fields AS target
USING
(
    VALUES
        ('Chipset', 18),
        ('Memory Slots', 9),
        ('Max Supported Memory', 18),
        ('LAN', 18),
        ('Bluetooth', 18),
        ('Interfaces', 18),
        ('Ports', 18),
        ('Size', 18),
        ('Socket Type', 18),
        ('RAM Type', 18),
        ('Motherboard Form Factor', 18),
        ('Max RAM Speed (MHz)', 9),
        ('M.2 Slots', 9),
        ('SATA Ports', 9)
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
    ON f.Name IN ('Chipset', 'Memory Slots', 'Max Supported Memory',
                  'LAN', 'Bluetooth', 'Interfaces', 'Ports', 'Size',
                  'Socket Type', 'RAM Type', 'Motherboard Form Factor',
                  'Max RAM Speed (MHz)', 'M.2 Slots', 'SATA Ports')
WHERE c.Name = 'Motherboard'
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
('Motherboard', '911-7C96-001', 'MSI', 'A520M-A PRO', 36, 53.00, 0.00, 10, 'https://ardes.bg/uploads/original/a520-am4-2xddr4-1xpci-ex16-4xsata-1xm2-dvi-hdmi-m-299596.jpg', 'Socket Type', 'AM4'),
('Motherboard', '911-7C96-001', 'MSI', 'A520M-A PRO', 36, 53.00, 0.00, 10, 'https://ardes.bg/uploads/original/a520-am4-2xddr4-1xpci-ex16-4xsata-1xm2-dvi-hdmi-m-299596.jpg', 'RAM Type', 'DDR4'),
('Motherboard', '911-7C96-001', 'MSI', 'A520M-A PRO', 36, 53.00, 0.00, 10, 'https://ardes.bg/uploads/original/a520-am4-2xddr4-1xpci-ex16-4xsata-1xm2-dvi-hdmi-m-299596.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', '911-7C96-001', 'MSI', 'A520M-A PRO', 36, 53.00, 0.00, 10, 'https://ardes.bg/uploads/original/a520-am4-2xddr4-1xpci-ex16-4xsata-1xm2-dvi-hdmi-m-299596.jpg', 'Max RAM Speed (MHz)', '4600'),
('Motherboard', '911-7C96-001', 'MSI', 'A520M-A PRO', 36, 53.00, 0.00, 10, 'https://ardes.bg/uploads/original/a520-am4-2xddr4-1xpci-ex16-4xsata-1xm2-dvi-hdmi-m-299596.jpg', 'M.2 Slots', '1'),
('Motherboard', '911-7C96-001', 'MSI', 'A520M-A PRO', 36, 53.00, 0.00, 10, 'https://ardes.bg/uploads/original/a520-am4-2xddr4-1xpci-ex16-4xsata-1xm2-dvi-hdmi-m-299596.jpg', 'SATA Ports', '4'),
('Motherboard', 'A520MHP', 'BIOSTAR', 'A520MHP', 24, 54.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-a520mhp-socket-am4-606403.jpg', 'Socket Type', 'AM4'),
('Motherboard', 'A520MHP', 'BIOSTAR', 'A520MHP', 24, 54.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-a520mhp-socket-am4-606403.jpg', 'RAM Type', 'DDR4'),
('Motherboard', 'A520MHP', 'BIOSTAR', 'A520MHP', 24, 54.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-a520mhp-socket-am4-606403.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', 'A520MHP', 'BIOSTAR', 'A520MHP', 24, 54.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-a520mhp-socket-am4-606403.jpg', 'Max RAM Speed (MHz)', '4933'),
('Motherboard', 'A520MHP', 'BIOSTAR', 'A520MHP', 24, 54.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-a520mhp-socket-am4-606403.jpg', 'M.2 Slots', '0'),
('Motherboard', 'A520MHP', 'BIOSTAR', 'A520MHP', 24, 54.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-a520mhp-socket-am4-606403.jpg', 'SATA Ports', '4'),
('Motherboard', '90-MXBE60-A0UAYZ', 'ASRock', 'A520M-HVS', 24, 56.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hvs-313900.jpg', 'Socket Type', 'AM4'),
('Motherboard', '90-MXBE60-A0UAYZ', 'ASRock', 'A520M-HVS', 24, 56.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hvs-313900.jpg', 'RAM Type', 'DDR4'),
('Motherboard', '90-MXBE60-A0UAYZ', 'ASRock', 'A520M-HVS', 24, 56.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hvs-313900.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', '90-MXBE60-A0UAYZ', 'ASRock', 'A520M-HVS', 24, 56.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hvs-313900.jpg', 'Max RAM Speed (MHz)', '4733'),
('Motherboard', '90-MXBE60-A0UAYZ', 'ASRock', 'A520M-HVS', 24, 56.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hvs-313900.jpg', 'M.2 Slots', '0'),
('Motherboard', '90-MXBE60-A0UAYZ', 'ASRock', 'A520M-HVS', 24, 56.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hvs-313900.jpg', 'SATA Ports', '4'),
('Motherboard', '90-MXBE50-A0UAYZ', 'ASRock', 'A520M-HDV', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hdv-293153.jpg', 'Socket Type', 'AM4'),
('Motherboard', '90-MXBE50-A0UAYZ', 'ASRock', 'A520M-HDV', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hdv-293153.jpg', 'RAM Type', 'DDR4'),
('Motherboard', '90-MXBE50-A0UAYZ', 'ASRock', 'A520M-HDV', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hdv-293153.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', '90-MXBE50-A0UAYZ', 'ASRock', 'A520M-HDV', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hdv-293153.jpg', 'Max RAM Speed (MHz)', '4733'),
('Motherboard', '90-MXBE50-A0UAYZ', 'ASRock', 'A520M-HDV', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hdv-293153.jpg', 'M.2 Slots', '1'),
('Motherboard', '90-MXBE50-A0UAYZ', 'ASRock', 'A520M-HDV', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-a520m-hdv-293153.jpg', 'SATA Ports', '4'),
('Motherboard', 'GB-A520M-K-V2', 'GIGABYTE', 'A520M K V2', 36, 58.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-a520m-k-v2-socket-am4-matx-2-460092.jpg', 'Socket Type', 'AM4'),
('Motherboard', 'GB-A520M-K-V2', 'GIGABYTE', 'A520M K V2', 36, 58.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-a520m-k-v2-socket-am4-matx-2-460092.jpg', 'RAM Type', 'DDR4'),
('Motherboard', 'GB-A520M-K-V2', 'GIGABYTE', 'A520M K V2', 36, 58.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-a520m-k-v2-socket-am4-matx-2-460092.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', 'GB-A520M-K-V2', 'GIGABYTE', 'A520M K V2', 36, 58.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-a520m-k-v2-socket-am4-matx-2-460092.jpg', 'Max RAM Speed (MHz)', '5100'),
('Motherboard', 'GB-A520M-K-V2', 'GIGABYTE', 'A520M K V2', 36, 58.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-a520m-k-v2-socket-am4-matx-2-460092.jpg', 'M.2 Slots', '1'),
('Motherboard', 'GB-A520M-K-V2', 'GIGABYTE', 'A520M K V2', 36, 58.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-a520m-k-v2-socket-am4-matx-2-460092.jpg', 'SATA Ports', '4'),
('Motherboard', '911-7D48-007', 'MSI', 'PRO H610M-E DDR4', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-pro-h610m-e-ddr4-441107.jpg', 'Socket Type', 'LGA1700'),
('Motherboard', '911-7D48-007', 'MSI', 'PRO H610M-E DDR4', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-pro-h610m-e-ddr4-441107.jpg', 'RAM Type', 'DDR4'),
('Motherboard', '911-7D48-007', 'MSI', 'PRO H610M-E DDR4', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-pro-h610m-e-ddr4-441107.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', '911-7D48-007', 'MSI', 'PRO H610M-E DDR4', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-pro-h610m-e-ddr4-441107.jpg', 'Max RAM Speed (MHz)', '3200'),
('Motherboard', '911-7D48-007', 'MSI', 'PRO H610M-E DDR4', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-pro-h610m-e-ddr4-441107.jpg', 'M.2 Slots', '1'),
('Motherboard', '911-7D48-007', 'MSI', 'PRO H610M-E DDR4', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-pro-h610m-e-ddr4-441107.jpg', 'SATA Ports', '4'),
('Motherboard', '90-MXBJJ0-A0UAYZ', 'ASRock', 'H610M-HVS/M.2 R2.0', 36, 61.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-hvs-m-2-r2-0-425643.jpg', 'Socket Type', 'LGA1700'),
('Motherboard', '90-MXBJJ0-A0UAYZ', 'ASRock', 'H610M-HVS/M.2 R2.0', 36, 61.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-hvs-m-2-r2-0-425643.jpg', 'RAM Type', 'DDR4'),
('Motherboard', '90-MXBJJ0-A0UAYZ', 'ASRock', 'H610M-HVS/M.2 R2.0', 36, 61.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-hvs-m-2-r2-0-425643.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', '90-MXBJJ0-A0UAYZ', 'ASRock', 'H610M-HVS/M.2 R2.0', 36, 61.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-hvs-m-2-r2-0-425643.jpg', 'Max RAM Speed (MHz)', '3200'),
('Motherboard', '90-MXBJJ0-A0UAYZ', 'ASRock', 'H610M-HVS/M.2 R2.0', 36, 61.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-hvs-m-2-r2-0-425643.jpg', 'M.2 Slots', '1'),
('Motherboard', '90-MXBJJ0-A0UAYZ', 'ASRock', 'H610M-HVS/M.2 R2.0', 36, 61.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-hvs-m-2-r2-0-425643.jpg', 'SATA Ports', '4'),
('Motherboard', 'H610MHC-2.0', 'BIOSTAR', 'H610MHC 2.0', 24, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-h610mhc-2-0-728149.jpg', 'Socket Type', 'LGA1700'),
('Motherboard', 'H610MHC-2.0', 'BIOSTAR', 'H610MHC 2.0', 24, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-h610mhc-2-0-728149.jpg', 'RAM Type', 'DDR4'),
('Motherboard', 'H610MHC-2.0', 'BIOSTAR', 'H610MHC 2.0', 24, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-h610mhc-2-0-728149.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', 'H610MHC-2.0', 'BIOSTAR', 'H610MHC 2.0', 24, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-h610mhc-2-0-728149.jpg', 'Max RAM Speed (MHz)', '3200'),
('Motherboard', 'H610MHC-2.0', 'BIOSTAR', 'H610MHC 2.0', 24, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-h610mhc-2-0-728149.jpg', 'M.2 Slots', '0'),
('Motherboard', 'H610MHC-2.0', 'BIOSTAR', 'H610MHC 2.0', 24, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-biostar-h610mhc-2-0-728149.jpg', 'SATA Ports', '4'),
('Motherboard', 'H610M-H2/M.2', 'ASRock', 'H610M-H2/M.2', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-h2-m-2-592023.jpg', 'Socket Type', 'LGA1700'),
('Motherboard', 'H610M-H2/M.2', 'ASRock', 'H610M-H2/M.2', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-h2-m-2-592023.jpg', 'RAM Type', 'DDR4'),
('Motherboard', 'H610M-H2/M.2', 'ASRock', 'H610M-H2/M.2', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-h2-m-2-592023.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', 'H610M-H2/M.2', 'ASRock', 'H610M-H2/M.2', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-h2-m-2-592023.jpg', 'Max RAM Speed (MHz)', '3200'),
('Motherboard', 'H610M-H2/M.2', 'ASRock', 'H610M-H2/M.2', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-h2-m-2-592023.jpg', 'M.2 Slots', '1'),
('Motherboard', 'H610M-H2/M.2', 'ASRock', 'H610M-H2/M.2', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h610m-h2-m-2-592023.jpg', 'SATA Ports', '4'),
('Motherboard', '90-MXBMQ0-A0UAYZ', 'ASRock', 'H510M-H2/M.2 SE', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h510m-h2-m-2-se-1200-529021.jpg', 'Socket Type', 'LGA1200'),
('Motherboard', '90-MXBMQ0-A0UAYZ', 'ASRock', 'H510M-H2/M.2 SE', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h510m-h2-m-2-se-1200-529021.jpg', 'RAM Type', 'DDR4'),
('Motherboard', '90-MXBMQ0-A0UAYZ', 'ASRock', 'H510M-H2/M.2 SE', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h510m-h2-m-2-se-1200-529021.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', '90-MXBMQ0-A0UAYZ', 'ASRock', 'H510M-H2/M.2 SE', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h510m-h2-m-2-se-1200-529021.jpg', 'Max RAM Speed (MHz)', '3200'),
('Motherboard', '90-MXBMQ0-A0UAYZ', 'ASRock', 'H510M-H2/M.2 SE', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h510m-h2-m-2-se-1200-529021.jpg', 'M.2 Slots', '1'),
('Motherboard', '90-MXBMQ0-A0UAYZ', 'ASRock', 'H510M-H2/M.2 SE', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-h510m-h2-m-2-se-1200-529021.jpg', 'SATA Ports', '4'),
('Motherboard', 'GB-H610M-K', 'GIGABYTE', 'H610M-K', 36, 63.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-h610m-k-socket-1700-2xddr4-m-435900.jpg', 'Socket Type', 'LGA1700'),
('Motherboard', 'GB-H610M-K', 'GIGABYTE', 'H610M-K', 36, 63.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-h610m-k-socket-1700-2xddr4-m-435900.jpg', 'RAM Type', 'DDR4'),
('Motherboard', 'GB-H610M-K', 'GIGABYTE', 'H610M-K', 36, 63.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-h610m-k-socket-1700-2xddr4-m-435900.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', 'GB-H610M-K', 'GIGABYTE', 'H610M-K', 36, 63.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-h610m-k-socket-1700-2xddr4-m-435900.jpg', 'Max RAM Speed (MHz)', '3200'),
('Motherboard', 'GB-H610M-K', 'GIGABYTE', 'H610M-K', 36, 63.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-h610m-k-socket-1700-2xddr4-m-435900.jpg', 'M.2 Slots', '1'),
('Motherboard', 'GB-H610M-K', 'GIGABYTE', 'H610M-K', 36, 63.00, 0.00, 10, 'https://ardes.bg/uploads/original/danna-platka-gigabyte-h610m-k-socket-1700-2xddr4-m-435900.jpg', 'SATA Ports', '4'),
('Motherboard', 'GB-A520M-S2H/AM4', 'GIGABYTE', 'A520M S2H', 36, 64.00, 0.00, 10, 'https://ardes.bg/uploads/original/gb-a520m-s2h-am4-290889.jpg', 'Socket Type', 'AM4'),
('Motherboard', 'GB-A520M-S2H/AM4', 'GIGABYTE', 'A520M S2H', 36, 64.00, 0.00, 10, 'https://ardes.bg/uploads/original/gb-a520m-s2h-am4-290889.jpg', 'RAM Type', 'DDR4'),
('Motherboard', 'GB-A520M-S2H/AM4', 'GIGABYTE', 'A520M S2H', 36, 64.00, 0.00, 10, 'https://ardes.bg/uploads/original/gb-a520m-s2h-am4-290889.jpg', 'Motherboard Form Factor', 'mATX'),
('Motherboard', 'GB-A520M-S2H/AM4', 'GIGABYTE', 'A520M S2H', 36, 64.00, 0.00, 10, 'https://ardes.bg/uploads/original/gb-a520m-s2h-am4-290889.jpg', 'Max RAM Speed (MHz)', '5100'),
('Motherboard', 'GB-A520M-S2H/AM4', 'GIGABYTE', 'A520M S2H', 36, 64.00, 0.00, 10, 'https://ardes.bg/uploads/original/gb-a520m-s2h-am4-290889.jpg', 'M.2 Slots', '1'),
('Motherboard', 'GB-A520M-S2H/AM4', 'GIGABYTE', 'A520M S2H', 36, 64.00, 0.00, 10, 'https://ardes.bg/uploads/original/gb-a520m-s2h-am4-290889.jpg', 'SATA Ports', '4');

INSERT INTO #SeedProducts
    (CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image, FieldName, FieldValue)
SELECT
    p.CategoryName, p.ProductKey, p.Manufacturer, p.Model, p.Warranty, p.Price, p.Discount, p.Quantity, p.Image,
    s.FieldName, s.FieldValue
FROM
(
    SELECT DISTINCT CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image
    FROM #SeedProducts
    WHERE CategoryName = 'Motherboard'
) p
INNER JOIN
(
    VALUES
        ('911-7C96-001', 'Chipset', 'AMD A520'),
        ('911-7C96-001', 'Memory Slots', '2'),
        ('911-7C96-001', 'Max Supported Memory', 'Up to 64 GB'),
        ('911-7C96-001', 'LAN', '10/100/1000 Mb/s'),
        ('911-7C96-001', 'Bluetooth', 'No'),
        ('911-7C96-001', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('911-7C96-001', 'Ports', 'HDMI, DVI-D, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('911-7C96-001', 'Size', '226 x 173 mm'),
        ('A520MHP', 'Chipset', 'AMD A520'),
        ('A520MHP', 'Memory Slots', '2'),
        ('A520MHP', 'Max Supported Memory', 'Up to 64 GB'),
        ('A520MHP', 'LAN', '10/100/1000 Mb/s'),
        ('A520MHP', 'Bluetooth', 'No'),
        ('A520MHP', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 4 x SATA 6 Gb/s'),
        ('A520MHP', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('A520MHP', 'Size', '226 x 174 mm'),
        ('90-MXBE60-A0UAYZ', 'Chipset', 'AMD A520'),
        ('90-MXBE60-A0UAYZ', 'Memory Slots', '2'),
        ('90-MXBE60-A0UAYZ', 'Max Supported Memory', 'Up to 64 GB'),
        ('90-MXBE60-A0UAYZ', 'LAN', '10/100/1000 Mb/s'),
        ('90-MXBE60-A0UAYZ', 'Bluetooth', 'No'),
        ('90-MXBE60-A0UAYZ', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 4 x SATA 6 Gb/s'),
        ('90-MXBE60-A0UAYZ', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('90-MXBE60-A0UAYZ', 'Size', '226 x 184 mm'),
        ('90-MXBE50-A0UAYZ', 'Chipset', 'AMD A520'),
        ('90-MXBE50-A0UAYZ', 'Memory Slots', '2'),
        ('90-MXBE50-A0UAYZ', 'Max Supported Memory', 'Up to 64 GB'),
        ('90-MXBE50-A0UAYZ', 'LAN', '10/100/1000 Mb/s'),
        ('90-MXBE50-A0UAYZ', 'Bluetooth', 'No'),
        ('90-MXBE50-A0UAYZ', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('90-MXBE50-A0UAYZ', 'Ports', 'HDMI, DVI-D, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('90-MXBE50-A0UAYZ', 'Size', '226 x 184 mm'),
        ('GB-A520M-K-V2', 'Chipset', 'AMD A520'),
        ('GB-A520M-K-V2', 'Memory Slots', '2'),
        ('GB-A520M-K-V2', 'Max Supported Memory', 'Up to 64 GB'),
        ('GB-A520M-K-V2', 'LAN', '10/100/1000 Mb/s'),
        ('GB-A520M-K-V2', 'Bluetooth', 'No'),
        ('GB-A520M-K-V2', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('GB-A520M-K-V2', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('GB-A520M-K-V2', 'Size', '226 x 174 mm'),
        ('911-7D48-007', 'Chipset', 'Intel H610'),
        ('911-7D48-007', 'Memory Slots', '2'),
        ('911-7D48-007', 'Max Supported Memory', 'Up to 64 GB'),
        ('911-7D48-007', 'LAN', '10/100/1000 Mb/s'),
        ('911-7D48-007', 'Bluetooth', 'No'),
        ('911-7D48-007', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('911-7D48-007', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('911-7D48-007', 'Size', '227 x 184 mm'),
        ('90-MXBJJ0-A0UAYZ', 'Chipset', 'Intel H610'),
        ('90-MXBJJ0-A0UAYZ', 'Memory Slots', '2'),
        ('90-MXBJJ0-A0UAYZ', 'Max Supported Memory', 'Up to 64 GB'),
        ('90-MXBJJ0-A0UAYZ', 'LAN', '10/100/1000 Mb/s'),
        ('90-MXBJJ0-A0UAYZ', 'Bluetooth', 'No'),
        ('90-MXBJJ0-A0UAYZ', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('90-MXBJJ0-A0UAYZ', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('90-MXBJJ0-A0UAYZ', 'Size', '226 x 174 mm'),
        ('H610MHC-2.0', 'Chipset', 'Intel H610'),
        ('H610MHC-2.0', 'Memory Slots', '2'),
        ('H610MHC-2.0', 'Max Supported Memory', 'Up to 64 GB'),
        ('H610MHC-2.0', 'LAN', '10/100/1000 Mb/s'),
        ('H610MHC-2.0', 'Bluetooth', 'No'),
        ('H610MHC-2.0', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 4 x SATA 6 Gb/s'),
        ('H610MHC-2.0', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('H610MHC-2.0', 'Size', '224 x 174 mm'),
        ('H610M-H2/M.2', 'Chipset', 'Intel H610'),
        ('H610M-H2/M.2', 'Memory Slots', '2'),
        ('H610M-H2/M.2', 'Max Supported Memory', 'Up to 64 GB'),
        ('H610M-H2/M.2', 'LAN', '10/100/1000 Mb/s'),
        ('H610M-H2/M.2', 'Bluetooth', 'No'),
        ('H610M-H2/M.2', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('H610M-H2/M.2', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('H610M-H2/M.2', 'Size', '226 x 184 mm'),
        ('90-MXBMQ0-A0UAYZ', 'Chipset', 'Intel H510'),
        ('90-MXBMQ0-A0UAYZ', 'Memory Slots', '2'),
        ('90-MXBMQ0-A0UAYZ', 'Max Supported Memory', 'Up to 64 GB'),
        ('90-MXBMQ0-A0UAYZ', 'LAN', '10/100/1000 Mb/s'),
        ('90-MXBMQ0-A0UAYZ', 'Bluetooth', 'No'),
        ('90-MXBMQ0-A0UAYZ', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('90-MXBMQ0-A0UAYZ', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('90-MXBMQ0-A0UAYZ', 'Size', '226 x 184 mm'),
        ('GB-H610M-K', 'Chipset', 'Intel H610'),
        ('GB-H610M-K', 'Memory Slots', '2'),
        ('GB-H610M-K', 'Max Supported Memory', 'Up to 64 GB'),
        ('GB-H610M-K', 'LAN', '10/100/1000 Mb/s'),
        ('GB-H610M-K', 'Bluetooth', 'No'),
        ('GB-H610M-K', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('GB-H610M-K', 'Ports', 'HDMI, DisplayPort, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('GB-H610M-K', 'Size', '226 x 175 mm'),
        ('GB-A520M-S2H/AM4', 'Chipset', 'AMD A520'),
        ('GB-A520M-S2H/AM4', 'Memory Slots', '2'),
        ('GB-A520M-S2H/AM4', 'Max Supported Memory', 'Up to 64 GB'),
        ('GB-A520M-S2H/AM4', 'LAN', '10/100/1000 Mb/s'),
        ('GB-A520M-S2H/AM4', 'Bluetooth', 'No'),
        ('GB-A520M-S2H/AM4', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('GB-A520M-S2H/AM4', 'Ports', 'HDMI, DVI-D, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('GB-A520M-S2H/AM4', 'Size', '226 x 174 mm')
) AS s(ProductKey, FieldName, FieldValue)
    ON s.ProductKey = p.ProductKey;

-- =====================================================================
-- RAM
-- =====================================================================
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

-- =====================================================================
-- GPU
-- =====================================================================
MERGE dbo.Categories AS target
USING (VALUES ('GPU')) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name);

MERGE dbo.Fields AS target
USING
(
    VALUES
        ('Length (mm)', 9),
        ('Recommended PSU (W)', 9),
        ('TDP (W)', 9),
        ('Graphics Processor', 18),
        ('Chip Manufacturer', 18),
        ('Memory Capacity', 18),
        ('Memory Type', 18),
        ('Memory Clock', 18),
        ('Bus Width', 18),
        ('Slot', 18),
        ('Power Connector', 18),
        ('Interfaces', 18),
        ('Supported Resolution', 18),
        ('DirectX Support', 18),
        ('Core Clock', 18),
        ('Stream Processors', 9),
        ('Architecture', 18),
        ('Manufacturing Process', 18),
        ('Transistor Count', 18),
        ('Cooling', 18),
        ('Extras', 18),
        ('Weight', 18),
        ('Size', 18)
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
    ON f.Name IN ('Length (mm)', 'Recommended PSU (W)', 'TDP (W)',
                  'Graphics Processor', 'Chip Manufacturer', 'Memory Capacity',
                  'Memory Type', 'Memory Clock', 'Bus Width', 'Slot',
                  'Power Connector', 'Interfaces', 'Supported Resolution',
                  'DirectX Support', 'Core Clock', 'Stream Processors',
                  'Architecture', 'Manufacturing Process', 'Transistor Count',
                  'Cooling', 'Extras', 'Weight', 'Size')
WHERE c.Name = 'GPU'
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
('GPU', '4711377379960', 'MSI', 'GeForce RTX 5050 8GB SHADOW 2X OC DLSS 4', 36, 309.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-geforce-rtx-5050-8gb-shadow-2x-oc-dlss-4-683013.jpg', 'Length (mm)', '227'),
('GPU', '4711377379960', 'MSI', 'GeForce RTX 5050 8GB SHADOW 2X OC DLSS 4', 36, 309.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-geforce-rtx-5050-8gb-shadow-2x-oc-dlss-4-683013.jpg', 'Recommended PSU (W)', '550'),
('GPU', 'NE7506T019P1-GB2062D', 'Palit', 'GeForce RTX 5060 Ti 8GB Dual DLSS 4', 36, 419.00, 0.00, 10, 'https://ardes.bg/uploads/original/palit-geforce-rtx-5060-ti-dual-8gb-gddr7-128-bit-1-635790.jpg', 'Length (mm)', '250'),
('GPU', 'NE7506T019P1-GB2062D', 'Palit', 'GeForce RTX 5060 Ti 8GB Dual DLSS 4', 36, 419.00, 0.00, 10, 'https://ardes.bg/uploads/original/palit-geforce-rtx-5060-ti-dual-8gb-gddr7-128-bit-1-635790.jpg', 'Recommended PSU (W)', '650'),
('GPU', '912-V537-004', 'MSI', 'GeForce RTX 5060 8GB SHADOW 2X OC DLSS 4', 36, 385.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-video-card-nvidia-geforce-rtx-5060-8g-shadow-2-637704.jpg', 'Length (mm)', '227'),
('GPU', '912-V537-004', 'MSI', 'GeForce RTX 5060 8GB SHADOW 2X OC DLSS 4', 36, 385.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-video-card-nvidia-geforce-rtx-5060-8g-shadow-2-637704.jpg', 'Recommended PSU (W)', '550'),
('GPU', '912-V532-005', 'MSI', 'GeForce RTX 5070 12GB SHADOW 2X OC DLSS 4', 36, 689.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-geforce-rtx-5070-12gb-shadow-2x-oc-dlss-4-625870.jpg', 'Length (mm)', '242'),
('GPU', '912-V532-005', 'MSI', 'GeForce RTX 5070 12GB SHADOW 2X OC DLSS 4', 36, 689.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-geforce-rtx-5070-12gb-shadow-2x-oc-dlss-4-625870.jpg', 'Recommended PSU (W)', '700'),
('GPU', 'N50602-08D7-195071N', 'Inno3D', 'GeForce RTX 5060 8GB TWIN X2 DLSS 4', 36, 375.00, 0.00, 10, 'https://ardes.bg/uploads/original/inno3d-geforce-rtx-5060-8gb-twin-x2-dlss-4-640022.jpg', 'Length (mm)', '250'),
('GPU', 'N50602-08D7-195071N', 'Inno3D', 'GeForce RTX 5060 8GB TWIN X2 DLSS 4', 36, 375.00, 0.00, 10, 'https://ardes.bg/uploads/original/inno3d-geforce-rtx-5060-8gb-twin-x2-dlss-4-640022.jpg', 'Recommended PSU (W)', '550'),
('GPU', 'GV-R9070XTGAMING-16GD', 'GIGABYTE', 'RADEON RX 9070 XT 16GB GAMING 16GB GDDR6', 36, 729.00, 0.00, 10, 'https://ardes.bg/uploads/original/gigabyte-radeon-rx-9070-xt-16gb-gaming-16gb-gddr6-729464.jpg', 'Length (mm)', '290'),
('GPU', 'GV-R9070XTGAMING-16GD', 'GIGABYTE', 'RADEON RX 9070 XT 16GB GAMING 16GB GDDR6', 36, 729.00, 0.00, 10, 'https://ardes.bg/uploads/original/gigabyte-radeon-rx-9070-xt-16gb-gaming-16gb-gddr6-729464.jpg', 'Recommended PSU (W)', '850'),
('GPU', '90-GA5DZZ-00UANF', 'ASRock', 'Radeon RX 9070 XT 16GB Steel Legend', 36, 749.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-radeon-rx-9070-xt-16gb-steel-legend-620880.jpg', 'Length (mm)', '298'),
('GPU', '90-GA5DZZ-00UANF', 'ASRock', 'Radeon RX 9070 XT 16GB Steel Legend', 36, 749.00, 0.00, 10, 'https://ardes.bg/uploads/original/asrock-radeon-rx-9070-xt-16gb-steel-legend-620880.jpg', 'Recommended PSU (W)', '850'),
('GPU', '90YV0LG2-M0NA00', 'ASUS', 'Radeon RX 9060 XT 16GB Dual', 36, 499.00, 0.00, 10, 'https://ardes.bg/uploads/original/asus-radeon-rx-9060-xt-16gb-dual-678663.jpg', 'Length (mm)', '230'),
('GPU', '90YV0LG2-M0NA00', 'ASUS', 'Radeon RX 9060 XT 16GB Dual', 36, 499.00, 0.00, 10, 'https://ardes.bg/uploads/original/asus-radeon-rx-9060-xt-16gb-dual-678663.jpg', 'Recommended PSU (W)', '650'),
('GPU', 'N507T3-16D7-176068N', 'Inno3D', 'GeForce RTX 5070 Ti 16GB X3 DLSS 4', 36, 979.00, 0.00, 10, 'https://ardes.bg/uploads/original/inno3d-video-karta-n507t3-16d7-176068n-620492.jpg', 'Length (mm)', '300'),
('GPU', 'N507T3-16D7-176068N', 'Inno3D', 'GeForce RTX 5070 Ti 16GB X3 DLSS 4', 36, 979.00, 0.00, 10, 'https://ardes.bg/uploads/original/inno3d-video-karta-n507t3-16d7-176068n-620492.jpg', 'Recommended PSU (W)', '850'),
('GPU', 'NE75070019K9-GB2050S', 'Palit', 'GeForce RTX 5070 12GB Infinity 3 DLSS 4', 36, 649.00, 0.00, 10, 'https://ardes.bg/uploads/original/palit-geforce-rtx-5070-12gb-infinity-3-dlss-4-628789.jpg', 'Length (mm)', '292'),
('GPU', 'NE75070019K9-GB2050S', 'Palit', 'GeForce RTX 5070 12GB Infinity 3 DLSS 4', 36, 649.00, 0.00, 10, 'https://ardes.bg/uploads/original/palit-geforce-rtx-5070-12gb-infinity-3-dlss-4-628789.jpg', 'Recommended PSU (W)', '700'),
('GPU', 'N50702-12D7X-195064N', 'Inno3D', 'GeForce RTX 5070 12GB Twin X2 OC DLSS 4', 36, 719.00, 0.00, 10, 'https://ardes.bg/uploads/original/inno3d-geforce-rtx-5070-12gb-twin-x2-oc-dlss-4-625387.jpg', 'Length (mm)', '250'),
('GPU', 'N50702-12D7X-195064N', 'Inno3D', 'GeForce RTX 5070 12GB Twin X2 OC DLSS 4', 36, 719.00, 0.00, 10, 'https://ardes.bg/uploads/original/inno3d-geforce-rtx-5070-12gb-twin-x2-oc-dlss-4-625387.jpg', 'Recommended PSU (W)', '700'),
('GPU', 'N50602-08D7X-195070N', 'Inno3D', 'GeForce RTX 5060 8GB TWIN X2 OC DLSS 4', 36, 379.00, 0.00, 10, 'https://ardes.bg/uploads/original/inno3d-geforce-rtx-5060-twin-x2-oc-dlss-4-651685.jpg', 'Length (mm)', '250'),
('GPU', 'N50602-08D7X-195070N', 'Inno3D', 'GeForce RTX 5060 8GB TWIN X2 OC DLSS 4', 36, 379.00, 0.00, 10, 'https://ardes.bg/uploads/original/inno3d-geforce-rtx-5060-twin-x2-oc-dlss-4-651685.jpg', 'Recommended PSU (W)', '550');

INSERT INTO #SeedProducts
    (CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image, FieldName, FieldValue)
SELECT
    p.CategoryName, p.ProductKey, p.Manufacturer, p.Model, p.Warranty, p.Price, p.Discount, p.Quantity, p.Image,
    s.FieldName, s.FieldValue
FROM
(
    SELECT DISTINCT CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image
    FROM #SeedProducts
    WHERE CategoryName = 'GPU'
) p
INNER JOIN
(
    VALUES
        ('4711377379960', 'Graphics Processor',   'GeForce RTX 5050'),
        ('4711377379960', 'Chip Manufacturer',    'Nvidia'),
        ('4711377379960', 'Memory Capacity',      '8 GB'),
        ('4711377379960', 'Memory Type',          'GDDR6'),
        ('4711377379960', 'Memory Clock',         '2500 MHz'),
        ('4711377379960', 'Bus Width',            '128 bit'),
        ('4711377379960', 'Slot',                 'PCI Express 5.0'),
        ('4711377379960', 'Power Connector',      '1 x 8-pin'),
        ('4711377379960', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('4711377379960', 'Supported Resolution', '7680 x 4320'),
        ('4711377379960', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('4711377379960', 'Core Clock',           '2572 MHz'),
        ('4711377379960', 'Stream Processors',    '2560'),
        ('4711377379960', 'Architecture',         'Blackwell'),
        ('4711377379960', 'Manufacturing Process','4 nm'),
        ('4711377379960', 'Transistor Count',     '18 800 million'),
        ('4711377379960', 'Cooling',              'Active (dual fan)'),
        ('4711377379960', 'TDP (W)',              '130'),
        ('4711377379960', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing, G-Sync'),
        ('4711377379960', 'Weight',               '0.7 kg'),
        ('4711377379960', 'Size',                 '227 x 122 x 41 mm'),
        ('NE7506T019P1-GB2062D', 'Graphics Processor',   'GeForce RTX 5060 Ti'),
        ('NE7506T019P1-GB2062D', 'Chip Manufacturer',    'Nvidia'),
        ('NE7506T019P1-GB2062D', 'Memory Capacity',      '8 GB'),
        ('NE7506T019P1-GB2062D', 'Memory Type',          'GDDR7'),
        ('NE7506T019P1-GB2062D', 'Memory Clock',         '3500 MHz'),
        ('NE7506T019P1-GB2062D', 'Bus Width',            '128 bit'),
        ('NE7506T019P1-GB2062D', 'Slot',                 'PCI Express 5.0'),
        ('NE7506T019P1-GB2062D', 'Power Connector',      '1 x 8-pin'),
        ('NE7506T019P1-GB2062D', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('NE7506T019P1-GB2062D', 'Supported Resolution', '7680 x 4320'),
        ('NE7506T019P1-GB2062D', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('NE7506T019P1-GB2062D', 'Core Clock',           '2407 MHz'),
        ('NE7506T019P1-GB2062D', 'Stream Processors',    '4608'),
        ('NE7506T019P1-GB2062D', 'Architecture',         'Blackwell'),
        ('NE7506T019P1-GB2062D', 'Manufacturing Process','4 nm'),
        ('NE7506T019P1-GB2062D', 'Transistor Count',     '21 900 million'),
        ('NE7506T019P1-GB2062D', 'Cooling',              'Active (dual fan)'),
        ('NE7506T019P1-GB2062D', 'TDP (W)',              '180'),
        ('NE7506T019P1-GB2062D', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing'),
        ('NE7506T019P1-GB2062D', 'Weight',               '0.9 kg'),
        ('NE7506T019P1-GB2062D', 'Size',                 '250 x 121 x 41 mm'),
        ('912-V537-004', 'Graphics Processor',   'GeForce RTX 5060'),
        ('912-V537-004', 'Chip Manufacturer',    'Nvidia'),
        ('912-V537-004', 'Memory Capacity',      '8 GB'),
        ('912-V537-004', 'Memory Type',          'GDDR7'),
        ('912-V537-004', 'Memory Clock',         '3500 MHz'),
        ('912-V537-004', 'Bus Width',            '128 bit'),
        ('912-V537-004', 'Slot',                 'PCI Express 5.0'),
        ('912-V537-004', 'Power Connector',      '1 x 8-pin'),
        ('912-V537-004', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('912-V537-004', 'Supported Resolution', '7680 x 4320'),
        ('912-V537-004', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('912-V537-004', 'Core Clock',           '2497 MHz'),
        ('912-V537-004', 'Stream Processors',    '3840'),
        ('912-V537-004', 'Architecture',         'Blackwell'),
        ('912-V537-004', 'Manufacturing Process','4 nm'),
        ('912-V537-004', 'Transistor Count',     '21 900 million'),
        ('912-V537-004', 'Cooling',              'Active (dual fan)'),
        ('912-V537-004', 'TDP (W)',              '145'),
        ('912-V537-004', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing'),
        ('912-V537-004', 'Weight',               '0.8 kg'),
        ('912-V537-004', 'Size',                 '227 x 122 x 41 mm'),
        ('912-V532-005', 'Graphics Processor',   'GeForce RTX 5070'),
        ('912-V532-005', 'Chip Manufacturer',    'Nvidia'),
        ('912-V532-005', 'Memory Capacity',      '12 GB'),
        ('912-V532-005', 'Memory Type',          'GDDR7'),
        ('912-V532-005', 'Memory Clock',         '3500 MHz'),
        ('912-V532-005', 'Bus Width',            '192 bit'),
        ('912-V532-005', 'Slot',                 'PCI Express 5.0'),
        ('912-V532-005', 'Power Connector',      '1 x 12V-2x6'),
        ('912-V532-005', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('912-V532-005', 'Supported Resolution', '7680 x 4320'),
        ('912-V532-005', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('912-V532-005', 'Core Clock',           '2512 MHz'),
        ('912-V532-005', 'Stream Processors',    '6144'),
        ('912-V532-005', 'Architecture',         'Blackwell'),
        ('912-V532-005', 'Manufacturing Process','4 nm'),
        ('912-V532-005', 'Transistor Count',     '31 100 million'),
        ('912-V532-005', 'Cooling',              'Active (dual fan)'),
        ('912-V532-005', 'TDP (W)',              '250'),
        ('912-V532-005', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing, NVIDIA Reflex'),
        ('912-V532-005', 'Weight',               '0.9 kg'),
        ('912-V532-005', 'Size',                 '242 x 122 x 41 mm'),
        ('N50602-08D7-195071N', 'Graphics Processor',   'GeForce RTX 5060'),
        ('N50602-08D7-195071N', 'Chip Manufacturer',    'Nvidia'),
        ('N50602-08D7-195071N', 'Memory Capacity',      '8 GB'),
        ('N50602-08D7-195071N', 'Memory Type',          'GDDR7'),
        ('N50602-08D7-195071N', 'Memory Clock',         '3500 MHz'),
        ('N50602-08D7-195071N', 'Bus Width',            '128 bit'),
        ('N50602-08D7-195071N', 'Slot',                 'PCI Express 5.0'),
        ('N50602-08D7-195071N', 'Power Connector',      '1 x 8-pin'),
        ('N50602-08D7-195071N', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('N50602-08D7-195071N', 'Supported Resolution', '7680 x 4320'),
        ('N50602-08D7-195071N', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('N50602-08D7-195071N', 'Core Clock',           '2497 MHz'),
        ('N50602-08D7-195071N', 'Stream Processors',    '3840'),
        ('N50602-08D7-195071N', 'Architecture',         'Blackwell'),
        ('N50602-08D7-195071N', 'Manufacturing Process','4 nm'),
        ('N50602-08D7-195071N', 'Transistor Count',     '21 900 million'),
        ('N50602-08D7-195071N', 'Cooling',              'Active (dual fan)'),
        ('N50602-08D7-195071N', 'TDP (W)',              '145'),
        ('N50602-08D7-195071N', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing'),
        ('N50602-08D7-195071N', 'Weight',               '0.8 kg'),
        ('N50602-08D7-195071N', 'Size',                 '250 x 121 x 41 mm'),
        ('GV-R9070XTGAMING-16GD', 'Graphics Processor',   'Radeon RX 9070 XT'),
        ('GV-R9070XTGAMING-16GD', 'Chip Manufacturer',    'AMD'),
        ('GV-R9070XTGAMING-16GD', 'Memory Capacity',      '16 GB'),
        ('GV-R9070XTGAMING-16GD', 'Memory Type',          'GDDR6'),
        ('GV-R9070XTGAMING-16GD', 'Memory Clock',         '2518 MHz'),
        ('GV-R9070XTGAMING-16GD', 'Bus Width',            '256 bit'),
        ('GV-R9070XTGAMING-16GD', 'Slot',                 'PCI Express 5.0'),
        ('GV-R9070XTGAMING-16GD', 'Power Connector',      '2 x 8-pin'),
        ('GV-R9070XTGAMING-16GD', 'Interfaces',           '2 x HDMI 2.1b, 2 x DisplayPort 2.1b'),
        ('GV-R9070XTGAMING-16GD', 'Supported Resolution', '7680 x 4320'),
        ('GV-R9070XTGAMING-16GD', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('GV-R9070XTGAMING-16GD', 'Core Clock',           '2400 MHz'),
        ('GV-R9070XTGAMING-16GD', 'Stream Processors',    '4096'),
        ('GV-R9070XTGAMING-16GD', 'Architecture',         'RDNA 4'),
        ('GV-R9070XTGAMING-16GD', 'Manufacturing Process','4 nm'),
        ('GV-R9070XTGAMING-16GD', 'Transistor Count',     '53 900 million'),
        ('GV-R9070XTGAMING-16GD', 'Cooling',              'Active (triple fan)'),
        ('GV-R9070XTGAMING-16GD', 'TDP (W)',              '304'),
        ('GV-R9070XTGAMING-16GD', 'Extras',               'AMD FSR 4, Ray Accelerators, AMD FreeSync'),
        ('GV-R9070XTGAMING-16GD', 'Weight',               '1.2 kg'),
        ('GV-R9070XTGAMING-16GD', 'Size',                 '290 x 130 x 50 mm'),
        ('90-GA5DZZ-00UANF', 'Graphics Processor',   'Radeon RX 9070 XT'),
        ('90-GA5DZZ-00UANF', 'Chip Manufacturer',    'AMD'),
        ('90-GA5DZZ-00UANF', 'Memory Capacity',      '16 GB'),
        ('90-GA5DZZ-00UANF', 'Memory Type',          'GDDR6'),
        ('90-GA5DZZ-00UANF', 'Memory Clock',         '2518 MHz'),
        ('90-GA5DZZ-00UANF', 'Bus Width',            '256 bit'),
        ('90-GA5DZZ-00UANF', 'Slot',                 'PCI Express 5.0'),
        ('90-GA5DZZ-00UANF', 'Power Connector',      '2 x 8-pin'),
        ('90-GA5DZZ-00UANF', 'Interfaces',           '2 x HDMI 2.1b, 2 x DisplayPort 2.1b'),
        ('90-GA5DZZ-00UANF', 'Supported Resolution', '7680 x 4320'),
        ('90-GA5DZZ-00UANF', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('90-GA5DZZ-00UANF', 'Core Clock',           '2400 MHz'),
        ('90-GA5DZZ-00UANF', 'Stream Processors',    '4096'),
        ('90-GA5DZZ-00UANF', 'Architecture',         'RDNA 4'),
        ('90-GA5DZZ-00UANF', 'Manufacturing Process','4 nm'),
        ('90-GA5DZZ-00UANF', 'Transistor Count',     '53 900 million'),
        ('90-GA5DZZ-00UANF', 'Cooling',              'Active (triple fan)'),
        ('90-GA5DZZ-00UANF', 'TDP (W)',              '304'),
        ('90-GA5DZZ-00UANF', 'Extras',               'AMD FSR 4, Ray Accelerators, AMD FreeSync'),
        ('90-GA5DZZ-00UANF', 'Weight',               '1.3 kg'),
        ('90-GA5DZZ-00UANF', 'Size',                 '298 x 130 x 55 mm'),
        ('90YV0LG2-M0NA00', 'Graphics Processor',   'Radeon RX 9060 XT'),
        ('90YV0LG2-M0NA00', 'Chip Manufacturer',    'AMD'),
        ('90YV0LG2-M0NA00', 'Memory Capacity',      '16 GB'),
        ('90YV0LG2-M0NA00', 'Memory Type',          'GDDR6'),
        ('90YV0LG2-M0NA00', 'Memory Clock',         '2518 MHz'),
        ('90YV0LG2-M0NA00', 'Bus Width',            '128 bit'),
        ('90YV0LG2-M0NA00', 'Slot',                 'PCI Express 5.0'),
        ('90YV0LG2-M0NA00', 'Power Connector',      '1 x 8-pin'),
        ('90YV0LG2-M0NA00', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('90YV0LG2-M0NA00', 'Supported Resolution', '7680 x 4320'),
        ('90YV0LG2-M0NA00', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('90YV0LG2-M0NA00', 'Core Clock',           '2280 MHz'),
        ('90YV0LG2-M0NA00', 'Stream Processors',    '2048'),
        ('90YV0LG2-M0NA00', 'Architecture',         'RDNA 4'),
        ('90YV0LG2-M0NA00', 'Manufacturing Process','4 nm'),
        ('90YV0LG2-M0NA00', 'Transistor Count',     '29 700 million'),
        ('90YV0LG2-M0NA00', 'Cooling',              'Active (dual fan)'),
        ('90YV0LG2-M0NA00', 'TDP (W)',              '160'),
        ('90YV0LG2-M0NA00', 'Extras',               'AMD FSR 4, AMD FreeSync'),
        ('90YV0LG2-M0NA00', 'Weight',               '0.8 kg'),
        ('90YV0LG2-M0NA00', 'Size',                 '230 x 130 x 42 mm'),
        ('N507T3-16D7-176068N', 'Graphics Processor',   'GeForce RTX 5070 Ti'),
        ('N507T3-16D7-176068N', 'Chip Manufacturer',    'Nvidia'),
        ('N507T3-16D7-176068N', 'Memory Capacity',      '16 GB'),
        ('N507T3-16D7-176068N', 'Memory Type',          'GDDR7'),
        ('N507T3-16D7-176068N', 'Memory Clock',         '3500 MHz'),
        ('N507T3-16D7-176068N', 'Bus Width',            '256 bit'),
        ('N507T3-16D7-176068N', 'Slot',                 'PCI Express 5.0'),
        ('N507T3-16D7-176068N', 'Power Connector',      '1 x 16-pin'),
        ('N507T3-16D7-176068N', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('N507T3-16D7-176068N', 'Supported Resolution', '7680 x 4320'),
        ('N507T3-16D7-176068N', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('N507T3-16D7-176068N', 'Core Clock',           '2497 MHz'),
        ('N507T3-16D7-176068N', 'Stream Processors',    '8960'),
        ('N507T3-16D7-176068N', 'Architecture',         'Blackwell'),
        ('N507T3-16D7-176068N', 'Manufacturing Process','4 nm'),
        ('N507T3-16D7-176068N', 'Transistor Count',     '45 600 million'),
        ('N507T3-16D7-176068N', 'Cooling',              'Active (triple fan)'),
        ('N507T3-16D7-176068N', 'TDP (W)',              '300'),
        ('N507T3-16D7-176068N', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing, NVIDIA Reflex'),
        ('N507T3-16D7-176068N', 'Weight',               '1.0 kg'),
        ('N507T3-16D7-176068N', 'Size',                 '300 x 130 x 60 mm'),
        ('NE75070019K9-GB2050S', 'Graphics Processor',   'GeForce RTX 5070'),
        ('NE75070019K9-GB2050S', 'Chip Manufacturer',    'Nvidia'),
        ('NE75070019K9-GB2050S', 'Memory Capacity',      '12 GB'),
        ('NE75070019K9-GB2050S', 'Memory Type',          'GDDR7'),
        ('NE75070019K9-GB2050S', 'Memory Clock',         '3500 MHz'),
        ('NE75070019K9-GB2050S', 'Bus Width',            '192 bit'),
        ('NE75070019K9-GB2050S', 'Slot',                 'PCI Express 5.0'),
        ('NE75070019K9-GB2050S', 'Power Connector',      '1 x 12V-2x6'),
        ('NE75070019K9-GB2050S', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('NE75070019K9-GB2050S', 'Supported Resolution', '7680 x 4320'),
        ('NE75070019K9-GB2050S', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('NE75070019K9-GB2050S', 'Core Clock',           '2512 MHz'),
        ('NE75070019K9-GB2050S', 'Stream Processors',    '6144'),
        ('NE75070019K9-GB2050S', 'Architecture',         'Blackwell'),
        ('NE75070019K9-GB2050S', 'Manufacturing Process','4 nm'),
        ('NE75070019K9-GB2050S', 'Transistor Count',     '31 100 million'),
        ('NE75070019K9-GB2050S', 'Cooling',              'Active (triple fan)'),
        ('NE75070019K9-GB2050S', 'TDP (W)',              '250'),
        ('NE75070019K9-GB2050S', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing, NVIDIA Reflex'),
        ('NE75070019K9-GB2050S', 'Weight',               '1.0 kg'),
        ('NE75070019K9-GB2050S', 'Size',                 '292 x 122 x 50 mm'),
        ('N50702-12D7X-195064N', 'Graphics Processor',   'GeForce RTX 5070'),
        ('N50702-12D7X-195064N', 'Chip Manufacturer',    'Nvidia'),
        ('N50702-12D7X-195064N', 'Memory Capacity',      '12 GB'),
        ('N50702-12D7X-195064N', 'Memory Type',          'GDDR7'),
        ('N50702-12D7X-195064N', 'Memory Clock',         '3500 MHz'),
        ('N50702-12D7X-195064N', 'Bus Width',            '192 bit'),
        ('N50702-12D7X-195064N', 'Slot',                 'PCI Express 5.0'),
        ('N50702-12D7X-195064N', 'Power Connector',      '1 x 12V-2x6'),
        ('N50702-12D7X-195064N', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('N50702-12D7X-195064N', 'Supported Resolution', '7680 x 4320'),
        ('N50702-12D7X-195064N', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('N50702-12D7X-195064N', 'Core Clock',           '2512 MHz'),
        ('N50702-12D7X-195064N', 'Stream Processors',    '6144'),
        ('N50702-12D7X-195064N', 'Architecture',         'Blackwell'),
        ('N50702-12D7X-195064N', 'Manufacturing Process','4 nm'),
        ('N50702-12D7X-195064N', 'Transistor Count',     '31 100 million'),
        ('N50702-12D7X-195064N', 'Cooling',              'Active (dual fan)'),
        ('N50702-12D7X-195064N', 'TDP (W)',              '250'),
        ('N50702-12D7X-195064N', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing, NVIDIA Reflex'),
        ('N50702-12D7X-195064N', 'Weight',               '0.9 kg'),
        ('N50702-12D7X-195064N', 'Size',                 '250 x 122 x 50 mm'),
        ('N50602-08D7X-195070N', 'Graphics Processor',   'GeForce RTX 5060'),
        ('N50602-08D7X-195070N', 'Chip Manufacturer',    'Nvidia'),
        ('N50602-08D7X-195070N', 'Memory Capacity',      '8 GB'),
        ('N50602-08D7X-195070N', 'Memory Type',          'GDDR7'),
        ('N50602-08D7X-195070N', 'Memory Clock',         '3500 MHz'),
        ('N50602-08D7X-195070N', 'Bus Width',            '128 bit'),
        ('N50602-08D7X-195070N', 'Slot',                 'PCI Express 5.0'),
        ('N50602-08D7X-195070N', 'Power Connector',      '1 x 8-pin'),
        ('N50602-08D7X-195070N', 'Interfaces',           '1 x HDMI 2.1b, 3 x DisplayPort 2.1b'),
        ('N50602-08D7X-195070N', 'Supported Resolution', '7680 x 4320'),
        ('N50602-08D7X-195070N', 'DirectX Support',      'DirectX 12 Ultimate'),
        ('N50602-08D7X-195070N', 'Core Clock',           '2520 MHz'),
        ('N50602-08D7X-195070N', 'Stream Processors',    '3840'),
        ('N50602-08D7X-195070N', 'Architecture',         'Blackwell'),
        ('N50602-08D7X-195070N', 'Manufacturing Process','4 nm'),
        ('N50602-08D7X-195070N', 'Transistor Count',     '21 900 million'),
        ('N50602-08D7X-195070N', 'Cooling',              'Active (dual fan)'),
        ('N50602-08D7X-195070N', 'TDP (W)',              '145'),
        ('N50602-08D7X-195070N', 'Extras',               'Nvidia DLSS 4, Real-Time Ray Tracing'),
        ('N50602-08D7X-195070N', 'Weight',               '0.8 kg'),
        ('N50602-08D7X-195070N', 'Size',                 '250 x 121 x 41 mm')
) AS s(ProductKey, FieldName, FieldValue)
    ON s.ProductKey = p.ProductKey;

-- =====================================================================
-- Storage
-- =====================================================================
MERGE dbo.Categories AS target
USING (VALUES ('Storage')) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name);

MERGE dbo.Fields AS target
USING
(
    VALUES
        ('Storage Interface', 18),
        ('Type', 18),
        ('Disk Form Factor', 18),
        ('Capacity', 18),
        ('Read Speed', 18),
        ('Write Speed', 18),
        ('Extras', 18)
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
    ON f.Name IN ('Storage Interface', 'Type', 'Disk Form Factor',
                  'Capacity', 'Read Speed', 'Write Speed', 'Extras')
WHERE c.Name = 'Storage'
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
('Storage', 'SA400S37/240G', 'Kingston', 'A400 240GB SSD', 36, 69.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-ssd-480gb-a400-sata3-2-5-ssd-7mm-height-t-159648.jpg', 'Storage Interface', 'SATA'),
('Storage', 'SA400S37/480G', 'Kingston', 'A400 480GB SSD', 36, 109.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-ssd-480gb-a400-sata3-2-5-ssd-7mm-height-t-159648.jpg', 'Storage Interface', 'SATA'),
('Storage', 'SNV3S/1000G', 'Kingston', 'NV3 1TB SSD', 36, 169.00, 0.00, 10, 'https://ardes.bg/uploads/original/ssd-kingston-nv3-m-2-2280-pcie-4-0-nvme-1000gb-572200.jpg', 'Storage Interface', 'NVMe'),
('Storage', 'MZ-V9P2T0BW', 'Samsung', '990 PRO 2TB SSD', 60, 369.00, 0.00, 10, 'https://ardes.bg/uploads/original/solid-state-drive-ssd-samsung-990-pro-2tb-m-2-type-421359.jpg', 'Storage Interface', 'NVMe'),
('Storage', 'MZ-V9P1T0BW', 'Samsung', '990 PRO 1TB SSD', 60, 229.00, 0.00, 10, 'https://ardes.bg/uploads/original/solid-state-drive-ssd-samsung-990-pro-1tb-m-2-type-421353.jpg', 'Storage Interface', 'NVMe'),
('Storage', 'MZ-77E500B/EU', 'Samsung', '870 EVO 500GB SSD', 60, 202.00, 0.00, 10, 'https://ardes.bg/uploads/original/solid-state-drive-ssd-samsung-870-evo-sata-2-5-rdq-303041.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ASU650SS-240GT-R', 'ADATA', 'Ultimate SU650 240GB SSD', 36, 49.00, 9.00, 10, 'https://ardes.bg/uploads/original/240gb-ssd-adata-ultimate-su650-514172.jpg', 'Storage Interface', 'SATA'),
('Storage', 'SP512GBSS3A55S25', 'Silicon Power', 'Ace A55 512GB SSD', 36, 85.00, 0.00, 10, 'https://ardes.bg/uploads/original/128gb-ssd-silicon-power-ace-a55-187308.jpg', 'Storage Interface', 'SATA'),
('Storage', 'MZ-77E1T0B/EU', 'Samsung', '870 EVO 1TB SSD', 60, 257.00, 0.00, 10, 'https://ardes.bg/uploads/original/solid-state-drive-ssd-samsung-870-evo-sata-2-5-rdq-303041.jpg', 'Storage Interface', 'SATA'),
('Storage', 'SA400S37/960G', 'Kingston', 'A400 960GB SSD', 36, 139.00, 13.00, 10, 'https://ardes.bg/uploads/original/kingston-ssd-480gb-a400-sata3-2-5-ssd-7mm-height-t-159648.jpg', 'Storage Interface', 'SATA'),
('Storage', 'SKC3000S/1024G', 'Kingston', 'KC3000 1TB SSD', 60, 209.00, 0.00, 10, 'https://ardes.bg/uploads/original/kingston-kc3000-1024gb-ssd-m-2-2280-pcie-4-0-nvme-350180.jpg', 'Storage Interface', 'NVMe'),
('Storage', 'SP256GBSS3A55S25', 'Silicon Power', 'Ace A55 256GB SSD', 36, 55.00, 0.00, 10, 'https://ardes.bg/uploads/original/silicon-power-solid-state-disk-2-5-sata-ssd-a55-25-178223.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST2000DM008', 'Seagate', 'Barracuda 2TB HDD', 24, 144.00, 0.00, 10, 'https://ardes.bg/uploads/original/seagate-hdd-desktop-barracuda-guardian-3-5-2tb-sat-202842.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST1000LM048', 'Seagate', 'BarraCuda ST1000LM048 1TB HDD', 24, 177.00, 0.00, 10, 'https://ardes.bg/uploads/original/seagate-hdd-mobile-barracuda25-guardian-2-5-1tb-sa-150498.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST2000LM015', 'Seagate', 'ST2000LM015 2TB HDD', 24, 206.00, 0.00, 10, 'https://ardes.bg/uploads/original/seagate-samsung-st2000lm015-2tb-sata3-128mb-tvard--146005.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST8000DM004', 'Seagate', 'Barracuda ST8000DM004 8TB HDD', 24, 280.00, 0.00, 10, 'https://ardes.bg/uploads/original/8t-sg-st8000dm004-256mb-5400-201196.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST4000VN006', 'Seagate', 'IronWolf 4TB HDD', 36, 250.00, 0.00, 10, 'https://ardes.bg/uploads/original/4tb-seagate-ironwolf-458666.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST8000VN004', 'Seagate', 'IronWolf 8TB HDD', 36, 354.00, 0.00, 10, 'https://ardes.bg/uploads/original/8tb-seagate-iron-wolf-st8000vn004-254766.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST4000LM024', 'Seagate', 'Barracuda ST4000LM024 4TB HDD', 24, 182.00, 0.00, 10, 'https://ardes.bg/uploads/original/4tb-seagate-barracuda-st4000lm024-159920.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST4000VX016', 'Seagate', 'AI Skyhawk 4TB HDD', 36, 188.00, 0.00, 10, 'https://ardes.bg/uploads/original/seagate-surv-skyhawk-4tb-hdd-cmr-5400rpm-sata-seri-376335.jpg', 'Storage Interface', 'SATA'),
('Storage', 'WD40EFPX', 'WD', 'Red Plus 4TB HDD', 60, 260.00, 0.00, 10, 'https://ardes.bg/uploads/original/hard-disk-wd-red-plus-4tb-nas-3-5-256mb-5400rpm-wd-465680.jpg', 'Storage Interface', 'SATA'),
('Storage', 'WD23PURZ', 'WD', 'Purple WD23PURZ 2TB HDD', 36, 186.00, 0.00, 10, 'https://ardes.bg/uploads/original/2tb-wd-purple-wd23purz-467687.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST5000LM000', 'Seagate', 'Barracuda 5TB HDD', 24, 200.00, 0.00, 10, 'https://ardes.bg/uploads/original/hard-disk-seagate-barracuda-5tb-5400rpm-2-5-quot-1-319564.jpg', 'Storage Interface', 'SATA'),
('Storage', 'ST10000VE001', 'Seagate', 'SkyHawk AI 10TB HDD', 60, 384.00, 0.00, 10, 'https://ardes.bg/uploads/original/seagate-surveillance-ai-skyhawk-10tb-hdd-sata-6gb-376334.jpg', 'Storage Interface', 'SATA');

INSERT INTO #SeedProducts
    (CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image, FieldName, FieldValue)
SELECT
    p.CategoryName, p.ProductKey, p.Manufacturer, p.Model, p.Warranty, p.Price, p.Discount, p.Quantity, p.Image,
    s.FieldName, s.FieldValue
FROM
(
    SELECT DISTINCT CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image
    FROM #SeedProducts
    WHERE CategoryName = 'Storage'
) p
INNER JOIN
(
    VALUES
        ('SA400S37/240G',     'Type', 'SSD'),
        ('SA400S37/240G',     'Disk Form Factor', '2.5"'),
        ('SA400S37/240G',     'Capacity', '240 GB'),
        ('SA400S37/240G',     'Read Speed', '500 MB/s'),
        ('SA400S37/240G',     'Write Speed', '350 MB/s'),
        ('SA400S37/240G',     'Extras', 'TRIM, S.M.A.R.T., GC Algorithm'),
        ('SA400S37/480G',     'Type', 'SSD'),
        ('SA400S37/480G',     'Disk Form Factor', '2.5"'),
        ('SA400S37/480G',     'Capacity', '480 GB'),
        ('SA400S37/480G',     'Read Speed', '500 MB/s'),
        ('SA400S37/480G',     'Write Speed', '450 MB/s'),
        ('SA400S37/480G',     'Extras', 'TRIM, S.M.A.R.T., GC Algorithm'),
        ('SNV3S/1000G',       'Type', 'SSD'),
        ('SNV3S/1000G',       'Disk Form Factor', 'M.2 (2280)'),
        ('SNV3S/1000G',       'Capacity', '1 TB'),
        ('SNV3S/1000G',       'Read Speed', '6000 MB/s'),
        ('SNV3S/1000G',       'Write Speed', '5000 MB/s'),
        ('SNV3S/1000G',       'Extras', 'NVMe PCIe 4.0, TRIM, S.M.A.R.T.'),
        ('MZ-V9P2T0BW',       'Type', 'SSD'),
        ('MZ-V9P2T0BW',       'Disk Form Factor', 'M.2 (2280)'),
        ('MZ-V9P2T0BW',       'Capacity', '2 TB'),
        ('MZ-V9P2T0BW',       'Read Speed', '7450 MB/s'),
        ('MZ-V9P2T0BW',       'Write Speed', '6900 MB/s'),
        ('MZ-V9P2T0BW',       'Extras', 'NVMe 2.0, Samsung V-NAND, AES 256-bit, TRIM, S.M.A.R.T.'),
        ('MZ-V9P1T0BW',       'Type', 'SSD'),
        ('MZ-V9P1T0BW',       'Disk Form Factor', 'M.2 (2280)'),
        ('MZ-V9P1T0BW',       'Capacity', '1 TB'),
        ('MZ-V9P1T0BW',       'Read Speed', '7450 MB/s'),
        ('MZ-V9P1T0BW',       'Write Speed', '6900 MB/s'),
        ('MZ-V9P1T0BW',       'Extras', 'NVMe 2.0, Samsung V-NAND, AES 256-bit, TRIM, S.M.A.R.T.'),
        ('MZ-77E500B/EU',     'Type', 'SSD'),
        ('MZ-77E500B/EU',     'Disk Form Factor', '2.5"'),
        ('MZ-77E500B/EU',     'Capacity', '500 GB'),
        ('MZ-77E500B/EU',     'Read Speed', '560 MB/s'),
        ('MZ-77E500B/EU',     'Write Speed', '530 MB/s'),
        ('MZ-77E500B/EU',     'Extras', 'Samsung V-NAND, AES 256-bit, TRIM, S.M.A.R.T.'),
        ('ASU650SS-240GT-R',  'Type', 'SSD'),
        ('ASU650SS-240GT-R',  'Disk Form Factor', '2.5"'),
        ('ASU650SS-240GT-R',  'Capacity', '240 GB'),
        ('ASU650SS-240GT-R',  'Read Speed', '520 MB/s'),
        ('ASU650SS-240GT-R',  'Write Speed', '450 MB/s'),
        ('ASU650SS-240GT-R',  'Extras', 'TRIM, S.M.A.R.T.'),
        ('SP512GBSS3A55S25',  'Type', 'SSD'),
        ('SP512GBSS3A55S25',  'Disk Form Factor', '2.5"'),
        ('SP512GBSS3A55S25',  'Capacity', '512 GB'),
        ('SP512GBSS3A55S25',  'Read Speed', '560 MB/s'),
        ('SP512GBSS3A55S25',  'Write Speed', '530 MB/s'),
        ('SP512GBSS3A55S25',  'Extras', 'TRIM, S.M.A.R.T.'),
        ('MZ-77E1T0B/EU',     'Type', 'SSD'),
        ('MZ-77E1T0B/EU',     'Disk Form Factor', '2.5"'),
        ('MZ-77E1T0B/EU',     'Capacity', '1 TB'),
        ('MZ-77E1T0B/EU',     'Read Speed', '560 MB/s'),
        ('MZ-77E1T0B/EU',     'Write Speed', '530 MB/s'),
        ('MZ-77E1T0B/EU',     'Extras', 'Samsung V-NAND, AES 256-bit, TRIM, S.M.A.R.T.'),
        ('SA400S37/960G',     'Type', 'SSD'),
        ('SA400S37/960G',     'Disk Form Factor', '2.5"'),
        ('SA400S37/960G',     'Capacity', '960 GB'),
        ('SA400S37/960G',     'Read Speed', '500 MB/s'),
        ('SA400S37/960G',     'Write Speed', '450 MB/s'),
        ('SA400S37/960G',     'Extras', 'TRIM, S.M.A.R.T., GC Algorithm'),
        ('SKC3000S/1024G',    'Type', 'SSD'),
        ('SKC3000S/1024G',    'Disk Form Factor', 'M.2 (2280)'),
        ('SKC3000S/1024G',    'Capacity', '1 TB'),
        ('SKC3000S/1024G',    'Read Speed', '7000 MB/s'),
        ('SKC3000S/1024G',    'Write Speed', '6000 MB/s'),
        ('SKC3000S/1024G',    'Extras', 'NVMe PCIe 4.0, TRIM, S.M.A.R.T.'),
        ('SP256GBSS3A55S25',  'Type', 'SSD'),
        ('SP256GBSS3A55S25',  'Disk Form Factor', '2.5"'),
        ('SP256GBSS3A55S25',  'Capacity', '256 GB'),
        ('SP256GBSS3A55S25',  'Read Speed', '550 MB/s'),
        ('SP256GBSS3A55S25',  'Write Speed', '450 MB/s'),
        ('SP256GBSS3A55S25',  'Extras', 'TRIM, S.M.A.R.T.'),
        ('ST2000DM008',       'Type', 'HDD'),
        ('ST2000DM008',       'Disk Form Factor', '3.5"'),
        ('ST2000DM008',       'Capacity', '2 TB'),
        ('ST2000DM008',       'Read Speed', '220 MB/s'),
        ('ST2000DM008',       'Write Speed', '220 MB/s'),
        ('ST2000DM008',       'Extras', '7200 RPM, 256 MB Cache'),
        ('ST1000LM048',       'Type', 'HDD'),
        ('ST1000LM048',       'Disk Form Factor', '2.5"'),
        ('ST1000LM048',       'Capacity', '1 TB'),
        ('ST1000LM048',       'Read Speed', '140 MB/s'),
        ('ST1000LM048',       'Write Speed', '140 MB/s'),
        ('ST1000LM048',       'Extras', '5400 RPM, 128 MB Cache'),
        ('ST2000LM015',       'Type', 'HDD'),
        ('ST2000LM015',       'Disk Form Factor', '2.5"'),
        ('ST2000LM015',       'Capacity', '2 TB'),
        ('ST2000LM015',       'Read Speed', '140 MB/s'),
        ('ST2000LM015',       'Write Speed', '140 MB/s'),
        ('ST2000LM015',       'Extras', '5400 RPM, 128 MB Cache'),
        ('ST8000DM004',       'Type', 'HDD'),
        ('ST8000DM004',       'Disk Form Factor', '3.5"'),
        ('ST8000DM004',       'Capacity', '8 TB'),
        ('ST8000DM004',       'Read Speed', '190 MB/s'),
        ('ST8000DM004',       'Write Speed', '190 MB/s'),
        ('ST8000DM004',       'Extras', '5400 RPM, 256 MB Cache'),
        ('ST4000VN006',       'Type', 'HDD'),
        ('ST4000VN006',       'Disk Form Factor', '3.5"'),
        ('ST4000VN006',       'Capacity', '4 TB'),
        ('ST4000VN006',       'Read Speed', '180 MB/s'),
        ('ST4000VN006',       'Write Speed', '180 MB/s'),
        ('ST4000VN006',       'Extras', 'NAS, 5400 RPM, 256 MB Cache, IronWolf'),
        ('ST8000VN004',       'Type', 'HDD'),
        ('ST8000VN004',       'Disk Form Factor', '3.5"'),
        ('ST8000VN004',       'Capacity', '8 TB'),
        ('ST8000VN004',       'Read Speed', '210 MB/s'),
        ('ST8000VN004',       'Write Speed', '210 MB/s'),
        ('ST8000VN004',       'Extras', 'NAS, 7200 RPM, 256 MB Cache, IronWolf'),
        ('ST4000LM024',       'Type', 'HDD'),
        ('ST4000LM024',       'Disk Form Factor', '2.5"'),
        ('ST4000LM024',       'Capacity', '4 TB'),
        ('ST4000LM024',       'Read Speed', '140 MB/s'),
        ('ST4000LM024',       'Write Speed', '140 MB/s'),
        ('ST4000LM024',       'Extras', '5400 RPM, 128 MB Cache'),
        ('ST4000VX016',       'Type', 'HDD'),
        ('ST4000VX016',       'Disk Form Factor', '3.5"'),
        ('ST4000VX016',       'Capacity', '4 TB'),
        ('ST4000VX016',       'Read Speed', '180 MB/s'),
        ('ST4000VX016',       'Write Speed', '180 MB/s'),
        ('ST4000VX016',       'Extras', 'Surveillance, 5400 RPM, 256 MB Cache, SkyHawk AI'),
        ('WD40EFPX',          'Type', 'HDD'),
        ('WD40EFPX',          'Disk Form Factor', '3.5"'),
        ('WD40EFPX',          'Capacity', '4 TB'),
        ('WD40EFPX',          'Read Speed', '180 MB/s'),
        ('WD40EFPX',          'Write Speed', '180 MB/s'),
        ('WD40EFPX',          'Extras', 'NAS, 5400 RPM, 256 MB Cache, WD Red Plus'),
        ('WD23PURZ',          'Type', 'HDD'),
        ('WD23PURZ',          'Disk Form Factor', '3.5"'),
        ('WD23PURZ',          'Capacity', '2 TB'),
        ('WD23PURZ',          'Read Speed', '175 MB/s'),
        ('WD23PURZ',          'Write Speed', '175 MB/s'),
        ('WD23PURZ',          'Extras', 'Surveillance, 5400 RPM, 64 MB Cache, WD Purple'),
        ('ST5000LM000',       'Type', 'HDD'),
        ('ST5000LM000',       'Disk Form Factor', '2.5"'),
        ('ST5000LM000',       'Capacity', '5 TB'),
        ('ST5000LM000',       'Read Speed', '140 MB/s'),
        ('ST5000LM000',       'Write Speed', '140 MB/s'),
        ('ST5000LM000',       'Extras', '5400 RPM, 128 MB Cache'),
        ('ST10000VE001',      'Type', 'HDD'),
        ('ST10000VE001',      'Disk Form Factor', '3.5"'),
        ('ST10000VE001',      'Capacity', '10 TB'),
        ('ST10000VE001',      'Read Speed', '250 MB/s'),
        ('ST10000VE001',      'Write Speed', '250 MB/s'),
        ('ST10000VE001',      'Extras', 'Surveillance, 7200 RPM, 256 MB Cache, SkyHawk AI')
) AS s(ProductKey, FieldName, FieldValue)
    ON s.ProductKey = p.ProductKey;

-- =====================================================================
-- PSU
-- =====================================================================
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
        ('ACPB-LD65AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LD65AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 4 x SATA, 3 x Molex'),
        ('ACPB-LD65AEC.11', 'Cooling',                   '120 mm fan'),
        ('ACPB-LD65AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LD65AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LD65AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LD65AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LD65AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LD65AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        ('ACPB-LX65AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LX65AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 4 x SATA, 3 x Molex'),
        ('ACPB-LX65AEC.11', 'Cooling',                   '120 mm RGB fan'),
        ('ACPB-LX65AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LX65AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LX65AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LX65AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LX65AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LX65AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        ('ACPB-LD75AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LD75AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 6 x SATA, 3 x Molex'),
        ('ACPB-LD75AEC.11', 'Cooling',                   '120 mm fan'),
        ('ACPB-LD75AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LD75AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LD75AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LD75AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LD75AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LD75AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        ('ZM700-TXII',      'Form Factor',               'ATX'),
        ('ZM700-TXII',      'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 6 x SATA, 3 x Molex'),
        ('ZM700-TXII',      'Cooling',                   '120 mm fan'),
        ('ZM700-TXII',      'Efficiency',                '85%'),
        ('ZM700-TXII',      'Energy Efficiency Rating',  '80 Plus'),
        ('ZM700-TXII',      'PFC',                       'Active PFC'),
        ('ZM700-TXII',      'Cabling Type',              'Non-Modular'),
        ('ZM700-TXII',      'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ZM700-TXII',      'Dimensions',                '140 x 150 x 86 mm'),
        ('306-7ZP2B11-CE0', 'Form Factor',               'ATX'),
        ('306-7ZP2B11-CE0', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 6 x SATA, 3 x Molex'),
        ('306-7ZP2B11-CE0', 'Cooling',                   '120 mm low-noise fan'),
        ('306-7ZP2B11-CE0', 'Efficiency',                '88%'),
        ('306-7ZP2B11-CE0', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('306-7ZP2B11-CE0', 'PFC',                       'Active PFC'),
        ('306-7ZP2B11-CE0', 'Cabling Type',              'Non-Modular'),
        ('306-7ZP2B11-CE0', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('306-7ZP2B11-CE0', 'Dimensions',                '140 x 150 x 86 mm'),
        ('PS-600FK',        'Form Factor',               'ATX'),
        ('PS-600FK',        'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 1 x 6+2 PCIe, 4 x SATA, 2 x Molex'),
        ('PS-600FK',        'Cooling',                   '120 mm fan'),
        ('PS-600FK',        'Efficiency',                '82%'),
        ('PS-600FK',        'Energy Efficiency Rating',  '80 Plus'),
        ('PS-600FK',        'PFC',                       'Active PFC'),
        ('PS-600FK',        'Cabling Type',              'Non-Modular'),
        ('PS-600FK',        'Protections',               'OVP, UVP, SCP, OCP, OPP'),
        ('PS-600FK',        'Dimensions',                '140 x 150 x 86 mm'),
        ('ACPB-LD55AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LD55AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 4 x SATA, 3 x Molex'),
        ('ACPB-LD55AEC.11', 'Cooling',                   '120 mm fan'),
        ('ACPB-LD55AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LD55AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LD55AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LD55AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LD55AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LD55AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
        ('306-7ZP8B11-CE0', 'Form Factor',               'ATX'),
        ('306-7ZP8B11-CE0', 'Connectors',                '1 x 24-pin ATX, 2 x 4+4 CPU, 1 x 12+4 PCIe (12VHPWR), 3 x 6+2 PCIe, 8 x SATA, 4 x Molex'),
        ('306-7ZP8B11-CE0', 'Cooling',                   '120 mm fluid dynamic bearing fan'),
        ('306-7ZP8B11-CE0', 'Efficiency',                '90%'),
        ('306-7ZP8B11-CE0', 'Energy Efficiency Rating',  '80 Plus Gold'),
        ('306-7ZP8B11-CE0', 'PFC',                       'Active PFC'),
        ('306-7ZP8B11-CE0', 'Cabling Type',              'Fully Modular'),
        ('306-7ZP8B11-CE0', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('306-7ZP8B11-CE0', 'Dimensions',                '140 x 150 x 86 mm'),
        ('90YE00S2-B0NA00', 'Form Factor',               'ATX'),
        ('90YE00S2-B0NA00', 'Connectors',                '1 x 24-pin ATX, 2 x 4+4 CPU, 4 x 6+2 PCIe, 8 x SATA, 4 x Molex'),
        ('90YE00S2-B0NA00', 'Cooling',                   '135 mm dual ball bearing fan'),
        ('90YE00S2-B0NA00', 'Efficiency',                '90%'),
        ('90YE00S2-B0NA00', 'Energy Efficiency Rating',  '80 Plus Gold'),
        ('90YE00S2-B0NA00', 'PFC',                       'Active PFC'),
        ('90YE00S2-B0NA00', 'Cabling Type',              'Fully Modular'),
        ('90YE00S2-B0NA00', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('90YE00S2-B0NA00', 'Dimensions',                '150 x 150 x 86 mm'),
        ('306-7ZP8A11-CE0', 'Form Factor',               'ATX'),
        ('306-7ZP8A11-CE0', 'Connectors',                '1 x 24-pin ATX, 2 x 4+4 CPU, 1 x 12+4 PCIe (12VHPWR), 3 x 6+2 PCIe, 8 x SATA, 4 x Molex'),
        ('306-7ZP8A11-CE0', 'Cooling',                   '120 mm fluid dynamic bearing fan'),
        ('306-7ZP8A11-CE0', 'Efficiency',                '90%'),
        ('306-7ZP8A11-CE0', 'Energy Efficiency Rating',  '80 Plus Gold'),
        ('306-7ZP8A11-CE0', 'PFC',                       'Active PFC'),
        ('306-7ZP8A11-CE0', 'Cabling Type',              'Fully Modular'),
        ('306-7ZP8A11-CE0', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('306-7ZP8A11-CE0', 'Dimensions',                '140 x 150 x 86 mm'),
        ('ACPB-LX55AEC.11', 'Form Factor',               'ATX'),
        ('ACPB-LX55AEC.11', 'Connectors',                '1 x 24-pin ATX, 1 x 4+4 CPU, 2 x 6+2 PCIe, 4 x SATA, 3 x Molex'),
        ('ACPB-LX55AEC.11', 'Cooling',                   '120 mm RGB fan'),
        ('ACPB-LX55AEC.11', 'Efficiency',                '85%'),
        ('ACPB-LX55AEC.11', 'Energy Efficiency Rating',  '80 Plus Bronze'),
        ('ACPB-LX55AEC.11', 'PFC',                       'Active PFC'),
        ('ACPB-LX55AEC.11', 'Cabling Type',              'Non-Modular'),
        ('ACPB-LX55AEC.11', 'Protections',               'OVP, UVP, SCP, OCP, OPP, OTP'),
        ('ACPB-LX55AEC.11', 'Dimensions',                '150 x 140 x 86 mm'),
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

-- =====================================================================
-- Case
-- =====================================================================
MERGE dbo.Categories AS target
USING (VALUES ('Case')) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name)
    VALUES (CONVERT(nvarchar(36), NEWID()), source.Name);

MERGE dbo.Fields AS target
USING
(
    VALUES
        ('Supported Motherboard Form Factors', 18),
        ('Max GPU Length (mm)', 9),
        ('Case Type', 18),
        ('Case Length (mm)', 9),
        ('Case Width (mm)', 9),
        ('Case Height (mm)', 9),
        ('Max CPU Cooler Height (mm)', 9),
        ('Drive Bays', 18),
        ('Expansion Slots', 9),
        ('Power Supply', 18),
        ('Power Supply Placement', 18),
        ('Supported Fans', 18),
        ('Supported Liquid Cooling Radiators', 18),
        ('Ports', 18),
        ('Included Fans', 18),
        ('Panels', 18),
        ('Extras', 18),
        ('Color', 18),
        ('Dimensions', 18),
        ('Weight', 18)
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
    ON f.Name IN ('Supported Motherboard Form Factors', 'Max GPU Length (mm)',
                  'Case Type', 'Case Length (mm)', 'Case Width (mm)', 'Case Height (mm)',
                  'Max CPU Cooler Height (mm)', 'Drive Bays', 'Expansion Slots',
                  'Power Supply', 'Power Supply Placement', 'Supported Fans',
                  'Supported Liquid Cooling Radiators', 'Ports', 'Included Fans',
                  'Panels', 'Extras', 'Color', 'Dimensions', 'Weight')
WHERE c.Name = 'Case'
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
('Case', 'R-CC360-BKAPM3-G-1', 'Deepcool', 'CC360 ARGB Black', 24, 42.00, 0.00, 10, 'https://ardes.bg/uploads/original/kutiya-za-nastolen-kompyutar-deepcool-cc360-argb-r-483279.jpg', 'Supported Motherboard Form Factors', 'Micro ATX, Mini ITX, mATX, ITX'),
('Case', 'R-CC360-BKAPM3-G-1', 'Deepcool', 'CC360 ARGB Black', 24, 42.00, 0.00, 10, 'https://ardes.bg/uploads/original/kutiya-za-nastolen-kompyutar-deepcool-cc360-argb-r-483279.jpg', 'Max GPU Length (mm)', '320'),
('Case', 'R-CC360-BKAPM3-G-1', 'Deepcool', 'CC360 ARGB Black', 24, 42.00, 0.00, 10, 'https://ardes.bg/uploads/original/kutiya-za-nastolen-kompyutar-deepcool-cc360-argb-r-483279.jpg', 'Case Type', 'M-ATX Case'),
('Case', 'R-CC360-BKAPM3-G-1', 'Deepcool', 'CC360 ARGB Black', 24, 42.00, 0.00, 10, 'https://ardes.bg/uploads/original/kutiya-za-nastolen-kompyutar-deepcool-cc360-argb-r-483279.jpg', 'Case Length (mm)', '418'),
('Case', 'R-CC360-BKAPM3-G-1', 'Deepcool', 'CC360 ARGB Black', 24, 42.00, 0.00, 10, 'https://ardes.bg/uploads/original/kutiya-za-nastolen-kompyutar-deepcool-cc360-argb-r-483279.jpg', 'Case Width (mm)', '215'),
('Case', 'R-CC360-BKAPM3-G-1', 'Deepcool', 'CC360 ARGB Black', 24, 42.00, 0.00, 10, 'https://ardes.bg/uploads/original/kutiya-za-nastolen-kompyutar-deepcool-cc360-argb-r-483279.jpg', 'Case Height (mm)', '431'),
('Case', 'R-CC360-BKAPM3-G-1', 'Deepcool', 'CC360 ARGB Black', 24, 42.00, 0.00, 10, 'https://ardes.bg/uploads/original/kutiya-za-nastolen-kompyutar-deepcool-cc360-argb-r-483279.jpg', 'Max CPU Cooler Height (mm)', '165'),
('Case', 'G99.LAN217X.00', 'Lian Li', 'LANCOOL 217 Black', 24, 115.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-lancool-217-cheren-656422.jpg', 'Supported Motherboard Form Factors', 'SSI EEB, E-ATX, ATX, Micro ATX, Mini ITX, mATX, ITX'),
('Case', 'G99.LAN217X.00', 'Lian Li', 'LANCOOL 217 Black', 24, 115.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-lancool-217-cheren-656422.jpg', 'Max GPU Length (mm)', '380'),
('Case', 'G99.LAN217X.00', 'Lian Li', 'LANCOOL 217 Black', 24, 115.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-lancool-217-cheren-656422.jpg', 'Case Type', 'Mid Tower'),
('Case', 'G99.LAN217X.00', 'Lian Li', 'LANCOOL 217 Black', 24, 115.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-lancool-217-cheren-656422.jpg', 'Case Length (mm)', '482'),
('Case', 'G99.LAN217X.00', 'Lian Li', 'LANCOOL 217 Black', 24, 115.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-lancool-217-cheren-656422.jpg', 'Case Width (mm)', '238'),
('Case', 'G99.LAN217X.00', 'Lian Li', 'LANCOOL 217 Black', 24, 115.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-lancool-217-cheren-656422.jpg', 'Case Height (mm)', '503'),
('Case', 'G99.LAN217X.00', 'Lian Li', 'LANCOOL 217 Black', 24, 115.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-lancool-217-cheren-656422.jpg', 'Max CPU Cooler Height (mm)', '180'),
('Case', 'POC0000297', 'Fortron', 'S120 Black', 24, 25.00, 0.00, 10, 'https://ardes.bg/uploads/original/fortron-s120-cheren-719094.jpg', 'Supported Motherboard Form Factors', 'Micro ATX, mATX'),
('Case', 'POC0000297', 'Fortron', 'S120 Black', 24, 25.00, 0.00, 10, 'https://ardes.bg/uploads/original/fortron-s120-cheren-719094.jpg', 'Max GPU Length (mm)', '250'),
('Case', 'POC0000297', 'Fortron', 'S120 Black', 24, 25.00, 0.00, 10, 'https://ardes.bg/uploads/original/fortron-s120-cheren-719094.jpg', 'Case Type', 'Mini Tower'),
('Case', 'POC0000297', 'Fortron', 'S120 Black', 24, 25.00, 0.00, 10, 'https://ardes.bg/uploads/original/fortron-s120-cheren-719094.jpg', 'Case Length (mm)', '309'),
('Case', 'POC0000297', 'Fortron', 'S120 Black', 24, 25.00, 0.00, 10, 'https://ardes.bg/uploads/original/fortron-s120-cheren-719094.jpg', 'Case Width (mm)', '185'),
('Case', 'POC0000297', 'Fortron', 'S120 Black', 24, 25.00, 0.00, 10, 'https://ardes.bg/uploads/original/fortron-s120-cheren-719094.jpg', 'Case Height (mm)', '354'),
('Case', 'POC0000297', 'Fortron', 'S120 Black', 24, 25.00, 0.00, 10, 'https://ardes.bg/uploads/original/fortron-s120-cheren-719094.jpg', 'Max CPU Cooler Height (mm)', '145'),
('Case', 'DS900M-6F-BK', 'darkFlash', 'DS900M Black', 24, 51.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-ds900m-cheren-677534.jpg', 'Supported Motherboard Form Factors', 'Micro ATX, Mini ITX, mATX, ITX'),
('Case', 'DS900M-6F-BK', 'darkFlash', 'DS900M Black', 24, 51.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-ds900m-cheren-677534.jpg', 'Max GPU Length (mm)', '350'),
('Case', 'DS900M-6F-BK', 'darkFlash', 'DS900M Black', 24, 51.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-ds900m-cheren-677534.jpg', 'Case Type', 'Mini Tower'),
('Case', 'DS900M-6F-BK', 'darkFlash', 'DS900M Black', 24, 51.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-ds900m-cheren-677534.jpg', 'Case Length (mm)', '434'),
('Case', 'DS900M-6F-BK', 'darkFlash', 'DS900M Black', 24, 51.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-ds900m-cheren-677534.jpg', 'Case Width (mm)', '218'),
('Case', 'DS900M-6F-BK', 'darkFlash', 'DS900M Black', 24, 51.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-ds900m-cheren-677534.jpg', 'Case Height (mm)', '414'),
('Case', 'DS900M-6F-BK', 'darkFlash', 'DS900M Black', 24, 51.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-ds900m-cheren-677534.jpg', 'Max CPU Cooler Height (mm)', '165'),
('Case', '306-7G16X23-809', 'MSI', 'MAG FORGE 112R Black', 24, 68.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-forge-112r-mid-tower-atx-m-atx-itx-2x-usb-555438.jpg', 'Supported Motherboard Form Factors', 'ATX, Micro ATX, ITX, mATX, Mini ITX'),
('Case', '306-7G16X23-809', 'MSI', 'MAG FORGE 112R Black', 24, 68.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-forge-112r-mid-tower-atx-m-atx-itx-2x-usb-555438.jpg', 'Max GPU Length (mm)', '330'),
('Case', '306-7G16X23-809', 'MSI', 'MAG FORGE 112R Black', 24, 68.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-forge-112r-mid-tower-atx-m-atx-itx-2x-usb-555438.jpg', 'Case Type', 'Mid Tower'),
('Case', '306-7G16X23-809', 'MSI', 'MAG FORGE 112R Black', 24, 68.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-forge-112r-mid-tower-atx-m-atx-itx-2x-usb-555438.jpg', 'Case Length (mm)', '409'),
('Case', '306-7G16X23-809', 'MSI', 'MAG FORGE 112R Black', 24, 68.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-forge-112r-mid-tower-atx-m-atx-itx-2x-usb-555438.jpg', 'Case Width (mm)', '214'),
('Case', '306-7G16X23-809', 'MSI', 'MAG FORGE 112R Black', 24, 68.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-forge-112r-mid-tower-atx-m-atx-itx-2x-usb-555438.jpg', 'Case Height (mm)', '485'),
('Case', '306-7G16X23-809', 'MSI', 'MAG FORGE 112R Black', 24, 68.00, 0.00, 10, 'https://ardes.bg/uploads/original/msi-mag-forge-112r-mid-tower-atx-m-atx-itx-2x-usb-555438.jpg', 'Max CPU Cooler Height (mm)', '160'),
('Case', '382DA40.0004', 'COUGAR', 'CFV235 Vision White', 24, 168.00, 0.00, 10, 'https://ardes.bg/uploads/original/cougar-cfv235-vision-byal-711812.jpg', 'Supported Motherboard Form Factors', 'SSI CEB, ATX, Micro ATX, Mini ITX, mATX, ITX'),
('Case', '382DA40.0004', 'COUGAR', 'CFV235 Vision White', 24, 168.00, 0.00, 10, 'https://ardes.bg/uploads/original/cougar-cfv235-vision-byal-711812.jpg', 'Max GPU Length (mm)', '400'),
('Case', '382DA40.0004', 'COUGAR', 'CFV235 Vision White', 24, 168.00, 0.00, 10, 'https://ardes.bg/uploads/original/cougar-cfv235-vision-byal-711812.jpg', 'Case Type', 'Mid Tower'),
('Case', '382DA40.0004', 'COUGAR', 'CFV235 Vision White', 24, 168.00, 0.00, 10, 'https://ardes.bg/uploads/original/cougar-cfv235-vision-byal-711812.jpg', 'Case Length (mm)', '235'),
('Case', '382DA40.0004', 'COUGAR', 'CFV235 Vision White', 24, 168.00, 0.00, 10, 'https://ardes.bg/uploads/original/cougar-cfv235-vision-byal-711812.jpg', 'Case Width (mm)', '493'),
('Case', '382DA40.0004', 'COUGAR', 'CFV235 Vision White', 24, 168.00, 0.00, 10, 'https://ardes.bg/uploads/original/cougar-cfv235-vision-byal-711812.jpg', 'Case Height (mm)', '460'),
('Case', '382DA40.0004', 'COUGAR', 'CFV235 Vision White', 24, 168.00, 0.00, 10, 'https://ardes.bg/uploads/original/cougar-cfv235-vision-byal-711812.jpg', 'Max CPU Cooler Height (mm)', '180'),
('Case', 'CM-H92FW-01', 'NZXT', 'H9 Flow White', 24, 160.00, 0.00, 10, 'https://ardes.bg/uploads/original/nzxt-h9-flow-byal-709991.jpg', 'Supported Motherboard Form Factors', 'E-ATX, ATX, Micro ATX, Mini ITX, mATX, ITX'),
('Case', 'CM-H92FW-01', 'NZXT', 'H9 Flow White', 24, 160.00, 0.00, 10, 'https://ardes.bg/uploads/original/nzxt-h9-flow-byal-709991.jpg', 'Max GPU Length (mm)', '435'),
('Case', 'CM-H92FW-01', 'NZXT', 'H9 Flow White', 24, 160.00, 0.00, 10, 'https://ardes.bg/uploads/original/nzxt-h9-flow-byal-709991.jpg', 'Case Type', 'Mid Tower'),
('Case', 'CM-H92FW-01', 'NZXT', 'H9 Flow White', 24, 160.00, 0.00, 10, 'https://ardes.bg/uploads/original/nzxt-h9-flow-byal-709991.jpg', 'Case Length (mm)', '506'),
('Case', 'CM-H92FW-01', 'NZXT', 'H9 Flow White', 24, 160.00, 0.00, 10, 'https://ardes.bg/uploads/original/nzxt-h9-flow-byal-709991.jpg', 'Case Width (mm)', '481'),
('Case', 'CM-H92FW-01', 'NZXT', 'H9 Flow White', 24, 160.00, 0.00, 10, 'https://ardes.bg/uploads/original/nzxt-h9-flow-byal-709991.jpg', 'Case Height (mm)', '315'),
('Case', 'CM-H92FW-01', 'NZXT', 'H9 Flow White', 24, 160.00, 0.00, 10, 'https://ardes.bg/uploads/original/nzxt-h9-flow-byal-709991.jpg', 'Max CPU Cooler Height (mm)', '165'),
('Case', 'EN49033', 'XIGMATEK', 'Aqua 7 White', 24, 91.00, 0.00, 10, 'https://ardes.bg/uploads/original/xigmatek-aqua-7-cheren-754557.jpg', 'Supported Motherboard Form Factors', 'E-ATX, ATX, Micro ATX, Mini ITX, mATX, ITX'),
('Case', 'EN49033', 'XIGMATEK', 'Aqua 7 White', 24, 91.00, 0.00, 10, 'https://ardes.bg/uploads/original/xigmatek-aqua-7-cheren-754557.jpg', 'Max GPU Length (mm)', '400'),
('Case', 'EN49033', 'XIGMATEK', 'Aqua 7 White', 24, 91.00, 0.00, 10, 'https://ardes.bg/uploads/original/xigmatek-aqua-7-cheren-754557.jpg', 'Case Type', 'Mid Tower'),
('Case', 'EN49033', 'XIGMATEK', 'Aqua 7 White', 24, 91.00, 0.00, 10, 'https://ardes.bg/uploads/original/xigmatek-aqua-7-cheren-754557.jpg', 'Case Length (mm)', '448'),
('Case', 'EN49033', 'XIGMATEK', 'Aqua 7 White', 24, 91.00, 0.00, 10, 'https://ardes.bg/uploads/original/xigmatek-aqua-7-cheren-754557.jpg', 'Case Width (mm)', '290'),
('Case', 'EN49033', 'XIGMATEK', 'Aqua 7 White', 24, 91.00, 0.00, 10, 'https://ardes.bg/uploads/original/xigmatek-aqua-7-cheren-754557.jpg', 'Case Height (mm)', '467'),
('Case', 'EN49033', 'XIGMATEK', 'Aqua 7 White', 24, 91.00, 0.00, 10, 'https://ardes.bg/uploads/original/xigmatek-aqua-7-cheren-754557.jpg', 'Max CPU Cooler Height (mm)', '170'),
('Case', 'G99.O11DMIV2FX.00', 'Lian Li', 'O11 DYNAMIC MINI V2 Flow Black', 24, 99.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-o11-dynamic-mini-v2-cheren-689213.jpg', 'Supported Motherboard Form Factors', 'ATX, Micro ATX, Mini ITX, mATX, ITX'),
('Case', 'G99.O11DMIV2FX.00', 'Lian Li', 'O11 DYNAMIC MINI V2 Flow Black', 24, 99.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-o11-dynamic-mini-v2-cheren-689213.jpg', 'Max GPU Length (mm)', '370'),
('Case', 'G99.O11DMIV2FX.00', 'Lian Li', 'O11 DYNAMIC MINI V2 Flow Black', 24, 99.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-o11-dynamic-mini-v2-cheren-689213.jpg', 'Case Type', 'Mini Tower'),
('Case', 'G99.O11DMIV2FX.00', 'Lian Li', 'O11 DYNAMIC MINI V2 Flow Black', 24, 99.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-o11-dynamic-mini-v2-cheren-689213.jpg', 'Case Length (mm)', '424'),
('Case', 'G99.O11DMIV2FX.00', 'Lian Li', 'O11 DYNAMIC MINI V2 Flow Black', 24, 99.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-o11-dynamic-mini-v2-cheren-689213.jpg', 'Case Width (mm)', '273'),
('Case', 'G99.O11DMIV2FX.00', 'Lian Li', 'O11 DYNAMIC MINI V2 Flow Black', 24, 99.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-o11-dynamic-mini-v2-cheren-689213.jpg', 'Case Height (mm)', '392'),
('Case', 'G99.O11DMIV2FX.00', 'Lian Li', 'O11 DYNAMIC MINI V2 Flow Black', 24, 99.00, 0.00, 10, 'https://ardes.bg/uploads/original/lian-li-o11-dynamic-mini-v2-cheren-689213.jpg', 'Max CPU Cooler Height (mm)', '165'),
('Case', 'R-CG380-BKAGM3-G', 'Deepcool', 'CG380 3F Black', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-kutiya-case-atx-cg380-3f-3-argb-fans-usb-717101.jpg', 'Supported Motherboard Form Factors', 'Micro ATX, ITX, mATX, Mini ITX'),
('Case', 'R-CG380-BKAGM3-G', 'Deepcool', 'CG380 3F Black', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-kutiya-case-atx-cg380-3f-3-argb-fans-usb-717101.jpg', 'Max GPU Length (mm)', '410'),
('Case', 'R-CG380-BKAGM3-G', 'Deepcool', 'CG380 3F Black', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-kutiya-case-atx-cg380-3f-3-argb-fans-usb-717101.jpg', 'Case Type', 'Mini Tower'),
('Case', 'R-CG380-BKAGM3-G', 'Deepcool', 'CG380 3F Black', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-kutiya-case-atx-cg380-3f-3-argb-fans-usb-717101.jpg', 'Case Length (mm)', '437'),
('Case', 'R-CG380-BKAGM3-G', 'Deepcool', 'CG380 3F Black', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-kutiya-case-atx-cg380-3f-3-argb-fans-usb-717101.jpg', 'Case Width (mm)', '235'),
('Case', 'R-CG380-BKAGM3-G', 'Deepcool', 'CG380 3F Black', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-kutiya-case-atx-cg380-3f-3-argb-fans-usb-717101.jpg', 'Case Height (mm)', '451'),
('Case', 'R-CG380-BKAGM3-G', 'Deepcool', 'CG380 3F Black', 24, 57.00, 0.00, 10, 'https://ardes.bg/uploads/original/deepcool-kutiya-case-atx-cg380-3f-3-argb-fans-usb-717101.jpg', 'Max CPU Cooler Height (mm)', '175'),
('Case', 'DRX70-MESH-BK', 'darkFlash', 'DRX70 Black', 24, 55.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-drx70-cheren-650369.jpg', 'Supported Motherboard Form Factors', 'ATX, Micro ATX, Mini ITX, mATX, ITX'),
('Case', 'DRX70-MESH-BK', 'darkFlash', 'DRX70 Black', 24, 55.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-drx70-cheren-650369.jpg', 'Max GPU Length (mm)', '380'),
('Case', 'DRX70-MESH-BK', 'darkFlash', 'DRX70 Black', 24, 55.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-drx70-cheren-650369.jpg', 'Case Type', 'Mid Tower'),
('Case', 'DRX70-MESH-BK', 'darkFlash', 'DRX70 Black', 24, 55.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-drx70-cheren-650369.jpg', 'Case Length (mm)', '462'),
('Case', 'DRX70-MESH-BK', 'darkFlash', 'DRX70 Black', 24, 55.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-drx70-cheren-650369.jpg', 'Case Width (mm)', '230'),
('Case', 'DRX70-MESH-BK', 'darkFlash', 'DRX70 Black', 24, 55.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-drx70-cheren-650369.jpg', 'Case Height (mm)', '396'),
('Case', 'DRX70-MESH-BK', 'darkFlash', 'DRX70 Black', 24, 55.00, 0.00, 10, 'https://ardes.bg/uploads/original/darkflash-drx70-cheren-650369.jpg', 'Max CPU Cooler Height (mm)', '165'),
('Case', 'PGW-CH-KOL-110', 'Kolink', 'Observatory HF Mesh ARGB Black', 24, 59.00, 0.00, 10, 'https://ardes.bg/uploads/original/kolink-observatory-hf-mesh-argb-cheren-526399.jpg', 'Supported Motherboard Form Factors', 'ATX, Micro ATX, Mini ITX, mATX, ITX'),
('Case', 'PGW-CH-KOL-110', 'Kolink', 'Observatory HF Mesh ARGB Black', 24, 59.00, 0.00, 10, 'https://ardes.bg/uploads/original/kolink-observatory-hf-mesh-argb-cheren-526399.jpg', 'Max GPU Length (mm)', '330'),
('Case', 'PGW-CH-KOL-110', 'Kolink', 'Observatory HF Mesh ARGB Black', 24, 59.00, 0.00, 10, 'https://ardes.bg/uploads/original/kolink-observatory-hf-mesh-argb-cheren-526399.jpg', 'Case Type', 'Mid Tower'),
('Case', 'PGW-CH-KOL-110', 'Kolink', 'Observatory HF Mesh ARGB Black', 24, 59.00, 0.00, 10, 'https://ardes.bg/uploads/original/kolink-observatory-hf-mesh-argb-cheren-526399.jpg', 'Case Length (mm)', '390'),
('Case', 'PGW-CH-KOL-110', 'Kolink', 'Observatory HF Mesh ARGB Black', 24, 59.00, 0.00, 10, 'https://ardes.bg/uploads/original/kolink-observatory-hf-mesh-argb-cheren-526399.jpg', 'Case Width (mm)', '200'),
('Case', 'PGW-CH-KOL-110', 'Kolink', 'Observatory HF Mesh ARGB Black', 24, 59.00, 0.00, 10, 'https://ardes.bg/uploads/original/kolink-observatory-hf-mesh-argb-cheren-526399.jpg', 'Case Height (mm)', '449'),
('Case', 'PGW-CH-KOL-110', 'Kolink', 'Observatory HF Mesh ARGB Black', 24, 59.00, 0.00, 10, 'https://ardes.bg/uploads/original/kolink-observatory-hf-mesh-argb-cheren-526399.jpg', 'Max CPU Cooler Height (mm)', '160');

INSERT INTO #SeedProducts
    (CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image, FieldName, FieldValue)
SELECT
    p.CategoryName, p.ProductKey, p.Manufacturer, p.Model, p.Warranty, p.Price, p.Discount, p.Quantity, p.Image,
    s.FieldName, s.FieldValue
FROM
(
    SELECT DISTINCT CategoryName, ProductKey, Manufacturer, Model, Warranty, Price, Discount, Quantity, Image
    FROM #SeedProducts
    WHERE CategoryName = 'Case'
) p
INNER JOIN
(
    VALUES
        ('R-CC360-BKAPM3-G-1', 'Drive Bays',                         '2 x 2.5", 2 x 3.5"'),
        ('R-CC360-BKAPM3-G-1', 'Expansion Slots',                    '4'),
        ('R-CC360-BKAPM3-G-1', 'Power Supply',                       'Not included'),
        ('R-CC360-BKAPM3-G-1', 'Power Supply Placement',             'Bottom'),
        ('R-CC360-BKAPM3-G-1', 'Supported Fans',                     'Top: 2x 120 mm, Side: 3x 120 mm'),
        ('R-CC360-BKAPM3-G-1', 'Supported Liquid Cooling Radiators', 'Top: 1x 240 mm'),
        ('R-CC360-BKAPM3-G-1', 'Ports',                              '1 x USB 3.0, 1 x USB 2.0, 1 x Audio In, 1 x Audio Out'),
        ('R-CC360-BKAPM3-G-1', 'Included Fans',                      'Front: 3x 120 mm ARGB'),
        ('R-CC360-BKAPM3-G-1', 'Panels',                             'Side: Tempered Glass'),
        ('R-CC360-BKAPM3-G-1', 'Extras',                             'ARGB LED'),
        ('R-CC360-BKAPM3-G-1', 'Color',                              'Black'),
        ('R-CC360-BKAPM3-G-1', 'Dimensions',                         '418 x 215 x 431 mm'),
        ('R-CC360-BKAPM3-G-1', 'Weight',                             '4.0 kg'),
        ('G99.LAN217X.00',     'Drive Bays',                         '2 x 2.5", 2 x 3.5"'),
        ('G99.LAN217X.00',     'Expansion Slots',                    '7'),
        ('G99.LAN217X.00',     'Power Supply',                       'Not included'),
        ('G99.LAN217X.00',     'Power Supply Placement',             'Bottom'),
        ('G99.LAN217X.00',     'Supported Fans',                     'Top: 3x 120 mm, Front: 3x 140 mm, Rear: 1x 120 mm'),
        ('G99.LAN217X.00',     'Supported Liquid Cooling Radiators', 'Top: 1x 360 mm, Front: 1x 360 mm'),
        ('G99.LAN217X.00',     'Ports',                              '2 x USB 3.0, 1 x USB-C, 1 x Audio'),
        ('G99.LAN217X.00',     'Included Fans',                      'Front: 2x 170 mm, Rear: 1x 140 mm'),
        ('G99.LAN217X.00',     'Panels',                             'Front: Mesh, Side: Tempered Glass'),
        ('G99.LAN217X.00',     'Extras',                             'Magnetic dust filters'),
        ('G99.LAN217X.00',     'Color',                              'Black'),
        ('G99.LAN217X.00',     'Dimensions',                         '482 x 238 x 503 mm'),
        ('G99.LAN217X.00',     'Weight',                             '8.4 kg'),
        ('POC0000297',         'Drive Bays',                         '1 x 2.5", 1 x 3.5"'),
        ('POC0000297',         'Expansion Slots',                    '4'),
        ('POC0000297',         'Power Supply',                       'Not included'),
        ('POC0000297',         'Power Supply Placement',             'Top'),
        ('POC0000297',         'Supported Fans',                     'Rear: 1x 80/92 mm'),
        ('POC0000297',         'Supported Liquid Cooling Radiators', 'N/A'),
        ('POC0000297',         'Ports',                              '1 x USB 3.0, 1 x USB 2.0, 1 x Audio'),
        ('POC0000297',         'Included Fans',                      'None'),
        ('POC0000297',         'Panels',                             'Steel'),
        ('POC0000297',         'Extras',                             'Compact build'),
        ('POC0000297',         'Color',                              'Black'),
        ('POC0000297',         'Dimensions',                         '309 x 185 x 354 mm'),
        ('POC0000297',         'Weight',                             '3.0 kg'),
        ('DS900M-6F-BK',       'Drive Bays',                         '2 x 2.5", 2 x 3.5"'),
        ('DS900M-6F-BK',       'Expansion Slots',                    '4'),
        ('DS900M-6F-BK',       'Power Supply',                       'Not included'),
        ('DS900M-6F-BK',       'Power Supply Placement',             'Bottom'),
        ('DS900M-6F-BK',       'Supported Fans',                     'Top: 2x 120 mm, Side: 3x 120 mm, Rear: 1x 120 mm'),
        ('DS900M-6F-BK',       'Supported Liquid Cooling Radiators', 'Top: 1x 240 mm, Side: 1x 360 mm'),
        ('DS900M-6F-BK',       'Ports',                              '1 x USB 3.0, 2 x USB 2.0, 1 x Audio'),
        ('DS900M-6F-BK',       'Included Fans',                      'Front: 3x 120 mm ARGB, Rear: 1x 120 mm ARGB'),
        ('DS900M-6F-BK',       'Panels',                             'Front: Tempered Glass, Side: Tempered Glass'),
        ('DS900M-6F-BK',       'Extras',                             'ARGB LED'),
        ('DS900M-6F-BK',       'Color',                              'Black'),
        ('DS900M-6F-BK',       'Dimensions',                         '434 x 218 x 414 mm'),
        ('DS900M-6F-BK',       'Weight',                             '4.2 kg'),
        ('306-7G16X23-809',    'Drive Bays',                         '2 x 2.5", 2 x 3.5"'),
        ('306-7G16X23-809',    'Expansion Slots',                    '7'),
        ('306-7G16X23-809',    'Power Supply',                       'Not included'),
        ('306-7G16X23-809',    'Power Supply Placement',             'Bottom'),
        ('306-7G16X23-809',    'Supported Fans',                     'Top: 2x 120 mm, Front: 3x 120 mm, Rear: 1x 120 mm'),
        ('306-7G16X23-809',    'Supported Liquid Cooling Radiators', 'Top: 1x 240 mm, Front: 1x 360 mm'),
        ('306-7G16X23-809',    'Ports',                              '2 x USB 3.0, 1 x Audio'),
        ('306-7G16X23-809',    'Included Fans',                      'Rear: 1x 120 mm ARGB'),
        ('306-7G16X23-809',    'Panels',                             'Side: Tempered Glass'),
        ('306-7G16X23-809',    'Extras',                             'ARGB LED'),
        ('306-7G16X23-809',    'Color',                              'Black'),
        ('306-7G16X23-809',    'Dimensions',                         '409 x 214 x 485 mm'),
        ('306-7G16X23-809',    'Weight',                             '5.5 kg'),
        ('382DA40.0004',       'Drive Bays',                         '2 x 2.5", 2 x 3.5"'),
        ('382DA40.0004',       'Expansion Slots',                    '7'),
        ('382DA40.0004',       'Power Supply',                       'Not included'),
        ('382DA40.0004',       'Power Supply Placement',             'Bottom'),
        ('382DA40.0004',       'Supported Fans',                     'Top: 3x 120 mm, Front: 3x 120 mm, Rear: 1x 120 mm'),
        ('382DA40.0004',       'Supported Liquid Cooling Radiators', 'Top: 1x 360 mm, Front: 1x 360 mm'),
        ('382DA40.0004',       'Ports',                              '2 x USB 3.0, 1 x USB-C, 1 x Audio'),
        ('382DA40.0004',       'Included Fans',                      'Front: 3x 120 mm ARGB, Rear: 1x 120 mm ARGB'),
        ('382DA40.0004',       'Panels',                             'Front: Tempered Glass, Side: Tempered Glass'),
        ('382DA40.0004',       'Extras',                             'ARGB LED'),
        ('382DA40.0004',       'Color',                              'White'),
        ('382DA40.0004',       'Dimensions',                         '235 x 493 x 460 mm'),
        ('382DA40.0004',       'Weight',                             '7.2 kg'),
        ('CM-H92FW-01',        'Drive Bays',                         '4 x 2.5", 2 x 3.5"'),
        ('CM-H92FW-01',        'Expansion Slots',                    '7'),
        ('CM-H92FW-01',        'Power Supply',                       'Not included'),
        ('CM-H92FW-01',        'Power Supply Placement',             'Bottom'),
        ('CM-H92FW-01',        'Supported Fans',                     'Top: 3x 120 mm, Side: 3x 120 mm, Bottom: 3x 120 mm, Rear: 1x 120 mm'),
        ('CM-H92FW-01',        'Supported Liquid Cooling Radiators', 'Top: 1x 360 mm, Side: 1x 360 mm, Bottom: 1x 360 mm'),
        ('CM-H92FW-01',        'Ports',                              '2 x USB 3.0, 1 x USB-C, 1 x Audio'),
        ('CM-H92FW-01',        'Included Fans',                      'Side: 3x 120 mm, Top: 1x 120 mm'),
        ('CM-H92FW-01',        'Panels',                             'Front: Tempered Glass, Side: Tempered Glass'),
        ('CM-H92FW-01',        'Extras',                             'Dual chamber design'),
        ('CM-H92FW-01',        'Color',                              'White'),
        ('CM-H92FW-01',        'Dimensions',                         '506 x 481 x 315 mm'),
        ('CM-H92FW-01',        'Weight',                             '11.6 kg'),
        ('EN49033',            'Drive Bays',                         '2 x 2.5", 2 x 3.5"'),
        ('EN49033',            'Expansion Slots',                    '7'),
        ('EN49033',            'Power Supply',                       'Not included'),
        ('EN49033',            'Power Supply Placement',             'Bottom'),
        ('EN49033',            'Supported Fans',                     'Top: 3x 120 mm, Front: 3x 120 mm, Rear: 1x 120 mm'),
        ('EN49033',            'Supported Liquid Cooling Radiators', 'Top: 1x 360 mm, Front: 1x 360 mm'),
        ('EN49033',            'Ports',                              '2 x USB 3.0, 1 x USB-C, 1 x Audio'),
        ('EN49033',            'Included Fans',                      'Front: 3x 120 mm ARGB, Rear: 1x 120 mm ARGB'),
        ('EN49033',            'Panels',                             'Front: Mesh, Side: Tempered Glass'),
        ('EN49033',            'Extras',                             'ARGB LED'),
        ('EN49033',            'Color',                              'White'),
        ('EN49033',            'Dimensions',                         '448 x 290 x 467 mm'),
        ('EN49033',            'Weight',                             '7.0 kg'),
        ('G99.O11DMIV2FX.00',  'Drive Bays',                         '4 x 2.5", 2 x 3.5"'),
        ('G99.O11DMIV2FX.00',  'Expansion Slots',                    '5'),
        ('G99.O11DMIV2FX.00',  'Power Supply',                       'Not included'),
        ('G99.O11DMIV2FX.00',  'Power Supply Placement',             'Bottom'),
        ('G99.O11DMIV2FX.00',  'Supported Fans',                     'Top: 3x 120 mm, Side: 3x 120 mm, Bottom: 3x 120 mm'),
        ('G99.O11DMIV2FX.00',  'Supported Liquid Cooling Radiators', 'Top: 1x 360 mm, Side: 1x 360 mm, Bottom: 1x 360 mm'),
        ('G99.O11DMIV2FX.00',  'Ports',                              '2 x USB 3.0, 1 x USB-C, 1 x Audio'),
        ('G99.O11DMIV2FX.00',  'Included Fans',                      'None'),
        ('G99.O11DMIV2FX.00',  'Panels',                             'Front: Mesh, Side: Tempered Glass'),
        ('G99.O11DMIV2FX.00',  'Extras',                             'Dual chamber design'),
        ('G99.O11DMIV2FX.00',  'Color',                              'Black'),
        ('G99.O11DMIV2FX.00',  'Dimensions',                         '424 x 273 x 392 mm'),
        ('G99.O11DMIV2FX.00',  'Weight',                             '8.5 kg'),
        ('R-CG380-BKAGM3-G',   'Drive Bays',                         '2 x 2.5", 1 x 3.5"'),
        ('R-CG380-BKAGM3-G',   'Expansion Slots',                    '5'),
        ('R-CG380-BKAGM3-G',   'Power Supply',                       'Not included'),
        ('R-CG380-BKAGM3-G',   'Power Supply Placement',             'Bottom'),
        ('R-CG380-BKAGM3-G',   'Supported Fans',                     'Top: 2x 120 mm, Front: 3x 120 mm, Rear: 1x 120 mm'),
        ('R-CG380-BKAGM3-G',   'Supported Liquid Cooling Radiators', 'Top: 1x 240 mm, Front: 1x 360 mm'),
        ('R-CG380-BKAGM3-G',   'Ports',                              '1 x USB 3.0, 1 x USB-C, 1 x Audio'),
        ('R-CG380-BKAGM3-G',   'Included Fans',                      'Front: 3x 120 mm ARGB'),
        ('R-CG380-BKAGM3-G',   'Panels',                             'Side: Tempered Glass'),
        ('R-CG380-BKAGM3-G',   'Extras',                             'ARGB LED'),
        ('R-CG380-BKAGM3-G',   'Color',                              'Black'),
        ('R-CG380-BKAGM3-G',   'Dimensions',                         '437 x 235 x 451 mm'),
        ('R-CG380-BKAGM3-G',   'Weight',                             '4.7 kg'),
        ('DRX70-MESH-BK',      'Drive Bays',                         '2 x 2.5", 2 x 3.5"'),
        ('DRX70-MESH-BK',      'Expansion Slots',                    '7'),
        ('DRX70-MESH-BK',      'Power Supply',                       'Not included'),
        ('DRX70-MESH-BK',      'Power Supply Placement',             'Bottom'),
        ('DRX70-MESH-BK',      'Supported Fans',                     'Top: 3x 120 mm, Front: 3x 120 mm, Rear: 1x 120 mm'),
        ('DRX70-MESH-BK',      'Supported Liquid Cooling Radiators', 'Top: 1x 360 mm, Front: 1x 360 mm'),
        ('DRX70-MESH-BK',      'Ports',                              '1 x USB 3.0, 2 x USB 2.0, 1 x Audio'),
        ('DRX70-MESH-BK',      'Included Fans',                      'Front: 3x 120 mm ARGB, Rear: 1x 120 mm ARGB'),
        ('DRX70-MESH-BK',      'Panels',                             'Front: Mesh, Side: Tempered Glass'),
        ('DRX70-MESH-BK',      'Extras',                             'ARGB LED'),
        ('DRX70-MESH-BK',      'Color',                              'Black'),
        ('DRX70-MESH-BK',      'Dimensions',                         '462 x 230 x 396 mm'),
        ('DRX70-MESH-BK',      'Weight',                             '5.2 kg'),
        ('PGW-CH-KOL-110',     'Drive Bays',                         '2 x 2.5", 2 x 3.5"'),
        ('PGW-CH-KOL-110',     'Expansion Slots',                    '7'),
        ('PGW-CH-KOL-110',     'Power Supply',                       'Not included'),
        ('PGW-CH-KOL-110',     'Power Supply Placement',             'Bottom'),
        ('PGW-CH-KOL-110',     'Supported Fans',                     'Top: 2x 120 mm, Front: 3x 120 mm, Rear: 1x 120 mm'),
        ('PGW-CH-KOL-110',     'Supported Liquid Cooling Radiators', 'Top: 1x 240 mm, Front: 1x 360 mm'),
        ('PGW-CH-KOL-110',     'Ports',                              '1 x USB 3.0, 2 x USB 2.0, 1 x Audio'),
        ('PGW-CH-KOL-110',     'Included Fans',                      'Front: 3x 120 mm ARGB, Rear: 1x 120 mm ARGB'),
        ('PGW-CH-KOL-110',     'Panels',                             'Front: Mesh, Side: Tempered Glass'),
        ('PGW-CH-KOL-110',     'Extras',                             'ARGB LED'),
        ('PGW-CH-KOL-110',     'Color',                              'Black'),
        ('PGW-CH-KOL-110',     'Dimensions',                         '390 x 200 x 449 mm'),
        ('PGW-CH-KOL-110',     'Weight',                             '4.8 kg')
) AS s(ProductKey, FieldName, FieldValue)
    ON s.ProductKey = p.ProductKey;

-- =====================================================================
-- CPU Cooler
-- =====================================================================
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
        ('NH-D15',              'Cooling Type', 'Air'),
        ('NH-D15',              'Connectors',   '4 Pin PWM'),
        ('NH-D15',              'Voltage',      '12 V'),
        ('NH-D15',              'Fan Speed',    '1500 RPM'),
        ('NH-D15',              'Fans',         '2 x 140 mm'),
        ('NH-D15.CH.BK',        'Cooling Type', 'Air'),
        ('NH-D15.CH.BK',        'Connectors',   '4 Pin PWM'),
        ('NH-D15.CH.BK',        'Voltage',      '12 V'),
        ('NH-D15.CH.BK',        'Fan Speed',    '1500 RPM'),
        ('NH-D15.CH.BK',        'Fans',         '2 x 140 mm'),
        ('R-AK400-BKADMN-G',    'Cooling Type', 'Air'),
        ('R-AK400-BKADMN-G',    'Connectors',   '4 Pin PWM'),
        ('R-AK400-BKADMN-G',    'Voltage',      '12 V'),
        ('R-AK400-BKADMN-G',    'Fan Speed',    '1850 RPM'),
        ('R-AK400-BKADMN-G',    'Fans',         '1 x 120 mm'),
        ('NH-D15S',             'Cooling Type', 'Air'),
        ('NH-D15S',             'Connectors',   '4 Pin PWM'),
        ('NH-D15S',             'Voltage',      '12 V'),
        ('NH-D15S',             'Fan Speed',    '1500 RPM'),
        ('NH-D15S',             'Fans',         '1 x 140 mm'),
        ('NH-U12S',             'Cooling Type', 'Air'),
        ('NH-U12S',             'Connectors',   '4 Pin PWM'),
        ('NH-U12S',             'Voltage',      '12 V'),
        ('NH-U12S',             'Fan Speed',    '1500 RPM'),
        ('NH-U12S',             'Fans',         '1 x 120 mm'),
        ('NH-U12A',             'Cooling Type', 'Air'),
        ('NH-U12A',             'Connectors',   '4 Pin PWM'),
        ('NH-U12A',             'Voltage',      '12 V'),
        ('NH-U12A',             'Fan Speed',    '2000 RPM'),
        ('NH-U12A',             'Fans',         '2 x 120 mm'),
        ('R-AG300-BKNNMN-G',    'Cooling Type', 'Air'),
        ('R-AG300-BKNNMN-G',    'Connectors',   '4 Pin PWM'),
        ('R-AG300-BKNNMN-G',    'Voltage',      '12 V'),
        ('R-AG300-BKNNMN-G',    'Fan Speed',    '2050 RPM'),
        ('R-AG300-BKNNMN-G',    'Fans',         '1 x 92 mm'),
        ('R-AK620-BKNNMT-G',    'Cooling Type', 'Air'),
        ('R-AK620-BKNNMT-G',    'Connectors',   '4 Pin PWM'),
        ('R-AK620-BKNNMT-G',    'Voltage',      '12 V'),
        ('R-AK620-BKNNMT-G',    'Fan Speed',    '1850 RPM'),
        ('R-AK620-BKNNMT-G',    'Fans',         '2 x 120 mm'),
        ('R-AK620-BKNNMT-G-1',  'Cooling Type', 'Air'),
        ('R-AK620-BKNNMT-G-1',  'Connectors',   '4 Pin PWM'),
        ('R-AK620-BKNNMT-G-1',  'Voltage',      '12 V'),
        ('R-AK620-BKNNMT-G-1',  'Fan Speed',    '1850 RPM'),
        ('R-AK620-BKNNMT-G-1',  'Fans',         '2 x 120 mm'),
        ('ACFRE00123A',         'Cooling Type', 'Air'),
        ('ACFRE00123A',         'Connectors',   '4 Pin PWM'),
        ('ACFRE00123A',         'Voltage',      '12 V'),
        ('ACFRE00123A',         'Fan Speed',    '1800 RPM'),
        ('ACFRE00123A',         'Fans',         '2 x 120 mm'),
        ('R-AK400-BKNNMN-G-1',  'Cooling Type', 'Air'),
        ('R-AK400-BKNNMN-G-1',  'Connectors',   '4 Pin PWM'),
        ('R-AK400-BKNNMN-G-1',  'Voltage',      '12 V'),
        ('R-AK400-BKNNMN-G-1',  'Fan Speed',    '1850 RPM'),
        ('R-AK400-BKNNMN-G-1',  'Fans',         '1 x 120 mm'),
        ('ACFRE00124A',         'Cooling Type', 'Air'),
        ('ACFRE00124A',         'Connectors',   '4 Pin PWM'),
        ('ACFRE00124A',         'Voltage',      '12 V'),
        ('ACFRE00124A',         'Fan Speed',    '1800 RPM'),
        ('ACFRE00124A',         'Fans',         '2 x 120 mm A-RGB')
) AS s(ProductKey, FieldName, FieldValue)
    ON s.ProductKey = p.ProductKey;

-- =====================================================================
-- Upsert Products (all categories)
-- =====================================================================
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
        ROW_NUMBER() OVER (PARTITION BY Manufacturer, Model, Image ORDER BY ProductKey) AS RowNumber
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

-- =====================================================================
-- Re-seed ProductFieldValues for every (Product, Category) pair touched
-- =====================================================================
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
WHERE p.IsRemoved = 0
    AND c.Name IN ('CPU', 'Motherboard', 'RAM', 'GPU', 'Storage', 'PSU', 'Case', 'CPU Cooler')
GROUP BY c.Name
ORDER BY c.Name;
