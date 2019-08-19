using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public sealed class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            // ВЫКЛЮЧИТЬ ПРИ ДЕПЛОЕ НА СЕРВАК
//            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AppSettings.DatabaseConnectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.TelegramId);
            
            
            modelBuilder.Entity<EventCode>()
                .HasData(
                    new EventCode() {EventCodeId = 1, Code = "evect_kim"},
                    new EventCode() { EventCodeId = 2, Code = "evect_kim_admin", IsForOrganizer = true});
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCode> EventCodes { get; set; }
    }
}