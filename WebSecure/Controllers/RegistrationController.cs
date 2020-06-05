using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebSecure.Helpers;
using WebSecure.Models;
using WebSecure.Repository;
using WebSecure.ViewModel;

namespace WebSecure.Controllers
{
    [AllowAnonymous]
    public class RegistrationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationRepository _verificationRepository;
        private readonly AppSettings _appSettings;
        public RegistrationController(IUserRepository userRepository, IVerificationRepository verificationRepository, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _verificationRepository = verificationRepository;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.CheckUserExists(registerViewModel.UserName))
                {
                    ModelState.AddModelError("", "Entered Username Already Exists");
                    return View(registerViewModel);
                }

                if (_userRepository.CheckUserExists(registerViewModel.Email))
                {
                    ModelState.AddModelError("", "Entered EmailId Already Exists");
                    return View(registerViewModel);
                }

                if (_userRepository.CheckUserExists(registerViewModel.Phoneno))
                {
                    ModelState.AddModelError("", "Entered Phoneno Already Exists");
                    return View(registerViewModel);
                }

                if (!string.Equals(registerViewModel.Password, registerViewModel.ConfirmPassword,
                    StringComparison.Ordinal))
                {
                    TempData["Registered_Error_Message"] = "Password Does not Match";
                    return View(registerViewModel);
                }

                var salt = GenerateRandomNumbers.RandomNumbers(20);
                var saltedpassword = GenerateHashSha512.Sha512(registerViewModel.Password, salt);

                User user = new User()
                {
                    FullName = registerViewModel.FullName,
                    CreatedDate = DateTime.Now,
                    PasswordHash = saltedpassword,
                    Status = true,
                    UserId = 0,
                    Username = registerViewModel.UserName,
                    Email= registerViewModel.Email,
                    Phoneno =registerViewModel.Phoneno
                };

                var userId = _userRepository.RegisterUser(user, salt);
                if (userId > 0)
                {
                    Send(userId, registerViewModel);
                    TempData["Registered_Success_Message"] = "User Registered Successfully";
                }
                else
                {
                    TempData["Registered_Error_Message"] = "Error While Registrating User Successfully";
                }
            }

            return RedirectToAction("Register", "Registration");
        }


        private void Send(long userid, RegisterViewModel registerViewModel)
        {
            var emailVerficationToken = GenerateHashSha256.ComputeSha256Hash((GenerateRandomNumbers.RandomNumbers(6)));
            _verificationRepository.SendRegistrationVerificationToken(userid, emailVerficationToken);

            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            try
            {
                MailAddress fromAddress = new MailAddress(_appSettings.EmailFrom);
                message.From = fromAddress;
                message.To.Add(registerViewModel.Email);
                message.Subject = "Welcome to Web Secure";
                message.IsBodyHtml = true;
                message.Body = SendVerificationEmail(registerViewModel, emailVerficationToken, userid);
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

        public string SendVerificationEmail(RegisterViewModel registerViewModel, string token , long userid)
        {
            AesAlgorithm aesAlgorithm = new AesAlgorithm();
            var key = string.Join(":", new string[] { DateTime.Now.Ticks.ToString(), userid.ToString() });
            var encrypt = aesAlgorithm.EncryptToBase64String(key);

            var linktoverify = _appSettings.VerifyRegistrationUrl + "?key=" + HttpUtility.UrlEncode(encrypt) + "&hashtoken=" + HttpUtility.UrlEncode(token);
            var stringtemplate = new StringBuilder();
            stringtemplate.Append("Welcome");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("Dear " + registerViewModel.FullName);
            stringtemplate.Append("<br/>");
            stringtemplate.Append("Thanks for joining Web Secure.");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("To activate your Web Secure account, please confirm your email address.");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("<a target='_blank' href=" + linktoverify + ">Confirm Email</a>");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("Yours sincerely,");
            stringtemplate.Append("<br/>");
            stringtemplate.Append("Ticket's");
            stringtemplate.Append("<br/>");
            return stringtemplate.ToString();
        }
    }
}