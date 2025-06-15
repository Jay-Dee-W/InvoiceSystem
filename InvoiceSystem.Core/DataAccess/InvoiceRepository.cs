using InvoiceSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InvoiceSystem.Core.DataAccess
{
    public class InvoiceRepository
    {
        private readonly string _connectionString;

        public InvoiceRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public int InsertInvoice(Invoice invoice)
        {
            int newInvoiceId = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("usp_InsertInvoice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 50).Value = invoice.InvoiceNumber;
                        command.Parameters.Add("@SupplierName", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(invoice.SupplierName) ? (object)DBNull.Value : invoice.SupplierName;
                        command.Parameters.Add("@SupplierAddress", SqlDbType.NVarChar, 255).Value = string.IsNullOrEmpty(invoice.SupplierAddress) ? (object)DBNull.Value : invoice.SupplierAddress;
                        command.Parameters.Add("@SupplierContact", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(invoice.SupplierContact) ? (object)DBNull.Value : invoice.SupplierContact;
                        command.Parameters.Add("@CapturedIP", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(invoice.CapturedIP) ? (object)DBNull.Value : invoice.CapturedIP;
                        command.Parameters.Add("@CapturedMachineName", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(invoice.CapturedMachineName) ? (object)DBNull.Value : invoice.CapturedMachineName;
                        command.Parameters.Add("@InvoiceTotal", SqlDbType.Decimal).Value = invoice.InvoiceTotal;

                        SqlParameter outputIdParam = new SqlParameter("@NewInvoiceId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputIdParam);

                        command.ExecuteNonQuery();
                        newInvoiceId = (int)outputIdParam.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting invoice: {ex.Message}");
                throw;
            }
            return newInvoiceId;
        }

        public void InsertInvoiceLineItem(InvoiceLineItem item)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("usp_InsertInvoiceLineItem", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@InvoiceId", SqlDbType.Int).Value = item.InvoiceId;
                        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = item.Name;
                        command.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value = string.IsNullOrEmpty(item.Description) ? (object)DBNull.Value : item.Description;
                        command.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                        command.Parameters.Add("@PriceExVAT", SqlDbType.Decimal).Value = item.PriceExVAT;
                        command.Parameters.Add("@VATAmount", SqlDbType.Decimal).Value = item.VATAmount;
                        command.Parameters.Add("@Remarks", SqlDbType.NVarChar, 255).Value = string.IsNullOrEmpty(item.Remarks) ? (object)DBNull.Value : item.Remarks;

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting invoice line item: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Invoice> GetAllInvoices()
        {
            List<Invoice> invoices = new List<Invoice>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("usp_GetAllInvoices", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                invoices.Add(new Invoice
                                {
                                    InvoiceId = reader.GetInt32(reader.GetOrdinal("InvoiceId")),
                                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                                    SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName")) ? null : reader.GetString(reader.GetOrdinal("SupplierName")),
                                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                    InvoiceTotal = reader.GetDecimal(reader.GetOrdinal("InvoiceTotal"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllInvoices error: {ex.Message}");
                throw;
            }

            return invoices;
        }

        public Invoice GetInvoiceDetails(int invoiceId)
        {
            Invoice invoice = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("usp_GetInvoiceDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@InvoiceId", invoiceId);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                invoice = new Invoice
                                {
                                    InvoiceId = reader.GetInt32(reader.GetOrdinal("InvoiceId")),
                                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                                    SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName")) ? null : reader.GetString(reader.GetOrdinal("SupplierName")),
                                    SupplierAddress = reader.IsDBNull(reader.GetOrdinal("SupplierAddress")) ? null : reader.GetString(reader.GetOrdinal("SupplierAddress")),
                                    SupplierContact = reader.IsDBNull(reader.GetOrdinal("SupplierContact")) ? null : reader.GetString(reader.GetOrdinal("SupplierContact")),
                                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                    CapturedIP = reader.IsDBNull(reader.GetOrdinal("CapturedIP")) ? null : reader.GetString(reader.GetOrdinal("CapturedIP")),
                                    CapturedMachineName = reader.IsDBNull(reader.GetOrdinal("CapturedMachineName")) ? null : reader.GetString(reader.GetOrdinal("CapturedMachineName")),
                                    InvoiceTotal = reader.GetDecimal(reader.GetOrdinal("InvoiceTotal")),
                                    LineItems = new List<InvoiceLineItem>()
                                };
                            }

                            if (reader.NextResult() && invoice != null)
                            {
                                while (reader.Read())
                                {
                                    invoice.LineItems.Add(new InvoiceLineItem
                                    {
                                        LineItemId = reader.GetInt32(reader.GetOrdinal("LineItemId")),
                                        InvoiceId = reader.IsDBNull(reader.GetOrdinal("InvoiceId")) ? 0 : reader.GetInt32(reader.GetOrdinal("InvoiceId")),
                                        Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                        Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity")),
                                        PriceExVAT = reader.GetDecimal(reader.GetOrdinal("PriceExVAT")),
                                        VATAmount = reader.GetDecimal(reader.GetOrdinal("VATAmount")),
                                        PriceInclVAT = reader.GetDecimal(reader.GetOrdinal("PriceInclVAT")),
                                        Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                                        LineTotal = reader.GetDecimal(reader.GetOrdinal("LineTotal"))
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetInvoiceDetails error: {ex.Message}");
                throw;
            }

            return invoice;
        }

        public void UpdateInvoice(Invoice invoice)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("usp_UpdateInvoice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@InvoiceId", SqlDbType.Int).Value = invoice.InvoiceId;
                        command.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 50).Value = invoice.InvoiceNumber;
                        command.Parameters.Add("@SupplierName", SqlDbType.NVarChar, 255).Value = string.IsNullOrEmpty(invoice.SupplierName) ? (object)DBNull.Value : invoice.SupplierName;
                        command.Parameters.Add("@SupplierAddress", SqlDbType.NVarChar, 500).Value = string.IsNullOrEmpty(invoice.SupplierAddress) ? (object)DBNull.Value : invoice.SupplierAddress;
                        command.Parameters.Add("@SupplierContact", SqlDbType.NVarChar, 255).Value = string.IsNullOrEmpty(invoice.SupplierContact) ? (object)DBNull.Value : invoice.SupplierContact;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateInvoice error: {ex.Message}");
                throw;
            }
        }

        public void DeleteInvoiceLineItems(int invoiceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("usp_DeleteInvoiceLineItems", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@InvoiceId", invoiceId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteInvoiceLineItems error: {ex.Message}");
                throw;
            }
        }

        public void DeleteInvoice(int invoiceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("usp_DeleteInvoice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@InvoiceId", invoiceId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteInvoice error: {ex.Message}");
                throw;
            }
        }

        public void RecalculateInvoiceTotal(int invoiceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("usp_RecalculateInvoiceTotal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@InvoiceId", invoiceId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RecalculateInvoiceTotal error: {ex.Message}");
                throw;
            }
        }
    }
}
