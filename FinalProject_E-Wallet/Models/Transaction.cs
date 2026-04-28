namespace FinalProject_E_Wallet.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; } // CashIn, Send
    }
}
