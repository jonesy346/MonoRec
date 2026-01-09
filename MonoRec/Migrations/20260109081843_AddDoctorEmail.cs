using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonoRec.Migrations
{
    public partial class AddDoctorEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoctorEmail",
                table: "Doctors",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorEmail",
                table: "Doctors");
        }
    }
}
