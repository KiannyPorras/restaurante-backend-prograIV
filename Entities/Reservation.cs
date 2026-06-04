namespace RestauranteApi.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan ReservationTime { get; set; }

        public int GuestCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ClientId { get; set; }

        public int TableId { get; set; }

        public int TurnId { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Active;

        public Client Client { get; set; } = null!;

        public Table Table { get; set; } = null!;

        public Turn Turn { get; set; } = null!;

        public ICollection<ReservationHistory> ReservationHistories { get; set; } = new List<ReservationHistory>();
    }
}
