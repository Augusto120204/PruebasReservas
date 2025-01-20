using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReservasHotel
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ServicioAdicional> ServiciosAdicionales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServicioAdicional>()
                .HasOne(p => p.Reserva) // Un servicio tiene una reserva
                .WithMany(c => c.ServiciosAdicionales) // Una reserva tiene muchos servicios
                .HasForeignKey(p => p.IdReserva); // Clave foránea

            modelBuilder.Entity<Reserva>()
                .HasOne(p => p.Habitacion)
                .WithMany(c => c.Reservas)
                .HasForeignKey(p => p.IdHabitacion);

            modelBuilder.Entity<Reserva>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(p => p.IdCliente);

            modelBuilder.Entity<ServicioAdicional>()
                .Property(p => p.Costo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Reserva>()
                .Property(p => p.Costo)
                .HasPrecision(10, 2);

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Habitacion
    {
        [Key]
        public int IdHabitacion { get; set; }
        public string? Tipo { get; set; }
        public bool? Disponible { get; set; }
        [JsonIgnore]
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }

    public class Cliente
    {
        [Key]
        public int IdCLiente { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? CI { get; set; }
        [JsonIgnore]
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }

    public class ServicioAdicional
    {
        [Key]
        public int IdServicio { get; set; }
        public string? Descripcion { get; set; }
        public Decimal? Costo { get; set; }
        public int? IdReserva { get; set; }
        [JsonIgnore]
        public Reserva? Reserva { get; set; }
    }

    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }
        public int? IdCliente { get; set; }
        [JsonIgnore]
        public Cliente? Cliente { get; set; }
        public int? IdHabitacion { get; set; }
        [JsonIgnore]
        public Habitacion? Habitacion { get; set; }
        public Decimal? Costo { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        [JsonIgnore]
        public ICollection<ServicioAdicional> ServiciosAdicionales { get; set; } = new List<ServicioAdicional>();
    }
}
