CREATE PROCEDURE usp_GetInvoiceDetails
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