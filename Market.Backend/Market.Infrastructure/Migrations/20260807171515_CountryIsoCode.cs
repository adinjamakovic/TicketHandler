using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CountryIsoCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsoCode",
                table: "Countries",
                type: "nchar(2)",
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            // Backfill the already-seeded countries — DynamicDataSeeder only
            // inserts when the table is empty, so it won't fix existing rows.
            migrationBuilder.Sql(@"
                UPDATE [Countries] SET [IsoCode] = 'BA' WHERE [Name] = 'Bosnia and Herzegovina';
                UPDATE [Countries] SET [IsoCode] = 'US' WHERE [Name] = 'United States of America';
                UPDATE [Countries] SET [IsoCode] = 'SE' WHERE [Name] = 'Sweden';
                UPDATE [Countries] SET [IsoCode] = 'SI' WHERE [Name] = 'Slovenia';
                UPDATE [Countries] SET [IsoCode] = 'OM' WHERE [Name] = 'Oman';
                UPDATE [Countries] SET [IsoCode] = 'HR' WHERE [Name] = 'Croatia';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsoCode",
                table: "Countries");
        }
    }
}
