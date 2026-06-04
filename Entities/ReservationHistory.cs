namespace RestauranteApi.Entities
{
    public class ReservationHistory
    {
        public int  Id { get; set; }

        public ReservationStatus PrevState { get; set; }

        public ReservationStatus NewState { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public int ReservationId { get; set; }

        public Reservation Reservation { get; set; } = null!;
    }
}
