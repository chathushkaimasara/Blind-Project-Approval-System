using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlindMatchPAS.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisorIdToProjectProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupervisorId",
                table: "ProjectProposals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_SupervisorId",
                table: "ProjectProposals",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectProposals_AspNetUsers_SupervisorId",
                table: "ProjectProposals",
                column: "SupervisorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectProposals_AspNetUsers_SupervisorId",
                table: "ProjectProposals");

            migrationBuilder.DropIndex(
                name: "IX_ProjectProposals_SupervisorId",
                table: "ProjectProposals");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "ProjectProposals");
        }
    }
}
