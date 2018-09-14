using Microsoft.AspNetCore.Mvc;

namespace SeniorAssistant.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        [Route("Home")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Heartbeat")]
        public IActionResult Heartbeat()
        {
            return View();
        }
    }
}