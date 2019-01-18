using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Data;
using SeniorAssistant.Models.Users;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace SeniorAssistant.Controllers
{
    public abstract class BaseController : Controller
    {
        protected static readonly string MustBeLogged = "Devi essere loggato per vedere/modificare questo dato";
        protected static readonly string InvalidModel = "Modello non valido";
        protected static readonly string NoAuthorized = "Non sei autorizzato ad accedere a questi dati";
        protected static readonly string ExceptionSer = "Il server ha riscontrato un problema: ";
        protected static readonly string Username = "username";
        protected readonly JsonResponse OkJson = new JsonResponse();

        private IDataContextFactory<SeniorDataContext> dbFactory;
        private SeniorDataContext db;

        protected T TryResolve<T>() => (T)HttpContext.RequestServices.GetService(typeof(T));

        protected IDataContextFactory<SeniorDataContext> DbFactory => dbFactory ?? (dbFactory = TryResolve<IDataContextFactory<SeniorDataContext>>());

        protected SeniorDataContext Db => db ?? (db = DbFactory.Create());

        protected override void Dispose(bool disposing)
        {
            db?.Dispose();

            base.Dispose(disposing);
        }

        protected bool IsLogged()
        {
            return HttpContext.Session.GetString(Username) != null;
        }
        
        protected async Task<ActionResult> LoggedAction(Func<ActionResult> success)
        {
            try
            {
                if (IsLogged())
                    return success.Invoke();

                return Json(new JsonResponse()
                {
                    Success = false,
                    Message = MustBeLogged
                });
            }
            catch (Exception e)
            {
                return Json(new JsonResponse()
                {
                    Success = false,
                    Message = ExceptionSer + Environment.NewLine +
                              e.Message + Environment.NewLine +
                              e.StackTrace + Environment.NewLine +
                              e.TargetSite + Environment.NewLine +
                              e.InnerException
                });
            }
        }

        protected async Task<ActionResult> LoggedAccessDataOf(string username, bool patients, Func<ActionResult> success)
        {
            return await LoggedAction(() =>
            {
                var session = HttpContext.Session.GetString(Username);
                var condition = username.Equals(session);
                var query = from patient in Db.Patients
                            where patient.Doctor.Equals(session) && patient.Username.Equals(username)
                            select patient;
                var num = query.ToList().Count();
                condition = condition || (patients && num != 0);

                return condition ?
                success.Invoke() :
                Json(new JsonResponse()
                {
                    Success = false,
                    Message = NoAuthorized
                });
            });
        }

        static protected JsonResponse LoggedAction(SeniorDataContext db, string session, Func<JsonResponse> success)
        {
            try
            {
                return session != null ?
                    success.Invoke() :
                    new JsonResponse()
                    {
                        Success = false,
                        Message = MustBeLogged
                    };
            }
            catch (Exception e)
            {
                return new JsonResponse()
                {
                    Success = false,
                    Message = ExceptionSer + e.Message
                };
            }
        }

        static protected JsonResponse LoggedAccessDataOf(SeniorDataContext db, string session, string username, bool patients, Func<JsonResponse> success)
        {
            return LoggedAction(db, session, () =>
            {
                var condition = username.Equals(session);
                condition = condition || (patients && (from patient in db.Patients
                                                       where patient.Doctor.Equals(session) && patient.Username.Equals(username)
                                                       select patient).ToArray().FirstOrDefault() != null);

                return condition ?
                success.Invoke() :
                new JsonResponse()
                {
                    Success = false,
                    Message = NoAuthorized
                };
            });
        }
    }

    public class JsonResponse
    {
        public JsonResponse(bool success=true, string message="")
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
