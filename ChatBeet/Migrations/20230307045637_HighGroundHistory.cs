using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBeet.Migrations
{
    /// <inheritdoc />
    public partial class HighGroundHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_high_ground_users_user_id",
                schema: "interactions",
                table: "high_ground");

            migrationBuilder.DropPrimaryKey(
                name: "pk_high_ground",
                schema: "interactions",
                table: "high_ground");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "interactions",
                table: "high_ground",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_high_ground",
                schema: "interactions",
                table: "high_ground",
                columns: new[] { "guild_id", "updated_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_high_ground_users_user_id",
                schema: "interactions",
                table: "high_ground",
                column: "user_id",
                principalSchema: "core",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_high_ground_users_user_id",
                schema: "interactions",
                table: "high_ground");

            migrationBuilder.DropPrimaryKey(
                name: "pk_high_ground",
                schema: "interactions",
                table: "high_ground");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "interactions",
                table: "high_ground",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_high_ground",
                schema: "interactions",
                table: "high_ground",
                column: "guild_id");

            migrationBuilder.AddForeignKey(
                name: "fk_high_ground_users_user_id",
                schema: "interactions",
                table: "high_ground",
                column: "user_id",
                principalSchema: "core",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
