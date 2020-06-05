using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebSecure.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(Convert.ToString(context.HttpContext.Session.GetString("UserId"))))
            {
              
                    ViewResult result = new ViewResult();
                    result.ViewName = "Error";

                    if (context.Controller is Controller controller)
                    {
                        controller.TempData["ErrorMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                    }

                    context.Result = result;
                
            }
        }
    }
}