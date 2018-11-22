using System;
using System.Collections.Generic;
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
using SeniorAssistant.Models;
using SeniorAssistant.Extensions;
using Swashbuckle.AspNetCore.Swagger;

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Senior REST APIs",
                    Description = "REST APIs for old people",
                    TermsOfService = "None"
                });

                // Se decommento ottengo un'eccezione quando ho un controller (es. CategoriesController) che estende un altro controller.
                // Set the comments path for the Swagger JSON and UI.
                //var basePath = AppContext.BaseDirectory;
                //var xmlPath = Path.Combine(basePath, "REST.xml");
                //c.IncludeXmlComments(xmlPath);
            });

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
                        new MenuItem("Sleep", "/sleep"),
                        new MenuItem("Step", "/step")
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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/v1.json", "V1");
            });

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
                const string baseUsername = "vecchio";
                string[] users = { "Mario", "Giovanni", "Aldo", "Giacomo", "Marcello", "Filippo" };
                    
                db.CreateTableIfNotExists<Heartbeat>();
                db.CreateTableIfNotExists<Sleep>();
                db.CreateTableIfNotExists<Step>();
                try
                {
                    db.CreateTable<User>();
                    int count = 0;
                    foreach (string user in users)
                    {
                        db.InsertOrReplace(new User { Name = user, Username = baseUsername + (count == 0 ? "" : "" + count) });
                        count++;
                    }
                }
                catch
                { }

                Random rnd = new Random();
                DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                now = now.AddHours(DateTime.Now.Hour).AddMinutes(30);
                try
                {
                    double totalHours = 50;
                    try {
                        DateTime maxTimeInDB = db.GetTable<Heartbeat>().MaxAsync(x => x.Time).Result;
                        TimeSpan span = now.Subtract(maxTimeInDB);
                        totalHours = span.TotalHours;
                    } catch { }
                
                    for (int i = 0; i<totalHours; i++)
                    {
                        DateTime time = now.AddHours(-i);
                        for (int num = 0; num < users.Length; num++)
                        {
                            string user = baseUsername + num;

                            if (time.Day != now.Day)
                            {
                                db.Insert(new Sleep() { Username = user, Time = time, Value = rnd.Next(5 * 3600000, 9 * 3600000) });
                            }
                            db.Insert(new Heartbeat() { Username = user, Time = time, Value = rnd.Next(50, 120) });
                            db.Insert(new Step() { Username = user, Time = time, Value = rnd.Next(100, 500) });
                        }
                        if (time.Day != now.Day)
                        {
                            now = now.AddDays(-1);
                        }
                    }
                }
                catch { }
            }
        }
    }
}
