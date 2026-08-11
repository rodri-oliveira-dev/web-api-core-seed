using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPratosPaginationOrderingIndex : Migration
    {
        private static readonly string[] PratoOrderingColumns = { "Titulo", "Id" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pratos_Titulo_Id",
                table: "Pratos",
                columns: PratoOrderingColumns);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pratos_Titulo_Id",
                table: "Pratos");
        }
    }
}
