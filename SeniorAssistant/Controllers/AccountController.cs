using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
using SeniorAssistant.Controllers;
using LinqToDB;
using System.Linq;

namespace IdentityDemo.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        /*
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        /*
        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        */

        [HttpPost]
        public ActionResult _login(string username, string password)
        {
            JsonResponse response = new JsonResponse
            {
                Success = false,
                Message = "Username or password is invalid."
            };

            var strunz = Db.GetTable<User>().Where(user => user.Username.Equals(username) && user.Password.Equals(password)).ToListAsync().Result;

            if (strunz.Count == 1)
            {
                var loggedUser = HttpContext.Session.GetString("username");
                if (loggedUser==null || !loggedUser.Equals(username))
                {
                    HttpContext.Session.SetString("username", username);
                    HttpContext.Session.SetString("email", strunz.First().Email);
                    HttpContext.Session.SetString("name", strunz.First().Name);
                    HttpContext.Session.SetString("isdoc", strunz.First().Doctor?"true":"false");
                    //HttpContext.Session.SetString("lastname", strunz.First().LastName);
                    response.Success = true;
                    response.Message = "";
                }
                else
                {
                    response.Message = "User already logged";
                }
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult _logout()
        {
            HttpContext.Session.Clear();
            return Json(new JsonResponse());
        }

        [HttpPost]
        public ActionResult _register(Register register)
        {
            if(ModelState.IsValid)
            {
                User user = new User() { Username = register.Username, Email = register.Email, Password = register.Password};
                try
                {
                    Db.Insert(user);
                }
                catch
                {
                    return Json(new JsonResponse() { Success = false, Message = "Username already exist" });
                }
                _login(user.Username, user.Password);
                return Json(new JsonResponse() { Success = true });
            }
            return Json(new JsonResponse() { Success = false, Message = "Modello non valido" });
        }
        internal class JsonResponse
        {
            public bool Success { get; internal set; }
            public string Message { get; internal set; }
        }
    }
}