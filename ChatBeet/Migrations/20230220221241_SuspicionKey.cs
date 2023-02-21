using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBeet.Migrations
{
    /// <inheritdoc />
    public partial class SuspicionKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_suspicion_report",
                schema: "interactions",
                table: "suspicion_report");

            migrationBuilder.AddPrimaryKey(
                name: "pk_suspicion_report",
                schema: "interactions",
                table: "suspicion_report",
                columns: new[] { "id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_suspicion_report",
                schema: "interactions",
                table: "suspicion_report");

            migrationBuilder.AddPrimaryKey(
                name: "pk_suspicion_report",
                schema: "interactions",
                table: "suspicion_report",
                column: "id");
        }
    }
}
