using System.ComponentModel.DataAnnotations;
namespace InvoiceSystem.Core.Models
{
    public class InvoiceLineItem
    {
        public int LineItemId { get; set; }

        [Required]
        public int InvoiceId { get; set; } // Required if saving, since it’s a FK

        [Required(ErrorMessage = "Line Item Name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int Quantity { get; set; }
        public decimal PriceExVAT { get; set; }
        public decimal VATAmount { get; set; }
        public decimal PriceInclVAT { get; set; } // computed

        [StringLength(255)]
        public string Remarks { get; set; }

        public decimal LineTotal { get; set; } // computed
    }
}
