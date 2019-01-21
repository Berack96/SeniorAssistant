using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
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
            string username = HttpContext.Session.GetString(Username);
            return View("Index", GetUser(username));
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
            return CheckAuthorized("User", GetUser(user));
        }

        [Route("Message/{User}")]
        public IActionResult Message(string user)
        {
            return CheckAuthorized("Message", GetUser(user));
        }

        private User GetUser(string username)
        {
            return Db.Users
                .LoadWith(u => u.Doc)
                .LoadWith(u => u.Pat)
                .Where(u => u.Username.Equals(username))
                .FirstOrDefault();
        }

        private IActionResult CheckAuthorized(string view, object model = null)
        {
            if (!IsLogged())
                return View("Index", "/" + view);
            return View(view, model);
        }
    }
}