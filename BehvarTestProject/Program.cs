using BehvarTestProject.ApiModels;
using BehvarTestProject.Installers;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDpContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Me",
            ValidAudience = "You",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key"))
        };
    });

builder.Services.Configure<IdentityOptions>(options =>
 {
     // Password settings.
     options.Password.RequireDigit = false;
     options.Password.RequireLowercase = false;
     options.Password.RequireNonAlphanumeric = false;
     options.Password.RequireUppercase = false;
     options.Password.RequiredLength = 2;
     options.Password.RequiredUniqueChars = 0;
 });

// Add services to the container.
builder.Services.InstallServicesInAssembly(builder.Configuration);

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.AddSwaggerExamplesFromAssemblyOf<ReportApiModel>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Test API", Version = "v1" });
    c.IncludeXmlComments(xmlPath);
    c.ExampleFilters();
    c.OperationFilter<AddResponseHeadersFilter>();
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
