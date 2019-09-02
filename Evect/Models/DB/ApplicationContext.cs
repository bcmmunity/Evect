using Microsoft.EntityFrameworkCore;

namespace Evect.Models.DB
{
    public sealed class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            // ВЫКЛЮЧИТЬ ПРИ ДЕПЛОЕ НА СЕРВАК
            //Database.EnsureDeleted();
            
            
            
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
            
            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    UserId = 1,
                    TelegramId = 12312312,
                    FirstName = "artem",
                    LastName = "kim",
                    Email = "moranmr8@gmail.com"
                });
         /*   modelBuilder.Entity<InfoAboutUsers>().HasData(new InfoAboutUsers { EventId = 1 }, new InfoAboutUsers { EventId = 2 },
                new InfoAboutUsers { EventId = 3 },
                new InfoAboutUsers { EventId = 4 },
                new InfoAboutUsers { EventId = 5 },
                new InfoAboutUsers { EventId = 6 });*/
            modelBuilder.Entity<Event>()
                .HasData(
                    new Event { 
                        EventId = 1,
                        Name = "Тестовое мероприятие, оч крутое", 
                        Info = "Крутое мероприятия для разномастных разработчиков", 
                        EventCode = "event_kim",
                        AdminCode = "event_admin",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    }, 
                    new Event { 
                        EventId = 2,
                        Name = "Второе тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim2",
                        AdminCode = "event_admin2",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    },
                    new Event { 
                        EventId = 3,
                        Name = "Третье тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim3",
                        AdminCode = "event_admin3",
                        TelegraphLink = "https://teletype.in/@verrok/SyOMtebSB"
                    },
                    new Event { 
                        EventId = 4,
                        Name = "Четвертое тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim4",
                        AdminCode = "event_admin4",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    },
                    new Event { 
                        EventId = 5,
                        Name = "Пятое тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim5",
                        AdminCode = "event_admin5",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    },
                    new Event { 
                        EventId = 6,
                        Name = "Шестое тестовое", 
                        Info = "DIFFFFFFFFFFFFFFFFFFFIND", 
                        EventCode = "event_kim6",
                        AdminCode = "event_admin6",
                        TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                    }
                    );

            modelBuilder.Entity<Tag>().HasData(
            new Tag
            {
                TagId = 1,
                ParentTagID = 0,
                Name = "IT",
                Level = 1
            },
            new Tag
                {
                    TagId = 2,
                    ParentTagID = 0,
                    Name = "Бизнес",
                    Level = 1
                },
            new Tag
                {
                    TagId = 3,
                    ParentTagID = 1,
                    Name = "Фронтэнд",
                    Level = 2
                },
            new Tag
                {
                    TagId = 4,
                    ParentTagID = 1,
                    Name = "Бэкэнд",
                    Level = 2
                },
            new Tag
            {
                TagId = 5,
                ParentTagID = 1,
                Name = "Девопс",
                Level = 2
            },
            new Tag
                {
                    TagId = 5,
                    ParentTagID = 1,
                    Name = "Дата сайенс",
                    Level = 2
                },
            new Tag
            {
                TagId = 6,
                ParentTagID = 1,
                Name = "Продуктовый дизайн",
                Level = 2
            },
            new Tag
                {
                    TagId = 7,
                    ParentTagID = 1,
                    Name = "Веб дизайн",
                    Level = 2
                },
            new Tag
                {
                    TagId = 8,
                    ParentTagID = 1,
                    Name = "UX/UI",
                    Level = 2
                },
            new Tag
            {
                TagId = 9,
                ParentTagID = 1,
                Name = "Продакт менеджер",
                Level = 2
            },
            new Tag
            {
                TagId = 10,
                ParentTagID = 1,
                Name = "Проджект менеджер",
                Level = 2
            },
            new Tag
            {
                TagId = 11,
                ParentTagID = 1,
                Name = "QA",
                Level = 2
            },
            new Tag 
                {
                    TagId = 12,
                    ParentTagID = 2,
                    Name = "Маркетинг",
                    Level = 2
                },
            new Tag
            {
                TagId = 13,
                ParentTagID = 2,
                Name = "Инвестор",
                Level = 2
            },
            new Tag
            {
                TagId = 14,
                ParentTagID = 2,
                Name = "Предприниматель",
                Level = 2
            },
            new Tag
            {
                TagId = 15,
                ParentTagID = 2,
                Name = "Финансы",
                Level = 2
            },
            new Tag
            {
                TagId = 16,
                ParentTagID = 2,
                Name = "Консультант",
                Level = 2
            },
            new Tag
            {
                TagId = 17,
                ParentTagID = 2,
                Name = "Аналитик",
                Level = 2
            },
            new Tag
            {
                TagId = 18,
                ParentTagID = 2,
                Name = "C-level",
                Level = 2
            },
            new Tag
            {
                TagId = 19,
                ParentTagID = 2,
                Name = "Продажи",
                Level = 2
            },
            new Tag
            {
                TagId = 20,
                ParentTagID = 2,
                Name = "Трекер",
                Level = 2
            },
            new Tag
            {
                TagId = 21,
                ParentTagID = 2,
                Name = "Стартап",
                Level = 2
            }
            );


            modelBuilder.Entity<Tag>().HasData(
            new Tag
            {
                
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

        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserValidation> Validations { get; set; }
        public DbSet<InfoAboutUsers> InfoAboutUsers { get; set; }
    }
}