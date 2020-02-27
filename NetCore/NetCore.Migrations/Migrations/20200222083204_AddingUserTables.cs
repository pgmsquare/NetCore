using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCore.Migrations.Migrations
{
    public partial class AddingUserTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserEmail = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false),
                    IsMembershipWithdrawn = table.Column<bool>(nullable: false, defaultValue: false),
                    JoinedUtcDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RolePriority = table.Column<byte>(nullable: false),
                    ModifiedUtcDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "UserRolesByUser",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RoleId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    OwnedUtcDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRolesByUser", x => new { x.UserId, x.RoleId });
                    table.UniqueConstraint("AK_UserRolesByUser_RoleId_UserId", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRolesByUser_UserRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRole",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRolesByUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_UserEmail",
                table: "User",
                column: "UserEmail",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRolesByUser");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
