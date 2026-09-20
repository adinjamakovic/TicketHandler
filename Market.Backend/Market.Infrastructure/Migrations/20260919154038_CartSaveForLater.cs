using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <summary>
    /// Lets a cart line be parked instead of bought now. Existing rows default to false,
    /// so every cart already in the table stays exactly as it was.
    /// </summary>
    public partial class CartSaveForLater : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSavedForLater",
                table: "Cart Items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSavedForLater",
                table: "Cart Items");
        }
    }
}
