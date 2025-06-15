CREATE PROCEDURE usp_InsertInvoice
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