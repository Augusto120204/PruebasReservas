using ReservasHotel;

namespace TestReservas
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            var reserva = new Reserva();
            // Act
            reserva.IdReserva = 1;
            // Assert
            Assert.Equal(1, reserva.IdReserva);
        }
    }
}