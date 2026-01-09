using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonoRec.Migrations
{
    public partial class PopulateDoctorEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Email population is handled in DbInitializer.cs instead
            // This migration is intentionally empty as we can't join across databases in SQLite
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
