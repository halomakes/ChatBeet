using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBeet.Migrations
{
    /// <inheritdoc />
    public partial class StatEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "events");

            migrationBuilder.AlterColumn<string>(
                name: "before_range_message",
                schema: "interactions",
                table: "progress_spans",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "after_range_message",
                schema: "interactions",
                table: "progress_spans",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.CreateTable(
                name: "stats",
                schema: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    guildid = table.Column<decimal>(name: "guild_id", type: "numeric(20,0)", nullable: false),
                    eventtype = table.Column<string>(name: "event_type", type: "text", nullable: false),
                    triggeringuserid = table.Column<Guid>(name: "triggering_user_id", type: "uuid", nullable: false),
                    targeteduserid = table.Column<Guid>(name: "targeted_user_id", type: "uuid", nullable: true),
                    occurredat = table.Column<DateTime>(name: "occurred_at", type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    sampletext = table.Column<string>(name: "sample_text", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stats", x => x.id);
                    table.ForeignKey(
                        name: "fk_stats_guilds_guild_id",
                        column: x => x.guildid,
                        principalSchema: "core",
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_stats_users_user_id",
                        column: x => x.targeteduserid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_stats_users_user_id1",
                        column: x => x.triggeringuserid,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_stats_guild_id",
                schema: "events",
                table: "stats",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_stats_targeted_user_id",
                schema: "events",
                table: "stats",
                column: "targeted_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_stats_triggering_user_id",
                schema: "events",
                table: "stats",
                column: "triggering_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stats",
                schema: "events");

            migrationBuilder.AlterColumn<string>(
                name: "before_range_message",
                schema: "interactions",
                table: "progress_spans",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "after_range_message",
                schema: "interactions",
                table: "progress_spans",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);
        }
    }
}
