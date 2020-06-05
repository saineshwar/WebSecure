using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebSecure.Helpers;
using WebSecure.Models;
using WebSecure.Repository;
using WebSecure.ViewModel;

namespace WebSecure.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly AppSettings _appSettings;
        private readonly IVerificationRepository _verificationRepository;
        public ForgotPasswordController(IUserRepository userRepository, IOptions<AppSettings> appSettings, IVerificationRepository verificationRepository)
        {
            _userRepository = userRepository;
            _verificationRepository = verificationRepository;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult Process()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Process(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!_userRepository.CheckUserExists(forgotPasswordViewModel.UserName))
            {
                ModelState.AddModelError("", "Entered Username or Password is Invalid");
            }
            else
            {
                var userdetails = _userRepository.GetUserbyUserName(forgotPasswordViewModel.UserName);
                Send(userdetails);
                //HttpContext.Session.SetString("TempUserName", Convert.ToString(forgotPasswordViewModel.UserName));
                TempData["ForgotPasswordMessage"] = "An email has been sent to the address you have registered." +
                                                    "Please follow the link in the email to complete your password reset request";
                return RedirectToAction("Process", "ForgotPassword");
            }

            return View();
        }

        private void Send(User user)
        {
            var emailVerficationToken = GenerateHashSha256.ComputeSha256Hash((GenerateRandomNumbers.RandomNumbers(6)));
            _verificationRepository.SendResetVerificationToken(user.UserId, emailVerficationToken);

            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            try
            {
                MailAddress fromAddress = new MailAddress(_appSettings.EmailFrom);
                message.From = fromAddress;
                message.To.Add(user.Email);
                message.Subject = "Welcome to Web Secure";
                message.IsBodyHtml = true;
                message.Body = SendVerificationEmail(user, emailVerficationToken);
                smtpClient.Host = _appSettings.Host;
                smtpClient.Port = Convert.ToInt32(_appSettings.Port);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(_appSettings.EmailFrom, _appSettings.Password);
                smtpClient.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string SendVerificationEmail(User user, string token)
        {
            AesAlgorithm aesAlgorithm = new AesAlgorithm();
            var key = string.Join(":", new string[] { DateTime.Now.Ticks.ToString(), user.UserId.ToString() });
            var encrypt = aesAlgorithm.EncryptToBase64String(key);

            var linktoverify = _appSettings.VerifyResetPasswordUrl + "?key=" + HttpUtility.UrlEncode(encrypt) + "&hashtoken=" + HttpUtility.UrlEncode(token);
            var stringtemplate = new StringBuilder();
            stringtemplate.Append("Welcome");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("Dear " + user.FullName);
            stringtemplate.Append("<br/>");
            stringtemplate.Append("Please click the following link to reset your password.");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("Reset password link : <a target='_blank' href=" + linktoverify + ">Link</a>");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("If the link does not work, copy and paste the URL into a new browser window. The URL will expire in 24 hours for security reasons.");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("Best regards,");
            stringtemplate.Append("Saineshwar Begari");
            stringtemplate.Append("<br/>");
            return stringtemplate.ToString();
        }
    }
}