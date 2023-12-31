using CityInfo.API;
using CityInfo.API.DBContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// using Serilog
//Log.Logger = new LoggerConfiguration()
//	.MinimumLevel.Debug()
//	.WriteTo.Console()
//	.WriteTo.File("logs/cityInfo.txt", rollingInterval: RollingInterval.Day)
//	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders(); //clears all logging providers
builder.Logging.AddConsole();

//builder.Host.useSerilog();

// Add services to the container.

builder.Services.AddControllers(options =>
{
	options.ReturnHttpNotAcceptable = true;	//http 406 -> requested format cannot be provided
})
	.AddNewtonsoftJson()	//for patch updates
	.AddXmlDataContractSerializerFormatters();	//for returning in format as requested

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); // invalid file format is returned

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>(); // mail service registration
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddSingleton<CitiesDataStore>();

// registering the city info entity class service and sqlite databasae
builder.Services.AddDbContext<CityInfoContext> (
	dbContextOptions => dbContextOptions.UseSqlite(
		builder.Configuration["ConnectionStrings:CityInfoConnectionString"])   //Connection string
);

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer(
		options =>
		{
			options.TokenValidationParameters = new()
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = builder.Configuration["Authentication:Issuer"],
				ValidAudience = builder.Configuration["Authentication.Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"])
					)
			};
		}
	);

//Adding Authorization policy service
builder.Services.AddAuthorization(options => 
{
	options.AddPolicy("MustBeFromChennai", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("city", "Chennai");
	});
});

builder.Services.AddApiVersioning(setupAction =>
{
	setupAction.AssumeDefaultVersionWhenUnspecified = true;
	setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
	setupAction.ReportApiVersions = true;
});

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});

app.Run();
