using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecAll.Core.List.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "itemseq",
                schema: "list",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    ContribId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserIdentityGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_items_listtypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "listtypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_items_sets_SetId",
                        column: x => x.SetId,
                        principalTable: "sets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_items_ContribId",
                table: "items",
                column: "ContribId");

            migrationBuilder.CreateIndex(
                name: "IX_items_SetId",
                table: "items",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_items_TypeId",
                table: "items",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropSequence(
                name: "itemseq",
                schema: "list");
        }
    }
}
