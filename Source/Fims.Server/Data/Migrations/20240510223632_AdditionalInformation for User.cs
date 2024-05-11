using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fims.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalInformationforUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformation",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInformation",
                table: "AspNetUsers");
        }
    }
}
