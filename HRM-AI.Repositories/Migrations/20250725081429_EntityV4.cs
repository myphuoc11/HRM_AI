using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_AI.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class EntityV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CVApplicants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "CVApplicants");
        }
    }
}
