using System.ComponentModel.DataAnnotations;
namespace FinalProject_E_Wallet.Models
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters allowed")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters allowed")]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters allowed")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "Enter valid PH number (09XXXXXXXXX)")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        
        public string? ProfilePicturePath { get; set; }

        // PASSWORD (OPTIONAL)
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
