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
        }
    }
}