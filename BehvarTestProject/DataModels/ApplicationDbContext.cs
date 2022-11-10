using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BehvarTestProject.DataModels
{
    public class ApplicationDpContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<ReportDataModel> Reports  { get; set; }

        public ApplicationDpContext(DbContextOptions<ApplicationDpContext> contextOptions) : base(contextOptions)
        {
        }
    }
}
