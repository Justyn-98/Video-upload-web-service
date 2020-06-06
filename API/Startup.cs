using API.DataAccessLayer;
using API.DataSeedServices;
using API.Models.Entities;
using API.Services.AccountManagementService;
using API.Services.AccountService;
using API.Services.CommentsService;
using API.Services.LikesService;
using API.Services.PlayListsService;
using API.Services.UserRolesServices;
using API.Services.UserRolesServices.Interfaces;
using API.Services.UserSignInService;
using API.Services.VideoCategoriesService;
using API.Services.VideosService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace API
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
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options
                => options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                /*Password settings.*/
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
            });


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtToken:SecretKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddAuthorization();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IVideoCategoryService, VideoCategoryService>();
            services.AddScoped<IRolesCreateService, RolesCreateService>();
            services.AddScoped<IDefaultAdminService, DefaultAdminService>();
            services.AddScoped<IAccountDetailsService, AccountDeatilsService>();
            services.AddScoped<IVideosService, VideosService>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<IPlaylistService, PlayListsSerivce>();
            services.AddScoped<ILikesService, LikesService>();
            services.AddScoped<IDataSeedService, DataSeedService>();
            services.AddScoped<IUserSignInHelper, UserSignInHelper>();





        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseCors("MyPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

       
            var roleService = serviceProvider.GetRequiredService<IRolesCreateService>();
            roleService.AddRoles();

            var defaultAdminService = serviceProvider.GetRequiredService<IDefaultAdminService>();
            defaultAdminService.CreateTestUser();

            var dataSeedService =serviceProvider.GetRequiredService<IDataSeedService>();
            //var result = dataSeedService.SeedData();
            //result.Wait();

        }
    }
}

