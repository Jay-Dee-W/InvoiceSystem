CREATE PROCEDURE usp_InsertInvoiceLineItem
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