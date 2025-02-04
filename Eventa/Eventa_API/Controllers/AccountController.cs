using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
