using Microsoft.EntityFrameworkCore.Migrations;

namespace Shops.Migrations
{
    public partial class edit_cascade_relationShip_user_hummanClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_HumanClass_HumanClassId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "HumanClassId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_HumanClass_HumanClassId",
                table: "AspNetUsers",
                column: "HumanClassId",
                principalTable: "HumanClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_HumanClass_HumanClassId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "HumanClassId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_HumanClass_HumanClassId",
                table: "AspNetUsers",
                column: "HumanClassId",
                principalTable: "HumanClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
