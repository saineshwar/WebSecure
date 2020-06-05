using System.ComponentModel.DataAnnotations;

namespace WebSecure.ViewModel
{
    public class RegisterViewModel
    {
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Required(ErrorMessage = "Enter FullName")]
        public string FullName { get; set; }

        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Required(ErrorMessage = "Enter UserName")]
        public string UserName { get; set; }

        [StringLength(150, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password Required")]
        [Compare("Password", ErrorMessage = "Enter Valid Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "EmailId Required")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phoneno Required")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Wrong Phoneno")]
        [MaxLength(20)]
        public string Phoneno { get; set; }
    }
}