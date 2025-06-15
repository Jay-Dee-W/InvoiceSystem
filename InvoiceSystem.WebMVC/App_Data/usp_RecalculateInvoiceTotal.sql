CREATE PROCEDURE usp_RecalculateInvoiceTotal
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