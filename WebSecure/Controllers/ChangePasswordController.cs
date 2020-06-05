using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSecure.Filters;
using WebSecure.Helpers;
using WebSecure.Repository;
using WebSecure.ViewModel;

namespace WebSecure.Controllers
{
    [AuthorizeUser]
    public class ChangePasswordController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationRepository _verificationRepository;
        public ChangePasswordController(IUserRepository userRepository, IVerificationRepository verificationRepository)
        {
            _userRepository = userRepository;
            _verificationRepository = verificationRepository;
        }

        public IActionResult Process()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Process(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var userid = Convert.ToString(HttpContext.Session.GetString("UserId"));
                var getuserdetails = _userRepository.GetUserbyUserId(Convert.ToInt64(userid));
                var usersalt = _userRepository.GetUserSaltbyUserid(getuserdetails.UserId);
                var generatehash = GenerateHashSha512.Sha512(changePasswordViewModel.CurrentPassword, usersalt.PasswordSalt);

                if (changePasswordViewModel.CurrentPassword == changePasswordViewModel.Password)
                {
                    ModelState.AddModelError("", @"New Password Cannot be same as Old Password");
                    return View(changePasswordViewModel);
                }

                if (!string.Equals(getuserdetails.PasswordHash, generatehash, StringComparison.Ordinal))
                {
                    ModelState.AddModelError("", "Current Password Entered is InValid");
                    return View(changePasswordViewModel);
                }

                if (!string.Equals(changePasswordViewModel.Password, changePasswordViewModel.ConfirmPassword, StringComparison.Ordinal))
                {
                    TempData["Reset_Error_Message"] = "Password Does not Match";
                    return View(changePasswordViewModel);
                }
                else
                {
                    var salt = GenerateRandomNumbers.RandomNumbers(20);
                    var saltedpassword = GenerateHashSha512.Sha512(changePasswordViewModel.Password, salt);
                    var result = _userRepository.UpdatePasswordandHistory(getuserdetails.UserId, saltedpassword, salt,"C");

                    if (result > 0)
                    {
                        //
                        TempData["ChangePassword_Success_Message"] = "Password Changed Successfully";
                        var updateresult = _verificationRepository.UpdateRegisterVerification(getuserdetails.UserId);
                        return RedirectToAction("Process", "ChangePassword");
                    }
                    else
                    {
                        TempData["Reset_Error_Message"] = "Something Went Wrong Please try again!";
                        return View(changePasswordViewModel);
                    }
                }
            }

            return View(changePasswordViewModel);
        }

        private void CheckIsPasswordAlreadyExists(ResetPasswordViewModel resetPasswordViewModel)
        {
            var salt = GenerateRandomNumbers.RandomNumbers(20);
            var saltedpassword = GenerateHashSha512.Sha512(resetPasswordViewModel.Password, salt);
        }

    }
}