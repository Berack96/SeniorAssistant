using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeniorAssistant.Controllers.Services
{
    public abstract class CrudController<TEntity> : BaseController
        where TEntity : class, IHasUsername
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> Read(string username)
        {
            return LoggedAccessDataOf(username, () =>
            {
                return Json(Db.GetTable<TEntity>().Where((u) => u.Username.Equals(username)).ToArray());
            });
        }

        [HttpPut("{username}")]
        public async Task<IActionResult> Update(string username, [FromBody] TEntity entity)
        {
            return LoggedAccessDataOf(username, () =>
            {
                entity.Username = username;
                Db.Update(entity);
                return Json(OkJson);
            }, false);
        }
    }
}
