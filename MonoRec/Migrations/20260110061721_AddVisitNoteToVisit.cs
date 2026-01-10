using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonoRec.Migrations
{
    public partial class AddVisitNoteToVisit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VisitNote",
                table: "Visits",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitNote",
                table: "Visits");
        }
    }
}
