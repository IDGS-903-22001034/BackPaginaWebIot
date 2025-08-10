using AuthAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {

        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options)
        {
        }

        // Tablas personalizadas
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Pieza> Piezas { get; set; }
        public DbSet<MovimientosPieza> MovimientosPieza { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetalleCompra { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ComponentesProducto> ComponentesProducto { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Manual> Manuales { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVenta { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para evitar eliminación en cascada
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Configuraciones específicas de modelos

            // Proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.Property(p => p.NombreEmpresa).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Contacto).HasMaxLength(100);
                entity.Property(p => p.Telefono).HasMaxLength(20);
                entity.Property(p => p.Email).HasMaxLength(100);
            });

            // Pieza
            modelBuilder.Entity<Pieza>(entity =>
            {
                entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(p => p.UnidadMedida).HasMaxLength(50);
                entity.Property(p => p.Descripcion).HasMaxLength(255);
                entity.Property(p => p.FechaRegistro).HasDefaultValueSql("GETDATE()");
            });

            // MovimientosPieza
            modelBuilder.Entity<MovimientosPieza>(entity =>
            {
                entity.Property(m => m.TipoMovimiento)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion<string>();

                entity.Property(m => m.Cantidad).IsRequired();
                entity.Property(m => m.CostoPromedio).IsRequired();
                entity.Property(m => m.Fecha).HasDefaultValueSql("GETDATE()");

                entity.HasOne(m => m.Pieza)
                    .WithMany(p => p.MovimientosPieza)
                    .HasForeignKey(m => m.PiezaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioId)
                .IsRequired(false);


            // Configuración para ComponentesProducto (tabla intermedia)
            // ComponentesProducto
            modelBuilder.Entity<ComponentesProducto>()
                .HasKey(cp => new { cp.ProductoId, cp.PiezaId });

            modelBuilder.Entity<ComponentesProducto>()
                .HasOne(cp => cp.Producto)
                .WithMany(p => p.ComponentesProducto)
                .HasForeignKey(cp => cp.ProductoId);

            modelBuilder.Entity<ComponentesProducto>()
                .HasOne(cp => cp.Pieza)
                .WithMany(p => p.Componentes)
                .HasForeignKey(cp => cp.PiezaId);

            modelBuilder.Entity<ComponentesProducto>()
                .ToTable("ComponentesProducto"); // fuerza el nombre exacto


            // Configuración para DetalleCompra
            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.MovimientosPieza)
                .WithOne(m => m.DetalleCompra)
                .HasForeignKey<DetalleCompra>(d => d.MovimientosPiezaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Descripcion).HasMaxLength(255);
                entity.Property(p => p.Imagen).HasMaxLength(250);
                entity.Property(p => p.FechaRegistro).HasDefaultValueSql("GETDATE()");
            });

            // Comentario - Nueva configuración
            modelBuilder.Entity<Comentario>(entity =>
            {
                entity.Property(c => c.Calificacion).HasAnnotation("Range", new[] { 1, 5 });
                entity.Property(c => c.Fecha).HasDefaultValueSql("GETDATE()");

                // Relación con Venta
                entity.HasOne(c => c.Venta)
                    .WithMany(v => v.Comentarios)
                    .HasForeignKey(c => c.VentaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Producto
                entity.HasOne(c => c.Producto)
                    .WithMany(p => p.Comentarios)
                    .HasForeignKey(c => c.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice único para evitar comentarios duplicados del mismo producto en la misma venta
                entity.HasIndex(c => new { c.VentaId, c.ProductoId })
                    .IsUnique()
                    .HasDatabaseName("IX_Comentarios_VentaId_ProductoId");
            });

            // Venta
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.Property(v => v.Fecha).HasDefaultValueSql("GETDATE()");
                entity.Property(v => v.Estado).HasDefaultValue("Completada").HasMaxLength(50);
            });

            // DetalleVenta
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.DetallesVenta)
                    .HasForeignKey(d => d.ProductoId);
            });

            modelBuilder.Entity<Cotizacion>(entity =>
            {
                entity.Property(c => c.Estado)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pendiente");

                entity.Property(c => c.PrecioCalculado)
                    .HasColumnType("decimal(18,2)");

                entity.Property(c => c.PrecioFinal)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(c => c.UsuarioAsignado)
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioAsignadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Cliente)
                    .WithMany()
                    .HasForeignKey(c => c.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Venta)
                    .WithMany()
                    .HasForeignKey(c => c.VentaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}
