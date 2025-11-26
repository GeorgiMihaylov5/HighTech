using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using HighTech.Options;
using HighTech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PernikComputers.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddTransient<IFieldService, FieldService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IOrderService, OrderService>();

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

builder.Services.Configure<IdentityOptions>(option =>
{
	option.SignIn.RequireConfirmedEmail = false;
	option.Password.RequireDigit = false;
	option.Password.RequiredLength = 5;
	option.Password.RequireLowercase = false;
	option.Password.RequireNonAlphanumeric = false;
	option.Password.RequireUppercase = false;
	option.Password.RequiredUniqueChars = 0;
});

var app = builder.Build();

await app.PrepareDatabase();

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
