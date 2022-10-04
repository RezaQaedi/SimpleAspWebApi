using BehvarTestProject.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BehvarTestProject.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add ApplicationDbContext to DI
            services.AddDbContext<ApplicationDpContext>(o =>
            {
                o.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }
    }
}
