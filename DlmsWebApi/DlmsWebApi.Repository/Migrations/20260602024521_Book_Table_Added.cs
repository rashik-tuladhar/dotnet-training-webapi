using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DlmsWebApi.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Book_Table_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Publication = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Isbn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TotalCopies = table.Column<int>(type: "INTEGER", nullable: false),
                    AvailableCopies = table.Column<int>(type: "INTEGER", nullable: false),
                    Edition = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
