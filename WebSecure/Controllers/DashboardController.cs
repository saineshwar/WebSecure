using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebSecure.Filters;

namespace WebSecure.Controllers
{
    [AuthorizeUser]
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Portal");
        }
    }
}