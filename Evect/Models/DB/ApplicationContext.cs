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

            modelBuilder.Entity<Event>()
                .HasData(
                    new Event { 
                        Name = "Тестовое мероприятие, оч крутое", 
                        Info = "Крутое мероприятия для разномастных разработчиков", 
                        EventCode = "event_kim",
                        AdminCode = "event_admin"
                    }, 
                    new Event { 
                        Name = "Второе тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim2",
                        AdminCode = "event_admin2"
                    }
                    );

            modelBuilder.Entity<UserEvent>()
                .HasKey(us => new {us.EventId, us.UserEventId});
            
            modelBuilder.Entity<UserEvent>()
                .HasOne(us => us.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(k => k.EventId);
                
            
            modelBuilder.Entity<UserEvent>()
                .HasOne(us => us.User)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(k => k.UserEventId);
                


        }


        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}