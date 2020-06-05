using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSecure.Helpers;
using WebSecure.Repository;

namespace WebSecure.Controllers
{
    public class VerifyRegistrationController : Controller
    {
        private readonly IVerificationRepository _verificationRepository;
        public VerifyRegistrationController(IVerificationRepository verificationRepository)
        {
            _verificationRepository = verificationRepository;
        }


        [HttpGet]
        public IActionResult Verify(string key, string hashtoken)
        {
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(hashtoken))
                {
                    var arrayVakue = SecurityManager.SplitToken(key);
                    if (arrayVakue != null)
                    {
                        // arrayVakue[1] "UserId"
                        var rvModel = _verificationRepository.GetRegistrationGeneratedToken(arrayVakue[1]);
                        if (rvModel != null)
                        {
                            var result = SecurityManager.IsTokenValid(arrayVakue, hashtoken, rvModel.GeneratedToken);

                            if (result == 1)
                            {
                                TempData["TokenErrorMessage"] = "Sorry Verification Link Expired Please request a new Verification link!";
                                return RedirectToAction("Login", "Portal");
                            }

                            if (result == 2)
                            {
                                TempData["TokenErrorMessage"] = "Sorry Verification Link Expired Please request a new Verification link!";
                                return RedirectToAction("Login", "Portal");
                            }

                            if (result == 0)
                            {
                                if (_verificationRepository.CheckIsAlreadyVerifiedRegistration(Convert.ToInt64(arrayVakue[1])))
                                {
                                    TempData["TokenErrorMessage"] = "Sorry Link Expired";
                                    return RedirectToAction("Login", "Portal");
                                }

                                HttpContext.Session.SetString("VerificationUserId", arrayVakue[1]);
                                var updateresult = _verificationRepository.UpdateRegisterVerification(Convert.ToInt64(arrayVakue[1]));
                                if (updateresult)
                                {
                                    TempData["Verify"] = "Done";
                                    return RedirectToAction("Completed", "VerifyRegistration");
                                }
                                else
                                {
                                    TempData["TokenErrorMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                                    return RedirectToAction("Login", "Portal");
                                }
                              
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
                TempData["TokenMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                return RedirectToAction("Login", "Portal");
            }

            TempData["TokenMessage"] = "Sorry Verification Failed Please request a new Verification link!";
            return RedirectToAction("Login", "Portal");
        }


        [HttpGet]
        public IActionResult Completed()
        {
            if (Convert.ToString(TempData["Verify"]) == "Done")
            {
                TempData["RegistrationCompleted"] = "Registration Process Completed. Now you can Login and Access Account.";
                return View();
            }
            else
            {
                TempData["TokenMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                return RedirectToAction("Login", "Portal");
            }

        }

    }
}