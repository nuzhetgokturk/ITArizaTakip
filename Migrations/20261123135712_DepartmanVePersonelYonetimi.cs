using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ITArizaTakip.Migrations
{
    /// <inheritdoc />
    public partial class DepartmanVePersonelYonetimi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AktifMi",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmanId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departmanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departmanlar", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmanId",
                table: "AspNetUsers",
                column: "DepartmanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departmanlar_DepartmanId",
                table: "AspNetUsers",
                column: "DepartmanId",
                principalTable: "Departmanlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departmanlar_DepartmanId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Departmanlar");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DepartmanId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AktifMi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DepartmanId",
                table: "AspNetUsers");
        }
    }
}
