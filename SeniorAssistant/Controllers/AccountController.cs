using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
using SeniorAssistant.Controllers;
using LinqToDB;
using System.Linq;
using System;
using SeniorAssistant.Models.Users;
using SeniorAssistant.Data;
using System.Threading.Tasks;

namespace IdentityDemo.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        private static readonly string NoteModified = "Il tuo dottore ha modificato la nota per te";
        private static readonly string InvalidLogIn = "Username o Password sbagliati";
        private static readonly string AlreadyLogIn = "L'utente e' gia' loggato";
        private static readonly string UsernameDupl = "Lo username selezionato e' gia' in uso";
        private static readonly string ModNotExists = "L'oggetto da modificare non esiste";
        private static readonly string AlreadyPatie = "Sei gia' un paziente";
        private static readonly string DocNotExists = "Il dottore selezionato non esiste";
        private static readonly string InsertAsDoct = "Ti ha inserito come il suo dottore: ";

        [HttpPost]
        public async Task<ActionResult> _login(string username, string password)
        {
            var result = await (from u in Db.Users
                                where u.Username.Equals(username)
                                && u.Password.Equals(password)
                                select u).ToListAsync();

            if (result.Count == 1)
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

                return Json(OkJson);
            }
            return Json(new JsonResponse()
            {
                Success = false,
                Message = InvalidLogIn
            });
        }

        [HttpPost]
        public ActionResult _logout()
        {
            HttpContext.Session.Clear();
            return Json(OkJson);
        }

        [HttpPost]
        public async Task<ActionResult> _register(User user)
        {
            try
            {
                Db.Insert(user);
                return await _login(user.Username, user.Password);
            }
            catch
            {
                return Json(new JsonResponse()
                {
                    Success = false,
                    Message = UsernameDupl
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> _notification(string username, string message, string redirectUrl = "#")
        {
            return await LoggedAction(() =>
            {
                Db.Insert(new Notification()
                {
                    Body = message,
                    Username = HttpContext.Session.GetString(Username),
                    Receiver = username,
                    Url = redirectUrl,
                    Time = DateTime.Now
                });
                return Json(OkJson);
            });
        }

        [HttpPut]
        public async Task<ActionResult> _notification(int id)
        {
            return await LoggedAction(() =>
            {
                JsonResponse response = OkJson;

                Notification note = Db.Notifications.Where(n => n.Id == id).ToArray().FirstOrDefault();
                if(note != null)
                {
                    note.Seen = DateTime.Now;
                    Db.Update(note);
                }
                else
                {
                    response.Success = false;
                    response.Message = ModNotExists;
                }
                return Json(response);
            });
        }

        [HttpPost]
        public async Task<ActionResult> _addDoc(string doctor)
        {
            return await LoggedAction(() =>
            {
                string username = HttpContext.Session.GetString(Username);
                var isAlreadyPatient = Db.Patients.Where(p => p.Username.Equals(username)).ToArray().FirstOrDefault() != null;
                if (isAlreadyPatient)
                    return Json(new JsonResponse()
                    {
                        Success = false,
                        Message = AlreadyPatie
                    });

                var docExist = Db.Doctors.Where(d => d.Username.Equals(doctor)).ToArray().FirstOrDefault() != null;
                if(!docExist)
                    return Json(new JsonResponse()
                    {
                        Success = false,
                        Message = DocNotExists
                    });

                Db.Insert(new Patient()
                {
                    Doctor = doctor,
                    Username = username
                });

                var a = _notification(doctor, InsertAsDoct + username);
                return Json(OkJson);
            });
        }

        [HttpPost]
        public async Task<ActionResult> _sendMessage(string receiver, string body)
        {
            return await LoggedAction(() => {
                string username = HttpContext.Session.GetString(Username);
                Message message = new Message()
                {
                    Receiver = receiver,
                    Body = body,
                    Time = DateTime.Now,
                    Username = username
                };

                Db.Insert(message);

                return Json(OkJson);
            });
        }

        [HttpPut]
        public async Task<ActionResult> _addNote(string patient, string text)
        {
            return await LoggedAccessDataOf(patient, true, () =>
            {
                var pat = Db.Patients.Where((p) => p.Username.Equals(patient)).FirstOrDefault();
                pat.Notes = text;
                Db.Update(pat);
                var a =  _notification(patient, NoteModified);

                return Json(OkJson);
            });
        }

        [HttpPut]
        public async Task<ActionResult> _minHeartToPatient(string patient, int value)
        {
            return await LoggedAccessDataOf(patient, true, () =>
            {
                var pat = Db.Patients.Where((p) => p.Username.Equals(patient)).FirstOrDefault();
                pat.MinHeart = value;
                Db.Update(pat);

                return Json(OkJson);
            });
        }

        [HttpPut]
        public async Task<ActionResult> _maxHeartToPatient(string patient, int value)
        {
            return await LoggedAccessDataOf(patient, true, () =>
            {
                var pat = Db.Patients.Where((p) => p.Username.Equals(patient)).FirstOrDefault();
                pat.MaxHeart = value;
                Db.Update(pat);

                return Json(OkJson);
            });
        }
    }
}