using System;
using System.Collections.Generic;
using System.Text;

namespace WebSecure.ViewModel
{
    public class AppSettings
    {
        public string EmailFrom { get; set; }
        public string Port { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
        public string VerifyRegistrationUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string VerifyResetPasswordUrl { get; set; }
        
    }
}
