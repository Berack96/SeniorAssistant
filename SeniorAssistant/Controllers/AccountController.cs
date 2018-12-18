using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
using SeniorAssistant.Controllers;
using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace IdentityDemo.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        [HttpPost]
        public ActionResult _login(string username, string password)
        {
            JsonResponse response = new JsonResponse
            {
                Success = false,
                Message = "Username or password is invalid."
            };

            var result = Db.GetTable<User>().Where(user => user.Username.Equals(username) && user.Password.Equals(password)).ToListAsync().Result;

            if (result.Count == 1)
            {
                var loggedUser = HttpContext.Session.GetString(Username);
                if (loggedUser==null || !loggedUser.Equals(username))
                {
                    User user = result.First();
                    HttpContext.Session.SetString(Username, username);
                    HttpContext.Session.SetString("email", user.Email);
                    HttpContext.Session.SetString("name", user.Name);
                    HttpContext.Session.SetString("role", user.Role);
                    //HttpContext.Session.SetString("lastname", user.LastName);

                    response.Success = true;
                    response.Message = Request.Query["ReturnUrl"];
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
        public ActionResult _register(User user)
        {
            JsonResponse response = new JsonResponse() { Success = true };

            if(ModelState.IsValid)
            {
                try
                {
                    Db.Insert(user);
                    _login(user.Username, user.Password);
                }
                catch
                {
                    response.Success = false;
                    response.Message = "Username already exists";
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Modello non valido";
            }

            return Json(response);
        }

        internal class JsonResponse
        {
            public bool Success { get; internal set; }
            public string Message { get; internal set; }
        }
    }
}