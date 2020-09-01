using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Restaurante.IO.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Atendentes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    TipoAtendente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atendentes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Loggin",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    Escopo = table.Column<string>(type: "varchar(100)", nullable: true),
                    LogLevel = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "varchar(6000)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loggin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mesas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Numero = table.Column<string>(type: "varchar(50)", nullable: false),
                    Lugares = table.Column<int>(type: "int", nullable: false, defaultValue: 4),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LocalizacaoMesa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pratos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Titulo = table.Column<string>(type: "varchar(200)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(800)", nullable: false),
                    Foto = table.Column<string>(type: "varchar(200)", nullable: false),
                    Preco = table.Column<double>(type: "float", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TipoPrato = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pratos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AtendenteId = table.Column<Guid>(nullable: false),
                    MesaId = table.Column<Guid>(nullable: false),
                    Numero = table.Column<string>(type: "varchar(50)", nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DataHoraEncerrado = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedidos_Atendentes",
                        column: x => x.AtendenteId,
                        principalTable: "Atendentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Mesas",
                        column: x => x.MesaId,
                        principalTable: "Mesas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoPrato",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PedidoId = table.Column<Guid>(nullable: false),
                    PratoId = table.Column<Guid>(nullable: false),
                    StatusProducao = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(1000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoPrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoPrato_Pedidos",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoPrato_Pratos",
                        column: x => x.PratoId,
                        principalTable: "Pratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoPrato_PedidoId",
                table: "PedidoPrato",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoPrato_PratoId",
                table: "PedidoPrato",
                column: "PratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_AtendenteId",
                table: "Pedidos",
                column: "AtendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_MesaId",
                table: "Pedidos",
                column: "MesaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loggin");

            migrationBuilder.DropTable(
                name: "PedidoPrato");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Pratos");

            migrationBuilder.DropTable(
                name: "Atendentes");

            migrationBuilder.DropTable(
                name: "Mesas");
        }
    }
}
