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
WHERE c.Name = 'Storage'
    AND p.IsRemoved = 0
GROUP BY c.Name;
