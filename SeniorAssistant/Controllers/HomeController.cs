using LinqToDB;
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
        public IActionResult Login()
        {
            if (IsLogged())
                return View("Profile");
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
        
        [Route("Profile")]
        public IActionResult Profile()
        {
            return CheckAuthorized("Profile");
        }
        
        [Route("Register")]
        public IActionResult Register()
        {
            if (IsLogged())
                return View("Profile");
            return View();
        }

        [Route("Forgot")]
        public IActionResult Forgot(string username = "")
        {
            if (IsLogged())
                return View("Profile");
            var forgot = Db.Forgot.Where(f => f.Username.Equals(username)).FirstOrDefault();
            return View(forgot);
        }
        
        protected IActionResult CheckAuthorized(string view, object model = null)
        {
            if (!IsLogged())
            {
                view = "Login";
                model = "/" + view;
            }
            return View(view, model);
        }
    }
}