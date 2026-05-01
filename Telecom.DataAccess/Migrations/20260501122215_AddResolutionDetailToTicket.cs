using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telecom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddResolutionDetailToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResolutionDetail",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolutionDetail",
                table: "Tickets");
        }
    }
}
