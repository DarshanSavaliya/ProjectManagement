using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Migrations
{
    public partial class UsingUserNameinsteadofID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "UserProjects",
                newName: "Member");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Projects",
                newName: "Admin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Member",
                table: "UserProjects",
                newName: "MemberId");

            migrationBuilder.RenameColumn(
                name: "Admin",
                table: "Projects",
                newName: "AdminId");
        }
    }
}
