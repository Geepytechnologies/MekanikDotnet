using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MekanikApi.Api.Migrations
{
    /// <inheritdoc />
    public partial class migrationv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleImages_Vehicles_VehicleId",
                table: "VehicleImages");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Vendors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleId",
                table: "VehicleImages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CloudinaryPublicId",
                table: "VehicleImages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "VehicleImages",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SubscriptionPlans",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "SubscriptionPlans",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CloudinaryImageId",
                table: "Mechanics",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionPlanId",
                table: "Mechanics",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_UserId",
                table: "Vendors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleImages_Vehicles_VehicleId",
                table: "VehicleImages",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_AspNetUsers_UserId",
                table: "Vendors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleImages_Vehicles_VehicleId",
                table: "VehicleImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_AspNetUsers_UserId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_UserId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CloudinaryPublicId",
                table: "VehicleImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VehicleImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CloudinaryImageId",
                table: "Mechanics");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "Mechanics");

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleId",
                table: "VehicleImages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleImages_Vehicles_VehicleId",
                table: "VehicleImages",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
