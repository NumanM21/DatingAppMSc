using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class SignalRGroupsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_appRoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_appUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_appRoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_appUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "appRoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "appUserId",
                table: "AspNetUserRoles");

            migrationBuilder.CreateTable(
                name: "GroupSignalR",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSignalR", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "GroupConnection",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    SignalRGroupName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupConnection", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_GroupConnection_GroupSignalR_SignalRGroupName",
                        column: x => x.SignalRGroupName,
                        principalTable: "GroupSignalR",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupConnection_SignalRGroupName",
                table: "GroupConnection",
                column: "SignalRGroupName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupConnection");

            migrationBuilder.DropTable(
                name: "GroupSignalR");

            migrationBuilder.AddColumn<int>(
                name: "appRoleId",
                table: "AspNetUserRoles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "appUserId",
                table: "AspNetUserRoles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_appRoleId",
                table: "AspNetUserRoles",
                column: "appRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_appUserId",
                table: "AspNetUserRoles",
                column: "appUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_appRoleId",
                table: "AspNetUserRoles",
                column: "appRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_appUserId",
                table: "AspNetUserRoles",
                column: "appUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
