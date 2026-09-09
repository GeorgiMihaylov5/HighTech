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
        -- Deepcool CC360 ARGB Black
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
        -- Lian Li LANCOOL 217 Black
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
        -- Fortron S120 Black
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
        -- darkFlash DS900M Black
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
        -- MSI MAG FORGE 112R Black
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
        -- COUGAR CFV235 Vision White
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
        -- NZXT H9 Flow White
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
        -- XIGMATEK Aqua 7 White
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
        -- Lian Li O11 DYNAMIC MINI V2 Flow Black
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
        -- Deepcool CG380 3F Black
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
        -- darkFlash DRX70 Black
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
        -- Kolink Observatory HF Mesh ARGB Black
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
WHERE c.Name = 'Case'
    AND p.IsRemoved = 0
GROUP BY c.Name;
