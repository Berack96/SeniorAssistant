using System;
using System.Collections.Generic;
using LinqToDB;
using LinqToDB.DataProvider.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeniorAssistant.Configuration;
using SeniorAssistant.Data;
using SeniorAssistant.Models;
using SeniorAssistant.Models.Data;
using SeniorAssistant.Models.Users;
using SeniorAssistant.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            services.AddMvc();// config =>
//            {
//                var policy = new AuthorizationPolicyBuilder()
//                                 .RequireAuthenticatedUser()
//                                 .Build();
//                config.Filters.Add(new AuthorizeFilter(policy));
//            })
//            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSession();

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

//            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//                .AddCookie(options => {
//                options.LoginPath = "/";
//                options.AccessDeniedPath = "/";
//            });

//            services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>()
//                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var dbFactory = new SeniorDataContextFactory(
                dataProvider: SQLiteTools.GetDataProvider(),
                connectionString: Configuration.GetConnectionString("SeniorDb")
            );

            services.AddSingleton<IDataContextFactory<SeniorDataContext>>(dbFactory);
            SetupDatabase(dbFactory);
            FillDatabase(dbFactory);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseSession();
            app.UseStaticFiles();
//            app.UseAuthentication();

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
                await context.Response.WriteAsync("Oops, this page doesn't exist");
            });
        }

        void SetupDatabase(IDataContextFactory<SeniorDataContext> dataContext)
        {
            using (var db = dataContext.Create())
            {
                db.CreateTableIfNotExists<Heartbeat>();
                db.CreateTableIfNotExists<Sleep>();
                db.CreateTableIfNotExists<Step>();
                db.CreateTableIfNotExists<User>();
                db.CreateTableIfNotExists<Doctor>();
                db.CreateTableIfNotExists<Patient>();
                db.CreateTableIfNotExists<Notification>();
                db.CreateTableIfNotExists<Message>();
                db.CreateTableIfNotExists<Forgot>();
            }
        }

        void FillDatabase(IDataContextFactory<SeniorDataContext> dataContext)
        {
            using (var db = dataContext.Create())
            {
                Random rnd = new Random();

                List<User> users = new List<User>();

                List<Doctor> docs = db.Doctors.ToListAsync().Result;
                if (docs.Count == 0)
                {
                    users.Add(new User { Name = "Alfredo", LastName = "Parise", Email = "alfred.pary@libero.it", Username = "alfredigno", Password = "alfy" });
                    users.Add(new User { Name = "Edoardo", LastName = "Marzio", Email = "edo.marzio@libero.it", Username = "marzietto", Password = "edo64" });

                    docs.Add(new Doctor { Username = "alfredigno", Location = "Brasile" });
                    docs.Add(new Doctor { Username = "marzietto", Location = "Uganda" });

                    foreach (var doc in docs)
                        db.InsertOrReplace(doc);
                }

                List<Patient> patients = db.Patients.ToListAsync().Result;
                if (patients.Count == 0)
                {
                    const string baseUsername = "vecchio";
                    string[] names = { "Mario", "Giovanni", "Aldo", "Giacomo", "Marcello", "Filippo" };
                    string[] lastnames = { "Rossi", "Storti", "Baglio", "Poretti", "Marcelli", "Martelli" };
                    int count = 0;
                    for (count=0; count<names.Length; count++)
                    {
                        var username = baseUsername + count;
                        users.Add(new User { Name = names[count], LastName = lastnames[count], Username = username, Password = username, Email = username + "@email.st" });
                        patients.Add(new Patient { Username = username, Doctor = docs[rnd.Next(docs.Count)].Username });
                    }
                    
                    foreach (var patient in patients)
                        db.InsertOrReplace(patient);
                }

                var forgot = new Forgot()
                {
                    Question = "Quale animale ti piace di piu'?",
                    Answer = "Rayquaza"
                };
                foreach (var user in users)
                {
                    forgot.Username = user.Username;
                    db.InsertOrReplace(forgot);
                    db.InsertOrReplace(user);
                }

                DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                now = now.AddHours(DateTime.Now.Hour).AddMinutes(30);
                try
                {
                    double totalHours = 48;
                    try
                    {
                        DateTime maxTimeInDB = db.GetTable<Heartbeat>().MaxAsync(x => x.Time).Result;
                        TimeSpan span = now.Subtract(maxTimeInDB);
                        totalHours = span.TotalHours;
                    }
                    catch { }

                    for (int i = 0; i < totalHours; i++)
                    {
                        DateTime time = now.AddHours(-i);
                        foreach (var patient in patients)
                        {
                            if (time.Day != now.Day && time.Hour == 21)
                            {
                                db.Insert(new Sleep() { Username = patient.Username, Time = time, Value = rnd.Next(5 * 3600000, 9 * 3600000) });
                            }
                            db.Insert(new Heartbeat() { Username = patient.Username, Time = time, Value = rnd.Next(50, 120) });
                            db.Insert(new Step() { Username = patient.Username, Time = time, Value = rnd.Next(100, 500) });
                        }
                    }
                }
                catch { }
            }
        }
    }
}
