using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueChallenge.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Credentials_Email_Alias = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Credentials_Email_Provider = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Credentials_Password = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PersonalInfo_Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    PersonalInfo_Phone_DDD = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    PersonalInfo_Phone_Number = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateRange_Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateRange_End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsAllDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    HourRange_Start = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    HourRange_End = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users_Emails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Alias = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_Emails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Emails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_UserId",
                table: "Schedules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Emails_UserId",
                table: "Users_Emails",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Users_Emails");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
