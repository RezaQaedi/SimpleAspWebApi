using BehvarTestProject.DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Evaluation;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace BehvarTestProject.Filters
{
    public class CustomSwaggerFilter : IDocumentFilter
    {
        //private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceScopeFactory _scope; // service for getting database context

        public CustomSwaggerFilter(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scope)
        {
            _httpContextAccessor = httpContextAccessor;
            _scope = scope;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            using var scope = _scope.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDpContext>();

            var nonMobileRoutes = swaggerDoc.Paths
                   .Where(x => !x.Key.ToLower().Contains("public"))
                   .ToList();
            nonMobileRoutes.ForEach(x =>
            {
                swaggerDoc.Paths.Remove(x.Key);
            });
        }
    }

    public class SwaggerGenOption : IConfigureNamedOptions<SwaggerGenOptions>
    {
        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Test API", Version = "v1" });
            options.IncludeXmlComments(xmlPath);

            options.DocumentFilter<CustomSwaggerFilter>();
            options.ExampleFilters();
            options.OperationFilter<AddResponseHeadersFilter>();
            options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
             {
            {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
            }
            });
        }
    }
}
