/****** Object:  Table [dbo].[InvoiceLineItems]    Script Date: 2025/06/07 14:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceLineItems]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InvoiceLineItems](
	[LineItemId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NULL,
	[Name] [nvarchar](100) NULL,
	[Description] [nvarchar](255) NULL,
	[Quantity] [int] NULL,
	[PriceExVAT] [decimal](18, 2) NULL,
	[VATAmount] [decimal](18, 2) NULL,
	[PriceInclVAT]  AS ([PriceExVAT]+[VATAmount]) PERSISTED,
	[Remarks] [nvarchar](255) NULL,
	[LineTotal]  AS ([Quantity]*([PriceExVAT]+[VATAmount])) PERSISTED,
PRIMARY KEY CLUSTERED 
(
	[LineItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 2025/06/07 14:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Invoices](
	[InvoiceId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [nvarchar](50) NOT NULL,
	[SupplierName] [nvarchar](100) NULL,
	[SupplierAddress] [nvarchar](255) NULL,
	[SupplierContact] [nvarchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[CapturedIP] [nvarchar](50) NULL,
	[CapturedMachineName] [nvarchar](100) NULL,
	[InvoiceTotal] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Invoices__Create__36B12243]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (getdate()) FOR [CreatedDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Invoices__Invoic__3A81B327]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ((0)) FOR [InvoiceTotal]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__InvoiceLi__Invoi__398D8EEE]') AND parent_object_id = OBJECT_ID(N'[dbo].[InvoiceLineItems]'))
ALTER TABLE [dbo].[InvoiceLineItems]  WITH CHECK ADD FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoices] ([InvoiceId])
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllInvoices]    Script Date: 2025/06/07 14:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetAllInvoices]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_GetAllInvoices] AS' 
END
GO

ALTER PROCEDURE [dbo].[usp_GetAllInvoices]
AS
BEGIN
    SET NOCOUNT ON; -- Prevents the count of rows affected from being returned.

    SELECT
        InvoiceId,
        InvoiceNumber,
        SupplierName,
        CreatedDate,
        InvoiceTotal
    FROM
        Invoices
    ORDER BY
        CreatedDate DESC; -- Order by the most recent invoices first
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetInvoiceDetails]    Script Date: 2025/06/07 14:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetInvoiceDetails]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_GetInvoiceDetails] AS' 
END
GO
ALTER PROCEDURE [dbo].[usp_GetInvoiceDetails]
    @InvoiceId INT -- Input parameter: the ID of the invoice to retrieve
AS
BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Invoice Header Details
    SELECT
        InvoiceId,
        InvoiceNumber,
        SupplierName,
        SupplierAddress,
        SupplierContact,
        CreatedDate,        
        CapturedIP,
        CapturedMachineName,
        InvoiceTotal
    FROM
        Invoices
    WHERE
        InvoiceId = @InvoiceId;

    -- Result Set 2: Invoice Line Items for the specified InvoiceId
    SELECT
        LineItemId,
        InvoiceId,
        Name,
        Description,
        Quantity,
        PriceExVAT,
        VATAmount,
        PriceInclVAT,   
        Remarks,
        LineTotal       
    FROM
        InvoiceLineItems
    WHERE
        InvoiceId = @InvoiceId
    ORDER BY
        LineItemId ASC; -- for consistent display order
END

GO
/****** Object:  StoredProcedure [dbo].[usp_InsertInvoice]    Script Date: 2025/06/07 14:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_InsertInvoice]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_InsertInvoice] AS' 
END
GO
ALTER PROCEDURE [dbo].[usp_InsertInvoice]
    @InvoiceNumber NVARCHAR(50),
    @SupplierName NVARCHAR(100),
    @SupplierAddress NVARCHAR(255),
    @SupplierContact NVARCHAR(100),
    @CapturedIP NVARCHAR(50),
    @CapturedMachineName NVARCHAR(100),
    @InvoiceTotal DECIMAL(18,2), -- Parameter for the calculated invoice total
    @NewInvoiceId INT OUTPUT -- This parameter will hold the new InvoiceId
AS
BEGIN
    SET NOCOUNT ON; -- Prevents the count of rows affected from being returned, which can be cleaner.

    INSERT INTO Invoices (
        InvoiceNumber,
        SupplierName,
        SupplierAddress,
        SupplierContact,
        CapturedIP,
        CapturedMachineName,
        InvoiceTotal -- Match the column name
    )
    VALUES (
        @InvoiceNumber,
        @SupplierName,
        @SupplierAddress,
        @SupplierContact,
        @CapturedIP,
        @CapturedMachineName,
        @InvoiceTotal
    );

    -- Get the InvoiceId of the newly inserted row
    SET @NewInvoiceId = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[usp_InsertInvoiceLineItem]    Script Date: 2025/06/07 14:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_InsertInvoiceLineItem]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_InsertInvoiceLineItem] AS' 
END
GO
ALTER PROCEDURE [dbo].[usp_InsertInvoiceLineItem]
    @InvoiceId INT, -- This links to the Invoice header
    @Name NVARCHAR(100),
    @Description NVARCHAR(255),
    @Quantity INT,
    @PriceExVAT DECIMAL(18,2),
    @VATAmount DECIMAL(18,2),
    @Remarks NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO InvoiceLineItems (
        InvoiceId,
        Name,
        Description,
        Quantity,
        PriceExVAT,
        VATAmount,
        Remarks
        -- Note: PriceInclVAT and LineTotal are computed columns, so they are not included here.
    )
    VALUES (
        @InvoiceId,
        @Name,
        @Description,
        @Quantity,
        @PriceExVAT,
        @VATAmount,
        @Remarks
    );
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RecalculateInvoiceTotal]    Script Date: 2025/06/07 14:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_RecalculateInvoiceTotal]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_RecalculateInvoiceTotal] AS' 
END
GO
ALTER PROCEDURE [dbo].[usp_RecalculateInvoiceTotal]
    @InvoiceId INT -- The ID of the invoice whose total needs to be recalculated
AS
BEGIN
    SET NOCOUNT ON;

    -- Update the InvoiceTotal in the Invoices table
    UPDATE I
    SET I.InvoiceTotal = (
        SELECT SUM(IL.LineTotal)
        FROM InvoiceLineItems IL
        WHERE IL.InvoiceId = I.InvoiceId
    )
    FROM Invoices I
    WHERE I.InvoiceId = @InvoiceId;

    -- Handle case where an invoice might have no line items (total should be 0)
    -- This ensures InvoiceTotal is not NULL if all line items are deleted
    UPDATE Invoices
    SET InvoiceTotal = 0
    WHERE InvoiceId = @InvoiceId AND InvoiceTotal IS NULL;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_UpdateInvoiceLineItem]    Script Date: 2025/06/07 14:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_UpdateInvoiceLineItem]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_UpdateInvoiceLineItem] AS' 
END
GO

ALTER PROCEDURE [dbo].[usp_UpdateInvoiceLineItem]
    @LineItemId INT,
    @Name NVARCHAR(100) = NULL,         -- parameters optional
    @Description NVARCHAR(255) = NULL,
    @Quantity INT = NULL,
    @PriceExVAT DECIMAL(18,2) = NULL,
    @VATAmount DECIMAL(18,2) = NULL,
    @Remarks NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE InvoiceLineItems
    SET
        -- Update only if the new value is provided (not NULL)
        Name = ISNULL(@Name, Name),
        Description = ISNULL(@Description, Description),
        Quantity = ISNULL(@Quantity, Quantity),
        PriceExVAT = ISNULL(@PriceExVAT, PriceExVAT),
        VATAmount = ISNULL(@VATAmount, VATAmount),
        Remarks = ISNULL(@Remarks, Remarks)
    WHERE
        LineItemId = @LineItemId;
END

GO
