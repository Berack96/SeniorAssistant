using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
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
        public ActionResult _login(string username, string password, bool rememberMe)
        {
            var result = username != null && password != null && username.Equals("acc1") && password.Equals("123"); //await _signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: false);
            JsonResponse response = new JsonResponse();
            response.Success = false;
            response.Message = "Username or password is invalid.";

            if (result)
            {
                var loggedUser = HttpContext.Session.GetString("username");
                if (loggedUser==null || !loggedUser.Equals(username))
                {
                    HttpContext.Session.SetString("username", username);
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

        public ActionResult _logout()
        {
            HttpContext.Session.Clear();
            return Json(new JsonResponse());
        }

        public ActionResult _register()
        {
            return Json(new JsonResponse());
        }
        internal class JsonResponse
        {
            public bool Success { get; internal set; }
            public string Message { get; internal set; }
        }
    }
}