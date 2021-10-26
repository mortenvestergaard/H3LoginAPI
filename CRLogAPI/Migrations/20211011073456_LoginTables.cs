using Microsoft.EntityFrameworkCore.Migrations;

namespace CRLogAPI.Migrations
{
    public partial class LoginTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    UserChoice = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    StatusMessage = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogTime = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
