using LubricentroApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LubricentroApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Ingreso> Ingresos { get; set; }
        public DbSet<DetalleIngreso> DetalleIngresos { get; set; }
        public DbSet<Salida> Salidas { get; set; }
        public DbSet<DetalleSalida> DetalleSalidas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------------------------
            // USUARIOS
            // -------------------------------------------

            // El nombre de usuario no puede repetirse.
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // -------------------------------------------
            // PRODUCTOS
            // -------------------------------------------

            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioCompra)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioVenta)
                .HasPrecision(18, 2);

            // Categoria 1:N Productos
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------------------------
            // INGRESOS
            // -------------------------------------------

            // Proveedor 1:N Ingresos
            modelBuilder.Entity<Ingreso>()
                .HasOne(i => i.Proveedor)
                .WithMany(p => p.Ingresos)
                .HasForeignKey(i => i.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario 1:N Ingresos
            modelBuilder.Entity<Ingreso>()
                .HasOne(i => i.Usuario)
                .WithMany(u => u.Ingresos)
                .HasForeignKey(i => i.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Ingreso 1:N DetalleIngresos
            modelBuilder.Entity<DetalleIngreso>()
                .HasOne(d => d.Ingreso)
                .WithMany(i => i.Detalles)
                .HasForeignKey(d => d.IdIngreso)
                .OnDelete(DeleteBehavior.Cascade);

            // Producto 1:N DetalleIngresos
            modelBuilder.Entity<DetalleIngreso>()
                .HasOne(d => d.Producto)
                .WithMany(p => p.DetallesIngresos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleIngreso>()
                .Property(d => d.PrecioCompraUnitario)
                .HasPrecision(18, 2);

            // -------------------------------------------
            // SALIDAS
            // -------------------------------------------

            // Cliente 1:N Salidas
            modelBuilder.Entity<Salida>()
                .HasOne(s => s.Cliente)
                .WithMany(c => c.Salidas)
                .HasForeignKey(s => s.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario 1:N Salidas
            modelBuilder.Entity<Salida>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Salidas)
                .HasForeignKey(s => s.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Salida 1:N DetalleSalidas
            modelBuilder.Entity<DetalleSalida>()
                .HasOne(d => d.Salida)
                .WithMany(s => s.Detalles)
                .HasForeignKey(d => d.IdSalida)
                .OnDelete(DeleteBehavior.Cascade);

            // Producto 1:N DetalleSalidas
            modelBuilder.Entity<DetalleSalida>()
                .HasOne(d => d.Producto)
                .WithMany(p => p.DetallesSalidas)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleSalida>()
                .Property(d => d.PrecioVentaUnitario)
                .HasPrecision(18, 2);

            // -------------------------------------------
            // DATOS INICIALES
            // -------------------------------------------

            // Categorías iniciales
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    IdCategoria = 1,
                    Nombre = "Aceite Motor Auto",
                    Descripcion = "Lubricantes para motores de automóviles"
                },
                new Categoria
                {
                    IdCategoria = 2,
                    Nombre = "Aceite Motor Moto",
                    Descripcion = "Lubricantes para motores de motocicletas"
                },
                new Categoria
                {
                    IdCategoria = 3,
                    Nombre = "Líquido Refrigerante",
                    Descripcion = "Refrigerantes para sistemas de enfriamiento"
                }
            );

            // Proveedores iniciales
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor
                {
                    IdProveedor = 1,
                    Nombre = "Total",
                    CUIT = "30-00000001-1",
                    Activo = true
                },
                new Proveedor
                {
                    IdProveedor = 2,
                    Nombre = "Motul",
                    CUIT = "30-00000002-2",
                    Activo = true
                },
                new Proveedor
                {
                    IdProveedor = 3,
                    Nombre = "Castrol",
                    CUIT = "30-00000003-3",
                    Activo = true
                },
                new Proveedor
                {
                    IdProveedor = 4,
                    Nombre = "Wander",
                    CUIT = "30-00000004-4",
                    Activo = true
                }
            );

            // Cliente especial para ventas sin identificación
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    IdCliente = 1,
                    Nombre = "Consumidor Final",
                    Activo = true
                }
            );

            // Productos iniciales
            modelBuilder.Entity<Producto>().HasData(

                // -------------------------------------------
                // ACEITES PARA MOTOR DE AUTO
                // -------------------------------------------

                new Producto
                {
                    IdProducto = 1,
                    Nombre = "Aceite Motor Auto",
                    Marca = "Total",
                    Variante = "5W-30",
                    PrecioCompra = 30000m,
                    PrecioVenta = 40000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 1,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 2,
                    Nombre = "Aceite Motor Auto",
                    Marca = "Motul",
                    Variante = "5W-30",
                    PrecioCompra = 32000m,
                    PrecioVenta = 42000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 1,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 3,
                    Nombre = "Aceite Motor Auto",
                    Marca = "Total",
                    Variante = "10W-40",
                    PrecioCompra = 28000m,
                    PrecioVenta = 38000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 1,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 4,
                    Nombre = "Aceite Motor Auto",
                    Marca = "Motul",
                    Variante = "10W-40",
                    PrecioCompra = 30000m,
                    PrecioVenta = 40000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 1,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 5,
                    Nombre = "Aceite Motor Auto",
                    Marca = "Total",
                    Variante = "15W-40",
                    PrecioCompra = 26000m,
                    PrecioVenta = 36000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 1,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 6,
                    Nombre = "Aceite Motor Auto",
                    Marca = "Motul",
                    Variante = "15W-40",
                    PrecioCompra = 28000m,
                    PrecioVenta = 38000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 1,
                    Activo = true
                },

                // -------------------------------------------
                // ACEITES PARA MOTOR DE MOTO
                // -------------------------------------------

                new Producto
                {
                    IdProducto = 7,
                    Nombre = "Aceite Motor Moto",
                    Marca = "Castrol",
                    Variante = "20W-50",
                    PrecioCompra = 15000m,
                    PrecioVenta = 22000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 2,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 8,
                    Nombre = "Aceite Motor Moto",
                    Marca = "Motul",
                    Variante = "20W-50",
                    PrecioCompra = 16000m,
                    PrecioVenta = 23000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 2,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 9,
                    Nombre = "Aceite Motor Moto",
                    Marca = "Castrol",
                    Variante = "15W-50",
                    PrecioCompra = 17000m,
                    PrecioVenta = 24000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 2,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 10,
                    Nombre = "Aceite Motor Moto",
                    Marca = "Motul",
                    Variante = "15W-50",
                    PrecioCompra = 18000m,
                    PrecioVenta = 25000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 2,
                    Activo = true
                },

                // -------------------------------------------
                // REFRIGERANTES
                // -------------------------------------------

                new Producto
                {
                    IdProducto = 11,
                    Nombre = "Líquido Refrigerante",
                    Marca = "Total",
                    Variante = "Verde",
                    PrecioCompra = 8000m,
                    PrecioVenta = 12000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 3,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 12,
                    Nombre = "Líquido Refrigerante",
                    Marca = "Wander",
                    Variante = "Verde",
                    PrecioCompra = 7500m,
                    PrecioVenta = 11000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 3,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 13,
                    Nombre = "Líquido Refrigerante",
                    Marca = "Total",
                    Variante = "Amarillo",
                    PrecioCompra = 8500m,
                    PrecioVenta = 12500m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 3,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 14,
                    Nombre = "Líquido Refrigerante",
                    Marca = "Wander",
                    Variante = "Amarillo",
                    PrecioCompra = 8000m,
                    PrecioVenta = 12000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 3,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 15,
                    Nombre = "Líquido Refrigerante",
                    Marca = "Total",
                    Variante = "Rojo",
                    PrecioCompra = 9000m,
                    PrecioVenta = 13000m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 3,
                    Activo = true
                },

                new Producto
                {
                    IdProducto = 16,
                    Nombre = "Líquido Refrigerante",
                    Marca = "Wander",
                    Variante = "Rojo",
                    PrecioCompra = 8500m,
                    PrecioVenta = 12500m,
                    Stock = 0,
                    Imagen = null,
                    IdCategoria = 3,
                    Activo = true
                }
            );

        }
    }
}