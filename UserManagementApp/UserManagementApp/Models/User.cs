using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementApp.Models
{
    public class User
    {
        public int Id { get; set; }



        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile Number must be 10 digits.")]
        public string MobileNumber { get; set; }

        [Required]
        public string Password { get; set; }

        public string Photo { get; set; }
      
        [NotMapped]
        public IFormFile PhotoFile { get; set; }  // This is used for file upload
        

    }

}
