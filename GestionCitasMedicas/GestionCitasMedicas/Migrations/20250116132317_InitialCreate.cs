using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionCitasMedicas.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        // Definir las constantes para evitar la repetición
        private const string IdentityAnnotation = "SqlServer:Identity";
        private const string NVarCharMax = "nvarchar(max)";
        private const string TableNameCitas = "Citas";
        private const string TableNameDoctores = "Doctores";
        private const string TableNamePacientes = "Pacientes";
        private const string TableNameProcedimientos = "Procedimientos";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: TableNameDoctores,
                columns: table => new
                {
                    IdDoctor = table.Column<int>(type: "int", nullable: false)
                        .Annotation(IdentityAnnotation, "1, 1"),  // Usar la constante
                    Nombre = table.Column<string>(type: NVarCharMax, nullable: false),  // Usar la constante
                    Especialidad = table.Column<string>(type: NVarCharMax, nullable: false),  // Usar la constante
                    Telefono = table.Column<string>(type: NVarCharMax, nullable: true),  // Usar la constante
                    Email = table.Column<string>(type: NVarCharMax, nullable: true)  // Usar la constante
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctores", x => x.IdDoctor);
                });

            migrationBuilder.CreateTable(
                name: TableNamePacientes,
                columns: table => new
                {
                    IdPaciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation(IdentityAnnotation, "1, 1"),  // Usar la constante
                    Nombre = table.Column<string>(type: NVarCharMax, nullable: false),  // Usar la constante
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Telefono = table.Column<string>(type: NVarCharMax, nullable: true),  // Usar la constante
                    Direccion = table.Column<string>(type: NVarCharMax, nullable: true)  // Usar la constante
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.IdPaciente);
                });

            migrationBuilder.CreateTable(
                name: TableNameCitas,
                columns: table => new
                {
                    IdCita = table.Column<int>(type: "int", nullable: false)
                        .Annotation(IdentityAnnotation, "1, 1"),  // Usar la constante
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false),
                    Motivo = table.Column<string>(type: NVarCharMax, nullable: true),  // Usar la constante
                    IdPaciente = table.Column<int>(type: "int", nullable: false),
                    IdDoctor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.IdCita);
                    table.ForeignKey(
                        name: "FK_Citas_Doctores_IdDoctor",
                        column: x => x.IdDoctor,
                        principalTable: TableNameDoctores,
                        principalColumn: "IdDoctor",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Citas_Pacientes_IdPaciente",
                        column: x => x.IdPaciente,
                        principalTable: TableNamePacientes,
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: TableNameProcedimientos,
                columns: table => new
                {
                    IdProcedimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation(IdentityAnnotation, "1, 1"),  // Usar la constante
                    Descripcion = table.Column<string>(type: NVarCharMax, nullable: false),  // Usar la constante
                    Costo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IdCita = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedimientos", x => x.IdProcedimiento);
                    table.ForeignKey(
                        name: "FK_Procedimientos_Citas_IdCita",
                        column: x => x.IdCita,
                        principalTable: TableNameCitas,
                        principalColumn: "IdCita",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_IdDoctor",
                table: TableNameCitas,
                column: "IdDoctor");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_IdPaciente",
                table: TableNameCitas,
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Procedimientos_IdCita",
                table: TableNameProcedimientos,
                column: "IdCita");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: TableNameProcedimientos);

            migrationBuilder.DropTable(
                name: TableNameCitas);

            migrationBuilder.DropTable(
                name: TableNameDoctores);

            migrationBuilder.DropTable(
                name: TableNamePacientes);
        }
    }
}
