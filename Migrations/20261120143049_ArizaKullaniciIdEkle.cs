using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITArizaTakip.Migrations
{
    /// <inheritdoc />
    public partial class ArizaKullaniciIdEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KullaniciId",
                table: "Arizalar",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "Arizalar");
        }
    }
}
