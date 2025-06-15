
CREATE PROCEDURE usp_UpdateInvoiceLineItem
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