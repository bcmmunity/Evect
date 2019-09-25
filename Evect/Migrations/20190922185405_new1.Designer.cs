﻿// <auto-generated />
using System;
using Evect.Models.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Evect.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20190922185405_new1")]
    partial class new1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Evect.Models.Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AnswerMark");

                    b.Property<string>("AnswerMessage");

                    b.Property<DateTime>("Date");

                    b.Property<int>("QuestionId");

                    b.Property<long>("TelegramId");

                    b.HasKey("AnswerId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("Evect.Models.ContactsBook", b =>
                {
                    b.Property<int>("ContactsBookId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AnotherUserId");

                    b.Property<bool>("IsAccepted");

                    b.Property<long>("OwnerId");

                    b.Property<int?>("UserId");

                    b.HasKey("ContactsBookId");

                    b.HasIndex("UserId");

                    b.ToTable("ContactsBooks");
                });

            modelBuilder.Entity("Evect.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdminCode");

                    b.Property<DateTime>("DateEnd");

                    b.Property<DateTime>("DateStart");

                    b.Property<string>("EventCode");

                    b.Property<string>("Info");

                    b.Property<string>("Name");

                    b.Property<string>("TelegraphLink");

                    b.HasKey("EventId");

                    b.ToTable("Events");

                    b.HasData(
                        new
                        {
                            EventId = 1,
                            AdminCode = "event_admin",
                            DateEnd = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateStart = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EventCode = "event_kim",
                            Info = "Крутое мероприятия для разномастных разработчиков",
                            Name = "Тестовое мероприятие, оч крутое",
                            TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                        },
                        new
                        {
                            EventId = 2,
                            AdminCode = "event_admin2",
                            DateEnd = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateStart = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EventCode = "event_kim2",
                            Info = "DIFFFFFFFFFFFFFFFFFFFIND",
                            Name = "Второе тестовое",
                            TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                        },
                        new
                        {
                            EventId = 3,
                            AdminCode = "event_admin3",
                            DateEnd = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateStart = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EventCode = "event_kim3",
                            Info = "DIFFFFFFFFFFFFFFFFFFFIND",
                            Name = "Третье тестовое",
                            TelegraphLink = "https://teletype.in/@verrok/SyOMtebSB"
                        },
                        new
                        {
                            EventId = 4,
                            AdminCode = "event_admin4",
                            DateEnd = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateStart = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EventCode = "event_kim4",
                            Info = "DIFFFFFFFFFFFFFFFFFFFIND",
                            Name = "Четвертое тестовое",
                            TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                        },
                        new
                        {
                            EventId = 5,
                            AdminCode = "event_admin5",
                            DateEnd = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateStart = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EventCode = "event_kim5",
                            Info = "DIFFFFFFFFFFFFFFFFFFFIND",
                            Name = "Пятое тестовое",
                            TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                        },
                        new
                        {
                            EventId = 6,
                            AdminCode = "event_admin6",
                            DateEnd = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateStart = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EventCode = "event_kim6",
                            Info = "DIFFFFFFFFFFFFFFFFFFFIND",
                            Name = "Шестое тестовое",
                            TelegraphLink = "https://telegra.ph/Testovaya-statya-dlya-event-08-24"
                        });
                });

            modelBuilder.Entity("Evect.Models.InfoAboutUsers", b =>
                {
                    b.Property<int>("InfoAboutUsersId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AmountCompletedMeetings");

                    b.Property<int>("AmountOfActivationsOfNetworking");

                    b.Property<int>("AmountOfRequestsOfContacts");

                    b.Property<int>("AverageAmountOfContact");

                    b.Property<int>("EventId");

                    b.HasKey("InfoAboutUsersId");

                    b.ToTable("InfoAboutUsers");

                    b.HasData(
                        new
                        {
                            InfoAboutUsersId = 1,
                            AmountCompletedMeetings = 0,
                            AmountOfActivationsOfNetworking = 0,
                            AmountOfRequestsOfContacts = 0,
                            AverageAmountOfContact = 0,
                            EventId = 1
                        },
                        new
                        {
                            InfoAboutUsersId = 2,
                            AmountCompletedMeetings = 0,
                            AmountOfActivationsOfNetworking = 0,
                            AmountOfRequestsOfContacts = 0,
                            AverageAmountOfContact = 0,
                            EventId = 2
                        },
                        new
                        {
                            InfoAboutUsersId = 3,
                            AmountCompletedMeetings = 0,
                            AmountOfActivationsOfNetworking = 0,
                            AmountOfRequestsOfContacts = 0,
                            AverageAmountOfContact = 0,
                            EventId = 3
                        },
                        new
                        {
                            InfoAboutUsersId = 4,
                            AmountCompletedMeetings = 0,
                            AmountOfActivationsOfNetworking = 0,
                            AmountOfRequestsOfContacts = 0,
                            AverageAmountOfContact = 0,
                            EventId = 4
                        },
                        new
                        {
                            InfoAboutUsersId = 5,
                            AmountCompletedMeetings = 0,
                            AmountOfActivationsOfNetworking = 0,
                            AmountOfRequestsOfContacts = 0,
                            AverageAmountOfContact = 0,
                            EventId = 5
                        },
                        new
                        {
                            InfoAboutUsersId = 6,
                            AmountCompletedMeetings = 0,
                            AmountOfActivationsOfNetworking = 0,
                            AmountOfRequestsOfContacts = 0,
                            AverageAmountOfContact = 0,
                            EventId = 6
                        });
                });

            modelBuilder.Entity("Evect.Models.Log", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Logss");

                    b.HasKey("LogId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Evect.Models.Question", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EventId");

                    b.Property<string>("Questions");

                    b.Property<int>("Type");

                    b.HasKey("QuestionId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("Evect.Models.SearchingTag", b =>
                {
                    b.Property<int>("SearchingTagId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Level");

                    b.Property<string>("Name");

                    b.Property<int>("ParentTagID");

                    b.HasKey("SearchingTagId");

                    b.ToTable("SearchingTags");

                    b.HasData(
                        new
                        {
                            SearchingTagId = 1,
                            Level = 1,
                            Name = "IT",
                            ParentTagID = 0
                        },
                        new
                        {
                            SearchingTagId = 2,
                            Level = 1,
                            Name = "Бизнес",
                            ParentTagID = 0
                        },
                        new
                        {
                            SearchingTagId = 3,
                            Level = 2,
                            Name = "Фронтэнд",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 4,
                            Level = 2,
                            Name = "Бэкэнд",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 5,
                            Level = 2,
                            Name = "Девопс",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 6,
                            Level = 2,
                            Name = "Дата сайенс",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 7,
                            Level = 2,
                            Name = "Продуктовый дизайн",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 8,
                            Level = 2,
                            Name = "Веб дизайн",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 9,
                            Level = 2,
                            Name = "UX/UI",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 10,
                            Level = 2,
                            Name = "Продакт менеджер",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 11,
                            Level = 2,
                            Name = "Проджект менеджер",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 12,
                            Level = 2,
                            Name = "QA",
                            ParentTagID = 1
                        },
                        new
                        {
                            SearchingTagId = 13,
                            Level = 2,
                            Name = "Маркетинг",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 14,
                            Level = 2,
                            Name = "Инвестор",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 15,
                            Level = 2,
                            Name = "Предприниматель",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 16,
                            Level = 2,
                            Name = "Финансы",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 17,
                            Level = 2,
                            Name = "Консультант",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 18,
                            Level = 2,
                            Name = "Аналитик",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 19,
                            Level = 2,
                            Name = "C-level",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 20,
                            Level = 2,
                            Name = "Продажи",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 21,
                            Level = 2,
                            Name = "Трекер",
                            ParentTagID = 2
                        },
                        new
                        {
                            SearchingTagId = 22,
                            Level = 2,
                            Name = "Стартап",
                            ParentTagID = 2
                        });
                });

            modelBuilder.Entity("Evect.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Level");

                    b.Property<string>("Name");

                    b.Property<int>("ParentTagID");

                    b.HasKey("TagId");

                    b.ToTable("Tags");

                    b.HasData(
                        new
                        {
                            TagId = 1,
                            Level = 1,
                            Name = "IT",
                            ParentTagID = 0
                        },
                        new
                        {
                            TagId = 2,
                            Level = 1,
                            Name = "Бизнес",
                            ParentTagID = 0
                        },
                        new
                        {
                            TagId = 3,
                            Level = 2,
                            Name = "Фронтэнд",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 4,
                            Level = 2,
                            Name = "Бэкэнд",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 5,
                            Level = 2,
                            Name = "Девопс",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 6,
                            Level = 2,
                            Name = "Дата сайенс",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 7,
                            Level = 2,
                            Name = "Продуктовый дизайн",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 8,
                            Level = 2,
                            Name = "Веб дизайн",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 9,
                            Level = 2,
                            Name = "UX/UI",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 10,
                            Level = 2,
                            Name = "Продакт менеджер",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 11,
                            Level = 2,
                            Name = "Проджект менеджер",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 12,
                            Level = 2,
                            Name = "QA",
                            ParentTagID = 1
                        },
                        new
                        {
                            TagId = 13,
                            Level = 2,
                            Name = "Маркетинг",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 14,
                            Level = 2,
                            Name = "Инвестор",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 15,
                            Level = 2,
                            Name = "Предприниматель",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 16,
                            Level = 2,
                            Name = "Финансы",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 17,
                            Level = 2,
                            Name = "Консультант",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 18,
                            Level = 2,
                            Name = "Аналитик",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 19,
                            Level = 2,
                            Name = "C-level",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 20,
                            Level = 2,
                            Name = "Продажи",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 21,
                            Level = 2,
                            Name = "Трекер",
                            ParentTagID = 2
                        },
                        new
                        {
                            TagId = 22,
                            Level = 2,
                            Name = "Стартап",
                            ParentTagID = 2
                        });
                });

            modelBuilder.Entity("Evect.Models.TimeToJoinToEvent", b =>
                {
                    b.Property<int>("TimeToJoinToEventId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EventId");

                    b.Property<long>("TelegramId");

                    b.Property<DateTime>("time");

                    b.HasKey("TimeToJoinToEventId");

                    b.ToTable("TimeToJoinToEvents");
                });

            modelBuilder.Entity("Evect.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Communication");

                    b.Property<string>("CompanyAndPosition");

                    b.Property<int>("CurrentAction");

                    b.Property<int>("CurrentEventId");

                    b.Property<int>("CurrentQuestionId");

                    b.Property<int>("CurrentSurveyId");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsAdminAuthorized");

                    b.Property<bool>("IsAuthed");

                    b.Property<string>("LastName");

                    b.Property<string>("Phone");

                    b.Property<int>("PreviousAction");

                    b.Property<long>("TelegramId");

                    b.Property<string>("TelegramUserName");

                    b.Property<string>("Utility");

                    b.Property<string>("apiKey");

                    b.Property<DateTime>("dateOfStarting");

                    b.HasKey("UserId");

                    b.HasIndex("TelegramId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            CurrentAction = 0,
                            CurrentEventId = 0,
                            CurrentQuestionId = 0,
                            CurrentSurveyId = 0,
                            Email = "moranmr8@gmail.com",
                            FirstName = "artem",
                            IsAdminAuthorized = false,
                            IsAuthed = false,
                            LastName = "kim",
                            PreviousAction = 0,
                            TelegramId = 12312312L,
                            dateOfStarting = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("Evect.Models.UserEvent", b =>
                {
                    b.Property<int>("EventId");

                    b.Property<int>("UserId");

                    b.Property<int>("UserEventId");

                    b.HasKey("EventId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserEvent");
                });

            modelBuilder.Entity("Evect.Models.UserSearchingTag", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("TagId");

                    b.Property<int>("UserSearchingTagId");

                    b.HasKey("UserId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("UserSearchingTags");
                });

            modelBuilder.Entity("Evect.Models.UserTag", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("TagId");

                    b.Property<int>("UserTagId");

                    b.HasKey("UserId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("UserTags");
                });

            modelBuilder.Entity("Evect.Models.UserValidation", b =>
                {
                    b.Property<int>("UserValidationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Email");

                    b.Property<long>("UserTelegramId");

                    b.HasKey("UserValidationId");

                    b.ToTable("Validations");
                });

            modelBuilder.Entity("EvectCorp.Models.AdminUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrentAction");

                    b.Property<bool>("IsAdmin");

                    b.Property<long>("TelegramId");

                    b.Property<string>("TempEventCode");

                    b.Property<string>("TempEventName");

                    b.Property<int>("TempParentTag");

                    b.HasKey("Id");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("Evect.Models.ContactsBook", b =>
                {
                    b.HasOne("Evect.Models.User")
                        .WithMany("Contacts")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Evect.Models.UserEvent", b =>
                {
                    b.HasOne("Evect.Models.Event", "Event")
                        .WithMany("UserEvents")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Evect.Models.User", "User")
                        .WithMany("UserEvents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Evect.Models.UserSearchingTag", b =>
                {
                    b.HasOne("Evect.Models.SearchingTag", "Tag")
                        .WithMany("Tags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Evect.Models.User", "User")
                        .WithMany("SearchingUserTags")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Evect.Models.UserTag", b =>
                {
                    b.HasOne("Evect.Models.Tag", "Tag")
                        .WithMany("UserTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Evect.Models.User", "User")
                        .WithMany("UserTags")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
