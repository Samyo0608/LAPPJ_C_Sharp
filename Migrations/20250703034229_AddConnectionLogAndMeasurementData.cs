using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionLogAndMeasurementData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Port = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    Voltage = table.Column<double>(type: "REAL", nullable: false),
                    MainGasFlow = table.Column<double>(type: "REAL", nullable: false),
                    MainGasFlowUnit = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CarrierGasFlow = table.Column<double>(type: "REAL", nullable: false),
                    CarrierGasFlowUnit = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CarrierGasType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LaserPowerPercentage = table.Column<double>(type: "REAL", nullable: false),
                    IsLaserOn = table.Column<bool>(type: "INTEGER", nullable: false),
                    HeaterTemperature = table.Column<double>(type: "REAL", nullable: false),
                    IsUltrasonicOn = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PhotoPath = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    SavePath = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionLogs");

            migrationBuilder.DropTable(
                name: "MeasurementDatas");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
