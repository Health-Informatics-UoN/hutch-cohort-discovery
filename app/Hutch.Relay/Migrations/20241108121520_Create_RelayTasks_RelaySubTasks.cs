using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hutch.Relay.Migrations
{
    /// <inheritdoc />
    public partial class Create_RelayTasks_RelaySubTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelayTasks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Collection = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelayTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelaySubTasks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: true),
                    RelayTaskId = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelaySubTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelaySubTasks_RelayTasks_RelayTaskId",
                        column: x => x.RelayTaskId,
                        principalTable: "RelayTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RelaySubTasks_SubNodes_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "SubNodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelaySubTasks_OwnerId",
                table: "RelaySubTasks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RelaySubTasks_RelayTaskId",
                table: "RelaySubTasks",
                column: "RelayTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelaySubTasks");

            migrationBuilder.DropTable(
                name: "RelayTasks");
        }
    }
}
