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
            
            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    UserId = 1,
                    TelegramId = 12312312,
                    FirstName = "artem",
                    LastName = "kim",
                    Email = "moranmr8@gmail.com"
                });
            
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
                    TagId = 6,
                    ParentTagID = 1,
                    Name = "Дата сайенс",
                    Level = 2
                },
            new Tag
            {
                TagId = 7,
                ParentTagID = 1,
                Name = "Продуктовый дизайн",
                Level = 2
            },
            new Tag
                {
                    TagId = 8,
                    ParentTagID = 1,
                    Name = "Веб дизайн",
                    Level = 2
                },
            new Tag
                {
                    TagId = 9,
                    ParentTagID = 1,
                    Name = "UX/UI",
                    Level = 2
                },
            new Tag
            {
                TagId = 10,
                ParentTagID = 1,
                Name = "Продакт менеджер",
                Level = 2
            },
            new Tag
            {
                TagId = 11,
                ParentTagID = 1,
                Name = "Проджект менеджер",
                Level = 2
            },
            new Tag
            {
                TagId = 12,
                ParentTagID = 1,
                Name = "QA",
                Level = 2
            },
            new Tag 
                {
                    TagId = 13,
                    ParentTagID = 2,
                    Name = "Маркетинг",
                    Level = 2
                },
            new Tag
            {
                TagId = 14,
                ParentTagID = 2,
                Name = "Инвестор",
                Level = 2
            },
            new Tag
            {
                TagId = 15,
                ParentTagID = 2,
                Name = "Предприниматель",
                Level = 2
            },
            new Tag
            {
                TagId = 16,
                ParentTagID = 2,
                Name = "Финансы",
                Level = 2
            },
            new Tag
            {
                TagId = 17,
                ParentTagID = 2,
                Name = "Консультант",
                Level = 2
            },
            new Tag
            {
                TagId = 18,
                ParentTagID = 2,
                Name = "Аналитик",
                Level = 2
            },
            new Tag
            {
                TagId = 19,
                ParentTagID = 2,
                Name = "C-level",
                Level = 2
            },
            new Tag
            {
                TagId = 20,
                ParentTagID = 2,
                Name = "Продажи",
                Level = 2
            },
            new Tag
            {
                TagId = 21,
                ParentTagID = 2,
                Name = "Трекер",
                Level = 2
            },
            new Tag
            {
                TagId = 22,
                ParentTagID = 2,
                Name = "Стартап",
                Level = 2
            }
            );
            
            modelBuilder.Entity<SearchingTag>().HasData(
            new SearchingTag
            {
                SearchingTagId = 1,
                ParentTagID = 0,
                Name = "IT",
                Level = 1
            },
            new SearchingTag
                {
                    SearchingTagId = 2,
                    ParentTagID = 0,
                    Name = "Бизнес",
                    Level = 1
                },
            new SearchingTag
                {
                    SearchingTagId = 3,
                    ParentTagID = 1,
                    Name = "Фронтэнд",
                    Level = 2
                },
            new SearchingTag
                {
                    SearchingTagId = 4,
                    ParentTagID = 1,
                    Name = "Бэкэнд",
                    Level = 2
                },
            new SearchingTag
            {
                SearchingTagId = 5,
                ParentTagID = 1,
                Name = "Девопс",
                Level = 2
            },
            new SearchingTag
                {
                    SearchingTagId = 6,
                    ParentTagID = 1,
                    Name = "Дата сайенс",
                    Level = 2
                },
            new SearchingTag
            {
                SearchingTagId = 7,
                ParentTagID = 1,
                Name = "Продуктовый дизайн",
                Level = 2
            },
            new SearchingTag
                {
                    SearchingTagId = 8,
                    ParentTagID = 1,
                    Name = "Веб дизайн",
                    Level = 2
                },
            new SearchingTag
                {
                    SearchingTagId = 9,
                    ParentTagID = 1,
                    Name = "UX/UI",
                    Level = 2
                },
            new SearchingTag
            {
                SearchingTagId = 10,
                ParentTagID = 1,
                Name = "Продакт менеджер",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 11,
                ParentTagID = 1,
                Name = "Проджект менеджер",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 12,
                ParentTagID = 1,
                Name = "QA",
                Level = 2
            },
            new SearchingTag 
                {
                    SearchingTagId = 13,
                    ParentTagID = 2,
                    Name = "Маркетинг",
                    Level = 2
                },
            new SearchingTag
            {
                SearchingTagId = 14,
                ParentTagID = 2,
                Name = "Инвестор",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 15,
                ParentTagID = 2,
                Name = "Предприниматель",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 16,
                ParentTagID = 2,
                Name = "Финансы",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 17,
                ParentTagID = 2,
                Name = "Консультант",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 18,
                ParentTagID = 2,
                Name = "Аналитик",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 19,
                ParentTagID = 2,
                Name = "C-level",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 20,
                ParentTagID = 2,
                Name = "Продажи",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 21,
                ParentTagID = 2,
                Name = "Трекер",
                Level = 2
            },
            new SearchingTag
            {
                SearchingTagId = 22,
                ParentTagID = 2,
                Name = "Стартап",
                Level = 2
            }
            );
            

            modelBuilder.Entity<UserTag>()
                .HasKey(ut => new {ut.UserId, ut.TagId});
            
            modelBuilder.Entity<UserTag>()
                .HasOne(us => us.Tag)
                .WithMany(e => e.UserTags)
                .HasForeignKey(k => k.TagId);
                
            
            modelBuilder.Entity<UserTag>()
                .HasOne(us => us.User)
                .WithMany(e => e.UserTags)
                .HasForeignKey(k => k.UserId);
            
            
            modelBuilder.Entity<UserSearchingTag>()
                .HasKey(ut => new {ut.UserId, ut.TagId});
            
            modelBuilder.Entity<UserSearchingTag>()
                .HasOne(us => us.Tag)
                .WithMany(e => e.Tags)
                .HasForeignKey(k => k.TagId);
                
            
            modelBuilder.Entity<UserSearchingTag>()
                .HasOne(us => us.User)
                .WithMany(e => e.SearchingUserTags)
                .HasForeignKey(k => k.UserId);
            
            
            modelBuilder.Entity<UserEvent>()
                .HasKey(us => new {us.EventId, us.UserId});
            
            modelBuilder.Entity<UserEvent>()
                .HasOne(us => us.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(k => k.EventId);
                
            
            modelBuilder.Entity<UserEvent>()
                .HasOne(us => us.User)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(k => k.UserId);
        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<SearchingTag> SearchingTags { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserValidation> Validations { get; set; }
        public DbSet<InfoAboutUsers> InfoAboutUsers { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<UserSearchingTag> UserSearchingTags { get; set; }
    }
}