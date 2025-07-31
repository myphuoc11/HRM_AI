using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_AI.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class EntityV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewType",
                table: "InterviewSchedules");

            migrationBuilder.AlterColumn<int>(
                name: "Round",
                table: "InterviewSchedules",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "InterviewTypeId",
                table: "InterviewSchedules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "InterviewTypeDictionarys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewTypeDictionarys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_InterviewTypeId",
                table: "InterviewSchedules",
                column: "InterviewTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedules_InterviewTypeDictionarys_InterviewTypeId",
                table: "InterviewSchedules",
                column: "InterviewTypeId",
                principalTable: "InterviewTypeDictionarys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedules_InterviewTypeDictionarys_InterviewTypeId",
                table: "InterviewSchedules");

            migrationBuilder.DropTable(
                name: "InterviewTypeDictionarys");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSchedules_InterviewTypeId",
                table: "InterviewSchedules");

            migrationBuilder.DropColumn(
                name: "InterviewTypeId",
                table: "InterviewSchedules");

            migrationBuilder.AlterColumn<int>(
                name: "Round",
                table: "InterviewSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterviewType",
                table: "InterviewSchedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
