using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetItem_Projects_ProjectId",
                table: "BudgetItem");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetItemGanttTask_BudgetItem_BudgetItemId",
                table: "BudgetItemGanttTask");

            migrationBuilder.DropForeignKey(
                name: "FK_KnownRiskBudgetItem_BudgetItem_BudgetItemId",
                table: "KnownRiskBudgetItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_BudgetItem_BudgetItemId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_QualityBudgetItem_BudgetItem_BudgetItemId",
                table: "QualityBudgetItem");

            migrationBuilder.DropForeignKey(
                name: "FK_RiskBudgetItem_BudgetItem_BudgetItemId",
                table: "RiskBudgetItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetItem",
                table: "BudgetItem");

            migrationBuilder.RenameTable(
                name: "BudgetItem",
                newName: "BudgetItems");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetItem_ProjectId",
                table: "BudgetItems",
                newName: "IX_BudgetItems_ProjectId");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPriceUSD",
                table: "BudgetItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "BudgetUSD",
                table: "BudgetItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetItems",
                table: "BudgetItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetItemGanttTask_BudgetItems_BudgetItemId",
                table: "BudgetItemGanttTask",
                column: "BudgetItemId",
                principalTable: "BudgetItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetItems_Projects_ProjectId",
                table: "BudgetItems",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnownRiskBudgetItem_BudgetItems_BudgetItemId",
                table: "KnownRiskBudgetItem",
                column: "BudgetItemId",
                principalTable: "BudgetItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_BudgetItems_BudgetItemId",
                table: "PurchaseOrderItems",
                column: "BudgetItemId",
                principalTable: "BudgetItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QualityBudgetItem_BudgetItems_BudgetItemId",
                table: "QualityBudgetItem",
                column: "BudgetItemId",
                principalTable: "BudgetItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskBudgetItem_BudgetItems_BudgetItemId",
                table: "RiskBudgetItem",
                column: "BudgetItemId",
                principalTable: "BudgetItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetItemGanttTask_BudgetItems_BudgetItemId",
                table: "BudgetItemGanttTask");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetItems_Projects_ProjectId",
                table: "BudgetItems");

            migrationBuilder.DropForeignKey(
                name: "FK_KnownRiskBudgetItem_BudgetItems_BudgetItemId",
                table: "KnownRiskBudgetItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_BudgetItems_BudgetItemId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_QualityBudgetItem_BudgetItems_BudgetItemId",
                table: "QualityBudgetItem");

            migrationBuilder.DropForeignKey(
                name: "FK_RiskBudgetItem_BudgetItems_BudgetItemId",
                table: "RiskBudgetItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetItems",
                table: "BudgetItems");

            migrationBuilder.RenameTable(
                name: "BudgetItems",
                newName: "BudgetItem");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetItems_ProjectId",
                table: "BudgetItem",
                newName: "IX_BudgetItem_ProjectId");

            migrationBuilder.AlterColumn<double>(
                name: "UnitPriceUSD",
                table: "BudgetItem",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "BudgetUSD",
                table: "BudgetItem",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetItem",
                table: "BudgetItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetItem_Projects_ProjectId",
                table: "BudgetItem",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetItemGanttTask_BudgetItem_BudgetItemId",
                table: "BudgetItemGanttTask",
                column: "BudgetItemId",
                principalTable: "BudgetItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnownRiskBudgetItem_BudgetItem_BudgetItemId",
                table: "KnownRiskBudgetItem",
                column: "BudgetItemId",
                principalTable: "BudgetItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_BudgetItem_BudgetItemId",
                table: "PurchaseOrderItems",
                column: "BudgetItemId",
                principalTable: "BudgetItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QualityBudgetItem_BudgetItem_BudgetItemId",
                table: "QualityBudgetItem",
                column: "BudgetItemId",
                principalTable: "BudgetItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskBudgetItem_BudgetItem_BudgetItemId",
                table: "RiskBudgetItem",
                column: "BudgetItemId",
                principalTable: "BudgetItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
