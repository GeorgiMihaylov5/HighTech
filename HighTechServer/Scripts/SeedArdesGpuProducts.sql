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
        -- MSI GeForce RTX 5050 8GB SHADOW 2X OC
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
        -- Palit GeForce RTX 5060 Ti 8GB Dual DLSS 4
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
        -- MSI GeForce RTX 5060 8GB SHADOW 2X OC
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
        -- MSI GeForce RTX 5070 12GB SHADOW 2X OC
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
        -- Inno3D GeForce RTX 5060 8GB TWIN X2
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
        -- GIGABYTE Radeon RX 9070 XT GAMING
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
        -- ASRock Radeon RX 9070 XT Steel Legend
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
        -- ASUS Radeon RX 9060 XT Dual
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
        -- Inno3D GeForce RTX 5070 Ti X3
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
        -- Palit GeForce RTX 5070 Infinity 3
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
        -- Inno3D GeForce RTX 5070 Twin X2 OC
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
        -- Inno3D GeForce RTX 5060 Twin X2 OC
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

DECLARE @GpuProductIds TABLE (Id nvarchar(36));

INSERT INTO @GpuProductIds (Id)
SELECT DISTINCT p.Id
FROM dbo.Products p
INNER JOIN dbo.ProductFieldValues pc ON pc.ProductId = p.Id
INNER JOIN dbo.CategoryFields cf ON cf.Id = pc.CategoryFieldId
INNER JOIN dbo.Categories c ON c.Id = cf.CategoryId
WHERE c.Name = 'GPU';

DELETE FROM dbo.ProductFieldValues
WHERE ProductId IN (SELECT Id FROM @GpuProductIds);

DELETE FROM dbo.Products
WHERE Id IN (SELECT Id FROM @GpuProductIds);

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
WHERE c.Name = 'GPU'
    AND p.IsRemoved = 0
GROUP BY c.Name;
