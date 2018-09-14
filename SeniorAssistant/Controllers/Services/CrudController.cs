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
        [HttpGet]
        public async Task<IEnumerable<TEntity>> Read() => await Db.GetTable<TEntity>().ToListAsync();

        [HttpGet("{username}")]
        public async Task<TEntity> Read(string username) => await Db.GetTable<TEntity>().FirstOrDefaultAsync(c => c.Username.Equals(username));

        [HttpPost]
        public async Task Create([FromBody]TEntity item) => await Db.InsertAsync(item);

        [HttpPut("{username}")]
        public async Task Update(string username, [FromBody]TEntity item)
        {
            item.Username = username;

            await Db.UpdateAsync(item);
        }

        [HttpDelete("{username}")]
        public async Task Delete(string username) => await Db.GetTable<TEntity>().Where(c => c.Username.Equals(username)).DeleteAsync();
    }
}
