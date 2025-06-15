using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InvoiceSystem.Core.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Invoice Number is required.")]
        [StringLength(50)]
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [StringLength(255)]
        [Display(Name = "Supplier Address")]
        [DataType(DataType.MultilineText)]
        public string SupplierAddress { get; set; }

        [StringLength(100)]
        [Display(Name = "Supplier Contact")]
        public string SupplierContact { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Captured IP")]
        public string CapturedIP { get; set; }

        [Display(Name = "Machine Name")]
        public string CapturedMachineName { get; set; }

        [Display(Name = "Invoice Total")]
        public decimal InvoiceTotal { get; set; }

        public List<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    }
}
