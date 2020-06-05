using System;
using System.ComponentModel.DataAnnotations;

namespace WebSecure.ViewModel
{
    public class LoginViewModel
    {
        [StringLength(30, ErrorMessage = "Not valid Username")]
        [Required(ErrorMessage = "Enter UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }
    }
}
