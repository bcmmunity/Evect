using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public abstract class DB
    {
        protected ApplicationContext Connect()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            // optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=eventbot;Trusted_Connection=True;MultipleActiveResultSets=true");
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=evect2;Trusted_Connection=True;");
            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}