using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using SeniorAssistant.Models.Data;
using SeniorAssistant.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeniorAssistant.Controllers.Services
{
    public class CrudTimeController<TEntity> : CrudController<TEntity>
        where TEntity : class, IHasTime
    {
        private static readonly string DateNotCorrect = "Il formato della data non e' corretto";
        private static readonly string AnomalDataHear = "Valore dei battiti cardiaci anomalo";

        [HttpGet("{username}/{date:regex((today|\\d{{4}}-\\d{{2}}-\\d{{2}}))}/{hour:range(0, 23)?}")]
        public async Task<IActionResult> Read(string username, string date, int hour = -1) => await Read(username, date, date, hour);

        [HttpGet("{username}/{from:regex((today|\\d{{4}}-\\d{{2}}-\\d{{2}}))}/{to:regex((today|\\d{{4}}-\\d{{2}}-\\d{{2}}))}/{hour:range(0, 23)?}")]
        public async Task<IActionResult> Read(string username, string from, string to, int hour = -1)
        {
            return await LoggedAccessDataOf(username, true, () =>
            {
                try
                {
                    DateTime dateFrom = (from.Equals("today") ? DateTime.Now : DateTime.ParseExact(from, "yyyy-MM-dd", null));
                    DateTime dateTo = (to.Equals("today") ? DateTime.Now : DateTime.ParseExact(to, "yyyy-MM-dd", null));

                    return Json((from entity in Db.GetTable<TEntity>()
                                 where entity.Username.Equals(username)
                                    && dateFrom.Date <= entity.Time.Date
                                    && dateTo.Date >= entity.Time.Date
                                    && (hour < 0 || entity.Time.Hour == hour)
                                 select entity).ToArray());
                }
                catch
                {
                    return Json(new JsonResponse(false, DateNotCorrect));
                }
            });
        }

        [HttpGet("{username}/last/{hour:min(1)}")]
        public async Task<IActionResult> Read(string username, int hour)
        {
            return await LoggedAccessDataOf(username, true, () =>
            {
                DateTime date = DateTime.Now.AddHours(-hour);
                return Json((from entity in Db.GetTable<TEntity>()
                             where entity.Username.Equals(username)
                                && date <= entity.Time
                             select entity).ToArray());
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(TEntity item)
        {
            return await LoggedAccessDataOf(item.Username, false, () =>
            {
                Db.Insert(item);

                if (item is Heartbeat temp)
                {
                    var result = (from p in Db.Patients
                                  where p.Username.Equals(item.Username)
                                  select p).ToArray().FirstOrDefault();
                    if (result.MinHeart > temp.Value || result.MaxHeart < temp.Value)
                    {
                        var date = WebUtility.UrlEncode(item.Time.ToString("yyyy/MM/dd"));
                        Db.Insert(new Notification() {
                            Username = item.Username,
                            Receiver = result.Doctor,
                            Body = item.Username + ":" + AnomalDataHear,
                            Url = "/user/" + item.Username + "?from=" + date + "&to=" + date,
                            Time = DateTime.Now
                        });
                    }
                }

                return Json(OkJson);
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(TEntity item)
        {
            return await LoggedAccessDataOf(item.Username, false, () =>
            {
                if (Read(item.Username, item.Time) == null)
                {
                    var a = Create(item);
                }
                else
                {
                    Db.UpdateAsync(item);
                }

                return Json(OkJson);
            });
        }

        [NonAction]
        private TEntity Read(string username, DateTime date) => Db.GetTable<TEntity>().FirstOrDefault(e => e.Username.Equals(username) && date == e.Time);
    }
}
