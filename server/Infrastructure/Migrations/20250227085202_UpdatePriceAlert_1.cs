using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePriceAlert_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceAlerts_Coins_CoinId",
                table: "PriceAlerts");

            migrationBuilder.AlterColumn<int>(
                name: "CoinId",
                table: "PriceAlerts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceAlerts_Coins_CoinId",
                table: "PriceAlerts",
                column: "CoinId",
                principalTable: "Coins",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceAlerts_Coins_CoinId",
                table: "PriceAlerts");

            migrationBuilder.AlterColumn<int>(
                name: "CoinId",
                table: "PriceAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceAlerts_Coins_CoinId",
                table: "PriceAlerts",
                column: "CoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
