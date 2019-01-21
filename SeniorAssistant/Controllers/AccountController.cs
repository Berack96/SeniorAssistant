using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
using SeniorAssistant.Controllers;
using LinqToDB;
using System.Linq;
using System;
using SeniorAssistant.Models.Users;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Net.Http.Headers;

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
        private static readonly string DefaultImage = "/uploads/default.jpg";
        private static readonly string UploadsDirec = "/uploads/";

        [HttpPost]
        public async Task<ActionResult> _login(string username, string password)
        {
            try
            {
                var user = await (from u in Db.Users
                                  where u.Username.Equals(username)
                                  && u.Password.Equals(password)
                                  select u).FirstOrDefaultAsync();

                if (user != null)
                {
                    HttpContext.Session.SetString(Username, username);
                    HttpContext.Session.SetString("email", user.Email);
                    HttpContext.Session.SetString("name", user.Name);
                    HttpContext.Session.SetString("lastname", user.LastName);
                    HttpContext.Session.SetString("avatar", user.Avatar ?? DefaultImage);

                    var isDoc = (from d in Db.Doctors
                                 where d.Username.Equals(username)
                                 select d).ToArray().FirstOrDefault() != null;
                    HttpContext.Session.SetString("role", isDoc ? "doctor" : "patient");

                    return Json(OkJson);
                }
                return Json(new JsonResponse()
                {
                    Success = false,
                    Message = InvalidLogIn
                });
            }
            catch (Exception e)
            {
                return Json(new JsonResponse()
                {
                    Success = false,
                    Message = e.Message + " " +e.Source + "</br>"+ e.StackTrace
                });
            }
        }

        [HttpPost]
        public ActionResult _logout()
        {
            HttpContext.Session.Clear();
            return Json(OkJson);
        }

        [HttpPost]
        public async Task<ActionResult> _register(User user, string code = "")
        {
            try
            {
                user.Avatar = DefaultImage;
                Db.Insert(user);
                if (code != null && code.Equals("444442220"))
                {
                    Db.Insert(new Doctor
                    {
                        Username = user.Username
                    });
                };
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
        
        [HttpPost]
        public async Task<ActionResult> _save(IEnumerable<IFormFile> files)
        {
            return await LoggedAction(() =>
            {
                if (files != null)
                {
                    var loggedUser = HttpContext.Session.GetString(Username);
                    foreach (var file in files)
                    {
                        var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                        
                        // We are only interested in the file name.
                        var fileName = loggedUser + Path.GetExtension(fileContent.FileName.ToString().Trim('"'));

                        var physicalPath = "wwwroot" + UploadsDirec;
                        Directory.CreateDirectory(physicalPath);

                        physicalPath = Path.Combine(physicalPath, fileName);
                        var externalPath = Path.Combine(UploadsDirec, fileName);
                        
                        using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var user = (from u in Db.Users
                                    where u.Username.Equals(loggedUser)
                                    select u).FirstOrDefault();
                        user.Avatar = externalPath;
                        HttpContext.Session.SetString("avatar", externalPath);
                        Db.Update(user);
                    }
                }

                return Json(OkJson);
                /*
                
                if (file.Length > 0)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    var name = loggedUser + ".jpg";
                    var path = Path.Combine(("/uploads/"), name);
                    var stream = new FileStream(path, FileMode.Create);
                    file.CopyTo(stream);
                    var user = (from u in Db.Users
                                where u.Username.Equals(loggedUser)
                                select u).FirstOrDefault();
                    user.Avatar = path;

                    Db.Update(User);
                }

                return Json(OkJson);
            });

            /*
            var loggedUser = HttpContext.Session.GetString(Username);

            long size = file.Length;

            // full path to file in temp location
            var filePathPart = Path.GetDirectoryName("~/AdminLTE-2.4.3/dist/img/");
            var fileName = Path.GetFileName(loggedUser + ".jpg");
            var filePath = Path.Combine(filePathPart,fileName);
            if (size > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Json(new JsonResponse());
            */
            });
        }
    }
}