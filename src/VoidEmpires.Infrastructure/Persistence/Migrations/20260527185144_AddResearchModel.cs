using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResearchModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResearchProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResearchType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchProjects", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResearchProjects_CivilizationId_ResearchType",
                table: "ResearchProjects",
                columns: new[] { "CivilizationId", "ResearchType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResearchProjects");
        }
    }
}
