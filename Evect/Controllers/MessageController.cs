using Microsoft.AspNetCore.Mvc;

namespace Evect.Controllers
{
    public class MessageController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}