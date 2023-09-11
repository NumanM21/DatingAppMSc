using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class MessageUserEntityUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    messageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    messageSenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    messageSenderUsername = table.Column<string>(type: "TEXT", nullable: true),
                    SenderUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    messageReceivingId = table.Column<int>(type: "INTEGER", nullable: false),
                    messageReceivingUsername = table.Column<string>(type: "TEXT", nullable: true),
                    ReceivingUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    messageContent = table.Column<string>(type: "TEXT", nullable: true),
                    messageReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    messageSentAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    messageSentDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    messageReceivingDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.messageId);
                    table.ForeignKey(
                        name: "FK_Message_Users_ReceivingUserId",
                        column: x => x.ReceivingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Message_Users_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_ReceivingUserId",
                table: "Message",
                column: "ReceivingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderUserId",
                table: "Message",
                column: "SenderUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message");
        }
    }
}
