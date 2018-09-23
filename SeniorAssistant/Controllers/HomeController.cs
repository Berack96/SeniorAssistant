using Microsoft.AspNetCore.Mvc;

namespace SeniorAssistant.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
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

        [Route("Sleep")]
        public IActionResult Sleep()
        {
            return View();
        }

        [Route("Step")]
        public IActionResult Step()
        {
            return View();
        }

        [Route("{User}")]
        public IActionResult SingleUser(string user)
        {
            return View("user", user);
        }
    }
}