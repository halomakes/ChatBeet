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
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Template = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    BeforeRangeMessage = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    AfterRangeMessage = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_progress_spans", x => new { x.Key, x.GuildId });
                });

            migrationBuilder.CreateTable(
                name: "TopTags",
                columns: table => new
                {
                    Nick = table.Column<string>(type: "text", nullable: false),
                    Tag = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscordId = table.Column<decimal>(name: "Discord_Id", type: "numeric(20,0)", nullable: true),
                    DiscordName = table.Column<string>(name: "Discord_Name", type: "character varying(200)", maxLength: 200, nullable: true),
                    DiscordDiscriminator = table.Column<string>(name: "Discord_Discriminator", type: "character varying(10)", maxLength: 10, nullable: true),
                    IrcNick = table.Column<string>(name: "Irc_Nick", type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "blacklisted_tags",
                schema: "booru",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tag = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blacklisted_tags", x => new { x.UserId, x.Tag });
                    table.ForeignKey(
                        name: "FK_blacklisted_tags_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guilds",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Label = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    AddedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guilds_users_AddedBy",
                        column: x => x.AddedBy,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tag_history",
                schema: "booru",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tag = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tag_history_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                schema: "core",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Preference = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preferences", x => new { x.UserId, x.Preference });
                    table.ForeignKey(
                        name: "FK_user_preferences_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "definitions",
                schema: "interactions",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_definitions", x => new { x.Key, x.GuildId });
                    table.ForeignKey(
                        name: "FK_definitions_guilds_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_definitions_users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "high_ground",
                schema: "interactions",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_high_ground", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_high_ground_guilds_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_high_ground_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "karma",
                schema: "interactions",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VoterId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_karma", x => new { x.GuildId, x.Key });
                    table.ForeignKey(
                        name: "FK_karma_guilds_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_karma_users_VoterId",
                        column: x => x.VoterId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "keywords",
                schema: "stats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Label = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Regex = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_keywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_keywords_guilds_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "suspicion_report",
                schema: "interactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    ReporterId = table.Column<Guid>(type: "uuid", nullable: false),
                    SuspectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suspicion_report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_suspicion_report_guilds_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suspicion_report_users_ReporterId",
                        column: x => x.ReporterId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suspicion_report_users_SuspectId",
                        column: x => x.SuspectId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "keyword_hits",
                schema: "stats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeywordId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_keyword_hits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_keyword_hits_keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalSchema: "stats",
                        principalTable: "keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_keyword_hits_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_definitions_CreatedBy",
                schema: "interactions",
                table: "definitions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_definitions_GuildId",
                schema: "interactions",
                table: "definitions",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_guilds_AddedBy",
                schema: "core",
                table: "guilds",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_high_ground_UserId",
                schema: "interactions",
                table: "high_ground",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_karma_VoterId",
                schema: "interactions",
                table: "karma",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_keyword_hits_KeywordId",
                schema: "stats",
                table: "keyword_hits",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_keyword_hits_UserId",
                schema: "stats",
                table: "keyword_hits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_keywords_GuildId",
                schema: "stats",
                table: "keywords",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_suspicion_report_GuildId",
                schema: "interactions",
                table: "suspicion_report",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_suspicion_report_ReporterId",
                schema: "interactions",
                table: "suspicion_report",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_suspicion_report_SuspectId",
                schema: "interactions",
                table: "suspicion_report",
                column: "SuspectId");

            migrationBuilder.CreateIndex(
                name: "IX_tag_history_UserId",
                schema: "booru",
                table: "tag_history",
                column: "UserId");
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
                name: "TopTags");

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
