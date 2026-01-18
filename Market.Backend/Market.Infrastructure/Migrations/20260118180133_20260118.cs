using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20260118 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizers_UserId",
                table: "Organizers");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAtUtc",
                table: "Cart Items",
                newName: "ModifiedAtUtc");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cart Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Organizers_UserId",
                table: "Organizers",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizers_UserId",
                table: "Organizers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cart Items");

            migrationBuilder.RenameColumn(
                name: "ModifiedAtUtc",
                table: "Cart Items",
                newName: "LastUpdatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Organizers_UserId",
                table: "Organizers",
                column: "UserId");
        }
    }
}
