using Microsoft.EntityFrameworkCore;
using CarSharePlusShared.Models;

namespace CarSharePlus.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Evaluacion> Evaluaciones { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Placa única en Vehiculo
            modelBuilder.Entity<Vehiculo>()
                .HasIndex(v => v.Placa)
                .IsUnique();

            // Correo único en Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            // Usuario → Vehiculos (uno a muchos)
            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Usuario)
                .WithMany(u => u.Vehiculos)
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → Reservas (uno a muchos)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehiculo → Reservas (uno a muchos)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Vehiculo)
                .WithMany(v => v.Reservas)
                .HasForeignKey(r => r.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reserva → Pagos (uno a muchos, cascade delete)
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Reserva)
                .WithMany(r => r.Pagos)
                .HasForeignKey(p => p.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Evaluaciones → Usuario y Vehiculo
            modelBuilder.Entity<Evaluacion>()
                .HasOne(e => e.Usuario)
                .WithMany(u => u.Evaluaciones)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluacion>()
                .HasOne(e => e.Vehiculo)
                .WithMany(v => v.Evaluaciones)
                .HasForeignKey(e => e.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .Property(s => s.Tipo)
                .HasConversion<string>();

            // Precisión para tarifas y montos (evita warnings y truncamiento)
            modelBuilder.Entity<Vehiculo>()
                .Property(v => v.TarifaPorHora)
                .HasPrecision(10, 2); // hasta 99,999,999.99

            modelBuilder.Entity<Reserva>()
                .Property(r => r.MontoPago)
                .HasPrecision(18, 2); // hasta 999,999,999,999,999.99

            modelBuilder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasPrecision(18, 2);
        }
    }
}
