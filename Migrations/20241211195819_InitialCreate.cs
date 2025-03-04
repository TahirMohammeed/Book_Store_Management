using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Store.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    adminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    role = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.adminId);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.categoryId);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    customerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    customerEmail = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    customerPhone = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    customerPassword = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    customerAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.customerId);
                });

            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    bookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fk_categoryId = table.Column<int>(type: "int", nullable: false),
                    bookName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bookPrice = table.Column<double>(type: "float", nullable: false),
                    bookQty = table.Column<int>(type: "int", nullable: false),
                    bookAuthor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bookImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bookDesc = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.bookId);
                    table.ForeignKey(
                        name: "FK_CategoryId_Books",
                        column: x => x.fk_categoryId,
                        principalTable: "categories",
                        principalColumn: "categoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fk_customerId = table.Column<int>(type: "int", nullable: false),
                    totalPrice = table.Column<double>(type: "float", nullable: false),
                    orderDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    addressDetail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    orderStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_CustomerId_Orders",
                        column: x => x.fk_customerId,
                        principalTable: "customers",
                        principalColumn: "customerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    cartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fk_customerId = table.Column<int>(type: "int", nullable: false),
                    fk_bookId = table.Column<int>(type: "int", nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false),
                    totalPrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart", x => x.cartId);
                    table.ForeignKey(
                        name: "FK_BookId_CART",
                        column: x => x.fk_bookId,
                        principalTable: "books",
                        principalColumn: "bookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerId_Cart",
                        column: x => x.fk_customerId,
                        principalTable: "customers",
                        principalColumn: "customerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    orderDetail_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fk_orderId = table.Column<int>(type: "int", nullable: false),
                    fk_bookId = table.Column<int>(type: "int", nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_details", x => x.orderDetail_Id);
                    table.ForeignKey(
                        name: "FK_BookId_Order_Details",
                        column: x => x.fk_bookId,
                        principalTable: "books",
                        principalColumn: "bookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderId_Order_Details",
                        column: x => x.fk_orderId,
                        principalTable: "orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_books_fk_categoryId",
                table: "books",
                column: "fk_categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_fk_bookId",
                table: "cart",
                column: "fk_bookId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_fk_customerId",
                table: "cart",
                column: "fk_customerId");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_fk_bookId",
                table: "order_details",
                column: "fk_bookId");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_fk_orderId",
                table: "order_details",
                column: "fk_orderId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_fk_customerId",
                table: "orders",
                column: "fk_customerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "customers");
        }
    }
}
