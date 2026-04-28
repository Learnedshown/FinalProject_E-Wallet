namespace FinalProject_E_Wallet.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string FullName { get; set; }

        public decimal Balance { get; set; } = 0;
    }
}
