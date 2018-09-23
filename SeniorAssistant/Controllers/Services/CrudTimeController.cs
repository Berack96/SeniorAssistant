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
        static readonly object Empty = new { };

        [HttpGet]
        public async Task<IEnumerable<TEntity>> Read() => await Db.GetTable<TEntity>().ToListAsync();

        [HttpGet("{username}")]
        public async Task<IEnumerable<TEntity>> Read(string username) => await Db.GetTable<TEntity>().Where(e => e.Username.Equals(username)).ToListAsync();

        [HttpGet("{username}/{date:regex((today|\\d{{4}}-\\d{{2}}-\\d{{2}}))}/{hour:range(0, 23)?}")]
        public async Task<IEnumerable<TEntity>> Read(string username, string date, int hour = -1) => await Read(username, date, date, hour);

        [HttpGet("{username}/{from:regex((today|\\d{{4}}-\\d{{2}}-\\d{{2}}))}/{to:regex((today|\\d{{4}}-\\d{{2}}-\\d{{2}}))}/{hour:range(0, 23)?}")]
        public async Task<IEnumerable<TEntity>> Read(string username, string from, string to, int hour = -1)
        {
            try
            {
                DateTime dateFrom = (from.Equals("today") ? DateTime.Now : DateTime.ParseExact(from, "yyyy-MM-dd", null));
                DateTime dateTo = (to.Equals("today") ? DateTime.Now : DateTime.ParseExact(to, "yyyy-MM-dd", null));

                return await Db.GetTable<TEntity>().Where(e => e.Username.Equals(username) && dateFrom.Date<=e.Time.Date && dateTo.Date>=e.Time.Date && (hour < 0 || e.Time.Hour == hour)).ToListAsync();
            }
            catch
            {
                return new List<TEntity>();
            }
        }

        [HttpGet("{username}/last/{hour:min(1)}")]
        public async Task<IEnumerable<TEntity>> Read(string username, int hour)
        {
            DateTime date = DateTime.Now.AddHours(-hour);
            return await Db.GetTable<TEntity>().Where(e => e.Username.Equals(username) && date <= e.Time).ToListAsync();
        }

        [NonAction]
        public async Task<TEntity> Read(string username, DateTime date) => await Db.GetTable<TEntity>().FirstOrDefaultAsync(e => e.Username.Equals(username) && date == e.Time);

        [HttpPost]
        public async Task Create([FromBody]TEntity item) => await Db.InsertAsync(item);
        
        [HttpPut]
        public async Task<object> Update([FromBody]TEntity item)
        {
            var e = await Read(item.Username, item.Time);
            if (e == null)
            {
                await Create(item);
            }
            else
            {
                await Db.UpdateAsync(item);
            }

            return Empty;
        }

        /*
        [HttpDelete("{username}")]
        public async Task Delete(string username) => await Db.GetTable<TEntity>().Where(c => c.Username.Equals(username)).DeleteAsync();
        */
    }
}
