using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class EmailCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmailCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CantidadDispositivos = table.Column<int>(type: "int", nullable: false),
                    CantidadAnimales = table.Column<int>(type: "int", nullable: false),
                    TipoGanado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Hectareas = table.Column<int>(type: "int", nullable: false),
                    FuncionalidadesRequeridas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    PrecioCalculado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PrecioFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NotasInternas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UsuarioAsignadoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClienteCreado = table.Column<bool>(type: "bit", nullable: false),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VentaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_AspNetUsers_UsuarioAsignadoId",
                        column: x => x.UsuarioAsignadoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ClienteId",
                table: "Cotizaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_UsuarioAsignadoId",
                table: "Cotizaciones",
                column: "UsuarioAsignadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_VentaId",
                table: "Cotizaciones",
                column: "VentaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "EmailTemplates");
        }
    }
}
