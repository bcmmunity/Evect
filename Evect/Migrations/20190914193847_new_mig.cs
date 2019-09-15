using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Evect.Migrations
{
    public partial class new_mig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    AnswerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TelegramId = table.Column<long>(nullable: false),
                    AnswerMark = table.Column<int>(nullable: false),
                    AnswerMessage = table.Column<string>(nullable: true),
                    QuestionId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.AnswerId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Info = table.Column<string>(nullable: true),
                    EventCode = table.Column<string>(nullable: true),
                    AdminCode = table.Column<string>(nullable: true),
                    TelegraphLink = table.Column<string>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: false),
                    DateEnd = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "InfoAboutUsers",
                columns: table => new
                {
                    InfoAboutUsersId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(nullable: false),
                    AmountOfActivationsOfNetworking = table.Column<int>(nullable: false),
                    AmountOfRequestsOfContacts = table.Column<int>(nullable: false),
                    AmountOfRequestsOfMettings = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoAboutUsers", x => x.InfoAboutUsersId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Logss = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SurveyId = table.Column<int>(nullable: false),
                    Questions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                });

            migrationBuilder.CreateTable(
                name: "SearchingTags",
                columns: table => new
                {
                    SearchingTagId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentTagID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchingTags", x => x.SearchingTagId);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    SurveyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.SurveyId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentTagID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TelegramId = table.Column<long>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    CompanyAndPosition = table.Column<string>(nullable: true),
                    Utility = table.Column<string>(nullable: true),
                    Communication = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    IsAuthed = table.Column<bool>(nullable: false),
                    IsAdminAuthorized = table.Column<bool>(nullable: false),
                    CurrentAction = table.Column<int>(nullable: false),
                    CurrentEventId = table.Column<int>(nullable: false),
                    CurrentSurveyId = table.Column<int>(nullable: false),
                    CurrentQuestionId = table.Column<int>(nullable: false),
                    PreviousAction = table.Column<int>(nullable: false),
                    TelegramUserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Validations",
                columns: table => new
                {
                    UserValidationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserTelegramId = table.Column<long>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Validations", x => x.UserValidationId);
                });

            migrationBuilder.CreateTable(
                name: "ContactsBook",
                columns: table => new
                {
                    ContactsBookId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsAccepted = table.Column<bool>(nullable: false),
                    OwnerId = table.Column<long>(nullable: false),
                    AnotherUserId = table.Column<long>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactsBook", x => x.ContactsBookId);
                    table.ForeignKey(
                        name: "FK_ContactsBook_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserEvent",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    UserEventId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvent", x => new { x.EventId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserEvent_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEvent_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSearchingTags",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false),
                    UserSearchingTagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSearchingTags", x => new { x.UserId, x.TagId });
                    table.ForeignKey(
                        name: "FK_UserSearchingTags_SearchingTags_TagId",
                        column: x => x.TagId,
                        principalTable: "SearchingTags",
                        principalColumn: "SearchingTagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSearchingTags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTags",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false),
                    UserTagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTags", x => new { x.UserId, x.TagId });
                    table.ForeignKey(
                        name: "FK_UserTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "AdminCode", "DateEnd", "DateStart", "EventCode", "Info", "Name", "TelegraphLink" },
                values: new object[,]
                {
                    { 1, "event_admin", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "event_kim", "Крутое мероприятия для разномастных разработчиков", "Тестовое мероприятие, оч крутое", "https://telegra.ph/Testovaya-statya-dlya-event-08-24" },
                    { 2, "event_admin2", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "event_kim2", "DIFFFFFFFFFFFFFFFFFFFIND", "Второе тестовое", "https://telegra.ph/Testovaya-statya-dlya-event-08-24" },
                    { 3, "event_admin3", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "event_kim3", "DIFFFFFFFFFFFFFFFFFFFIND", "Третье тестовое", "https://teletype.in/@verrok/SyOMtebSB" },
                    { 4, "event_admin4", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "event_kim4", "DIFFFFFFFFFFFFFFFFFFFIND", "Четвертое тестовое", "https://telegra.ph/Testovaya-statya-dlya-event-08-24" },
                    { 5, "event_admin5", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "event_kim5", "DIFFFFFFFFFFFFFFFFFFFIND", "Пятое тестовое", "https://telegra.ph/Testovaya-statya-dlya-event-08-24" },
                    { 6, "event_admin6", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "event_kim6", "DIFFFFFFFFFFFFFFFFFFFIND", "Шестое тестовое", "https://telegra.ph/Testovaya-statya-dlya-event-08-24" }
                });

            migrationBuilder.InsertData(
                table: "InfoAboutUsers",
                columns: new[] { "InfoAboutUsersId", "AmountOfActivationsOfNetworking", "AmountOfRequestsOfContacts", "AmountOfRequestsOfMettings", "EventId" },
                values: new object[,]
                {
                    { 6, 0, 0, 0, 6 },
                    { 4, 0, 0, 0, 4 },
                    { 5, 0, 0, 0, 5 },
                    { 2, 0, 0, 0, 2 },
                    { 1, 0, 0, 0, 1 },
                    { 3, 0, 0, 0, 3 }
                });

            migrationBuilder.InsertData(
                table: "SearchingTags",
                columns: new[] { "SearchingTagId", "Level", "Name", "ParentTagID" },
                values: new object[,]
                {
                    { 13, 2, "Маркетинг", 2 },
                    { 22, 2, "Стартап", 2 },
                    { 21, 2, "Трекер", 2 },
                    { 20, 2, "Продажи", 2 },
                    { 19, 2, "C-level", 2 },
                    { 18, 2, "Аналитик", 2 },
                    { 16, 2, "Финансы", 2 },
                    { 15, 2, "Предприниматель", 2 },
                    { 14, 2, "Инвестор", 2 },
                    { 12, 2, "QA", 1 },
                    { 17, 2, "Консультант", 2 },
                    { 10, 2, "Продакт менеджер", 1 },
                    { 11, 2, "Проджект менеджер", 1 },
                    { 2, 1, "Бизнес", 0 },
                    { 3, 2, "Фронтэнд", 1 },
                    { 4, 2, "Бэкэнд", 1 },
                    { 5, 2, "Девопс", 1 },
                    { 1, 1, "IT", 0 },
                    { 6, 2, "Дата сайенс", 1 },
                    { 7, 2, "Продуктовый дизайн", 1 },
                    { 8, 2, "Веб дизайн", 1 },
                    { 9, 2, "UX/UI", 1 }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "TagId", "Level", "Name", "ParentTagID" },
                values: new object[,]
                {
                    { 13, 2, "Маркетинг", 2 },
                    { 20, 2, "Продажи", 2 },
                    { 19, 2, "C-level", 2 },
                    { 18, 2, "Аналитик", 2 },
                    { 17, 2, "Консультант", 2 },
                    { 16, 2, "Финансы", 2 },
                    { 15, 2, "Предприниматель", 2 },
                    { 14, 2, "Инвестор", 2 },
                    { 21, 2, "Трекер", 2 },
                    { 12, 2, "QA", 1 },
                    { 6, 2, "Дата сайенс", 1 },
                    { 10, 2, "Продакт менеджер", 1 },
                    { 9, 2, "UX/UI", 1 },
                    { 8, 2, "Веб дизайн", 1 },
                    { 7, 2, "Продуктовый дизайн", 1 },
                    { 5, 2, "Девопс", 1 },
                    { 4, 2, "Бэкэнд", 1 },
                    { 3, 2, "Фронтэнд", 1 },
                    { 2, 1, "Бизнес", 0 },
                    { 1, 1, "IT", 0 },
                    { 22, 2, "Стартап", 2 },
                    { 11, 2, "Проджект менеджер", 1 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Communication", "CompanyAndPosition", "CurrentAction", "CurrentEventId", "CurrentQuestionId", "CurrentSurveyId", "Email", "FirstName", "IsAdminAuthorized", "IsAuthed", "LastName", "Phone", "PreviousAction", "TelegramId", "TelegramUserName", "Utility" },
                values: new object[] { 1, null, null, 0, 0, 0, 0, "moranmr8@gmail.com", "artem", false, false, "kim", null, 0, 12312312L, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_ContactsBook_UserId",
                table: "ContactsBook",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEvent_UserId",
                table: "UserEvent",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TelegramId",
                table: "Users",
                column: "TelegramId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSearchingTags_TagId",
                table: "UserSearchingTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_TagId",
                table: "UserTags",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "ContactsBook");

            migrationBuilder.DropTable(
                name: "InfoAboutUsers");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "UserEvent");

            migrationBuilder.DropTable(
                name: "UserSearchingTags");

            migrationBuilder.DropTable(
                name: "UserTags");

            migrationBuilder.DropTable(
                name: "Validations");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "SearchingTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
