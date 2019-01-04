using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Data;
using System.Linq;
using System;

namespace SeniorAssistant.Controllers
{
    public abstract class BaseController : Controller
    {
        protected static readonly string MustBeLogged = "Devi essere loggato per vedere/modificare questo dato";
        protected static readonly string InvalidModel = "Modello non valido";
        protected static readonly string NoAuthorized = "Non sei autorizzato a vedere questi dati";
        protected static readonly string Username = "username";

        IDataContextFactory<SeniorDataContext> dbFactory;
        SeniorDataContext db;

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

        protected ActionResult Action(Func<ActionResult> success)
        {
            return ModelState.IsValid ?
                success.Invoke() :
                Json(new JsonResponse()
                {
                    Success = false,
                    Message = InvalidModel
                });
        }

        protected ActionResult LoggedAction(Func<ActionResult> success)
        {
            return Action(() =>
            {
                return IsLogged() ?
                    success.Invoke() :
                    Json(new JsonResponse()
                    {
                        Success = false,
                        Message = MustBeLogged
                    });
            });
        }

        protected ActionResult LoggedAccessDataOf(string username, Func<ActionResult> success)
        {
            return LoggedAction(() =>
            {
                var loggedUser = HttpContext.Session.GetString(Username);
                var condition = username.Equals(loggedUser);

                condition = condition || (from patient in Db.Patients
                                          where patient.Doctor.Equals(loggedUser) && patient.Username.Equals(username)
                                          select patient).ToArray().FirstOrDefault() != null;

                return condition ?
                success.Invoke() :
                Json(new JsonResponse()
                {
                    Success = false,
                    Message = NoAuthorized
                });
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
