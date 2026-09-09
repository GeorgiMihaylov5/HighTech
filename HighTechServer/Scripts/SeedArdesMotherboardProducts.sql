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
        -- MSI A520M-A PRO
        ('911-7C96-001', 'Chipset', 'AMD A520'),
        ('911-7C96-001', 'Memory Slots', '2'),
        ('911-7C96-001', 'Max Supported Memory', 'Up to 64 GB'),
        ('911-7C96-001', 'LAN', '10/100/1000 Mb/s'),
        ('911-7C96-001', 'Bluetooth', 'No'),
        ('911-7C96-001', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('911-7C96-001', 'Ports', 'HDMI, DVI-D, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('911-7C96-001', 'Size', '226 x 173 mm'),
        -- BIOSTAR A520MHP
        ('A520MHP', 'Chipset', 'AMD A520'),
        ('A520MHP', 'Memory Slots', '2'),
        ('A520MHP', 'Max Supported Memory', 'Up to 64 GB'),
        ('A520MHP', 'LAN', '10/100/1000 Mb/s'),
        ('A520MHP', 'Bluetooth', 'No'),
        ('A520MHP', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 4 x SATA 6 Gb/s'),
        ('A520MHP', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('A520MHP', 'Size', '226 x 174 mm'),
        -- ASRock A520M-HVS
        ('90-MXBE60-A0UAYZ', 'Chipset', 'AMD A520'),
        ('90-MXBE60-A0UAYZ', 'Memory Slots', '2'),
        ('90-MXBE60-A0UAYZ', 'Max Supported Memory', 'Up to 64 GB'),
        ('90-MXBE60-A0UAYZ', 'LAN', '10/100/1000 Mb/s'),
        ('90-MXBE60-A0UAYZ', 'Bluetooth', 'No'),
        ('90-MXBE60-A0UAYZ', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 4 x SATA 6 Gb/s'),
        ('90-MXBE60-A0UAYZ', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('90-MXBE60-A0UAYZ', 'Size', '226 x 184 mm'),
        -- ASRock A520M-HDV
        ('90-MXBE50-A0UAYZ', 'Chipset', 'AMD A520'),
        ('90-MXBE50-A0UAYZ', 'Memory Slots', '2'),
        ('90-MXBE50-A0UAYZ', 'Max Supported Memory', 'Up to 64 GB'),
        ('90-MXBE50-A0UAYZ', 'LAN', '10/100/1000 Mb/s'),
        ('90-MXBE50-A0UAYZ', 'Bluetooth', 'No'),
        ('90-MXBE50-A0UAYZ', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('90-MXBE50-A0UAYZ', 'Ports', 'HDMI, DVI-D, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('90-MXBE50-A0UAYZ', 'Size', '226 x 184 mm'),
        -- GIGABYTE A520M K V2
        ('GB-A520M-K-V2', 'Chipset', 'AMD A520'),
        ('GB-A520M-K-V2', 'Memory Slots', '2'),
        ('GB-A520M-K-V2', 'Max Supported Memory', 'Up to 64 GB'),
        ('GB-A520M-K-V2', 'LAN', '10/100/1000 Mb/s'),
        ('GB-A520M-K-V2', 'Bluetooth', 'No'),
        ('GB-A520M-K-V2', 'Interfaces', '1 x PCIe 3.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('GB-A520M-K-V2', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('GB-A520M-K-V2', 'Size', '226 x 174 mm'),
        -- MSI PRO H610M-E DDR4
        ('911-7D48-007', 'Chipset', 'Intel H610'),
        ('911-7D48-007', 'Memory Slots', '2'),
        ('911-7D48-007', 'Max Supported Memory', 'Up to 64 GB'),
        ('911-7D48-007', 'LAN', '10/100/1000 Mb/s'),
        ('911-7D48-007', 'Bluetooth', 'No'),
        ('911-7D48-007', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('911-7D48-007', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('911-7D48-007', 'Size', '227 x 184 mm'),
        -- ASRock H610M-HVS/M.2 R2.0
        ('90-MXBJJ0-A0UAYZ', 'Chipset', 'Intel H610'),
        ('90-MXBJJ0-A0UAYZ', 'Memory Slots', '2'),
        ('90-MXBJJ0-A0UAYZ', 'Max Supported Memory', 'Up to 64 GB'),
        ('90-MXBJJ0-A0UAYZ', 'LAN', '10/100/1000 Mb/s'),
        ('90-MXBJJ0-A0UAYZ', 'Bluetooth', 'No'),
        ('90-MXBJJ0-A0UAYZ', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('90-MXBJJ0-A0UAYZ', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('90-MXBJJ0-A0UAYZ', 'Size', '226 x 174 mm'),
        -- BIOSTAR H610MHC 2.0
        ('H610MHC-2.0', 'Chipset', 'Intel H610'),
        ('H610MHC-2.0', 'Memory Slots', '2'),
        ('H610MHC-2.0', 'Max Supported Memory', 'Up to 64 GB'),
        ('H610MHC-2.0', 'LAN', '10/100/1000 Mb/s'),
        ('H610MHC-2.0', 'Bluetooth', 'No'),
        ('H610MHC-2.0', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 4 x SATA 6 Gb/s'),
        ('H610MHC-2.0', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('H610MHC-2.0', 'Size', '224 x 174 mm'),
        -- ASRock H610M-H2/M.2
        ('H610M-H2/M.2', 'Chipset', 'Intel H610'),
        ('H610M-H2/M.2', 'Memory Slots', '2'),
        ('H610M-H2/M.2', 'Max Supported Memory', 'Up to 64 GB'),
        ('H610M-H2/M.2', 'LAN', '10/100/1000 Mb/s'),
        ('H610M-H2/M.2', 'Bluetooth', 'No'),
        ('H610M-H2/M.2', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('H610M-H2/M.2', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('H610M-H2/M.2', 'Size', '226 x 184 mm'),
        -- ASRock H510M-H2/M.2 SE
        ('90-MXBMQ0-A0UAYZ', 'Chipset', 'Intel H510'),
        ('90-MXBMQ0-A0UAYZ', 'Memory Slots', '2'),
        ('90-MXBMQ0-A0UAYZ', 'Max Supported Memory', 'Up to 64 GB'),
        ('90-MXBMQ0-A0UAYZ', 'LAN', '10/100/1000 Mb/s'),
        ('90-MXBMQ0-A0UAYZ', 'Bluetooth', 'No'),
        ('90-MXBMQ0-A0UAYZ', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('90-MXBMQ0-A0UAYZ', 'Ports', 'HDMI, VGA, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('90-MXBMQ0-A0UAYZ', 'Size', '226 x 184 mm'),
        -- GIGABYTE H610M-K
        ('GB-H610M-K', 'Chipset', 'Intel H610'),
        ('GB-H610M-K', 'Memory Slots', '2'),
        ('GB-H610M-K', 'Max Supported Memory', 'Up to 64 GB'),
        ('GB-H610M-K', 'LAN', '10/100/1000 Mb/s'),
        ('GB-H610M-K', 'Bluetooth', 'No'),
        ('GB-H610M-K', 'Interfaces', '1 x PCIe 4.0 x16, 1 x PCIe 3.0 x1, 1 x M.2, 4 x SATA 6 Gb/s'),
        ('GB-H610M-K', 'Ports', 'HDMI, DisplayPort, PS/2, USB 3.2 Gen 1, USB 2.0, Audio Jacks'),
        ('GB-H610M-K', 'Size', '226 x 175 mm'),
        -- GIGABYTE A520M S2H
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
WHERE c.Name = 'Motherboard'
    AND p.IsRemoved = 0
GROUP BY c.Name;
