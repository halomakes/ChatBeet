using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatBeet.Migrations
{
    /// <inheritdoc />
    public partial class Quotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "keyword_hits",
                schema: "stats");

            migrationBuilder.DropTable(
                name: "keywords",
                schema: "stats");

            migrationBuilder.CreateTable(
                name: "quotes",
                schema: "interactions",
                columns: table => new
                {
                    guildid = table.Column<decimal>(name: "guild_id", type: "numeric(20,0)", nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    savedbyid = table.Column<Guid>(name: "saved_by_id", type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    channelname = table.Column<string>(name: "channel_name", type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quotes", x => new { x.guildid, x.slug });
                    table.ForeignKey(
                        name: "fk_quotes_guilds_guild_id",
                        column: x => x.guildid,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quotes_users_saved_by_id",
                        column: x => x.savedbyid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quote_message",
                schema: "interactions",
                columns: table => new
                {
                    quoteguildid = table.Column<decimal>(name: "quote_guild_id", type: "numeric(20,0)", nullable: false),
                    quoteslug = table.Column<string>(name: "quote_slug", type: "character varying(200)", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "text", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    authorid = table.Column<Guid>(name: "author_id", type: "uuid", nullable: false),
                    embeds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quote_message", x => new { x.quoteguildid, x.quoteslug, x.id });
                    table.ForeignKey(
                        name: "fk_quote_message_quotes_quote_temp_id",
                        columns: x => new { x.quoteguildid, x.quoteslug },
                        principalSchema: "interactions",
                        principalTable: "quotes",
                        principalColumns: new[] { "guild_id", "slug" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quote_message_users_author_id",
                        column: x => x.authorid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_quote_message_author_id",
                schema: "interactions",
                table: "quote_message",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotes_saved_by_id",
                schema: "interactions",
                table: "quotes",
                column: "saved_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quote_message",
                schema: "interactions");

            migrationBuilder.DropTable(
                name: "quotes",
                schema: "interactions");

            migrationBuilder.CreateTable(
                name: "keywords",
                schema: "stats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    guildid = table.Column<decimal>(name: "guild_id", type: "numeric(20,0)", nullable: false),
                    label = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    regex = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sortorder = table.Column<int>(name: "sort_order", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_keywords", x => x.id);
                    table.ForeignKey(
                        name: "fk_keywords_guilds_guild_id",
                        column: x => x.guildid,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "keyword_hits",
                schema: "stats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    keywordid = table.Column<Guid>(name: "keyword_id", type: "uuid", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_keyword_hits", x => x.id);
                    table.ForeignKey(
                        name: "fk_keyword_hits_keywords_keyword_id",
                        column: x => x.keywordid,
                        principalSchema: "stats",
                        principalTable: "keywords",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_keyword_hits_users_user_id",
                        column: x => x.userid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_keyword_hits_keyword_id",
                schema: "stats",
                table: "keyword_hits",
                column: "keyword_id");

            migrationBuilder.CreateIndex(
                name: "ix_keyword_hits_user_id",
                schema: "stats",
                table: "keyword_hits",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_keywords_guild_id",
                schema: "stats",
                table: "keywords",
                column: "guild_id");
        }
    }
}
