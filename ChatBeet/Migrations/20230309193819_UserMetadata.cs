using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBeet.Migrations
{
    /// <inheritdoc />
    public partial class UserMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_metadata",
                schema: "core",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_metadata", x => new { x.user_id, x.guild_id, x.key });
                    table.ForeignKey(
                        name: "fk_user_metadata_guilds_guild_id",
                        column: x => x.guild_id,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_metadata_users_author_id",
                        column: x => x.author_id,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_metadata_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_metadata_author_id",
                schema: "core",
                table: "user_metadata",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_metadata_guild_id",
                schema: "core",
                table: "user_metadata",
                column: "guild_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_metadata",
                schema: "core");
        }
    }
}
