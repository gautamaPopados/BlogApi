using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication1.Middleware;
using WebApplication1.Services;
using WebApplication1.Services.IServices;
using WebApplication1.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using Quartz;
using WebApplication1.Jobs;
using MailKit.Net.Smtp;
using WebApplication1.Data.BannedToken;
using WebApplication1.Data.Repository;
using WebApplication1.Helpers.IHelpers;
using WebApplication1.Helpers;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Data.Entities;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDbContext<AddressContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AddressConnection")));
        builder.Services.AddSingleton(new RedisRepository(builder.Configuration.GetConnectionString("RedisDatabase")));

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;

            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ@.123456789";
        });

        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddUserManager<UserManager<User>>()
            .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
            .AddSignInManager<SignInManager<User>>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<TokenProps>();
        builder.Services.AddScoped<ITokenHelper, TokenHelper>();
        builder.Services.AddScoped<TokenBlacklistFilterAttribute>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<SmtpClient>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ICommunityService, CommunityService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IAddressService, AddressService>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IAuthorService, AuthorService>();
        builder.Services.AddScoped<UserRepository>();
        //builder.ConfigureRedisDb();
        builder.configureJWTAuth();

        var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

        builder.Services.AddControllers();
        builder.Services.AddControllers(opt =>
        {
            opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var basePath = AppContext.BaseDirectory;

            var xmlPath = Path.Combine(basePath, "WebApplication1.xml");
            options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Type = SecuritySchemeType.Http,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = JwtBearerDefaults.AuthenticationScheme,

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
            });
        });

        builder.Services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            var jobKey = new JobKey("EmailNotificationJob");
            q.AddJob<EmailNotificationJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("EmailNotificationTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(30)
                    .RepeatForever()));
        });

        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        builder.Services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();
        var app = builder.Build();

        DatabaseInitilaizer.Seed(app);

        using var serviceScope = app.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
        dbContext?.Database.Migrate();

        var userDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
        userDbContext?.Database.Migrate();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        app.MapControllers();

        app.Run();
    }
}