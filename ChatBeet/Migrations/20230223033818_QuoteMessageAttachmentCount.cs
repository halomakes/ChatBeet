using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatBeet.Migrations
{
    /// <inheritdoc />
    public partial class QuoteMessageAttachmentCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "top_tags");

            migrationBuilder.DropPrimaryKey(
                name: "pk_quote_message",
                schema: "interactions",
                table: "quote_message");

            migrationBuilder.AddColumn<int>(
                name: "attachments",
                schema: "interactions",
                table: "quote_message",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_quote_message",
                schema: "interactions",
                table: "quote_message",
                columns: new[] { "quote_guild_id", "quote_slug", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_quote_message",
                schema: "interactions",
                table: "quote_message");

            migrationBuilder.DropColumn(
                name: "attachments",
                schema: "interactions",
                table: "quote_message");
            

            migrationBuilder.AddPrimaryKey(
                name: "pk_quote_message",
                schema: "interactions",
                table: "quote_message",
                column: "id");

            migrationBuilder.CreateTable(
                name: "top_tags",
                columns: table => new
                {
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    tag = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "fk_top_tags_users_user_id",
                        column: x => x.userid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_top_tags_user_id",
                table: "top_tags",
                column: "user_id");
        }
    }
}
