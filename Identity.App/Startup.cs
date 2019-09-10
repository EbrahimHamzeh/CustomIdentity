using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Identity.App.Models.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Identity.App.Models;
using Identity.App.Services;
using Identity.App.Services.Interface;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Identity.App.Settings;
using DNTCommon.Web.Core;
using DNTCaptcha.Core;
using Identity.App.Extention;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Principal;
using System.Security.Claims;

namespace Identity.App
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
            services.Configure<AppSettings>(options => Configuration.Bind(options));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
                                                .Replace("|DataDirectory|", Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "app_data")),
                            sqlServerOptionsAction =>
                            {
                                int minute = (int)TimeSpan.FromMinutes(3).TotalSeconds;
                                sqlServerOptionsAction.CommandTimeout(minute);
                                sqlServerOptionsAction.EnableRetryOnFailure();
                            }
                );
            });

            var siteSettings = services.GetSiteSettings();
            services.AddIdentity<User, Role>(identityBuilder => {
                identityBuilder.Password.RequireDigit = siteSettings.PasswordOptions.RequireDigit;
                identityBuilder.Password.RequireLowercase = siteSettings.PasswordOptions.RequireLowercase;
                identityBuilder.Password.RequireNonAlphanumeric = siteSettings.PasswordOptions.RequireNonAlphanumeric;
                identityBuilder.Password.RequireUppercase = siteSettings.PasswordOptions.RequireUppercase;
                identityBuilder.Password.RequiredLength = siteSettings.PasswordOptions.RequiredLength;
                identityBuilder.Password.RequiredUniqueChars = siteSettings.PasswordOptions.RequiredUniqueChars;

                identityBuilder.Lockout.AllowedForNewUsers = siteSettings.LockoutOptions.AllowedForNewUsers;
                identityBuilder.Lockout.DefaultLockoutTimeSpan = siteSettings.LockoutOptions.DefaultLockoutTimeSpan;
                identityBuilder.Lockout.MaxFailedAccessAttempts = siteSettings.LockoutOptions.MaxFailedAccessAttempts;

                identityBuilder.User.RequireUniqueEmail = true;

                //identityBuilder.SignIn.RequireConfirmedEmail = siteSettings.EnableEmailConfirmation;
            }).AddUserManager<ApplicationUserManager>()
              .AddRoleStore<ApplicationRoleStore>()
              .AddRoleManager<ApplicationRoleManager>()
              .AddSignInManager<ApplicationSignInManager>()
              .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(cookieAuthenticationOptions => {
                cookieAuthenticationOptions.Cookie.Name = siteSettings.CookieOptions.CookieName;
                cookieAuthenticationOptions.Cookie.HttpOnly = true;
                cookieAuthenticationOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                cookieAuthenticationOptions.Cookie.SameSite = SameSiteMode.Lax;
                cookieAuthenticationOptions.Cookie.IsEssential = true; //  this cookie will always be stored regardless of the user's consent

                cookieAuthenticationOptions.ExpireTimeSpan = siteSettings.CookieOptions.ExpireTimeSpan;
                cookieAuthenticationOptions.SlidingExpiration = siteSettings.CookieOptions.SlidingExpiration;
                cookieAuthenticationOptions.LoginPath = siteSettings.CookieOptions.LoginPath;
                cookieAuthenticationOptions.LogoutPath = siteSettings.CookieOptions.LogoutPath;
                cookieAuthenticationOptions.AccessDeniedPath = siteSettings.CookieOptions.AccessDeniedPath;

                // var provider = services.BuildServiceProvider();
                // var ticketStore = provider.GetService<ITicketStore>();
                // ticketStore.CheckArgumentIsNull(nameof(ticketStore));
                // cookieAuthenticationOptions.SessionStore = ticketStore; // To manage large identity cookies
            });

            services.AddScoped<IUnitOfWork, AppDbContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPrincipal>(provider =>
                provider.GetService<IHttpContextAccessor>()?.HttpContext?.User ?? ClaimsPrincipal.Current);

            services.AddScoped<ILookupNormalizer, CustomNormalizer>();
            
            services.AddScoped<IApplicationUserStore, ApplicationUserStore>();
            services.AddScoped<UserStore<User, Role, AppDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>, ApplicationUserStore>();

            services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
            services.AddScoped<UserManager<User>, ApplicationUserManager>();

            services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
            services.AddScoped<RoleManager<Role>, ApplicationRoleManager>();

            services.AddScoped<IApplicationSignInManager, ApplicationSignInManager>();
            services.AddScoped<SignInManager<User>, ApplicationSignInManager>();

            services.AddScoped<IApplicationRoleStore, ApplicationRoleStore>();
            services.AddScoped<RoleStore<Role, AppDbContext, int, UserRole, RoleClaim>, ApplicationRoleStore>();

            services.AddScoped<IIdentityDbInitializer, IdentityDbInitializer>();

            services.AddScoped<IEmailSender, AuthMessageSender>();
            services.AddScoped<ISmsSender, AuthMessageSender>();

            services.AddScoped<IUserValidator<User>, CustomUserValidator>();
            services.AddScoped<UserValidator<User>, CustomUserValidator>();

            services.AddScoped<IUserClaimsPrincipalFactory<User>, ApplicationClaimsPrincipalFactory>();
            services.AddScoped<UserClaimsPrincipalFactory<User, Role>, ApplicationClaimsPrincipalFactory>();

            services.AddSingleton<IMvcControllerDiscovery, MvcControllerDiscovery>();

            services.AddDNTCommonWeb();
            services.AddDNTCaptcha(options => options.UseCookieStorageProvider());

            services.AddMvc(options => {
                options.Filters.Add(typeof(DynamicAuthorizationFilter));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
