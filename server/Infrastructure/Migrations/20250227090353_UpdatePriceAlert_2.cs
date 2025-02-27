using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePriceAlert_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceAlerts_Coins_CoinId",
                table: "PriceAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceHistories_Coins_CoinId",
                table: "PriceHistories");

            migrationBuilder.DropTable(
                name: "Coins");

            migrationBuilder.DropIndex(
                name: "IX_PriceHistories_CoinId",
                table: "PriceHistories");

            migrationBuilder.DropIndex(
                name: "IX_PriceAlerts_CoinId",
                table: "PriceAlerts");

            migrationBuilder.DropColumn(
                name: "CoinId",
                table: "PriceAlerts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoinId",
                table: "PriceAlerts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Change24h = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CoinGeckoId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketCap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistories_CoinId",
                table: "PriceHistories",
                column: "CoinId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlerts_CoinId",
                table: "PriceAlerts",
                column: "CoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceAlerts_Coins_CoinId",
                table: "PriceAlerts",
                column: "CoinId",
                principalTable: "Coins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceHistories_Coins_CoinId",
                table: "PriceHistories",
                column: "CoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
