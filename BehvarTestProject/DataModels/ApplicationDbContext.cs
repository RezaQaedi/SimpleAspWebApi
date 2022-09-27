using Microsoft.EntityFrameworkCore;

namespace BehvarTestProject.DataModels
{
    public class ApplicationDpContext : DbContext
    {
        public DbSet<ReportDataModel> Reports  { get; set; }

        public ApplicationDpContext(DbContextOptions<ApplicationDpContext> contextOptions) : base(contextOptions)
        {
        }
    }
}
