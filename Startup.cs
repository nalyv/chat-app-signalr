using System;
using chat_application.Hubs;
using chat_application.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace chat_application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ChatDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddIdentity<AppIdentityUser, AppIdentityRole>()
                .AddEntityFrameworkStores<ChatDbContext>()
                .AddDefaultTokenProviders();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;//sayı zorunluluğu
                options.Password.RequireLowercase = true;//küçük harf
                options.Password.RequiredLength = 8;//minimum 8 karakter
                options.Password.RequireNonAlphanumeric = true;//alfanümerik olması
                options.Password.RequireUppercase = true;//büyük harf zorunluluğu

                options.Lockout.MaxFailedAccessAttempts = 5;//max hatalı giriş sayısı
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);//kullanıcı ne kadar süre boyunca sisteme giriş yapamasın
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/administrator/login";
                options.LogoutPath = "/administrator/log-out";
                options.AccessDeniedPath = "/administrator/access-denied";
                options.SlidingExpiration = true;
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = ".AspNetCoreIdentity",
                    Path = "/",
                    SameSite = SameSiteMode.Strict
                };
            });

            services.AddMvc();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSignalR(config => {
                config.MapHub<MessageHub>("/messages");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/");
            });
        }
    }
}
