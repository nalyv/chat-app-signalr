using Microsoft.AspNetCore.Mvc;

namespace chat_application.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new RedirectToActionResult("Dashboard", "Administrator", new {receiverName = "All"});
        }
    }
}