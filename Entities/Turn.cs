namespace RestauranteApi.Entities
{
    public class Turn
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
