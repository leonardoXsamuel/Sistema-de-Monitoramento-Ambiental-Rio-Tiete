using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApsMartChat.Migrations
{
    /// <inheritdoc />
    public partial class FixSentAtToDateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SentAt",
                table: "Messages",
                type: "datetimeoffset",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldMaxLength: 400);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SentAt",
                table: "Messages",
                type: "datetime2",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldMaxLength: 400);
        }
    }
}
