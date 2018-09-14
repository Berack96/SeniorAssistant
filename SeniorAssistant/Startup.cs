using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinqToDB;
using LinqToDB.DataProvider.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeniorAssistant.Configuration;
using SeniorAssistant.Data;
using SeniorAssistant.Extensions;
using SeniorAssistant.Models;

namespace SeniorAssistant
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.Configure<Kendo>(Configuration.GetSection("kendo"));
            services.Configure<Theme>(Configuration.GetSection("theme"));

            services.AddSingleton<IEnumerable<IMenuItem>>(new IMenuItem[]
            {
                new SubMenu
                {
                    Text = "Link veloci",
                    Items = new MenuItem[]
                    {
                        new MenuItem("User", "/"),
                        new MenuItem("Heartbeat", "/heartbeat"),
                        new MenuItem("Sleep"),
                        new MenuItem("Step")
                    }
                },
            });

            var dbFactory = new SeniorDataContextFactory(
                dataProvider: SQLiteTools.GetDataProvider(),
                connectionString: Configuration.GetConnectionString("SeniorDb")
            );

            services.AddSingleton<IDataContextFactory<SeniorDataContext>>(dbFactory);
            SetupDatabase(dbFactory);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();

            /*
             Shortcut per:

             routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
            */
            //app.UseMvcWithDefaultRoute();
            app.UseMvc();
            
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Oops, something went wrong");
            });
        }

        void SetupDatabase(IDataContextFactory<SeniorDataContext> dataContext)
        {
            using (var db = dataContext.Create())
            {
                try
                {
                    db.CreateTable<User>();
                    db.CreateTable<Heartbeat>();

                    const string baseUsername = "vecchio";

                    string[] users = { "Mario", "Giovanni", "Aldo", "Giacomo", "Marcello", "Filippo" };
                    int num = 0;
                    foreach (string user in users)
                    {
                        db.InsertOrReplace(new User { Name = user, Username = baseUsername+(num==0?"":""+num) });
                        num++;
                    }

                    Random rnd = new Random();
                    for (int i=0; i<50; i++)
                    {
                        int random = rnd.Next(num);
                        string user = baseUsername + (random==0? "":""+random);
                        DateTime time = DateTime.Now.AddHours(rnd.Next(-24, +24));
                        int beat = rnd.Next(50, 90);

                        Heartbeat heart = new Heartbeat { Username = user, Time = time, Value = beat };
                        db.Insert(heart);
                    }
                }
                catch (SqliteException)
                {
                    // Do nothing
                }

            }
        }
    }
}
