using InvoiceSystem.Core.Models;
using InvoiceSystem.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;

namespace InvoiceSystem.WebMVC.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class InvoiceController : Controller
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoiceController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["InvoiceSystemDBConnection"].ConnectionString;
            _invoiceRepository = new InvoiceRepository(connectionString);
        }

        public ActionResult Upsert(int? id)
        {
            Invoice model;

            if (id.HasValue && id.Value > 0)
            {
                model = _invoiceRepository.GetInvoiceDetails(id.Value);
                if (model == null)
                {
                    return HttpNotFound();
                }
            }
            else
            {
                model = new Invoice();
                model.LineItems.Add(new InvoiceLineItem());
                ModelState.Clear();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(Invoice invoice)
        {
            invoice.CapturedIP = Request.UserHostAddress;
            invoice.CapturedMachineName = Environment.MachineName;

            if (ModelState.IsValid)
            {
                try
                {
                    bool isNewInvoice = invoice.InvoiceId == 0;
                    int invoiceId = invoice.InvoiceId;

                    if (isNewInvoice)
                    {
                        invoiceId = _invoiceRepository.InsertInvoice(invoice);
                    }
                    else
                    {
                        _invoiceRepository.UpdateInvoice(invoice);
                        _invoiceRepository.DeleteInvoiceLineItems(invoiceId);
                    }

                    var validItems = invoice.LineItems
                        .Where(item => item != null && !string.IsNullOrWhiteSpace(item.Name))
                        .ToList();

                    foreach (var item in validItems)
                    {
                        item.InvoiceId = invoiceId;
                        _invoiceRepository.InsertInvoiceLineItem(item);
                    }

                    _invoiceRepository.RecalculateInvoiceTotal(invoiceId);

                    TempData["SuccessMessage"] = isNewInvoice
                        ? "New Invoice created successfully! Ready for another."
                        : "Invoice updated successfully!";

                    return RedirectToAction("Upsert", new { id = isNewInvoice ? (int?)null : invoiceId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");
                }
            }

            if (invoice.LineItems == null || !invoice.LineItems.Any())
            {
                invoice.LineItems = new List<InvoiceLineItem> { new InvoiceLineItem() };
            }

            return View(invoice);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _invoiceRepository.DeleteInvoice(id);
                return Json(new { success = true, message = $"Invoice {id} deleted successfully." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting invoice {id}: {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        public ActionResult Dashboard()
        {
            if (!IsUserAuthenticated())
                return RedirectToAction("Login", "Account");

            var invoices = _invoiceRepository.GetAllInvoices().ToList();
            return View(invoices);
        }

        private bool IsUserAuthenticated()
        {
            return Session["IsAuthenticated"] is bool authenticated && authenticated;
        }
    }
}
