using HighTech.Abstraction;
using HighTech.Configurator;
using HighTech.Data;
using HighTech.Infrastructure;
using HighTech.Models;
using HighTech.Options;
using HighTech.Services;
using HighTech.Services.Assistant;
using HighTech.Services.Assistant.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:7140");
builder.Services.AddHttpsRedirection(options => options.HttpsPort = 7140);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = "Bearer";
		options.DefaultChallengeScheme = "Bearer";
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
			ValidIssuer = builder.Configuration["JWT:Issuer"],
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidAudience = builder.Configuration["JWT:Issuer"]
		};
	});

builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IJWTService, JWTService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IFieldService, FieldService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddTransient<IReviewService, ReviewService>();
builder.Services.AddTransient<IFavoriteService, FavoriteService>();
builder.Services.AddTransient<IConfiguratorService, ConfiguratorService>();
builder.Services.AddTransient<IConfiguratorMetadataGuard, ConfiguratorMetadataGuard>();

builder.Services.Configure<AssistantOptions>(opts =>
{
	builder.Configuration.GetSection("Assistant").Bind(opts);
	opts.ApiKey = Environment.GetEnvironmentVariable("Assistant__ApiKey")
		?? throw new InvalidOperationException("Assistant__ApiKey environment variable is not set.");
});
builder.Services.AddTransient<IAiChatProvider>(sp =>
{
	var opts = sp.GetRequiredService<IOptions<AssistantOptions>>().Value;
	return opts.Provider switch
	{
		"OpenAI" => new OpenAiChatProvider(opts, sp.GetRequiredService<ILogger<OpenAiChatProvider>>()),
		_ => throw new InvalidOperationException($"Unknown AI provider: {opts.Provider}")
	};
});
builder.Services.AddTransient<IAssistantService, AssistantService>();
builder.Services.AddTransient<AssistantDataResolver>();
builder.Services.AddSingleton<IAssistantRateLimiter, AssistantRateLimiter>();
builder.Services.AddSingleton<IAssistantConversationStore, AssistantConversationStore>();
builder.Services.AddHostedService<AssistantConversationCleaningService>();

// Add CORS
//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("AllowAngularApp", policy =>
//	{
//		policy.WithOrigins("http://localhost:4200")
//			.AllowAnyHeader()
//			.AllowAnyMethod()
//			.AllowCredentials()
//			.SetPreflightMaxAge(TimeSpan.FromHours(1));
//	});
//});

builder.Services.Configure<JWTServiceOption>(options =>
{
	options.JwtKey = builder.Configuration["JWT:Key"];
	options.Issuer = builder.Configuration["JWT:Issuer"];
	options.ExpiresDays = int.Parse(builder.Configuration["JWT:ExpiresDays"]);
});

builder.Services.Configure<SeedingOptions>(builder.Configuration.GetSection("Seeding"));
builder.Services.Configure<EmployeeOptions>(builder.Configuration.GetSection("Employee"));
builder.Services.Configure<ProductOptions>(builder.Configuration.GetSection("Product"));

builder.Services.Configure<IdentityOptions>(option =>
{
	option.SignIn.RequireConfirmedEmail = false;
	option.Password.RequiredUniqueChars = 0;
	builder.Configuration.GetSection("PasswordPolicy").Bind(option.Password);
});

var app = builder.Build();

var seedingOptions = builder.Configuration.GetSection("Seeding").Get<SeedingOptions>();
await app.PrepareDatabase(seedingOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

//app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
