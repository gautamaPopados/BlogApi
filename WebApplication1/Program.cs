using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApplication1.Middleware;
using WebApplication1.Services;
using WebApplication1.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);
var tokenLifetimeManager = new JwtTokenLifetimeManager();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AddressContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AddressConnection")));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddSingleton<ITokenLifetimeManager>(tokenLifetimeManager);
builder.Services.AddSingleton<TokenService>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            LifetimeValidator = tokenLifetimeManager.ValidateTokenLifetime
        };

        x.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.HttpContext.GetEndpoint()?.Metadata.Any(m => m is AuthorizeAttribute) == true)
                {
                    throw new UnauthorizedAccessException();
                }

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                if (context.HttpContext.GetEndpoint()?.Metadata.Any(m => m is AuthorizeAttribute) == true)
                {
                    throw new UnauthorizedAccessException();
                }

                return Task.CompletedTask;
            }
        };

    });

builder.Services.AddControllers();
builder.Services.AddControllers(opt =>  
{
    opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
var app = builder.Build();

DatabaseInitilaizer.Seed(app);

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
dbContext?.Database.Migrate();

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
