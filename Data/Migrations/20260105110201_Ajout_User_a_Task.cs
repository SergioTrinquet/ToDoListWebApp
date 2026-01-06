using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoListWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Ajout_User_a_Task : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_Priorite_PrioriteNavigationId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_PrioriteNavigationId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "PrioriteNavigationId",
                table: "Task");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Task",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Priorite",
                table: "Task",
                column: "Priorite");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Priorite_Priorite",
                table: "Task",
                column: "Priorite",
                principalTable: "Priorite",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_Priorite_Priorite",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_Priorite",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Task");

            migrationBuilder.AddColumn<int>(
                name: "PrioriteNavigationId",
                table: "Task",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_PrioriteNavigationId",
                table: "Task",
                column: "PrioriteNavigationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Priorite_PrioriteNavigationId",
                table: "Task",
                column: "PrioriteNavigationId",
                principalTable: "Priorite",
                principalColumn: "Id");
        }
    }
}
