using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class ReviewBudgetItemGanttTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BudgetItems");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountAssigned",
                table: "BudgetItemGanttTask",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountAssigned",
                table: "BudgetItemGanttTask");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "BudgetItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
