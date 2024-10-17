using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymApplication.Repository.migrations
{
    /// <inheritdoc />
    public partial class Add_total_Maonth_in_Sub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "TotalMonth",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 1);
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "TotalMonth",
                table: "Subscriptions");
            

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "TotalWorkoutTime",
                table: "Subscriptions",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DayGroup",
                table: "DayGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_DayGroup_DayGroupId",
                table: "Subscriptions",
                column: "DayGroupId",
                principalTable: "DayGroup",
                principalColumn: "Id");
        }
    }
}
