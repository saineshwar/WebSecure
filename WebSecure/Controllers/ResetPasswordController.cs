using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSecure.Filters;
using WebSecure.Helpers;
using WebSecure.Repository;
using WebSecure.ViewModel;

namespace WebSecure.Controllers
{
    [AuthorizeResetPassword]
    public class ResetPasswordController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationRepository _verificationRepository;
        public ResetPasswordController(IUserRepository userRepository, IVerificationRepository verificationRepository)
        {
            _userRepository = userRepository;
            _verificationRepository = verificationRepository;
        }

        [HttpGet]
        public IActionResult Reset()
        {
            return View(new ResetPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var userid = Convert.ToString(HttpContext.Session.GetString("VerificationUserId"));
                var getuserdetails = _userRepository.GetUserbyUserId(Convert.ToInt64(userid));

                if (!string.Equals(resetPasswordViewModel.Password, resetPasswordViewModel.ConfirmPassword, StringComparison.Ordinal))
                {
                    TempData["Reset_Error_Message"] = "Password Does not Match";
                    return View(resetPasswordViewModel);
                }
                else
                {
                    var salt = GenerateRandomNumbers.RandomNumbers(20);
                    var saltedpassword = GenerateHashSha512.Sha512(resetPasswordViewModel.Password, salt);
                    var result = _userRepository.UpdatePasswordandHistory(getuserdetails.UserId, saltedpassword, salt,"R");

                    if (result > 0)
                    {
                        var updateresult = _verificationRepository.UpdateResetVerification(getuserdetails.UserId);
                        return RedirectToAction("Login", "Portal");
                    }
                    else
                    {
                        TempData["Reset_Error_Message"] = "Something Went Wrong Please try again!";
                        return View(resetPasswordViewModel);
                    }
                    
                }
            }

            return View(resetPasswordViewModel);
        }

        private void CheckIsPasswordAlreadyExists(ResetPasswordViewModel resetPasswordViewModel)
        {
            var salt = GenerateRandomNumbers.RandomNumbers(20);
            var saltedpassword = GenerateHashSha512.Sha512(resetPasswordViewModel.Password, salt);
        }



    }
}