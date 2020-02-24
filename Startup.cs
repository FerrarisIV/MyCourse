using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCourse.Models.Services.Application;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Customizations.ModelBinders;

namespace MyCourse
{
    public class Startup
    {
        public Startup(IConfiguration configuration) 
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddResponseCaching();

            services.AddMvc(options => 
                {
                    var homeProfile = new CacheProfile();
                    //homeProfile.Duration = Configuration.GetValue<int>("ResponseCache:Home:Duration");
                    //homeProfile.Location = Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location")
                    //homeProfile.VaryByQueryKeys = new string[] { "page" };

                    Configuration.Bind("ResponseCache:Home", homeProfile);
                    options.CacheProfiles.Add("Home", homeProfile);

                    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
                }
            )
            #if DEBUG
            .AddRazorRuntimeCompilation()
            #endif
            ;

            services.AddTransient<ICourseService, AdoNetCourseService>();
            //services.AddTransient<ICourseService, EfCoreCourseService>();
            services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();
            //services.AddTransient<ICachedCourseService, MemoryCacheCourseService>();

            
            
            //services.AddDbContext<MyCourseDbContext>();
            services.AddDbContextPool<MyCourseDbContext>(optionsBuilder => {
                string connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("Default");
                optionsBuilder.UseSqlite(connectionString);
            });

            
                

            //Options
            services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<CoursesOptions>(Configuration.GetSection("Courses"));

            services.Configure<MemoryCacheOptions>(Configuration.GetSection("MemoryCache"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {     
                app.UseExceptionHandler("/Error");           
                //app.UseDeveloperExceptionPage();
                //app.UseHttpsRedirection();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();            
            app.UseRouting();
            app.UseResponseCaching();

            app.UseEndpoints(routeBuilder =>
            {
                routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //routeBuilder.MapRazorPages();
            });    
        }
    }
}
