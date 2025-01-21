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
        public int? IdHabitacion { get; set; }

        [Required(ErrorMessage = "El tipo de habitación es obligatorio.")]
        [StringLength(50, ErrorMessage = "El tipo no puede superar los 50 caracteres.")]
        public string? Tipo { get; set; }

        [Required(ErrorMessage = "El estado de disponibilidad es obligatorio.")]
        public bool? Disponible { get; set; }

        [JsonIgnore]
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }

    public class Cliente
    {
        [Key]
        public int? IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 caracteres.")]
        public string? CI { get; set; }
        [JsonIgnore]
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }

    public class ServicioAdicional
    {
        [Key]
        public int? IdServicio { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        public decimal? Costo { get; set; }

        [Required(ErrorMessage = "El ID de la reserva es obligatorio.")]
        public int? IdReserva { get; set; }

        [JsonIgnore]
        public Reserva? Reserva { get; set; }
    }

    public class Reserva
    {
        [Key]
        public int? IdReserva { get; set; }
        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        public int? IdCliente { get; set; }

        [JsonIgnore]
        public Cliente? Cliente { get; set; }

        [Required(ErrorMessage = "El ID de la habitación es obligatorio.")]
        public int? IdHabitacion { get; set; }

        [JsonIgnore]
        public Habitacion? Habitacion { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        public decimal? Costo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateTime? FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        public DateTime? FechaFin { get; set; }

        [JsonIgnore]
        public ICollection<ServicioAdicional> ServiciosAdicionales { get; set; } = new List<ServicioAdicional>();
    }
}
