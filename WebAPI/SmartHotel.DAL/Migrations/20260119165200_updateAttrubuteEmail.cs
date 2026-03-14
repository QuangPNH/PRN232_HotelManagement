using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHotel.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateAttrubuteEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Accounts",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                newName: "IX_Accounts_Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Accounts",
                newName: "Username");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                newName: "IX_Accounts_Username");
        }
    }
}
