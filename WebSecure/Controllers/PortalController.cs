using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSecure.Helpers;
using WebSecure.Repository;
using WebSecure.ViewModel;

namespace WebSecure.Controllers
{
    public class PortalController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationRepository _verificationRepository;
        public PortalController(IUserRepository userRepository, IVerificationRepository verificationRepository)
        {
            _userRepository = userRepository;
            _verificationRepository = verificationRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!_userRepository.CheckUserExists(loginViewModel.UserName))
                {
                    ModelState.AddModelError("", "Entered Username or Password is Invalid");
                }
                else
                {
                    var getuserdetails = _userRepository.GetUserbyUserName(loginViewModel.UserName);

                    if (getuserdetails == null)
                    {
                        ModelState.AddModelError("", "Entered Username or Password is Invalid");
                        return View();
                    }

                    var usersalt = _userRepository.GetUserSaltbyUserid(getuserdetails.UserId);
                    if (usersalt == null)
                    {
                        ModelState.AddModelError("", "Entered Username or Password is Invalid");
                        return View();
                    }

                    if (!_verificationRepository.CheckIsAlreadyVerifiedRegistration(getuserdetails.UserId))
                    {
                        ModelState.AddModelError("", "Email Verification Pending");
                        return View();
                    }

                    var generatehash = GenerateHashSha512.Sha512(loginViewModel.Password, usersalt.PasswordSalt);

                    if (string.Equals(getuserdetails.PasswordHash, generatehash, StringComparison.Ordinal))
                    {
                         HttpContext.Session.SetString("UserId", Convert.ToString(getuserdetails.UserId));
                         HttpContext.Session.SetString("UserName", Convert.ToString(getuserdetails.Username));

                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Entered Username or Password is Invalid");
                    }

                    return View();
                }
            }

            return View();
        }
    }
}