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
        public IActionResult Login()
        {
            return CheckUnAuthorized("Login");
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
            try
            {
                string rm = HttpContext.Request.Query["removePatient"];
                string usr = HttpContext.Session.GetString(Username);

                var pt = Db.Patients
                    .Where(p => p.Username.Equals(rm) && p.Doctor.Equals(usr))
                    .FirstOrDefault();
                var mp = Db.MenuPatients
                    .Where(m => m.PatientUsername.Equals(rm) && m.Username.Equals(usr))
                    .FirstOrDefault();

                Db.Delete(pt);
                Db.Delete(mp);
            }
            catch { }

            return CheckAuthorized("User", GetUser(user));
        }

        [Route("Message/{User}")]
        public IActionResult Message(string user)
        {
            return CheckAuthorized("Message", GetUser(user));
        }

        [Route("Profile")]
        public IActionResult Profile()
        {
            string username = HttpContext.Session.GetString(Username);
            return CheckAuthorized("Profile", GetUser(username));
        }
        
        [Route("Register")]
        public IActionResult Register()
        {
            return CheckUnAuthorized("Register");
        }

        [Route("Forgot")]
        public IActionResult Forgot(string username = "")
        {
            if (IsLogged())
            {
                return RedirectToAction("Profile", "Home", GetUser(HttpContext.Session.GetString(Username)));
            }

            var forgot = Db.Forgot.Where(f => f.Username.Equals(username)).FirstOrDefault();
            if (forgot == null)
                return View("Login", "Utente non esiste");
            return View("Forgot", forgot);
        }
        
        protected IActionResult CheckAuthorized(string view, object model = null)
        {
            if (!IsLogged())
            {
                return RedirectToAction("Login", "Home", "/" + view);
            }
            return View(view, model);
        }

        protected IActionResult CheckUnAuthorized(string view, object model = null)
        {
            if (IsLogged())
            {
                return RedirectToAction("Profile", "Home", GetUser(HttpContext.Session.GetString(Username)));
            }
            return View(view, model);
        }

        private User GetUser(string username)
        {
            return Db.Users
                .LoadWith(u => u.Doc)
                .LoadWith(u => u.Pat)
                .Where(u => u.Username.Equals(username))
                .FirstOrDefault();
        }
    }
}