ALTER TABLE [Invoice].[InvoiceItems]  WITH NOCHECK ADD  CONSTRAINT [FK_Invoice.InvoiceItems_Invoice.Invoices_InvoiceId] FOREIGN KEY([InvoiceId])
REFERENCES [Invoice].[Invoices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Invoice].[InvoiceItems] CHECK CONSTRAINT [FK_Invoice.InvoiceItems_Invoice.Invoices_InvoiceId]
