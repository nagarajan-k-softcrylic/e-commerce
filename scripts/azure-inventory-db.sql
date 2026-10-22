-- =========================================================================
-- AZURE SQL QUERY EDITOR SCRIPT FOR: Ecommerce_InventoryDb
-- Run this directly inside Azure Portal -> Ecommerce_InventoryDb -> Query editor
-- =========================================================================

-- 1. Create Products Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id]                UNIQUEIDENTIFIER NOT NULL,
        [ProductCode]       NVARCHAR(50)     NOT NULL,
        [ProductName]       NVARCHAR(150)    NOT NULL,
        [Description]       NVARCHAR(1000)   NULL,
        [Price]             DECIMAL(18, 2)   NOT NULL,
        [AvailableQuantity] INT              NOT NULL,
        [CreatedDate]       DATETIME2(7)     NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_ProductCode] 
        ON [dbo].[Products]([ProductCode] ASC);

    CREATE NONCLUSTERED INDEX [IX_Products_ProductName] 
        ON [dbo].[Products]([ProductName] ASC);
END
GO

-- 2. Seed Sample Products
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products])
BEGIN
    INSERT INTO [dbo].[Products] ([Id], [ProductCode], [ProductName], [Description], [Price], [AvailableQuantity], [CreatedDate])
    VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'PROD-LAP-001', 'Dell XPS 15 Laptop', '15.6-inch OLED 3.5K display, Intel Core i9, 32GB RAM, 1TB SSD, NVIDIA RTX 4070', 1899.99, 25, SYSUTCDATETIME()),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'PROD-AUD-002', 'Sony WH-1000XM5 Headphones', 'Industry-leading noise cancelling wireless headphones with 30-hour battery life', 349.99, 50, SYSUTCDATETIME()),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'PROD-MON-003', 'LG 27GP850 UltraGear Gaming Monitor', '27-inch QHD (2560 x 1440) Nano IPS, 1ms, 165Hz (O/C 180Hz), G-SYNC Compatible', 399.99, 15, SYSUTCDATETIME()),
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'PROD-LAP-004', 'Apple MacBook Pro M3', '14-inch Liquid Retina XDR display, Apple M3 Pro chip, 18GB Unified Memory, 512GB SSD', 2499.00, 0, SYSUTCDATETIME()),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'PROD-PER-005', 'Logitech MX Master 3S Mouse', 'Performance wireless mouse with 8K DPI sensor, Quiet Clicks, and MagSpeed scrolling', 99.99, 80, SYSUTCDATETIME()),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'PROD-PER-006', 'Keychron K2 Mechanical Keyboard', 'Wireless 75% layout RGB mechanical keyboard with Gateron G Pro Brown switches', 89.99, 45, SYSUTCDATETIME()),
    ('10101010-1010-1010-1010-101010101010', 'PROD-STR-007', 'Samsung 990 Pro 2TB NVMe SSD', 'PCIe 4.0 M.2 2280 internal gaming solid state drive with up to 7450 MB/s speed', 179.99, 30, SYSUTCDATETIME());
END
GO

