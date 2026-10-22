-- =========================================================================
-- PRODUCTION-READY SQL SCRIPTS FOR ECOMMERCE MICROSERVICES ARCHITECTURE
-- Databases: Ecommerce_OrderDb & Ecommerce_InventoryDb
-- Database Engine: Microsoft SQL Server 2019/2022 / Azure SQL Database
-- =========================================================================

-- =========================================================================
-- 1. ORDER MICROSERVICE DATABASE (Ecommerce_OrderDb)
-- =========================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Ecommerce_OrderDb')
BEGIN
    CREATE DATABASE [Ecommerce_OrderDb];
    PRINT 'Database [Ecommerce_OrderDb] created successfully.';
END
GO

USE [Ecommerce_OrderDb];
GO

-- Create Orders Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE [dbo].[Orders] (
        [Id]            UNIQUEIDENTIFIER NOT NULL,
        [OrderNumber]   NVARCHAR(50)     NOT NULL,
        [CustomerId]    NVARCHAR(100)    NOT NULL,
        [TotalAmount]   DECIMAL(18, 2)   NOT NULL,
        [Status]        NVARCHAR(30)     NOT NULL,
        [CreatedDate]   DATETIME2(7)     NOT NULL,
        [FailureReason] NVARCHAR(500)    NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Orders_OrderNumber] 
        ON [dbo].[Orders]([OrderNumber] ASC);

    CREATE NONCLUSTERED INDEX [IX_Orders_CustomerId] 
        ON [dbo].[Orders]([CustomerId] ASC);

    CREATE NONCLUSTERED INDEX [IX_Orders_CreatedDate] 
        ON [dbo].[Orders]([CreatedDate] DESC);

    PRINT 'Table [Orders] created successfully.';
END
GO

-- Create OrderItems Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE [dbo].[OrderItems] (
        [Id]          UNIQUEIDENTIFIER NOT NULL,
        [OrderId]     UNIQUEIDENTIFIER NOT NULL,
        [ProductId]   UNIQUEIDENTIFIER NOT NULL,
        [ProductName] NVARCHAR(200)    NOT NULL,
        [Quantity]    INT              NOT NULL,
        [Price]       DECIMAL(18, 2)   NOT NULL,
        CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) 
            REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderId] 
        ON [dbo].[OrderItems]([OrderId] ASC);

    CREATE NONCLUSTERED INDEX [IX_OrderItems_ProductId] 
        ON [dbo].[OrderItems]([ProductId] ASC);

    PRINT 'Table [OrderItems] created successfully.';
END
GO

-- Seed Order Database Sample Data
IF NOT EXISTS (SELECT 1 FROM [dbo].[Orders])
BEGIN
    DECLARE @OrderId1 UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
    DECLARE @OrderId2 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
    DECLARE @OrderId3 UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
    DECLARE @OrderId4 UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';

    INSERT INTO [dbo].[Orders] ([Id], [OrderNumber], [CustomerId], [TotalAmount], [Status], [CreatedDate], [FailureReason])
    VALUES 
    (@OrderId1, 'ORD-20261001-101', 'customer@ecommerce.com', 1899.99, 'Confirmed', DATEADD(DAY, -5, SYSUTCDATETIME()), NULL),
    (@OrderId2, 'ORD-20261002-102', 'customer@ecommerce.com', 699.98,  'Confirmed', DATEADD(DAY, -3, SYSUTCDATETIME()), NULL),
    (@OrderId3, 'ORD-20261003-103', 'customer@ecommerce.com', 399.99,  'Pending',   DATEADD(DAY, -1, SYSUTCDATETIME()), NULL),
    (@OrderId4, 'ORD-20261004-104', 'customer@ecommerce.com', 2499.00, 'Failed',    SYSUTCDATETIME(), 'Insufficient inventory stock: Requested 1 but 0 available.');

    INSERT INTO [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [ProductName], [Quantity], [Price])
    VALUES
    (NEWID(), @OrderId1, 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Dell XPS 15 Laptop', 1, 1899.99),
    (NEWID(), @OrderId2, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Sony WH-1000XM5 Headphones', 2, 349.99),
    (NEWID(), @OrderId3, 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'LG 27GP850 UltraGear Gaming Monitor', 1, 399.99),
    (NEWID(), @OrderId4, 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Apple MacBook Pro M3', 1, 2499.00);

    PRINT 'Initial sample data for [Ecommerce_OrderDb] seeded successfully.';
END
GO


-- =========================================================================
-- 2. INVENTORY MICROSERVICE DATABASE (Ecommerce_InventoryDb)
-- =========================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Ecommerce_InventoryDb')
BEGIN
    CREATE DATABASE [Ecommerce_InventoryDb];
    PRINT 'Database [Ecommerce_InventoryDb] created successfully.';
END
GO

USE [Ecommerce_InventoryDb];
GO

-- Create Products Table
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

    PRINT 'Table [Products] created successfully.';
END
GO

-- Seed Inventory Database Products
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

    PRINT 'Initial product catalog for [Ecommerce_InventoryDb] seeded successfully.';
END
GO

