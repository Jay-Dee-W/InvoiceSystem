# ?? InvoiceSystemSolution

This ASP.NET MVC 4.8 application allows data capturers to record paper-based invoices for Company ABC and provides a secure dashboard for financial staff to view, edit, and correct invoice data.

## ?? Features

### ?? Roles
- **Data Capturers:** Enter invoices with line items, no login required
- **Financial Staff:** Secure login to view all invoices and edit as needed

### ?? Invoice Capture
- Add invoices and dynamically add/remove line items
- Input: Invoice number, supplier details, item names, quantities, prices
- **Automatically calculates:**
  - Line Item Total (based on Quantity × (PriceExVAT + VATAmount))
  - Invoice Total (sum of all Line Item Totals)

### ??? Secure Dashboard
- Authentication required to access dashboard
- View all captured invoices
- Edit any invoice, including its line items
- Changes update totals in real-time and are saved to the database

## ?? Tech Stack

- **ASP.NET MVC 4.8**
- **Bootstrap 5** (for responsive UI)
- **SQL Server LocalDB** with:
  - Stored Procedures
  - App_Data SQL backup + script

## ?? Database

Database contains:

- `Invoices` table
- `InvoiceLineItems` table

Stored procedures are used for:
- Adding and updating invoices
- Retrieving invoices with related line items

A SQL script is included in the `App_Data` folder to recreate the schema and seed sample data.

## ? Assignment Requirements Met

- ? ASP.NET MVC 4.8 structure
- ? Bootstrap 5 frontend
- ? SQL database with stored procedures
- ? Capture and edit invoice data
- ? Secure dashboard for financial staff
- ? Store IP and machine name of data capturer (audit fields)
- ? Save computed totals (line & invoice) to the database
- ? Include `.sql` script in App_Data

## ?? Notes

- For demo purposes, login uses a hardcoded username/password
- Client-side JavaScript is minimal and scoped for reliability

---