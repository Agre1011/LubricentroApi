using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LubricentroApi.Migrations
{
    /// <inheritdoc />
    public partial class DatosIniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "IdCategoria", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Lubricantes para motores de automóviles", "Aceite Motor Auto" },
                    { 2, "Lubricantes para motores de motocicletas", "Aceite Motor Moto" },
                    { 3, "Refrigerantes para sistemas de enfriamiento", "Líquido Refrigerante" }
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "IdCliente", "Activo", "Apellido", "CUIL", "Email", "Nombre", "Telefono" },
                values: new object[] { 1, true, null, null, null, "Consumidor Final", null });

            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "IdProveedor", "Activo", "CUIT", "Email", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, true, "30-00000001-1", null, "Total", null },
                    { 2, true, "30-00000002-2", null, "Motul", null },
                    { 3, true, "30-00000003-3", null, "Castrol", null },
                    { 4, true, "30-00000004-4", null, "Wander", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "IdCategoria",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "IdCategoria",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "IdCategoria",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "IdCliente",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Proveedores",
                keyColumn: "IdProveedor",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Proveedores",
                keyColumn: "IdProveedor",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Proveedores",
                keyColumn: "IdProveedor",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Proveedores",
                keyColumn: "IdProveedor",
                keyValue: 4);
        }
    }
}
