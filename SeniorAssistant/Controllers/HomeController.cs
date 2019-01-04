using Microsoft.AspNetCore.Mvc;

namespace SeniorAssistant.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : BaseController
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
            return CheckAuthorized("Heartbeat");
        }

        [Route("Sleep")]
        public IActionResult Sleep()
        {
            return CheckAuthorized("Sleep");
        }

        [Route("Step")]
        public IActionResult Step()
        {
            return CheckAuthorized("Step");
        }

        [Route("Users")]
        public IActionResult Users()
        {
            return CheckAuthorized("Users");
        }

        [Route("User/{User}")]
        public IActionResult SingleUser(string user)
        {
            return CheckAuthorized("Data", user);
        }

        [Route("Message/{Id}")]
        public IActionResult Message(int id)
        {
            return CheckAuthorized("Message", id);
        }

        private IActionResult CheckAuthorized(string view, object model = null)
        {
            if (!IsLogged())
            {
                model = "/" + view;
                view = "Index";
            }
            return View(view, model);
        }
    }
}