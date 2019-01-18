using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            return CheckAuthorized("Data", "Heartbeat");
        }

        [Route("Sleep")]
        public IActionResult Sleep()
        {
            return CheckAuthorized("Data", "Sleep");
        }

        [Route("Step")]
        public IActionResult Step()
        {
            return CheckAuthorized("Data", "Step");
        }

        [Route("Users")]
        public IActionResult Users()
        {
            return CheckAuthorized("Users");
        }

        [Route("User/{User}")]
        public IActionResult SingleUser(string user)
        {
            var u = (from us in Db.Users
                     where us.Username.Equals(user)
                     select us).FirstOrDefault();
            return CheckAuthorized("User", u);
        }

        [Route("Message/{User}")]
        public IActionResult Message(string user)
        {
            return CheckAuthorized("Message", user);
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