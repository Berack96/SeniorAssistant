using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
using SeniorAssistant.Controllers;
using LinqToDB;
using System.Linq;
using System;
using SeniorAssistant.Models.Users;

namespace IdentityDemo.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        private static readonly string NoteModified = "Il tuo dottore ha modificato la nota per te";

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
                if (loggedUser==null || !loggedUser.Equals(username)) // non ha senso
                {
                    User user = result.First();
                    HttpContext.Session.SetString(Username, username);
                    HttpContext.Session.SetString("email", user.Email);
                    HttpContext.Session.SetString("name", user.Name);
                    HttpContext.Session.SetString("lastname", user.LastName);
                    
                    var isDoc = (from d in Db.Doctors
                                 where d.Username.Equals(username)
                                 select d).ToArray().FirstOrDefault() != null;
                    HttpContext.Session.SetString("role", isDoc? "doctor":"patient");

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
            return Action(() =>
            {
                try
                {
                    Db.Insert(user);
                    return _login(user.Username, user.Password);
                }
                catch
                {
                    return Json(new JsonResponse(false, "Username already exists"));
                }
            });
        }

        [HttpPost]
        public ActionResult _notification(string username, string message)
        {
            return LoggedAction(() =>
            {
                Db.Insert(new Notification()
                {
                    Message = message,
                    Username = username,
                    Time = DateTime.Now,
                    Seen = false
                });
                return Json(OkJson);
            });
        }

        [HttpPut]
        public ActionResult _notification(int id)
        {
            return LoggedAction(() =>
            {
                JsonResponse response = OkJson;

                Notification note = Db.Notifications.Where(n => n.Id == id).ToArray().FirstOrDefault();
                if(note != null)
                {
                    note.Seen = true;
                    Db.Update(note);
                }
                else
                {
                    response.Success = false;
                    response.Message = "La notifica da modificare non esiste";
                }
                return Json(response);
            });
        }

        [HttpPost]
        public ActionResult _addDoc(string doctor)
        {
            return LoggedAction(() =>
            {
                string username = HttpContext.Session.GetString(Username);
                var isAlreadyPatient = Db.Patients.Where(p => p.Username.Equals(username)).ToArray().FirstOrDefault() != null;
                if (isAlreadyPatient)
                    return Json(new JsonResponse()
                    {
                        Success = false,
                        Message = "You are already a patient"
                    });

                var docExist = Db.Doctors.Where(d => d.Username.Equals(doctor)).ToArray().FirstOrDefault() != null;
                if(!docExist)
                    return Json(new JsonResponse()
                    {
                        Success = false,
                        Message = "Doctor doesn't exist"
                    });

                Db.Insert(new Patient()
                {
                    Doctor = doctor,
                    Username = username
                });

                _notification(doctor, "L'utente "+username+" ti ha inserito come il suo dottore.");
                return Json(new JsonResponse());
            });
        }

        [HttpPost]
        public ActionResult _sendMessage(string reciver, string body)
        {
            return LoggedAction(() => {
                string username = HttpContext.Session.GetString(Username);
                Message message = new Message()
                {
                    Reciver = reciver,
                    Body = body,
                    Time = DateTime.Now,
                    Username = username,
                    Seen = false
                };

                Db.Insert(message);

                return Json(new JsonResponse());
            });
        }

        [HttpPut]
        public ActionResult _addNote(string patient, string text)
        {
            return LoggedAccessDataOf(patient, () =>
            {
                var pat = Db.Patients.Where((p) => p.Username.Equals(patient)).FirstOrDefault();
                pat.Notes = text;
                Db.Update(pat);
                _notification(patient, NoteModified);

                return Json(OkJson);
            });
        }
    }
}