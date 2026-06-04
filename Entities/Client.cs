namespace RestauranteApi.Entities
{
    public class Client
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public ICollection<WaitingList> WaitingLists { get; set; } = new List<WaitingList>();
    }
}
