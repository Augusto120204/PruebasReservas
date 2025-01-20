using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ReservasHotel.AppDBContext;

namespace ReservasHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioController : ControllerBase
    {
        private readonly AppDBContext _appDBcontext;

        public ServicioController(AppDBContext appDBcontext)
        {
            _appDBcontext = appDBcontext;
        }

        //Servicios adicionales
        [HttpGet]
        public async Task<IActionResult> GetServicios()
        {
            return Ok(await _appDBcontext.ServiciosAdicionales.ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> CreateServicio(ServicioAdicional servicio)
        {
            if (_appDBcontext.ServiciosAdicionales.Find(servicio.IdServicio) != null) return BadRequest("Ya existe un servicio con este id");

            if(_appDBcontext.Reservas.Find(servicio.IdReserva) == null) return BadRequest("La reserva no existe");

            if(servicio.Costo < 0) return BadRequest("El servicio no puede tener un costo negativo");

            _appDBcontext.ServiciosAdicionales.Add(servicio);
            await _appDBcontext.SaveChangesAsync();
            return Ok(servicio);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServicio(int id, ServicioAdicional servicio)
        {
            var servicioExistente = await _appDBcontext.ServiciosAdicionales.FindAsync(id);

            if (servicioExistente == null) return NotFound("No existe un servicio con este id");

            if (_appDBcontext.Reservas.Find(servicio.IdReserva) == null) return BadRequest("La reserva no existe");

            if (servicio.Costo < 0) return BadRequest("El servicio no puede tener un costo negativo");

            servicioExistente.IdReserva = servicio.IdReserva;
            servicioExistente.Descripcion = servicio.Descripcion;
            servicioExistente.Costo = servicio.Costo;

            await _appDBcontext.SaveChangesAsync();
            return Ok(servicioExistente);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var servicio = await _appDBcontext.ServiciosAdicionales.FindAsync(id);

            if (servicio == null) return NotFound("No existe un servicio con este id");

            _appDBcontext.ServiciosAdicionales.Remove(servicio);
            await _appDBcontext.SaveChangesAsync();
            return Ok(servicio);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class  ReservaController : ControllerBase
    {
        private readonly AppDBContext _appDBcontext;

        public ReservaController(AppDBContext appDBcontext)
        {
            _appDBcontext = appDBcontext;
        }

        //Reservas
        [HttpGet]
        public async Task<IActionResult> GetReservas()
        {
            return Ok(await _appDBcontext.Reservas.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateReserva(Reserva reserva)
        {
            if (_appDBcontext.Reservas.Find(reserva.IdReserva) != null) return BadRequest("Ya existe una reserva con este id");

            if (_appDBcontext.Habitaciones.Find(reserva.IdHabitacion) == null) return BadRequest("La habitación no existe.");

            if (_appDBcontext.Clientes.Find(reserva.IdCliente) == null) return BadRequest("El cliente no existe.");

            if (reserva.FechaFin < reserva.FechaInicio) return BadRequest("La fecha de fin debe ser mayor a la de inicio.");

            if (reserva.Costo < 0) return BadRequest("La reserva no puede tener un costo negativo");

            // Guardar reserva
            _appDBcontext.Reservas.Add(reserva);
            await _appDBcontext.SaveChangesAsync();
            return Ok(reserva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReserva(int id, Reserva reserva)
        {
            var reservaExistente = await _appDBcontext.Reservas.FindAsync(id);

            if (reservaExistente == null) return NotFound("No existe una reserva con este id");

            if (_appDBcontext.Habitaciones.Find(reserva.IdHabitacion) == null) return BadRequest("La habitación no existe");

            if (_appDBcontext.Clientes.Find(reserva.IdCliente) == null) return BadRequest("El cliente no existe");

            if (reserva.FechaFin < reserva.FechaInicio) return BadRequest("La fecha de fin debe ser mayor a la de inicio");

            if (reserva.Costo < 0) return BadRequest("La reserva no puede tener un costo negativo");

            reservaExistente.IdHabitacion = reserva.IdHabitacion;
            reservaExistente.IdCliente = reserva.IdCliente;
            reservaExistente.FechaInicio = reserva.FechaInicio;
            reservaExistente.FechaFin = reserva.FechaFin;
            reservaExistente.Costo = reserva.Costo;

            await _appDBcontext.SaveChangesAsync();
            return Ok(reservaExistente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _appDBcontext.Reservas.FindAsync(id);

            if (reserva == null) return NotFound("No existe una reserva con este id");

            if (reserva.ServiciosAdicionales.Count > 0) return BadRequest("No se puede eliminar la reserva porque tiene relaciones");

            _appDBcontext.Reservas.Remove(reserva);
            await _appDBcontext.SaveChangesAsync();
            return Ok(reserva);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDBContext _appDBcontext;
        public ClienteController(AppDBContext appDBcontext)
        {
            _appDBcontext = appDBcontext;
        }

        //Clientes
        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            return Ok(await _appDBcontext.Clientes.ToListAsync());
        }

        private bool EsCedulaValida(string ci)
        {
            // Verificar que tenga 10 dígitos
            if (ci.Length != 10 || !ci.All(char.IsDigit))
                return false;

            // Extraer provincia (dos primeros dígitos) y verificar rango
            int provincia = int.Parse(ci.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
                return false;

            // Extraer el tercer dígito (tipo de persona)
            int tercerDigito = int.Parse(ci[2].ToString());
            if (tercerDigito >= 6) // No válido para personas naturales
                return false;

            // Aplicar algoritmo de validación
            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;

            for (int i = 0; i < 9; i++)
            {
                int producto = int.Parse(ci[i].ToString()) * coeficientes[i];
                suma += producto >= 10 ? producto - 9 : producto;
            }

            int digitoVerificador = (10 - (suma % 10)) % 10;
            return digitoVerificador == int.Parse(ci[9].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente(Cliente cliente)
        {
            if(_appDBcontext.Clientes.Find(cliente.IdCLiente) != null) return BadRequest("Ya existe un cliente con este id");

            if(_appDBcontext.Clientes.Find(cliente.CI) != null) return BadRequest("Ya existe un cliente con esta cedula");

            if (!EsCedulaValida(cliente.CI)) return BadRequest("La cédula ingresada no es válida");

            _appDBcontext.Clientes.Add(cliente);
            await _appDBcontext.SaveChangesAsync();
            return Ok(cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, Cliente cliente)
        {
            var clienteExistente = await _appDBcontext.Clientes.FindAsync(id);

            if (clienteExistente == null) return NotFound("No existe un cliente con este id");

            if (!EsCedulaValida(cliente.CI)) return BadRequest("La cédula ingresada no es válida");

            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Apellido = cliente.Apellido;
            clienteExistente.CI = cliente.CI;

            await _appDBcontext.SaveChangesAsync();
            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _appDBcontext.Clientes.FindAsync(id);

            if (cliente == null) return NotFound("No existe un cliente con este id");

            if (cliente.Reservas.Count > 0) return BadRequest("No se puede eliminar el cliente porque tiene relaciones");


            _appDBcontext.Clientes.Remove(cliente);
            await _appDBcontext.SaveChangesAsync();
            return Ok(cliente);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionController : ControllerBase
    {
        private readonly AppDBContext _appDBcontext;
        public HabitacionController(AppDBContext appDBcontext)
        {
            _appDBcontext = appDBcontext;
        }

        //Habitaciones
        [HttpGet]
        public async Task<IActionResult> GetHabitaciones()
        {
            return Ok(await _appDBcontext.Habitaciones.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateHabitacion(Habitacion habitacion)
        {
            if (_appDBcontext.Habitaciones.Find(habitacion.IdHabitacion) != null) return BadRequest("Ya existe una habitación con este id");

            _appDBcontext.Habitaciones.Add(habitacion);
            await _appDBcontext.SaveChangesAsync();
            return Ok(habitacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHabitacion(int id, Habitacion habitacion)
        {
            var habitacionExistente = await _appDBcontext.Habitaciones.FindAsync(id);

            if (habitacionExistente == null) return NotFound("No existe una habitación con este id");

            habitacionExistente.Tipo = habitacion.Tipo;
            habitacionExistente.Disponible = habitacion.Disponible;

            await _appDBcontext.SaveChangesAsync();
            return Ok(habitacion);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabitacion(int id)
        {
            var habitacion = await _appDBcontext.Habitaciones.FindAsync(id);

            if (habitacion == null) return NotFound("No existe una habitación con este id");

            _appDBcontext.Habitaciones.Remove(habitacion);
            await _appDBcontext.SaveChangesAsync();
            return Ok(habitacion);
        }
    }
}
