using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DeadPigeonsDB");

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "DeadPigeonsDB",
                columns: table => new
                {
                    GameId = table.Column<string>(type: "text", nullable: false),
                    WeekIdentity = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WinningNumbers = table.Column<int[]>(type: "integer[]", nullable: false),
                    CutoffTime = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "DeadPigeonsDB",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "Boards",
                schema: "DeadPigeonsDB",
                columns: table => new
                {
                    BoardId = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    GameId = table.Column<string>(type: "text", nullable: false),
                    ChosenNumbers = table.Column<int>(type: "integer", nullable: false),
                    IsWinningBoard = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.BoardId);
                    table.ForeignKey(
                        name: "FK_Boards_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "DeadPigeonsDB",
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Boards_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "DeadPigeonsDB",
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "DeadPigeonsDB",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    MobilepayTransactionNumber = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "transaction_status", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "DeadPigeonsDB",
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boards_GameId",
                schema: "DeadPigeonsDB",
                table: "Boards",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Boards_PlayerId",
                schema: "DeadPigeonsDB",
                table: "Boards",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PlayerId",
                schema: "DeadPigeonsDB",
                table: "Transactions",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boards",
                schema: "DeadPigeonsDB");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "DeadPigeonsDB");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "DeadPigeonsDB");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "DeadPigeonsDB");
        }
    }
}
