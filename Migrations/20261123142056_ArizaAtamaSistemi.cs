using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITArizaTakip.Migrations
{
    /// <inheritdoc />
    public partial class ArizaAtamaSistemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AtananDepartmanId",
                table: "Arizalar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AtananPersonelId",
                table: "Arizalar",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Arizalar_AtananDepartmanId",
                table: "Arizalar",
                column: "AtananDepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_Arizalar_AtananPersonelId",
                table: "Arizalar",
                column: "AtananPersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arizalar_AspNetUsers_AtananPersonelId",
                table: "Arizalar",
                column: "AtananPersonelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Arizalar_Departmanlar_AtananDepartmanId",
                table: "Arizalar",
                column: "AtananDepartmanId",
                principalTable: "Departmanlar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arizalar_AspNetUsers_AtananPersonelId",
                table: "Arizalar");

            migrationBuilder.DropForeignKey(
                name: "FK_Arizalar_Departmanlar_AtananDepartmanId",
                table: "Arizalar");

            migrationBuilder.DropIndex(
                name: "IX_Arizalar_AtananDepartmanId",
                table: "Arizalar");

            migrationBuilder.DropIndex(
                name: "IX_Arizalar_AtananPersonelId",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "AtananDepartmanId",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "AtananPersonelId",
                table: "Arizalar");
        }
    }
}
