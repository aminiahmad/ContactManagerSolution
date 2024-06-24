using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUDExample.Controllers
{
    public class HomeController : Controller
    {
        //all person see this page , not required login 
        [AllowAnonymous]    
        [Route("/Error")]
        public IActionResult Error()
        {
            var Exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (Exception != null && Exception.Error != null)
            {
                ViewBag.ErrorMessage=Exception.Error.Message;
            }
            return View();
        }
    }
}
