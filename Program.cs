using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Entities;
using Unitic_BE.Handlers;
using Unitic_BE.Options;
using Unitic_BE.Processors;
using Unitic_BE.Repositories;
using Unitic_BE.Services;

using Quartz;
using Unitic_BE.QuartzJob;
using Unitic_BE.QuartzScheduler;
using Unitic_BE.Seeder;



namespace Unitic_BE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            //DI service, repository
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IGoogleService, GoogleService>();
            builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
            builder.Services.AddScoped<IUniversityService, UniversityService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            //add quartz job DI
            builder.Services.AddScoped<IEventJobScheduler, QuartzEventJobScheduler>();
            builder.Services.AddScoped<UpdateEventStatusJob>();

            //Add Quartz
            builder.Services.AddQuartz(opt =>
            {

                opt.UsePersistentStore(s =>
                {
                    s.UseProperties = true; //cho phép sử dụng properties
                    s.UseSqlServer(builder.Configuration.GetConnectionString("QuartzConnection"));
                    s.UseNewtonsoftJsonSerializer(); //sử dụng Newtonsoft.Json để serialize/deserialize job data
                });
            });
            // Add the Quartz.NET hosted service
            builder.Services.AddQuartzHostedService(q =>
            {
                q.WaitForJobsToComplete = true;
            });

            //lấy JwtOptions từ appsettings.json
            //ánh xạ vào property trong JwtOptions class qua DI
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(JwtOptions.JwtOptionsKey));

            //add validate
            builder.Services.AddIdentity<User, IdentityRole<string>>(opt =>
            {
                opt.Lockout.AllowedForNewUsers = false;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(0);
                opt.Lockout.MaxFailedAccessAttempts = int.MaxValue;


            }).AddEntityFrameworkStores<ApplicationDbContext>();
            // .AddUserValidator<CustomUserValidator>();
            //add custom user validator
            builder.Services.AddScoped<CustomValidator>();

            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });



            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Google Authentication
            .AddCookie()
            .AddGoogleOpenIdConnect(options =>
            {
                options.ClientId = builder.Configuration["Google:ClientId"];
                options.ClientSecret = builder.Configuration["Google:ClientSecret"];
                options.CallbackPath = "/signin-google";
                //options.CallbackPath = "/Unitic/Auth/google-response";
                // options.SaveTokens = true;
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            // JWT Authentication (for API endpoints)
            .AddJwtBearer(options =>
            {
                //ánh xạ JwtOptions từ appsettings.json vào jwtOptions để lấy jwtOption
                var jwtOptions = builder.Configuration.GetSection(JwtOptions.JwtOptionsKey)
                    .Get<JwtOptions>() ?? throw new ArgumentException(nameof(JwtOptions));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero, // không cho phép clock skew, tức là token hết hạn ngay lập tức
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };

            });

            builder.Services.AddHttpContextAccessor();


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole(IdentityRoleConstants.Admin);
                });
                options.AddPolicy("Moderator", policy =>
                {
                    policy.RequireRole(IdentityRoleConstants.Moderator);
                });
                options.AddPolicy("Organizer", policy =>
                {
                    policy.RequireRole(IdentityRoleConstants.Organizer);
                });
                options.AddPolicy("Staff", policy =>
                {
                    policy.RequireRole(IdentityRoleConstants.Staff);
                });
                options.AddPolicy("User", policy =>
                {
                    policy.RequireRole(IdentityRoleConstants.User);
                });

            });

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails(); // <<== Cái này bắt buộc để tránh lỗi cấu hình

            //Setting Gmail Options
            builder.Services.Configure<GmailOptions>(
                builder.Configuration.GetSection(GmailOptions.GmailOptionsKey)
            );
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            //Tạo scope để chạy seeder khởi tạo data ban đầu sau đó dispose scope
            //khởi tạo data mỗi khi chạy app
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                //cách khác để chạy đồng bộ trong hàm không phải async
                Seeder.Seeder.SeedAdminDataAsync(userManager).GetAwaiter().GetResult();


            }

            app.UseHttpsRedirection(); //chuyển hướng http tới https
            app.UseExceptionHandler();
            
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
