using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trade.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    account_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    account_balance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "watchlist",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    stock_name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    date_added = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_watchlist", x => x.id);
                    table.UniqueConstraint("AK_watchlist_stock_name", x => x.stock_name);
                });

            migrationBuilder.CreateTable(
                name: "trade_signals",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    stock_name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    direction = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    signal_date = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trade_signals", x => x.id);
                    table.ForeignKey(
                        name: "FK_trade_signals_watchlist_stock_name",
                        column: x => x.stock_name,
                        principalTable: "watchlist",
                        principalColumn: "stock_name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trades",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    stock_name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    entry_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    current_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    close_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    position_size = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    open_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    close_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    account_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trades", x => x.id);
                    table.ForeignKey(
                        name: "FK_trades_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trades_watchlist_stock_name",
                        column: x => x.stock_name,
                        principalTable: "watchlist",
                        principalColumn: "stock_name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_account_name",
                table: "accounts",
                column: "account_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trade_signals_stock_name",
                table: "trade_signals",
                column: "stock_name");

            migrationBuilder.CreateIndex(
                name: "IX_trades_account_id",
                table: "trades",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_trades_stock_name",
                table: "trades",
                column: "stock_name");

            migrationBuilder.CreateIndex(
                name: "IX_watchlist_stock_name",
                table: "watchlist",
                column: "stock_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trade_signals");

            migrationBuilder.DropTable(
                name: "trades");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "watchlist");
        }
    }
}
