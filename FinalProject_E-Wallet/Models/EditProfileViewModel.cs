using System.ComponentModel.DataAnnotations;
namespace FinalProject_E_Wallet.Models
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public string? ProfilePicturePath { get; set; }

        // PASSWORD (OPTIONAL)
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
