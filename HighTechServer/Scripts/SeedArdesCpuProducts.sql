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
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Socket Type', 'AM4'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'TDP (W)', '65'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Graphic Core', 'Radeon Vega 8 Graphics'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'TDP (W)', '65'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Graphic Core', 'None'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Socket Type', 'AM4'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'TDP (W)', '65'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'TDP (W)', '65'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'TDP (W)', '65'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Graphic Core', 'None'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'TDP (W)', '58'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Graphic Core', 'None'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'TDP (W)', '58'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Graphic Core', 'None'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'TDP (W)', '58'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Graphic Core', 'None'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'TDP (W)', '58'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'TDP (W)', '65'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'TDP (W)', '35'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Graphic Core', 'Radeon Graphics'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Socket Type', 'AM4'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'TDP (W)', '65'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Graphic Core', 'None'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'TDP (W)', '58'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Graphic Core', 'None'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Socket Type', 'LGA1200'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'TDP (W)', '65'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Graphic Core', 'Intel UHD Graphics 630'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'TDP (W)', '65'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Graphic Core', 'None'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'TDP (W)', '60'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Graphic Core', 'Intel UHD Graphics 730'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Socket Type', 'AM5'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'TDP (W)', '65'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Graphic Core', 'None'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'TDP (W)', '65'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Graphic Core', 'None'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Socket Type', 'LGA1700'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'TDP (W)', '65'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Graphic Core', 'None'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Socket Type', 'AM4'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'TDP (W)', '65'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Graphic Core', 'Radeon Graphics'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Socket Type', 'AM5'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'TDP (W)', '65'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Graphic Core', 'None'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Socket Type', 'AM5'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'TDP (W)', '65'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Graphic Core', 'None'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Socket Type', 'AM4'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'TDP (W)', '65'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Graphic Core', 'None'),
-- Celeron G5905
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Base Clock', '3.5'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Turbo Boost Clock', '3.5'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Physical Cores', '2'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Logical Cores', '2'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Cache Memory', 'L2: 1 MB, L3: 4 MB'),
('CPU', 'CM8070104292115', 'Intel', 'Celeron G5905 (3.5GHz) TRAY', 36, 40.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-celeron-g5905-3-5ghz-tray-348332.jpg', 'Included Cooler', 'None'),
-- Ryzen 3 3200G
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Base Clock', '3.6'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Turbo Boost Clock', '4.0'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Physical Cores', '4'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Logical Cores', '4'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Cache Memory', 'L2: 2 MB, L3: 4 MB'),
('CPU', 'YD3200C5M4MFH', 'AMD', 'Ryzen 3 3200G (3.6GHz) TRAY', 36, 60.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-3-4c-4t-3200g-4-0ghz-6mb-65w-329684.jpg', 'Included Cooler', 'None'),
-- Ryzen 3 4100
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Base Clock', '3.8'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Turbo Boost Clock', '4.0'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Physical Cores', '4'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Logical Cores', '8'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Cache Memory', 'L2: 2 MB, L3: 4 MB'),
('CPU', '100-100000510BOX', 'AMD', 'Ryzen 3 4100 (3.8GHz)', 36, 62.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-3-4100-3-8ghz-380576.jpg', 'Included Cooler', 'Wraith Stealth'),
-- Ryzen 5 5500 TRAY (100-000000457)
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Base Clock', '3.6'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Turbo Boost Clock', '4.2'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Physical Cores', '6'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Logical Cores', '12'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Cache Memory', 'L2: 3 MB, L3: 16 MB'),
('CPU', '100-000000457', 'AMD', 'Ryzen 5 5500 (3.6GHz) TRAY', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-tray-394281.jpg', 'Included Cooler', 'None'),
-- Ryzen 5 4500
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Base Clock', '3.6'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Turbo Boost Clock', '4.1'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Physical Cores', '6'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Logical Cores', '12'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Cache Memory', 'L2: 3 MB, L3: 8 MB'),
('CPU', '100-100000644BOX', 'AMD', 'Ryzen 5 4500 (3.6GHz)', 36, 77.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-4500-3-6ghz-380577.jpg', 'Included Cooler', 'Wraith Stealth'),
-- Ryzen 5 5500 MPK (100-100000457MPK)
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Base Clock', '3.6'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Turbo Boost Clock', '4.2'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Physical Cores', '6'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Logical Cores', '12'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Cache Memory', 'L2: 3 MB, L3: 16 MB'),
('CPU', '100-100000457MPK', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 79.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5500-3-6ghz-377467.jpg', 'Included Cooler', 'None'),
-- Core i3-12100F TRAY (CM8071504651013)
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Base Clock', '3.3'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Turbo Boost Clock', '4.3'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Physical Cores', '4'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Logical Cores', '8'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'CM8071504651013', 'Intel', 'Core i3-12100F (3.3GHz) TRAY', 36, 90.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-tray-367842.jpg', 'Included Cooler', 'None'),
-- Core i3-12100F BOX (BX8071512100F)
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Base Clock', '3.3'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Turbo Boost Clock', '4.3'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Physical Cores', '4'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Logical Cores', '8'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'BX8071512100F', 'Intel', 'Core i3-12100F (3.3GHz)', 36, 93.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100f-3-3ghz-371174.jpg', 'Included Cooler', 'Intel Laminar RM1'),
-- Core i3-14100F (CM8071505092207)
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Base Clock', '3.5'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Turbo Boost Clock', '4.7'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Physical Cores', '4'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Logical Cores', '8'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'CM8071505092207', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-14100f-3-5ghz-729341.jpg', 'Included Cooler', 'None'),
-- Core i3-13100F (BX8071513100F)
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Base Clock', '3.4'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Turbo Boost Clock', '4.5'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Physical Cores', '4'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Logical Cores', '8'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'BX8071513100F', 'Intel', 'Core i3-13100F (3.4GHz)', 36, 102.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-13100f-3-4ghz-429124.jpg', 'Included Cooler', 'Intel Laminar RM1'),
-- Ryzen 5 5500 BOX (100-100000457BOX)
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Base Clock', '3.6'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Turbo Boost Clock', '4.2'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Physical Cores', '6'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Logical Cores', '12'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Cache Memory', 'L2: 3 MB, L3: 16 MB'),
('CPU', '100-100000457BOX', 'AMD', 'Ryzen 5 5500 (3.6GHz)', 36, 105.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5500-am4-socket-6-cores-12-t-377001.jpg', 'Included Cooler', 'Wraith Stealth'),
-- Ryzen 7 PRO 4750GE Bulk
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Base Clock', '3.1'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Turbo Boost Clock', '4.3'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Physical Cores', '8'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Logical Cores', '16'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Cache Memory', 'L2: 4 MB, L3: 8 MB'),
('CPU', '100-100000152MPK', 'AMD', 'Ryzen 7 PRO 4750GE (3.60GHz) Bulk', 36, 112.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-pro-4750ge-3-60ghz-bulk-749005.jpg', 'Included Cooler', 'None'),
-- Ryzen 5 5600 TRAY (100-000000927)
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Base Clock', '3.5'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Physical Cores', '6'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Logical Cores', '12'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Cache Memory', 'L2: 3 MB, L3: 32 MB'),
('CPU', '100-000000927', 'AMD', 'Ryzen 5 5600 (3.5GHz) TRAY', 36, 115.00, 10.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5-100-000000927-394282.jpg', 'Included Cooler', 'None'),
-- Core i3-14100F BOX (BX8071514100F)
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Base Clock', '3.5'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Turbo Boost Clock', '4.7'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Physical Cores', '4'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Logical Cores', '8'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'BX8071514100F', 'Intel', 'Core i3-14100F (3.5GHz)', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-cpu-desktop-core-i3-14100-up-to-4-70-ghz-12m-527021.jpg', 'Included Cooler', 'Intel Laminar RM1'),
-- Core i3-10105 TRAY
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Base Clock', '3.7'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Physical Cores', '4'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Logical Cores', '8'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Cache Memory', 'L2: 1 MB, L3: 6 MB'),
('CPU', 'CM8070104291321', 'Intel', 'Core i3-10105 (3.7GHz) TRAY', 36, 122.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-10105-3-7ghz-tray-342009.jpg', 'Included Cooler', 'None'),
-- Ryzen 5 5600 MPK (100-100000927MPK)
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Base Clock', '3.5'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Physical Cores', '6'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Logical Cores', '12'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Cache Memory', 'L2: 3 MB, L3: 32 MB'),
('CPU', '100-100000927MPK', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 124.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600-3-5ghz-419319.jpg', 'Included Cooler', 'None'),
-- Core i3-12100 TRAY
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Base Clock', '3.3'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Turbo Boost Clock', '4.3'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Physical Cores', '4'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Logical Cores', '8'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Cache Memory', 'L2: 5 MB, L3: 12 MB'),
('CPU', 'CM8071504651012', 'Intel', 'Core i3-12100 (3.3GHz) TRAY', 36, 126.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i3-12100-3-3ghz-tray-431148.jpg', 'Included Cooler', 'None'),
-- Ryzen 5 7500F TRAY
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Base Clock', '3.7'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Turbo Boost Clock', '5.0'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Physical Cores', '6'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Logical Cores', '12'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Cache Memory', 'L2: 6 MB, L3: 32 MB'),
('CPU', '100-000000597', 'AMD', 'Ryzen 5 7500F (3.7GHz) TRAY', 36, 135.00, 20.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-7500f-6-core-3-7-ghz-5-0-ghz-486410.jpg', 'Included Cooler', 'None'),
-- Ryzen 5 5600 BOX (100-100000927BOX)
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Base Clock', '3.5'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Physical Cores', '6'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Logical Cores', '12'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Cache Memory', 'L2: 3 MB, L3: 32 MB'),
('CPU', '100-100000927BOX', 'AMD', 'Ryzen 5 5600 (3.5GHz)', 36, 146.00, 0.00, 10, 'https://ardes.bg/uploads/original/protsesor-amd-ryzen-5-5600-am4-socket-6-cores-12-t-377000.jpg', 'Included Cooler', 'Wraith Stealth'),
-- Core i5-12400F TRAY
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Base Clock', '2.5'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Turbo Boost Clock', '4.4'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Physical Cores', '6'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Logical Cores', '12'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Cache Memory', 'L2: 7.5 MB, L3: 18 MB'),
('CPU', 'CM8071504555318', 'Intel', 'Core i5-12400F (2.5GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/intel-core-i5-12400f-2-5ghz-tray-371170.jpg', 'Included Cooler', 'None'),
-- Ryzen 5 5600GT TRAY
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Base Clock', '3.6'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Turbo Boost Clock', '4.6'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Physical Cores', '6'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Logical Cores', '12'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Cache Memory', 'L2: 3 MB, L3: 16 MB'),
('CPU', '100-100001488', 'AMD', 'Ryzen 5 5600GT (3.6GHz) TRAY', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-5-5600gt-3-6ghz-634875.jpg', 'Included Cooler', 'None'),
-- Ryzen 5 8400F Bulk
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Base Clock', '4.2'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Turbo Boost Clock', '4.7'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Physical Cores', '6'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Logical Cores', '12'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Cache Memory', 'L2: 6 MB, L3: 16 MB'),
('CPU', '100-100001591MPK', 'AMD', 'Ryzen 5 8400F (4.2GHz) Bulk', 36, 149.00, 0.00, 10, 'https://ardes.bg/uploads/original/-634876.jpg', 'Included Cooler', 'None'),
-- Ryzen 7 8700F Bulk
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Base Clock', '4.1'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Turbo Boost Clock', '5.0'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Physical Cores', '8'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Logical Cores', '16'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Cache Memory', 'L2: 8 MB, L3: 24 MB'),
('CPU', '100-100001590MPK', 'AMD', 'Ryzen 7 8700F (4.1GHz) Bulk', 36, 150.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-ryzen-7-8700f-4-1ghz-644979.jpg', 'Included Cooler', 'None'),
-- Ryzen 5 5600X TRAY
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Base Clock', '3.7'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Turbo Boost Clock', '4.6'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Physical Cores', '6'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Logical Cores', '12'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Cache Memory', 'L2: 3 MB, L3: 32 MB'),
('CPU', '100-000000065', 'AMD', 'Ryzen 5 5600X (3.7GHz) TRAY', 24, 151.00, 0.00, 10, 'https://ardes.bg/uploads/original/amd-cpu-desktop-ryzen-5-6c-12t-5600x-3-7-4-6ghz-ma-323838.jpg', 'Included Cooler', 'None');

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
WHERE c.Name = 'CPU'
    AND p.IsRemoved = 0
GROUP BY c.Name;
