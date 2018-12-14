using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SeniorAssistant.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly ISession session;
        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            this.session = httpContextAccessor.HttpContext.Session;
        }

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

        [Route("Users")]
        public IActionResult Users()
        {
            return View();
        }

        [Route("User/{User}")]
        public IActionResult SingleUser(string user)
        {
            if(session.GetString("username") == null)
                return RedirectToAction("Index");
            return View("data", user);
        }
    }
}