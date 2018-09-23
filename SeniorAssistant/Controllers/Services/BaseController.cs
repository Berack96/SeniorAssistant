using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Data;

namespace SeniorAssistant.Controllers
{
    public abstract class BaseController : Controller
    {
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
    }
}
