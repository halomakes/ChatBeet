using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBeet.Migrations
{
    /// <inheritdoc />
    public partial class StatEventTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_stats_guilds_guild_id",
                schema: "events",
                table: "stats");

            migrationBuilder.DropForeignKey(
                name: "fk_stats_users_user_id",
                schema: "events",
                table: "stats");

            migrationBuilder.DropForeignKey(
                name: "fk_stats_users_user_id1",
                schema: "events",
                table: "stats");

            migrationBuilder.DropPrimaryKey(
                name: "pk_stats",
                schema: "events",
                table: "stats");

            migrationBuilder.RenameTable(
                name: "stats",
                schema: "events",
                newName: "events",
                newSchema: "stats");

            migrationBuilder.RenameIndex(
                name: "ix_stats_triggering_user_id",
                schema: "stats",
                table: "events",
                newName: "ix_events_triggering_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_stats_targeted_user_id",
                schema: "stats",
                table: "events",
                newName: "ix_events_targeted_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_stats_guild_id",
                schema: "stats",
                table: "events",
                newName: "ix_events_guild_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_events",
                schema: "stats",
                table: "events",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_events_guilds_guild_id",
                schema: "stats",
                table: "events",
                column: "guild_id",
                principalSchema: "core",
                principalTable: "guilds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_events_users_user_id",
                schema: "stats",
                table: "events",
                column: "targeted_user_id",
                principalSchema: "core",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_events_users_user_id1",
                schema: "stats",
                table: "events",
                column: "triggering_user_id",
                principalSchema: "core",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_events_guilds_guild_id",
                schema: "stats",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "fk_events_users_user_id",
                schema: "stats",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "fk_events_users_user_id1",
                schema: "stats",
                table: "events");

            migrationBuilder.DropPrimaryKey(
                name: "pk_events",
                schema: "stats",
                table: "events");

            migrationBuilder.EnsureSchema(
                name: "events");

            migrationBuilder.RenameTable(
                name: "events",
                schema: "stats",
                newName: "stats",
                newSchema: "events");

            migrationBuilder.RenameIndex(
                name: "ix_events_triggering_user_id",
                schema: "events",
                table: "stats",
                newName: "ix_stats_triggering_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_events_targeted_user_id",
                schema: "events",
                table: "stats",
                newName: "ix_stats_targeted_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_events_guild_id",
                schema: "events",
                table: "stats",
                newName: "ix_stats_guild_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_stats",
                schema: "events",
                table: "stats",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_stats_guilds_guild_id",
                schema: "events",
                table: "stats",
                column: "guild_id",
                principalSchema: "core",
                principalTable: "guilds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_stats_users_user_id",
                schema: "events",
                table: "stats",
                column: "targeted_user_id",
                principalSchema: "core",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_stats_users_user_id1",
                schema: "events",
                table: "stats",
                column: "triggering_user_id",
                principalSchema: "core",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
