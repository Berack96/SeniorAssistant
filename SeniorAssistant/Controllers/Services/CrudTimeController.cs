using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeniorAssistant.Controllers.Services
{
    public class CrudTimeController<TEntity> : BaseController
        where TEntity : class, IHasTime
    {
        [HttpGet]
        public async Task<IEnumerable<TEntity>> Read() => await Db.GetTable<TEntity>().ToListAsync();

        [HttpGet("{username}")]
        public async Task<IEnumerable<TEntity>> Read(string username) => await Db.GetTable<TEntity>().Where(e => e.Username.Equals(username)).ToListAsync();

        [HttpGet("{username}/{date:regex((today|\\d{{4}}-\\d{{2}}-\\d{{2}}))}/{hour:range(0, 23)?}")]
        public async Task<IEnumerable<TEntity>> Read(string username, string date, int hour = -1)
        {
            DateTime time = (date.Equals("today") ? DateTime.Now : DateTime.ParseExact(date, "yyyy-MM-dd", null) );

            return await Db.GetTable<TEntity>().Where(e =>
                e.Username.Equals(username) &&
                (time.Year == 0 || e.Time.Year == time.Year) &&
                (time.Month == 0 || e.Time.Month == time.Month) &&
                (time.Day == 0 || e.Time.Day == time.Day) &&
                (hour < 0 || e.Time.Hour == hour)
                ).ToListAsync();
        }

        /*
        [HttpPost("{username}")]
        public async Task Create([FromBody]TEntity item) => await Db.InsertAsync(item);

        [HttpPut("{username}/{date}/{time}")]
        public async Task Update(string username, [FromBody]TEntity item)
        {
            item.Username = username;

            await Db.UpdateAsync(item);
        }

        [HttpDelete("{username}")]
        public async Task Delete(string username) => await Db.GetTable<TEntity>().Where(c => c.Username.Equals(username)).DeleteAsync();
        */
    }
}
