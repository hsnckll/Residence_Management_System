using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidenceMngSys.Migrations
{
    /// <inheritdoc />
    public partial class FixForgeinKeyV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_Roles_İd",
                table: "UserRoles",
                column: "Roles_İd");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_User_İd",
                table: "UserRoles",
                column: "User_İd");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_Roles_İd",
                table: "UserRoles",
                column: "Roles_İd",
                principalTable: "Roles",
                principalColumn: "İd");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_User_İd",
                table: "UserRoles",
                column: "User_İd",
                principalTable: "Users",
                principalColumn: "İd");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_Roles_İd",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_User_İd",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_Roles_İd",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_User_İd",
                table: "UserRoles");
        }
    }
}
