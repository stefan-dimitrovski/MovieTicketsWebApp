using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieShop.Repository.Migrations
{
    public partial class third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketInShoppingCarts",
                table: "TicketInShoppingCarts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketInShoppingCarts",
                table: "TicketInShoppingCarts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInShoppingCarts_TicketId",
                table: "TicketInShoppingCarts",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketInShoppingCarts",
                table: "TicketInShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_TicketInShoppingCarts_TicketId",
                table: "TicketInShoppingCarts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketInShoppingCarts",
                table: "TicketInShoppingCarts",
                columns: new[] { "TicketId", "ShoppingCartId" });
        }
    }
}
