-- =========================================================================
-- AZURE SQL QUERY EDITOR SCRIPT FOR: Ecommerce_OrderDb
-- Run this directly inside Azure Portal -> Ecommerce_OrderDb -> Query editor
-- =========================================================================

-- 1. Create Orders Table
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
END
GO

-- 2. Create OrderItems Table
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
END
GO

-- 3. Seed Sample Data
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
END
GO

