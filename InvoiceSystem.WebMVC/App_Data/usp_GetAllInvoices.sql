
CREATE PROCEDURE usp_GetAllInvoices
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