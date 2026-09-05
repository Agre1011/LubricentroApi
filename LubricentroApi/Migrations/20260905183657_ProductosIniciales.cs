using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LubricentroApi.Migrations
{
    /// <inheritdoc />
    public partial class ProductosIniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "IdProducto", "Activo", "IdCategoria", "Imagen", "Marca", "Nombre", "PrecioCompra", "PrecioVenta", "Stock", "Variante" },
                values: new object[,]
                {
                    { 1, true, 1, null, "Total", "Aceite Motor Auto", 30000m, 40000m, 0, "5W-30" },
                    { 2, true, 1, null, "Motul", "Aceite Motor Auto", 32000m, 42000m, 0, "5W-30" },
                    { 3, true, 1, null, "Total", "Aceite Motor Auto", 28000m, 38000m, 0, "10W-40" },
                    { 4, true, 1, null, "Motul", "Aceite Motor Auto", 30000m, 40000m, 0, "10W-40" },
                    { 5, true, 1, null, "Total", "Aceite Motor Auto", 26000m, 36000m, 0, "15W-40" },
                    { 6, true, 1, null, "Motul", "Aceite Motor Auto", 28000m, 38000m, 0, "15W-40" },
                    { 7, true, 2, null, "Castrol", "Aceite Motor Moto", 15000m, 22000m, 0, "20W-50" },
                    { 8, true, 2, null, "Motul", "Aceite Motor Moto", 16000m, 23000m, 0, "20W-50" },
                    { 9, true, 2, null, "Castrol", "Aceite Motor Moto", 17000m, 24000m, 0, "15W-50" },
                    { 10, true, 2, null, "Motul", "Aceite Motor Moto", 18000m, 25000m, 0, "15W-50" },
                    { 11, true, 3, null, "Total", "Líquido Refrigerante", 8000m, 12000m, 0, "Verde" },
                    { 12, true, 3, null, "Wander", "Líquido Refrigerante", 7500m, 11000m, 0, "Verde" },
                    { 13, true, 3, null, "Total", "Líquido Refrigerante", 8500m, 12500m, 0, "Amarillo" },
                    { 14, true, 3, null, "Wander", "Líquido Refrigerante", 8000m, 12000m, 0, "Amarillo" },
                    { 15, true, 3, null, "Total", "Líquido Refrigerante", 9000m, 13000m, 0, "Rojo" },
                    { 16, true, 3, null, "Wander", "Líquido Refrigerante", 8500m, 12500m, 0, "Rojo" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 16);
        }
    }
}
