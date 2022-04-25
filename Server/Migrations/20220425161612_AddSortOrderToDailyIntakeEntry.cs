using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazingRecept.Server.Migrations
{
    public partial class AddSortOrderToDailyIntakeEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "DailyIntakeEntry",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "DailyIntakeEntry");
        }
    }
}
