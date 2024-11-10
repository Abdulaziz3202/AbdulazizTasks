using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPMGTask.Migrations
{
    /// <inheritdoc />
    public partial class updatetest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistory_TransactionType_TransactionTypeId1",
                table: "TransactionHistory");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistory_TransactionTypeId1",
                table: "TransactionHistory");

            migrationBuilder.DropColumn(
                name: "TransactionTypeId1",
                table: "TransactionHistory");

            migrationBuilder.AlterColumn<long>(
                name: "TransactionTypeId",
                table: "TransactionHistory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_TransactionTypeId",
                table: "TransactionHistory",
                column: "TransactionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistory_TransactionType_TransactionTypeId",
                table: "TransactionHistory",
                column: "TransactionTypeId",
                principalTable: "TransactionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistory_TransactionType_TransactionTypeId",
                table: "TransactionHistory");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistory_TransactionTypeId",
                table: "TransactionHistory");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionTypeId",
                table: "TransactionHistory",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "TransactionTypeId1",
                table: "TransactionHistory",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_TransactionTypeId1",
                table: "TransactionHistory",
                column: "TransactionTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistory_TransactionType_TransactionTypeId1",
                table: "TransactionHistory",
                column: "TransactionTypeId1",
                principalTable: "TransactionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
