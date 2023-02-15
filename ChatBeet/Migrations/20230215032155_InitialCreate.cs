using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBeet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "booru");

            migrationBuilder.EnsureSchema(
                name: "interactions");

            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.EnsureSchema(
                name: "stats");

            migrationBuilder.CreateTable(
                name: "progress_spans",
                schema: "interactions",
                columns: table => new
                {
                    guildid = table.Column<decimal>(name: "guild_id", type: "numeric(20,0)", nullable: false),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    template = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    beforerangemessage = table.Column<string>(name: "before_range_message", type: "character varying(300)", maxLength: 300, nullable: false),
                    afterrangemessage = table.Column<string>(name: "after_range_message", type: "character varying(300)", maxLength: 300, nullable: false),
                    startdate = table.Column<DateTime>(name: "start_date", type: "timestamp with time zone", nullable: false),
                    enddate = table.Column<DateTime>(name: "end_date", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_progress_spans", x => new { x.key, x.guildid });
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    discordid = table.Column<decimal>(name: "discord_id", type: "numeric(20,0)", nullable: true),
                    discordname = table.Column<string>(name: "discord_name", type: "character varying(200)", maxLength: 200, nullable: true),
                    discorddiscriminator = table.Column<string>(name: "discord_discriminator", type: "character varying(10)", maxLength: 10, nullable: true),
                    ircnick = table.Column<string>(name: "irc_nick", type: "character varying(100)", maxLength: 100, nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blacklisted_tags",
                schema: "booru",
                columns: table => new
                {
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    tag = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_blacklisted_tags", x => new { x.userid, x.tag });
                    table.ForeignKey(
                        name: "fk_blacklisted_tags_users_user_id",
                        column: x => x.userid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guilds",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    label = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    addedat = table.Column<DateTime>(name: "added_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    addedby = table.Column<Guid>(name: "added_by", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guilds", x => x.id);
                    table.ForeignKey(
                        name: "fk_guilds_users_added_by_user_id",
                        column: x => x.addedby,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tag_history",
                schema: "booru",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    tag = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_tag_history_users_user_id",
                        column: x => x.userid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "user_preferences",
                schema: "core",
                columns: table => new
                {
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    preference = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_preferences", x => new { x.userid, x.preference });
                    table.ForeignKey(
                        name: "fk_user_preferences_users_user_id",
                        column: x => x.userid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "definitions",
                schema: "interactions",
                columns: table => new
                {
                    guildid = table.Column<decimal>(name: "guild_id", type: "numeric(20,0)", nullable: false),
                    key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    value = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    createdby = table.Column<Guid>(name: "created_by", type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_definitions", x => new { x.key, x.guildid });
                    table.ForeignKey(
                        name: "fk_definitions_guilds_guild_id",
                        column: x => x.guildid,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_definitions_users_author_id",
                        column: x => x.createdby,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "high_ground",
                schema: "interactions",
                columns: table => new
                {
                    guildid = table.Column<decimal>(name: "guild_id", type: "numeric(20,0)", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_high_ground", x => x.guildid);
                    table.ForeignKey(
                        name: "fk_high_ground_guilds_guild_id",
                        column: x => x.guildid,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_high_ground_users_user_id",
                        column: x => x.userid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "karma",
                schema: "interactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    guildid = table.Column<decimal>(name: "guild_id", type: "numeric(20,0)", nullable: false),
                    voterid = table.Column<Guid>(name: "voter_id", type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_karma", x => x.id);
                    table.ForeignKey(
                        name: "fk_karma_guilds_guild_id",
                        column: x => x.guildid,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_karma_users_voter_id",
                        column: x => x.voterid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "suspicion_report",
                schema: "interactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    guildid = table.Column<decimal>(name: "guild_id", type: "numeric(20,0)", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    reporterid = table.Column<Guid>(name: "reporter_id", type: "uuid", nullable: false),
                    suspectid = table.Column<Guid>(name: "suspect_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_suspicion_report", x => x.id);
                    table.ForeignKey(
                        name: "fk_suspicion_report_guilds_guild_id",
                        column: x => x.guildid,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_suspicion_report_users_reporter_id",
                        column: x => x.reporterid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_suspicion_report_users_suspect_id",
                        column: x => x.suspectid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "keyword_hits",
                schema: "stats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    keywordid = table.Column<Guid>(name: "keyword_id", type: "uuid", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
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
                name: "ix_definitions_created_by",
                schema: "interactions",
                table: "definitions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_definitions_guild_id",
                schema: "interactions",
                table: "definitions",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_guilds_added_by",
                schema: "core",
                table: "guilds",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_high_ground_user_id",
                schema: "interactions",
                table: "high_ground",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_karma_guild_id",
                schema: "interactions",
                table: "karma",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_karma_voter_id",
                schema: "interactions",
                table: "karma",
                column: "voter_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_suspicion_report_guild_id",
                schema: "interactions",
                table: "suspicion_report",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_suspicion_report_reporter_id",
                schema: "interactions",
                table: "suspicion_report",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "ix_suspicion_report_suspect_id",
                schema: "interactions",
                table: "suspicion_report",
                column: "suspect_id");

            migrationBuilder.CreateIndex(
                name: "ix_tag_history_user_id",
                schema: "booru",
                table: "tag_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_top_tags_user_id",
                table: "top_tags",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blacklisted_tags",
                schema: "booru");

            migrationBuilder.DropTable(
                name: "definitions",
                schema: "interactions");

            migrationBuilder.DropTable(
                name: "high_ground",
                schema: "interactions");

            migrationBuilder.DropTable(
                name: "karma",
                schema: "interactions");

            migrationBuilder.DropTable(
                name: "keyword_hits",
                schema: "stats");

            migrationBuilder.DropTable(
                name: "progress_spans",
                schema: "interactions");

            migrationBuilder.DropTable(
                name: "suspicion_report",
                schema: "interactions");

            migrationBuilder.DropTable(
                name: "tag_history",
                schema: "booru");

            migrationBuilder.DropTable(
                name: "top_tags");

            migrationBuilder.DropTable(
                name: "user_preferences",
                schema: "core");

            migrationBuilder.DropTable(
                name: "keywords",
                schema: "stats");

            migrationBuilder.DropTable(
                name: "guilds",
                schema: "core");

            migrationBuilder.DropTable(
                name: "users",
                schema: "core");
        }
    }
}
