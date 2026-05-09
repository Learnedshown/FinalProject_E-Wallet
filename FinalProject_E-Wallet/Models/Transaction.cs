using System.ComponentModel.DataAnnotations;

namespace FinalProject_E_Wallet.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Sender ID is required")]
        public int SenderId { get; set; }

        [Required(ErrorMessage = "Receiver ID is required")]
        public int ReceiverId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 100000, ErrorMessage = "Amount must be between ₱0.01 and ₱100,000")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Transaction type is required")]
        [RegularExpression("^(Cash In|Send)$", ErrorMessage = "Type must be 'Cash In' or 'Send'")]
        [StringLength(20)]  
        public string Type { get; set; } 
    }
}