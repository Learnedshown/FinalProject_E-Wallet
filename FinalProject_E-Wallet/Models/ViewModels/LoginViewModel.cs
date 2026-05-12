using System.ComponentModel.DataAnnotations;

namespace FinalProject_E_Wallet.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]   
        public string Identifier { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}