using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComentariosToVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VentaId",
                table: "Comentarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_VentaId_ProductoId",
                table: "Comentarios",
                columns: new[] { "VentaId", "ProductoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Ventas_VentaId",
                table: "Comentarios",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Ventas_VentaId",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_VentaId_ProductoId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "VentaId",
                table: "Comentarios");
        }
    }
}
